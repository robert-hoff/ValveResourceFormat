using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;

namespace MyShaderAnalysis.vcsparsing {

    public class DataReaderZFrameByteAnalysis : DataReader {

        FILETYPE filetype;

        public DataReaderZFrameByteAnalysis(byte[] data, FILETYPE filetype) : base(data) {
            if (filetype == FILETYPE.features_file) {
                throw new ShaderParserException("file type cannot be features, as they don't contain any zframes");
            }
            this.filetype = filetype;
        }


        bool writeAsHtml = true;
        public void ConfigureWriteFileAsHtml() {
            writeAsHtml = true;
        }

        bool saveGlslSources = false;
        string outputDir = null;

        public void RequestGlslFileSave(string outputDir) {
            saveGlslSources = true;
            this.outputDir = outputDir;
        }


        public void ParseFile() {
            List<(int, int, string)> glslSources = new();

            ShowZDataSection(-1);
            ShowZFrameHeaderUpdated();

            // this applies only to vs files (ps, gs and psrs files don't have this section)
            if (filetype == FILETYPE.vs_file) {
                // values seen
                // 1,2,4,5,8,10,12,16,20,40,48,80,120,160
                int blockCountInput = ReadInt16AtPosition();
                ShowByteCount("Some kind of state summary (uniforms/variables input?)");
                ShowBytes(2, false);
                TabComment($"nr of data-blocks ({blockCountInput})");
                ShowBytes(blockCountInput * 2);
                OutputWriteLine("");
            }

            int blockCount = ReadInt16AtPosition();
            ShowByteCount("Data blocks");
            ShowBytes(2, false);
            TabComment($"nr of data-blocks ({blockCount})");
            OutputWriteLine("");
            for (int i = 0; i < blockCount; i++) {
                ShowZDataSection(i);
            }
            BreakLine();

            ShowByteCount("Some kind of state summary (uniforms/variables output?)");
            int blockCountOutput = ReadInt16AtPosition();
            ShowBytes(2, false);
            TabComment($"nr of data-blocks ({blockCountOutput})", 1);
            ShowBytes(blockCountOutput * 2);
            BreakLine();

            ShowByteCount();
            ShowBytes(4, false);
            int bin0 = databytes[offset - 4];
            int bin1 = databytes[offset - 3];
            TabComment($"possible flags {Convert.ToString(bin0, 2).PadLeft(8, '0')} {Convert.ToString(bin1, 2).PadLeft(8, '0')}", 7);
            ShowBytes(1, false);
            TabComment("values seen 0,1", 16);

            ShowBytes(4, false);
            uint glslSourceCount = ReadUIntAtPosition(offset - 4);
            TabComment($"glsl source files ({glslSourceCount})", 7);
            ShowBytes(1, false);
            TabComment("values seen 0,1", 16);
            BreakLine();


            for (int i = 0; i < glslSourceCount; i++) {
                var glslSourceItem = ShowZSourceSection(i);
                glslSources.Add(glslSourceItem);
            }

            //  End blocks for vs and gs files
            if (filetype == FILETYPE.vs_file || filetype == FILETYPE.gs_file) {
                ShowZAllEndBlocksTypeVs();
                BreakLine();
            }

            //  End blocks for ps and psrs files
            if (filetype == FILETYPE.ps_file || filetype == FILETYPE.psrs_file) {
                ShowByteCount();
                ShowBytes(4, false);
                int nrEndBlocks = ReadIntAtPosition(offset - 4);
                TabComment($"nr of end blocks ({nrEndBlocks})");
                OutputWriteLine("");

                for (int i = 0; i < nrEndBlocks; i++) {
                    ShowByteCount($"End-block[{i}]");
                    ShowBytes(4, false);
                    int blockId = ReadInt16AtPosition(offset - 4);
                    TabComment($"blockId ref ({blockId})");
                    ShowBytes(4, false);
                    TabComment("always 0");
                    ShowBytes(4, false);
                    int sourceReference = ReadInt16AtPosition(offset - 4);
                    TabComment($"source ref ({sourceReference})");

                    ShowBytes(4, false);
                    uint glslPointer = ReadUIntAtPosition(offset - 4);
                    TabComment($"glsl source pointer ({glslPointer})");

                    ShowBytes(3, false);
                    bool hasData0 = databytes[offset - 3] == 0;
                    bool hasData1 = databytes[offset - 2] == 0;
                    bool hasData2 = databytes[offset - 1] == 0;
                    TabComment($"(data0={hasData0}, data1={hasData1}, data2={hasData2})", 7);

                    if (hasData0) {
                        OutputWriteLine("// data-section 0");
                        ShowBytes(16);
                    }
                    if (hasData1) {
                        OutputWriteLine("// data-section 1");
                        ShowBytes(20);
                    }
                    if (hasData2) {
                        OutputWriteLine("// data-section 2");
                        ShowBytes(3);
                        ShowBytes(8);
                        ShowBytes(64, 32);
                    }
                    OutputWriteLine("");
                }
            }
            EndOfFile();


            // write the gsls source, if indicated
            if (saveGlslSources && !writeAsHtml) {
                SaveGlslSourcestoTxt(glslSources);
            }
            if (saveGlslSources && writeAsHtml) {
                SaveGlslSourcestoHtml(glslSources);
            }


        }



        private void SaveGlslSourcestoHtml(List<(int, int, string)> glslSources) {
            foreach (var glslSourceItem in glslSources) {
                string htmlFilename = GetGlslHtmlFilename(glslSourceItem.Item3);
                string glslFilenamepath = @$"{outputDir}\{htmlFilename}";
                if (File.Exists(glslFilenamepath)) {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize);
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

        private void SaveGlslSourcestoTxt(List<(int, int, string)> glslSources) {
            foreach (var glslSourceItem in glslSources) {
                string glslFilenamepath = @$"{outputDir}\{GetGlslTxtFilename(glslSourceItem.Item3)}";
                if (File.Exists(glslFilenamepath)) {
                    continue;
                }
                int glslOffset = glslSourceItem.Item1;
                int glslSize = glslSourceItem.Item2;
                byte[] glslSourceContent = ReadBytesAtPosition(glslOffset, glslSize);

                Debug.WriteLine($"writing {glslFilenamepath}");
                File.WriteAllBytes(glslFilenamepath, glslSourceContent);
            }
        }

        public void PrintIntWithValue() {
            int intval = ReadIntAtPosition(offset);
            ShowBytes(4, false);
            TabComment($"{intval}");
        }

        private bool prevBlockWasZero = false;
        public void ShowZDataSection(int blockId) {
            int blockSize = ShowZBlockDataHeader(blockId);
            ShowZBlockDataBody(blockSize);

        }

        public int ShowZBlockDataHeader(int blockId) {
            int arg0 = ReadIntAtPosition(offset);
            int arg1 = ReadIntAtPosition(offset + 4);
            int arg2 = ReadIntAtPosition(offset + 8);

            if (arg0 == 0 && arg1 == 0 && arg2 == 0) {
                ShowBytes(12, false);
                TabComment($"data-block[{blockId}]");
                return 0;
            }
            string comment = "";
            if (blockId == -1) {
                comment = $"leading data";
            }
            if (blockId >= 0) {
                comment = $"data-block[{blockId}]";
            }
            int blockSize = ReadIntAtPosition(offset);
            if (prevBlockWasZero) {
                OutputWriteLine("");
            }
            ShowByteCount(comment);
            PrintIntWithValue();
            PrintIntWithValue();
            PrintIntWithValue();
            return blockSize * 4;
        }

        public void ShowZBlockDataBody(int byteSize) {
            if (byteSize == 0) {
                prevBlockWasZero = true;
                return;
            } else {
                prevBlockWasZero = false;
            }
            Comment($"{byteSize / 4}*4 bytes");
            ShowBytes(byteSize);
            BreakLine();
        }

        public void ShowZFrameHeaderUpdated() {
            ShowByteCount("Frame header");
            ShowBytes(2, false);
            uint nrArgs = ReadUInt16AtPosition(offset - 2);
            TabComment($"nr of arguments ({nrArgs})");
            OutputWriteLine("");

            for (int i = 0; i < nrArgs; i++) {
                ShowMurmurString();
                int headerOperator = databytes[offset];
                if (headerOperator == 0x0e) {
                    ShowBytes(3);
                    continue;
                }
                if (headerOperator == 1) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(8);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 9) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(8);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 5) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(11);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
            }
            if (nrArgs > 0) {
                BreakLine();
            }
        }

        public (int, int, string) ShowZSourceSection(int blockId) {
            int sourceSize = ShowZSourceOffsets();
            int sourceOffset = offset;
            ShowZGlslSourceSummary(blockId);
            ShowByteCount();
            byte[] fileIdBytes = ReadBytes(16);
            string fileIdStr = BytesToString(fileIdBytes);
            if (writeAsHtml) {
                OutputWrite(GetGlslHtmlLink(fileIdStr));
            } else {
                OutputWrite(fileIdStr);
            }
            TabComment($"File ID");
            BreakLine();
            return (sourceOffset, sourceSize, fileIdStr);
        }

        public int ShowZSourceOffsets() {
            ShowByteCount("glsl source offsets");
            PrintIntWithValue();
            uint offset1 = ReadUIntAtPosition(offset - 4);
            if (offset1 == 0) {
                return 0;
            }
            ShowBytes(4, false);
            TabComment("always 3");
            PrintIntWithValue();
            int sourceSize = ReadIntAtPosition(offset - 4) - 1; // one less because of null-term
            BreakLine();
            return sourceSize;
        }

        public void ShowZGlslSourceSummary(int sourceId) {
            int bytesToRead = ReadIntAtPosition(offset - 4);
            int endOfSource = offset + bytesToRead;
            ShowByteCount($"GLSL-SOURCE[{sourceId}]");
            if (bytesToRead == 0) {
                OutputWriteLine("// no source present");
            }
            if (bytesToRead > 100) {
                ShowBytes(100);
                ShowByteCount();
                Comment($"... ({endOfSource - offset} bytes of data not shown)");
            } else if (bytesToRead <= 100 && bytesToRead > 0) {
                ShowBytes(bytesToRead);
            }
            offset = endOfSource;
            BreakLine();
        }

        public void ShowZAllEndBlocksTypeVs() {
            ShowByteCount();
            ShowBytes(4, false);
            int nr_end_blocks = ReadIntAtPosition(offset - 4);
            TabComment($"nr end blocks ({nr_end_blocks})");
            BreakLine();
            for (int i = 0; i < nr_end_blocks; i++) {
                ShowBytes(16);
            }
        }

        private void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
            ShowByteCount();
            OutputWriteLine("EOF");
            BreakLine();
        }

        private void ShowMurmurString() {
            string nulltermstr = ReadNullTermStringAtPosition(offset);
            uint murmur32 = ReadUIntAtPosition(offset + nulltermstr.Length + 1);
            uint murmurCheck = MurmurHashPiSeed(nulltermstr.ToLower());
            if (murmur32 != murmurCheck) {
                throw new ShaderParserException("not a murmur string!");
            }
            Comment($"{nulltermstr} | 0x{murmur32:x08}");
            ShowBytes(nulltermstr.Length + 1 + 4);
        }

        private void ShowDynamicExpression(int dynExpLen) {
            byte[] dynExpDatabytes = ReadBytesAtPosition(offset, dynExpLen);
            string dynExp = getDynamicExpression(dynExpDatabytes);
            OutputWriteLine($"// {dynExp}");
            ShowBytes(dynExpLen);
        }

        private ParseDynamicExpressionShader myDynParser = new ParseDynamicExpressionShader();
        private string getDynamicExpression(byte[] dynExpDatabytes) {
            if (myDynParser == null) {
                myDynParser = new ParseDynamicExpressionShader();
            }
            myDynParser.ParseExpression(dynExpDatabytes);
            if (myDynParser.errorWhileParsing) {
                string errorMessage = $"problem occured parsing dynamic expression {myDynParser.errorMessage}";
                Debug.WriteLine(errorMessage);
                return errorMessage;
            }
            return myDynParser.dynamicExpressionResult;
        }





    }
}









