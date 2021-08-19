using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;

namespace MyShaderAnalysis.vcsparsing
{

    public class DataReaderZFrameByteAnalysis : ShaderDataReader
    {

        private VcsFileType filetype;
        private VcsSourceType sourceType;

        public DataReaderZFrameByteAnalysis(byte[] data, VcsFileType filetype, VcsSourceType vcsSourceType) : base(data)
        {
            if (filetype == VcsFileType.Features)
            {
                throw new ShaderParserException("file type cannot be features, as they don't contain any zframes");
            }
            this.filetype = filetype;
            this.sourceType = vcsSourceType;
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
            if (filetype == VcsFileType.VertexShader)
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


            ShowByteCount($"Start of source section, {GetOffset()} is the base offset for end-section source pointers");
            int gpuSourceCount = ReadIntAtPosition();
            ShowBytes(4, $"{sourceType} source files ({gpuSourceCount})");
            ShowBytes(1, "unknown boolean, values seen 0,1", tabLen: 13);
            BreakLine();


            if (sourceType == VcsSourceType.Glsl)
            {
                ShowGlslSources(gpuSourceCount);
            }

            if (sourceType == VcsSourceType.DXIL)
            {
                ShowDxilSources(gpuSourceCount);
            }

            if (sourceType == VcsSourceType.DXBC)
            {
                ShowDxbcSources(gpuSourceCount);
            }




            //  End blocks for vs and gs files
            if (filetype == VcsFileType.VertexShader || filetype == VcsFileType.GeometryShader)
            {
                ShowZAllEndBlocksTypeVs();
                BreakLine();
            }

            //  End blocks for ps and psrs files
            if (filetype == VcsFileType.PixelShader || filetype == VcsFileType.PotentialShadowReciever)
            {
                ShowByteCount();
                int nrEndBlocks = ReadIntAtPosition();
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

                    uint glslPointer = ReadUIntAtPosition();
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
            if (sourceType == VcsSourceType.DXIL && saveGlslSources && !writeAsHtml)
            {
                SaveGlslSourcestoTxt(glslSources);
            }
            if (sourceType == VcsSourceType.DXIL && saveGlslSources && writeAsHtml)
            {
                SaveGlslSourcestoHtml(glslSources);
            }
            if (sourceType != VcsSourceType.DXIL && saveGlslSources)
            {
                Debug.WriteLine($"glsl save indicated but source is not glsl");
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
            int arg0 = ReadIntAtPosition();
            int arg1 = ReadIntAtPosition(4);
            int arg2 = ReadIntAtPosition(8);

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
            int blockSize = ReadIntAtPosition();
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
                    int dynExpLen = ReadIntAtPosition(3);
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
                    int dynExpLen = ReadIntAtPosition(3);
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
                    int dynExpLen = ReadIntAtPosition(3);
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

        private void ShowDxilSources(int dxilSourceCount)
        {
            for (int i = 0; i < dxilSourceCount; i++)
            {
                int sourceOffset = ReadIntAtPosition();
                ShowByteCount();
                ShowBytes(4, $"offset to end of source {sourceOffset} (taken from {GetOffset() + 4})");
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
                int endOfSource = GetOffset() + additionalSourceBytes;
                if (additionalSourceBytes > SOURCE_BYTES_TO_SHOW)
                {
                    ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    OutputWrite(" ");
                    int remainingBytes = endOfSource - GetOffset();
                    if (remainingBytes < 50)
                    {
                        ShowBytes(remainingBytes);
                    } else
                    {
                        Comment($"... ({endOfSource - GetOffset()} bytes of data not shown)");
                    }
                } else if (additionalSourceBytes <= SOURCE_BYTES_TO_SHOW && additionalSourceBytes > 0)
                {
                    ShowBytes(additionalSourceBytes);
                } else
                {
                    OutputWriteLine("// no source present");
                }
                SetOffset(endOfSource);
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
                int sourceSize = ReadIntAtPosition();
                ShowByteCount();
                ShowBytes(4, $"Source size, {sourceSize} bytes");
                BreakLine();
                int endOfSource = GetOffset() + sourceSize;
                ShowByteCount($"DXBC-SOURCE[{sourceId}]");
                if (sourceSize == 0)
                {
                    OutputWriteLine("// no source present");
                }
                if (sourceSize > SOURCE_BYTES_TO_SHOW)
                {
                    ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    OutputWrite(" ");
                    Comment($"... ({endOfSource - GetOffset()} bytes of data not shown)");
                } else if (sourceSize <= SOURCE_BYTES_TO_SHOW && sourceSize > 0)
                {
                    ShowBytes(sourceSize);
                }
                SetOffset(endOfSource);

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
                int sourceOffset = GetOffset();
                ShowZGlslSourceSummary(sourceId);
                ShowByteCount();
                byte[] fileIdBytes = ReadBytes(16);
                string fileIdStr = BytesToString(fileIdBytes);
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
            uint offset1 = ReadUIntAtPosition();
            ShowBytesWithIntValue();
            if (offset1 == 0)
            {
                return 0;
            }
            ShowBytes(4, breakLine: false);
            TabComment("always 3");
            int sourceSize = ReadIntAtPosition() - 1; // one less because of null-term
            ShowBytesWithIntValue();
            BreakLine();
            return sourceSize;
        }


        const int SOURCE_BYTES_TO_SHOW = 100;

        // FIXME - can't I pass the source size here?
        public void ShowZGlslSourceSummary(int sourceId)
        {
            int bytesToRead = ReadIntAtPosition(-4);
            int endOfSource = GetOffset() + bytesToRead;
            ShowByteCount($"GLSL-SOURCE[{sourceId}]");
            if (bytesToRead == 0)
            {
                OutputWriteLine("// no source present");
            }
            if (bytesToRead > SOURCE_BYTES_TO_SHOW)
            {
                ShowBytes(SOURCE_BYTES_TO_SHOW);
                Comment($"... ({endOfSource - GetOffset()} bytes of data not shown)");
            } else if (bytesToRead <= SOURCE_BYTES_TO_SHOW && bytesToRead > 0)
            {
                ShowBytes(bytesToRead);
            }
            SetOffset(endOfSource);
            BreakLine();
        }

        public void ShowZAllEndBlocksTypeVs()
        {
            ShowByteCount();
            int nr_end_blocks = ReadIntAtPosition();
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
            uint murmur32 = ReadUIntAtPosition(nulltermstr.Length + 1);
            uint murmurCheck = MurmurHashPiSeed(nulltermstr.ToLower());
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

        private VfxEval myDynParser = new();
        private string GetDynamicExpression(byte[] dynExpDatabytes)
        {
            if (myDynParser == null)
            {
                myDynParser = new VfxEval();
            }
            myDynParser.ParseExpression(dynExpDatabytes);
            if (myDynParser.errorWhileParsing)
            {
                string errorMessage = $"problem occured parsing dynamic expression {myDynParser.errorMessage}";
                Debug.WriteLine(errorMessage);
                return errorMessage;
            }
            return myDynParser.dynamicExpressionResult;
        }




        private void SaveGlslSourcestoHtml(List<(int, int, string)> glslSources)
        {
            foreach (var glslSourceItem in glslSources)
            {
                string htmlFilename = GetGlslHtmlFilename(glslSourceItem.Item3);
                string glslFilenamepath = @$"{outputDir}\{htmlFilename}";
                if (File.Exists(glslFilenamepath))
                {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize, rel: false);
                Debug.WriteLine($"writing {glslFilenamepath}");
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
                string glslFilenamepath = @$"{outputDir}\{GetGlslTxtFilename(glslSourceItem.Item3)}";
                if (File.Exists(glslFilenamepath))
                {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize, rel: false);

                Debug.WriteLine($"writing {glslFilenamepath}");
                File.WriteAllBytes(glslFilenamepath, glslSourceContent);
            }
        }





    }
}









