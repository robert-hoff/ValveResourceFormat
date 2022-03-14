using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
// using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.utilhelpers
{

    public class DataReaderZFrameByteAnalysis : ShaderDataReader
    {

        private VcsProgramType vcsProgramType;
        private VcsPlatformType vcsSourceType;
        private VcsShaderModelType vcsModelType;

        public DataReaderZFrameByteAnalysis(byte[] data, VcsProgramType filetype,
            VcsPlatformType vcsSourceType, VcsShaderModelType vcsModelType) : base(new MemoryStream(data))
        {
            if (filetype == VcsProgramType.Features)
            {
                throw new ShaderParserException("file type cannot be features, as they don't contain any zframes");
            }
            this.vcsProgramType = filetype;
            this.vcsSourceType = vcsSourceType;
            this.vcsModelType = vcsModelType;
        }


        bool writeAsHtml = false;
        public void SetWriteAsHtml(bool writeAsHtml)
        {
            this.writeAsHtml = writeAsHtml;
        }

        bool saveGlslSources = false;
        string outputDir = null;

        public void RequestGlslFileSave(string outputDir)
        {
            saveGlslSources = true;
            this.outputDir = outputDir;
        }

        // these are recorded in case save is indicated
        private List<(int, int, string)> glslSources = new();

        public void PrintByteAnalysis()
        {
            ShowZDataSection(-1);
            ShowZFrameHeader();
            // this applies only to vs files (ps, gs and psrs files don't have this section)
            if (vcsProgramType == VcsProgramType.VertexShader)
            {
                // values seen
                // 1,2,4,5,8,10,12,16,20,40,48,80,120,160
                int blockCountInput = ReadInt16AtPosition();
                ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to specific configurations (block IDs)");
                ShowBytes(2, breakLine: false);
                TabComment($"nr of data-blocks ({blockCountInput})");
                ShowBytes(blockCountInput * 2);
                OutputWriteLine("");
            }
            int blockCount = ReadInt16AtPosition();
            ShowByteCount("Data blocks");
            ShowBytes(2, breakLine: false);
            TabComment($"nr of data-blocks ({blockCount})");
            OutputWriteLine("");
            for (int i = 0; i < blockCount; i++)
            {
                ShowZDataSection(i);
            }
            BreakLine();
            ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to active block IDs");
            int blockCountOutput = ReadInt16AtPosition();
            ShowBytes(2, breakLine: false);
            TabComment($"nr of data-blocks ({blockCountOutput})", 1);
            ShowBytes(blockCountOutput * 2);
            BreakLine();
            ShowByteCount();
            byte flagbyte = ReadByteAtPosition();
            ShowBytes(1, $"possible control byte ({flagbyte}) or flags ({Convert.ToString(flagbyte, 2).PadLeft(8, '0')})");
            ShowBytes(1, "values seen (0,1,2)");
            ShowBytes(1, "always 0");
            ShowBytes(1, "always 0");
            ShowBytes(1, "values seen (0,1)");
            BreakLine();
            ShowByteCount($"Start of source section, {BaseStream.Position} is the base offset for end-section source pointers");
            int gpuSourceCount = ReadInt32AtPosition();
            ShowBytes(4, $"{vcsSourceType} source files ({gpuSourceCount})");
            ShowBytes(1, "unknown boolean, values seen 0,1", tabLen: 13);
            BreakLine();

            if (vcsSourceType == VcsPlatformType.PC)
            {
                switch (vcsModelType)
                {
                    case VcsShaderModelType._20:
                    case VcsShaderModelType._2b:
                    case VcsShaderModelType._30:
                    case VcsShaderModelType._31:
                        ShowDxilSources(gpuSourceCount);
                        break;
                    case VcsShaderModelType._40:
                    case VcsShaderModelType._41:
                    case VcsShaderModelType._50:
                        ShowDxbcSources(gpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported model type {vcsModelType}");
                }
            } else
            {
                switch (vcsSourceType)
                {
                    case VcsPlatformType.PCGL:
                    case VcsPlatformType.MOBILE_GLES:
                        ShowGlslSources(gpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported source type {vcsSourceType}");
                }
            }

            //  End blocks for vs and gs files
            if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.GeometryShader)
            {
                ShowZAllEndBlocksTypeVs();
                BreakLine();
            }
            //  End blocks for ps and psrs files
            if (vcsProgramType == VcsProgramType.PixelShader || vcsProgramType == VcsProgramType.PixelShaderRenderState)
            {
                ShowByteCount();
                int nrEndBlocks = ReadInt32AtPosition();
                ShowBytes(4, breakLine: false);
                TabComment($"nr of end blocks ({nrEndBlocks})");
                OutputWriteLine("");
                for (int i = 0; i < nrEndBlocks; i++)
                {
                    ShowByteCount($"End-block[{i}]");
                    int blockId = ReadInt16AtPosition();
                    ShowBytes(4, breakLine: false);
                    TabComment($"blockId ref ({blockId})");
                    ShowBytes(4, breakLine: false);
                    TabComment("always 0");
                    int sourceReference = ReadInt16AtPosition();
                    ShowBytes(4, breakLine: false);
                    TabComment($"source ref ({sourceReference})");
                    uint glslPointer = ReadUInt32AtPosition();
                    ShowBytes(4, breakLine: false);
                    TabComment($"glsl source pointer ({glslPointer})");
                    bool hasData0 = ReadByteAtPosition(0) == 0;
                    bool hasData1 = ReadByteAtPosition(1) == 0;
                    bool hasData2 = ReadByteAtPosition(2) == 0;
                    ShowBytes(3, breakLine: false);
                    TabComment($"(data0={hasData0}, data1={hasData1}, data2={hasData2})", 7);
                    if (hasData0)
                    {
                        OutputWriteLine("// data-section 0");
                        ShowBytes(16);
                    }
                    if (hasData1)
                    {
                        OutputWriteLine("// data-section 1");
                        ShowBytes(20);
                    }
                    if (hasData2)
                    {
                        OutputWriteLine("// data-section 2");
                        ShowBytes(3);
                        ShowBytes(8);
                        ShowBytes(64, 32);
                    }
                    OutputWriteLine("");
                }
            }
            ShowEndOfFile();

            // write the gsls source, if indicated
            if (saveGlslSources && !writeAsHtml)
            {

                if (vcsSourceType == VcsPlatformType.PC)
                {
                    switch (vcsModelType)
                    {
                        case VcsShaderModelType._20:
                        case VcsShaderModelType._2b:
                        case VcsShaderModelType._30:
                        case VcsShaderModelType._31:
                            throw new ShaderParserException($"Source save not implemented for {vcsSourceType} {vcsModelType}");
                        case VcsShaderModelType._40:
                        case VcsShaderModelType._41:
                        case VcsShaderModelType._50:
                        case VcsShaderModelType._60:
                            throw new ShaderParserException($"Source save not implemented for {vcsSourceType} {vcsModelType}");
                        default:
                            throw new ShaderParserException($"Source save not implemented for {vcsSourceType} {vcsModelType}");
                    }
                } else
                {
                    switch (vcsSourceType)
                    {
                        case VcsPlatformType.PCGL:
                        case VcsPlatformType.MOBILE_GLES:
                            SaveGlslSourcestoTxt(glslSources);
                            break;
                        default:
                            throw new ShaderParserException($"glsl save indicated but source is not glsl");
                    }
                }
            }
        }

        private bool prevBlockWasZero = false;
        public void ShowZDataSection(int blockId)
        {
            int blockSize = ShowZBlockDataHeader(blockId);
            ShowZBlockDataBody(blockSize);
        }
        public int ShowZBlockDataHeader(int blockId)
        {
            int arg0 = ReadInt32AtPosition();
            int arg1 = ReadInt32AtPosition(4);
            int arg2 = ReadInt32AtPosition(8);

            if (blockId != -1 && arg0 == 0 && arg1 == 0 && arg2 == 0)
            {
                ShowBytes(12, breakLine: false);
                TabComment($"data-block[{blockId}]");
                return 0;
            }
            string comment = "";
            if (blockId == -1)
            {
                comment = $"leading data";
            }
            if (blockId >= 0)
            {
                comment = $"data-block[{blockId}]";
            }
            int blockSize = ReadInt32AtPosition();
            if (prevBlockWasZero)
            {
                OutputWriteLine("");
            }
            ShowByteCount(comment);
            ShowBytesWithIntValue();
            ShowBytesWithIntValue();
            ShowBytesWithIntValue();
            if (blockId == -1 && arg0 == 0 && arg1 == 0 && arg2 == 0)
            {
                BreakLine();
            }
            return blockSize * 4;
        }
        public void ShowZBlockDataBody(int byteSize)
        {
            if (byteSize == 0)
            {
                prevBlockWasZero = true;
                return;
            } else
            {
                prevBlockWasZero = false;
            }
            Comment($"{byteSize / 4}*4 bytes");
            ShowBytes(byteSize);
            BreakLine();
        }
        public void ShowZFrameHeader()
        {
            ShowByteCount("Frame header");
            uint nrArgs = ReadUInt16AtPosition();
            ShowBytes(2, breakLine: false);
            TabComment($"nr of arguments ({nrArgs})");
            OutputWriteLine("");

            for (int i = 0; i < nrArgs; i++)
            {
                ShowMurmurString();
                // int headerOperator = databytes[offset];
                int headerOperator = ReadByteAtPosition();
                if (headerOperator == 0x0e)
                {
                    ShowBytes(3);
                    continue;
                }
                if (headerOperator == 1)
                {
                    int dynExpLen = ReadInt32AtPosition(3);
                    if (dynExpLen == 0)
                    {
                        ShowBytes(8);
                        continue;
                    } else
                    {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 9)
                {
                    int dynExpLen = ReadInt32AtPosition(3);
                    if (dynExpLen == 0)
                    {
                        ShowBytes(8);
                        continue;
                    } else
                    {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 5)
                {
                    int dynExpLen = ReadInt32AtPosition(3);
                    if (dynExpLen == 0)
                    {
                        ShowBytes(11);
                        continue;
                    } else
                    {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
            }
            if (nrArgs > 0)
            {
                BreakLine();
            }
        }
        const int SOURCE_BYTES_TO_SHOW = 100;
        private void ShowDxilSources(int dxilSourceCount)
        {
            for (int i = 0; i < dxilSourceCount; i++)
            {
                int sourceOffset = ReadInt32AtPosition();
                ShowByteCount();
                ShowBytes(4, $"offset to end of source {sourceOffset} (taken from {BaseStream.Position + 4})");
                int additionalSourceBytes = 0;
                if (sourceOffset > 0)
                {
                    ShowBytes(4);
                    int unknown_prog_uint16 = (int)ReadUInt16AtPosition(2);
                    ShowBytes(4, $"({unknown_prog_uint16}) the first ({unknown_prog_uint16} * 4) bytes look like header data that may need to be processed");
                    BreakLine();
                    ShowByteCount($"DXIL-SOURCE[{i}]");
                    int sourceSize = sourceOffset - 8;
                    if (unknown_prog_uint16 > 0)
                    {
                        ShowBytes(unknown_prog_uint16 * 4);
                    }
                    additionalSourceBytes = sourceSize - unknown_prog_uint16 * 4;
                }
                int endOfSource = (int)BaseStream.Position + additionalSourceBytes;
                if (additionalSourceBytes > SOURCE_BYTES_TO_SHOW)
                {
                    ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    OutputWrite(" ");
                    int remainingBytes = endOfSource - (int)BaseStream.Position;
                    if (remainingBytes < 50)
                    {
                        ShowBytes(remainingBytes);
                    } else
                    {
                        Comment($"... ({endOfSource - BaseStream.Position} bytes of data not shown)");
                    }
                } else if (additionalSourceBytes <= SOURCE_BYTES_TO_SHOW && additionalSourceBytes > 0)
                {
                    ShowBytes(additionalSourceBytes);
                } else
                {
                    OutputWriteLine("// no source present");
                }
                BaseStream.Position = endOfSource;
                BreakLine();
                ShowByteCount();
                ShowBytes(16, "DXIL(hlsl) Editor ref.");
                BreakLine();
            }
        }
        private void ShowDxbcSources(int dxbcSourceCount)
        {
            for (int sourceId = 0; sourceId < dxbcSourceCount; sourceId++)
            {
                int sourceSize = ReadInt32AtPosition();
                ShowByteCount();
                ShowBytes(4, $"Source size, {sourceSize} bytes");
                BreakLine();
                long endOfSource = BaseStream.Position + sourceSize;
                ShowByteCount($"DXBC-SOURCE[{sourceId}]");
                if (sourceSize == 0)
                {
                    OutputWriteLine("// no source present");
                }
                if (sourceSize > SOURCE_BYTES_TO_SHOW)
                {
                    ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    OutputWrite(" ");
                    Comment($"... ({endOfSource - BaseStream.Position} bytes of data not shown)");
                } else if (sourceSize <= SOURCE_BYTES_TO_SHOW && sourceSize > 0)
                {
                    ShowBytes(sourceSize);
                }
                BaseStream.Position = endOfSource;

                BreakLine();
                ShowByteCount();
                ShowBytes(16, "DXBC(hlsl) Editor ref.");
                BreakLine();
            }
        }
        private void ShowGlslSources(int glslSourceCount)
        {
            for (int sourceId = 0; sourceId < glslSourceCount; sourceId++)
            {
                int sourceSize = ShowGlslSourceOffsets();
                int sourceOffset = (int)BaseStream.Position;
                ShowZGlslSourceSummary(sourceId);
                ShowByteCount();
                byte[] fileIdBytes = ReadBytes(16);
                string fileIdStr = ShaderUtilHelpers.BytesToString(fileIdBytes);
                if (writeAsHtml)
                {
                    OutputWrite(GetGlslHtmlLink(fileIdStr));
                } else
                {
                    OutputWrite(fileIdStr);
                }
                TabComment($" Editor ref.");
                BreakLine();
                glslSources.Add((sourceOffset, sourceSize, fileIdStr));
            }
        }
        public int ShowGlslSourceOffsets()
        {
            ShowByteCount("glsl source offsets");
            uint offset1 = ReadUInt32AtPosition();
            ShowBytesWithIntValue();
            if (offset1 == 0)
            {
                return 0;
            }
            ShowBytes(4, breakLine: false);
            TabComment("always 3");
            int sourceSize = ReadInt32AtPosition() - 1; // one less because of null-term
            ShowBytesWithIntValue();
            BreakLine();
            return sourceSize;
        }
        public void ShowZGlslSourceSummary(int sourceId)
        {
            int bytesToRead = ReadInt32AtPosition(-4);
            long endOfSource = BaseStream.Position + bytesToRead;
            ShowByteCount($"GLSL-SOURCE[{sourceId}]");
            if (bytesToRead == 0)
            {
                OutputWriteLine("// no source present");
            }
            if (bytesToRead > SOURCE_BYTES_TO_SHOW)
            {
                ShowBytes(SOURCE_BYTES_TO_SHOW);
                Comment($"... ({endOfSource - BaseStream.Position} bytes of data not shown)");
            } else if (bytesToRead <= SOURCE_BYTES_TO_SHOW && bytesToRead > 0)
            {
                ShowBytes(bytesToRead);
            }
            BaseStream.Position = endOfSource;
            BreakLine();
        }
        public void ShowZAllEndBlocksTypeVs()
        {
            ShowByteCount();
            int nr_end_blocks = ReadInt32AtPosition();
            ShowBytes(4, breakLine: false);
            TabComment($"nr end blocks ({nr_end_blocks})");
            BreakLine();
            for (int i = 0; i < nr_end_blocks; i++)
            {
                ShowBytes(16);
            }
        }
        private void ShowMurmurString()
        {
            string nulltermstr = ReadNullTermStringAtPosition();
            uint murmur32 = ReadUInt32AtPosition(nulltermstr.Length + 1);
            uint murmurCheck = MurmurHash2.Hash(nulltermstr.ToLower(), ShaderFile.PI_MURMURSEED);
            if (murmur32 != murmurCheck)
            {
                throw new ShaderParserException("not a murmur string!");
            }
            Comment($"{nulltermstr} | 0x{murmur32:x08}");
            ShowBytes(nulltermstr.Length + 1 + 4);
        }
        private void ShowDynamicExpression(int dynExpLen)
        {
            byte[] dynExpDatabytes = ReadBytesAtPosition(0, dynExpLen);
            string dynExp = GetDynamicExpression(dynExpDatabytes);
            OutputWriteLine($"// {dynExp}");
            ShowBytes(dynExpLen);
        }
        private string GetDynamicExpression(byte[] dynExpDatabytes)
        {
            try
            {
                return new VfxEval(dynExpDatabytes).DynamicExpressionResult;
            } catch (InvalidDataException)
            {
                return "[error in dyn-exp]";
            }
        }


        private void SaveGlslSourcestoHtml(List<(int, int, string)> glslSources)
        {
            foreach (var glslSourceItem in glslSources)
            {
                string htmlFilename = GetGlslHtmlFilename(glslSourceItem.Item3);
                string glslFilenamepath = @$"{outputDir}/{htmlFilename}";
                if (File.Exists(glslFilenamepath))
                {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize, rel: false);
                Console.WriteLine($"writing {glslFilenamepath}");
                StreamWriter glslFileWriter = new(glslFilenamepath);
                string htmlHeader = GetHtmlHeader(htmlFilename[0..^5], htmlFilename[0..^5]);
                glslFileWriter.WriteLine($"{htmlHeader}");
                glslFileWriter.Flush();
                glslFileWriter.BaseStream.Write(glslSourceContent, 0, glslSourceContent.Length);
                glslFileWriter.Flush();
                glslFileWriter.WriteLine($"{GetHtmlFooter()}");
                glslFileWriter.Flush();
                glslFileWriter.Close();
            }
        }

        private void SaveGlslSourcestoTxt(List<(int, int, string)> glslSources)
        {
            foreach (var glslSourceItem in glslSources)
            {
                string glslFilenamepath = @$"{outputDir}/{GetGlslTxtFilename(glslSourceItem.Item3)}";
                if (File.Exists(glslFilenamepath))
                {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize, rel: false);

                Console.WriteLine($"writing {glslFilenamepath}");
                File.WriteAllBytes(glslFilenamepath, glslSourceContent);
            }
        }





    }
}








