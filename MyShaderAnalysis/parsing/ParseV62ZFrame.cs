using System.IO;
using MyShaderFile.CompiledShader;
using MyShaderFile.ThirdParty;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.parsing
{
    public class ParseV62ZFrame : ShaderDataReader
    {
        private ShaderFile shaderFile;
        private VcsProgramType vcsProgramType;
        private VcsPlatformType vcsPlatformType;
        private VcsShaderModelType vcsShaderModelType;

        private bool showStatusMessage;

        public ParseV62ZFrame(byte[] data, ShaderFile shaderFile, HandleOutputWrite outputWriter = null) : base(new MemoryStream(data), outputWriter)
        {
            this.shaderFile = shaderFile;
            vcsProgramType = shaderFile.VcsProgramType;
            vcsPlatformType = shaderFile.VcsPlatformType;
            vcsShaderModelType = shaderFile.VcsShaderModelType;
        }

        public void PrintByteDetail()
        {
            BaseStream.Position = 0;

            int h0 = ReadInt32AtPosition();
            int h1 = ReadInt32AtPosition(4);
            int h2 = ReadInt32AtPosition(8);

            if (h1 != 0)
            {
                throw new ShaderParserException($"unexpected data {h1}");
            }
            if (h2 != 0)
            {
                throw new ShaderParserException($"unexpected data {h2}");
            }

            ShowByteCount();
            ShowBytes(4, $"{h0}");
            ShowBytes(4, $"{h1}");
            ShowBytes(4, $"{h2}");
            BreakLine();
            Comment($"{h0} 2-length arguments follow");
            ShowByteCount();
            for (int i = 0; i < h0; i++)
            {
                // the second byte is always zero, it may be because the parameter length is never long enough
                //byte secondByte = ReadByteAtPosition(1);
                //if (secondByte != 0)
                //{
                //    throw new ShaderParserException("unexpected data");
                //}
                uint paramId = ReadUInt16AtPosition();
                ShowBytes(2, $"{paramId,3}   {shaderFile.ParamBlocks[(int)paramId].Name}");
            }
            BreakLine();
            // uint nrHeaders = ReadUInt16AtPosition();
            ShowZFrameHeader();

            uint end_arg0 = ReadUInt16AtPosition(); // always 1
            uint end_arg1 = ReadUInt16AtPosition(2); // values seen [0,7]

            if (end_arg0 != 1)
            {
                throw new ShaderParserException($"unexpected data {end_arg0}");
            }
            // not really a safe check
            //if (end_arg1 > 7)
            //{
            //    throw new ShaderParserException($"unexpected data {end_arg1}");
            //}

            // Console.WriteLine($"{end_arg1}");

            ShowBytes(2, $"end_arg0 = {end_arg0}");
            ShowBytes(2, $"end_arg1 = {end_arg1}");

            BreakLine();
            ShowEndOfFile();

            return;

            //ShowZDataSection(-1);
            //ShowZFrameHeader();
            //if (vcsProgramType == VcsProgramType.VertexShader) {
            //    int blockCountInput = ReadInt16AtPosition();
            //    ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to configurations (block IDs)");
            //    ShowBytes(2, breakLine: false);
            //    TabComment($"nr of data-blocks ({blockCountInput})");
            //    ShowBytes(blockCountInput * 2);
            //    OutputWriteLine("");
            //}
            //int blockCount = ReadInt16AtPosition();
            //ShowByteCount("Data blocks");
            //ShowBytes(2, breakLine: false);
            //TabComment($"nr of data-blocks ({blockCount})");
            //OutputWriteLine("");
            //for (int i = 0; i < blockCount; i++) {
            //    ShowZDataSection(i);
            //}
            //BreakLine();
            //ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to active block IDs");
            //int blockCountOutput = ReadInt16AtPosition();
            //ShowBytes(2, breakLine: false);
            //TabComment($"nr of data-blocks ({blockCountOutput})", 1);
            //ShowBytes(blockCountOutput * 2);
            //BreakLine();
            //ShowByteCount();
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
                int headerOperator = ReadByteAtPosition();
                ShowBytes(3, "header-code");
                if (headerOperator == 0x0e)
                {
                    continue;
                }

                int dynExpLen = ReadInt32AtPosition();
                ShowBytes(4, $"dynamic expression length = {dynExpLen}");
                if (dynExpLen > 0)
                {
                    ShowDynamicExpression(dynExpLen);
                    continue;
                }
                if (headerOperator == 1)
                {
                    ShowBytes(4, "header argument");
                }
                if (headerOperator == 9)
                {
                    ShowBytes(1, "header argument");
                }
                if (headerOperator == 5)
                {
                    ShowBytes(4, "header argument");
                }
            }

            if (nrArgs > 0)
            {
                BreakLine();
            }
        }

        const int SOURCE_BYTES_TO_SHOW = 96;
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
                    ShowBytes(4, $"({unknown_prog_uint16}) the first ({unknown_prog_uint16} * 4) " +
                        $"bytes look like header data that may need to be processed");
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
                int endOfSource = (int)BaseStream.Position + sourceSize;
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

        const int VULKAN_SOURCE_BYTES_TO_SHOW = 192;
        private void ShowVulkanSources(int vulkanSourceCount)
        {
            for (int i = 0; i < vulkanSourceCount; i++)
            {
                int offsetToEditorId = ReadInt32AtPosition();
                if (offsetToEditorId == 0)
                {
                    ShowBytes(4);
                    OutputWriteLine("// no source present");
                    BreakLine();
                } else
                {
                    ShowByteCount();
                    ShowBytes(4, $"({offsetToEditorId}) offset to Editor ref. ID ");
                    int endOfSourceOffset = (int)BaseStream.Position + offsetToEditorId;
                    int arg0 = ReadInt32AtPosition();
                    ShowBytes(4, $"({arg0}) values seen for Vulkan sources are (2,3)");
                    int offset2 = ReadInt32AtPosition();
                    ShowBytes(4, $"({offset2}) - looks like an offset, unknown significance");
                    BreakLine();
                    ShowByteCount($"VULKAN-SOURCE[{i}]");
                    int sourceSize = offsetToEditorId - 8;
                    int bytesToShow = VULKAN_SOURCE_BYTES_TO_SHOW > sourceSize ? sourceSize : VULKAN_SOURCE_BYTES_TO_SHOW;
                    ShowBytes(bytesToShow);
                    int bytesNotShown = sourceSize - bytesToShow;
                    if (bytesNotShown > 0)
                    {
                        Comment($"... {bytesNotShown} bytes of data not shown)");
                    }
                    BreakLine();
                    BaseStream.Position = endOfSourceOffset;
                }
                ShowBytes(16, "Vulkan Editor ref. ID");
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
                string fileIdStr = BytesToString(fileIdBytes);
                OutputWrite(fileIdStr);
                TabComment($" Editor ref.");
                BreakLine();
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
            int endOfSource = (int)BaseStream.Position + bytesToRead;
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
            TabComment($"nr of end blocks ({nr_end_blocks})");
            BreakLine();
            for (int i = 0; i < nr_end_blocks; i++)
            {
                ShowBytes(16);
            }
        }

        public void ShowZAllEndBlocksTypeHs()
        {
            ShowByteCount();
            int nr_end_blocks = ReadInt32AtPosition();
            ShowBytes(4, breakLine: false);
            TabComment($"nr of end blocks ({nr_end_blocks})");
            BreakLine();
            for (int i = 0; i < nr_end_blocks; i++)
            {
                ShowBytes(17);
            }
        }

        private void ShowMurmurString()
        {
            ShowByteCount();
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
            string dynExp = ParseDynamicExpression(dynExpDatabytes);
            OutputWriteLine($"// {dynExp}");
            ShowBytes(dynExpLen);
        }

        /*
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
        */

        /*
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
        */
    }
}

