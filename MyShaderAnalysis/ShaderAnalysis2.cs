using MyShaderAnalysis.readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace MyShaderAnalysis {
    class ShaderAnalysis2 {

        const string ANALYSIS_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string ANALYSIS_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";


        public static void RunTrials() {
            // Trial3();
            // Trial2();
            // Trial1ScanHeaderData();

            // WriteZframeAnalysisToFile();
            // ScanSizes();


            RunParseSingleFile();
            // ParseZFrameFiles();

            // RunTrial1ZVsFrame01();
            // Trial1ZVsFrame00();
            // Trial1ZFrame05();
            // Trial1ZFrame02();
            // Trial1ZFrame01();
            // Trial1ZFrame00();
        }


        static void Trial4() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < 3335; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < 3335; i++) {
                // readers[i].ShowBytesAtPositionNoLineBreak(0, 200);
                // string bytebte = readers[i].ReadBytesAsStringAtPosition(0, 200);
                // Debug.WriteLine("");
                // sw.WriteLine(bytebte);

                int int0 = readers[i].ReadIntAtPosition(0);
                int int1 = readers[i].ReadIntAtPosition(4);
                int int2 = readers[i].ReadIntAtPosition(8);
                Debug.WriteLine($"{int0,3} {int1,3} {int2,3} ");


            }


            sw.Flush();
            sw.Close();

        }



        static void Trial3() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");
            List<SortedRow> entries = new();

            int NUMROWS = 3335;


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < NUMROWS; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < NUMROWS; i++) {
                string bytebyte = readers[i].ReadBytesAsStringAtPosition(0, 2000);
                int v1 = readers[i].ReadIntAtPosition(0);
                int v2 = readers[i].ReadIntAtPosition(4);
                int v3 = readers[i].ReadIntAtPosition(8);
                entries.Add(new SortedRow(v1, v2, v3, bytebyte));
            }

            entries.Sort();
            for (int i = 0; i < NUMROWS; i++) {
                sw.WriteLine(entries[i].bytebyte);
            }


            sw.Flush();
            sw.Close();
        }



        class SortedRow : IComparable<SortedRow> {

            public int v1;
            public int v2;
            public int v3;
            public string bytebyte;

            public SortedRow(int v1, int v2, int v3, string bytebyte) {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
                this.bytebyte = bytebyte;


                //Debug.WriteLine($"{v1}  {v2}  {v3}");

            }

            public int CompareTo(SortedRow other) {
                if (v2 != other.v2) {
                    return v2 - other.v2;
                }
                if (v1 != other.v1) {
                    return v1 - other.v1;
                }
                return v3 - other.v3;
            }
        }




        static void Trial2() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");
            List<string> entries = new();


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < 3335; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < 3335; i++) {
                string bytebyte = readers[i].ReadBytesAsStringAtPosition(0, 400);
                entries.Add(bytebyte);
            }

            entries.Sort();
            for (int i = 0; i < 3335; i++) {
                sw.WriteLine(entries[i]);
            }


            sw.Flush();
            sw.Close();
        }



        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\zframedump";


        static void WriteZframeAnalysisToFile() {
            // ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");

            for (int i = 0; i < 2; i++) {

                string outputfile = OUTPUT_DIR + @$"\testfile{i:d03}.txt";
                StreamWriter sw = new StreamWriter(outputfile);

                Trial1ZGenFilesave(sw, shaderReader, i);
                sw.Flush();
                sw.Close();
            }

        }




        static void Trial1ZGenFilesave(StreamWriter sw, ShaderReader shaderReader, int zframeId) {
            Debug.WriteLine($"ZFRAME = {zframeId}");
            // return;

            DataReader datareader = shaderReader.getZframeDataReader(zframeId);
            // Debug.WriteLine($"{zframeId}    {datareader.databytes.Length}");
            datareader.ConfigureWriteToFile(sw);
            datareader.ParseAndShowZFrame();
        }



        static Dictionary<int, int> collectValuesInt = new();
        static Dictionary<string, int> collectValuesString = new();
        static bool[] requestCount = { false };
        static bool[] requestShowFile = { false };


        static void ParseZFrameFiles() {

            // List<string> shaderFiles = getAllPsFiles();
            // List<string> shaderFiles = getAllVsFiles();
            List<string> shaderFiles = getAllZFrameFiles();
            foreach (string filenamepath in shaderFiles) {
                RunTrial1ZVsFrame01(filenamepath, true, 0, 100);
            }
            PrintReport();
        }



        static void RunParseSingleFile() {
            // string filename = ANALYSIS_DIR_NOT_CORE + @"\crystal_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\depth_only_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\grasstile_preview_pcgl_41_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\solidcolor_pcgl_30_ps.vcs";
            string filename = ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\physics_wireframe_pcgl_30_ps.vcs"; // frame 16
            // string filename = ANALYSIS_DIR_CORE + @"\tools_solid_pcgl_30_ps.vcs"; // frame 7
            // string filename = ANALYSIS_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs"; // frame 5
            // string filename = ANALYSIS_DIR_CORE + @"\visualize_nav_pcgl_40_ps.vcs"; // frame 10
            // string filename = ANALYSIS_DIR_CORE + @"\tools_sprite_pcgl_40_gs.vcs"; // gs file
            // string filename = ANALYSIS_DIR_NOT_CORE + @"\hero_pcgl_40_psrs.vcs"; // psrs file
            // string filename = ANALYSIS_DIR_CORE + @"\deferred_shading_pcgl_41_ps.vcs"; // interesting one
            // string filename = ANALYSIS_DIR_NOT_CORE + @"\grasstile_pcgl_30_vs.vcs";
            RunTrial1ZVsFrame01(filename, false, 78, 79);


            PrintReport();
        }




        static void RunTrial1ZVsFrame01(string filenamepath, bool runningTest, int min, int max) {
            int filetype = -1;

            string fileCode = filenamepath.Substring(filenamepath.Length - 6, 2);
            if (fileCode == "vs") {
                filetype = VS_FILE;
            }
            if (fileCode == "ps") {
                filetype = PS_FILE;
            }
            if (fileCode == "rs") {
                filetype = PSRS_FILE;
            }
            if (fileCode == "gs") {
                filetype = GS_FILE;
            }

            if (filetype == -1) {
                throw new ShaderParserException($"unknown file type! {filenamepath}");
            }
            ShaderReader shaderReader = new(filenamepath);


            int zcount = shaderReader.zFrames.Count;
            if (max == -1) {
                max = zcount;
            }
            int numberToParse = zcount > max ? max : zcount;

            if (min >= zcount) {
                Debug.WriteLine($"our of range [{min},{max}) for {shaderReader.filepath}, nothing to parse. zmax = {zcount - 1}");
            } else {
                Debug.WriteLine($"parsing {shaderReader.filepath} frames [{min},{numberToParse})");
            }


            for (int i = min; i < (numberToParse); i++) {
                Trial1ZVsFrame01(shaderReader, i, filetype, runningTest);
            }
        }



        const int PS_FILE = 0;
        const int VS_FILE = 1;
        const int PSRS_FILE = 2; // parses the same as PS
        const int GS_FILE = 3; // parses the same as VS


        static void Trial1ZVsFrame01(ShaderReader shaderReader, int zframeId, int filetype, bool runningTest) {

            DataReader datareader = shaderReader.getZframeDataReader(zframeId);
            datareader.collectValuesInt = collectValuesInt;
            datareader.collectValuesString = collectValuesString;
            datareader.requestCount = requestCount;
            datareader.requestShowFile = requestShowFile;

            if (!runningTest) {
                Debug.WriteLine($"parsing {shaderReader.filepath} ZFRAME{zframeId:d03}");
            }
            if (runningTest) {
                datareader.DisableOutput = true;
            }

            datareader.ShowZDataSection(-1);
            datareader.ShowZFrameHeaderUpdated();

            // this applies only for vs files (ps files don't have this section)
            if (filetype == VS_FILE) {
                // values seen
                // 1,2,4,5,8,10,12,16,20,40,48,80,120,160
                int blockCountInput = datareader.ReadInt16AtPosition(datareader.offset);
                datareader.ShowByteCount("Uniforms state");
                datareader.ShowBytesNoLineBreak(2); ;
                datareader.TabPrintComment($"nr of data-blocks ({blockCountInput}), input state");
                datareader.ShowBytes(blockCountInput * 2);
                datareader.OutputWriteLine("");
            }

            int blockCount = datareader.ReadInt16AtPosition(datareader.offset);
            datareader.ShowByteCount("Data blocks");
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment($"nr of data-blocks ({blockCount})");
            datareader.OutputWriteLine("");
            for (int i = 0; i < blockCount; i++) {
                datareader.ShowZDataSection(i);
            }
            datareader.OutputWriteLine("");

            datareader.ShowByteCount("Uniforms state");
            int blockCountOutput = datareader.ReadInt16AtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment($"nr of data-blocks ({blockCountOutput}), output state", 0);
            datareader.ShowBytes(blockCountOutput * 2);
            datareader.OutputWriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            int bin0 = datareader.databytes[datareader.offset - 4];
            int bin1 = datareader.databytes[datareader.offset - 3];
            datareader.TabPrintComment($"possible flags {Convert.ToString(bin0, 2).PadLeft(8, '0')} {Convert.ToString(bin1, 2).PadLeft(8, '0')}", 7);
            datareader.ShowBytesNoLineBreak(1);
            datareader.TabPrintComment("values seen 0,1", 16);

            datareader.ShowBytesNoLineBreak(4);
            uint glslSourceCount = datareader.ReadUIntAtPosition(datareader.offset - 4);
            datareader.TabPrintComment($"glsl source files ({glslSourceCount})", 7);
            datareader.ShowBytesNoLineBreak(1);
            datareader.TabPrintComment("values seen 0,1", 16);
            datareader.OutputWriteLine("");


            for (int i = 0; i < glslSourceCount; i++) {
                datareader.ShowZSourceSection(i);
            }

            if (filetype == VS_FILE || filetype == GS_FILE) {
                datareader.ShowZAllEndBlocksTypeVs();
                datareader.OutputWriteLine("");
            }

            if (filetype == PS_FILE || filetype == PSRS_FILE) {
                //  END BLOCKS
                datareader.ShowByteCount();
                datareader.ShowBytesNoLineBreak(4);
                int nrEndBlocks = datareader.ReadIntAtPosition(datareader.offset - 4);
                datareader.TabPrintComment($"nr of end blocks ({nrEndBlocks})");
                datareader.OutputWriteLine("");

                for (int i = 0; i < nrEndBlocks; i++) {
                    datareader.ShowByteCount($"End-block[{i}]");
                    datareader.ShowBytesNoLineBreak(4);
                    int blockId = datareader.ReadInt16AtPosition(datareader.offset - 4);
                    datareader.TabPrintComment($"blockId ref ({blockId})");
                    datareader.ShowBytesNoLineBreak(4);
                    datareader.TabPrintComment("always 0");
                    datareader.ShowBytesNoLineBreak(4);
                    int sourceReference = datareader.ReadInt16AtPosition(datareader.offset - 4);
                    datareader.TabPrintComment($"source ref ({sourceReference})");

                    datareader.ShowBytesNoLineBreak(4);
                    uint glslPointer = datareader.ReadUIntAtPosition(datareader.offset - 4);
                    datareader.TabPrintComment($"glsl source pointer ({glslPointer})");

                    datareader.ShowBytesNoLineBreak(3);
                    bool hasData0 = datareader.databytes[datareader.offset - 3] == 0;
                    bool hasData1 = datareader.databytes[datareader.offset - 2] == 0;
                    bool hasData2 = datareader.databytes[datareader.offset - 1] == 0;
                    datareader.TabPrintComment($"(data0={hasData0}, data1={hasData1}, data2={hasData2})", 7);

                    if (hasData0) {
                        datareader.OutputWriteLine("// data-section 0");
                        datareader.ShowBytes(16);
                    }
                    if (hasData1) {
                        datareader.OutputWriteLine("// data-section 1");
                        datareader.ShowBytes(20);
                    }
                    if (hasData2) {
                        datareader.OutputWriteLine("// data-section 2");
                        datareader.ShowBytes(3);
                        datareader.ShowBytes(8);
                        datareader.ShowBytes(64, 32);
                    }
                    datareader.OutputWriteLine("");
                }
            }


            // throws if not at end of file
            datareader.EndOfFile();



            // datareader.ShowByteCount();
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            //datareader.ShowBytesAtPosition(datareader.offset, 6000);
            //Debug.WriteLine("");
        }





        static void Trial1ZVsFrame00() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\3dskyboxstencil_pcgl_30_vs.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(2);


            datareader.ShowZDataSection(-1);

            Debug.WriteLine("");
            datareader.ShowBytes(2);

            datareader.ShowBytes(2);
            datareader.ShowBytes(20);
            datareader.ShowBytes(2);
            datareader.ShowBytes(240, 24);
            // datareader.ShowBytes(200, 20);
            datareader.ShowBytes(2);
            datareader.ShowBytes(20);



            datareader.ShowBytes(2);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);



            datareader.ShowZSourceSection(0);
            //return;
            datareader.ShowZSourceSection(1);
            datareader.ShowZSourceSection(2);
            datareader.ShowZSourceSection(3);
            datareader.ShowZSourceSection(4);
            datareader.ShowZSourceSection(5);
            datareader.ShowZSourceSection(6);
            datareader.ShowZSourceSection(7);
            datareader.ShowZSourceSection(8);
            datareader.ShowZSourceSection(9);




            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(4);
            datareader.ShowBytes(160, 16);




            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 400);
            Debug.WriteLine("");



        }




        static void Trial1ZFrame05() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(5);
            // datareader.DisableOutput = true;

            datareader.ShowZDataSection(-1);
            datareader.ShowByteCount("ZFrame header");
            datareader.ShowBytes(2);
            int nr_arguments = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);


            datareader.ShowMurmurString();
            datareader.ShowBytes(3);
            datareader.ShowMurmurString();
            datareader.ShowBytes(8);
            datareader.ShowMurmurString();
            int dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);

            if (nr_arguments == 6) {
                datareader.ShowMurmurString();
                // datareader.ShowZDataDelim();
                datareader.ShowBytes(11);
            }
            // Debug.WriteLine("");


            datareader.ShowBytesNoLineBreak(2);
            int nr_of_blocks = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            datareader.TabPrintComment($"nr of blocks ({nr_of_blocks})");
            for (int i = 0; i < nr_of_blocks; i++) {
                datareader.ShowZDataSection(i);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            int blockEntryCount = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            int blocksRegistered = datareader.ShowZBlocksRegistered(blockEntryCount);
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");


            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            Debug.WriteLine("");

            for (int i = 0; i < blocksRegistered; i++) {
                datareader.ShowZSourceSection(i);
            }


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.ShowZAllEndBlocks();
            datareader.EndOfFile();


            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            //datareader.ShowBytesAtPosition(datareader.offset, 1000);
            //Debug.WriteLine("");





        }



        static void Trial1ZFrame02() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(2);
            datareader.ShowZDataSection(-1);
            datareader.ShowZFrameHeader();
            datareader.ShowZDataDelim();
            for (int i = 0; i < 64; i++) {
                datareader.ShowZDataSection(i);
            }


            // this section may be showing what blocks are described in this file
            Debug.WriteLine("");
            datareader.ShowByteCount();
            int datasize = (int)datareader.ReadUInt16AtPosition(datareader.offset);
            datareader.ShowBytes(2);
            datareader.ShowBytes(datasize * 2);
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");



            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);



            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(1);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");



            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"nr end blocks ({datareader.ReadIntAtPosition(datareader.offset - 4)})");

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            datareader.EndOfFile();


        }







        static void Trial1ZFrame01() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(1);


            datareader.ShowZDataSection(-1);
            datareader.ShowZFrameHeader();
            datareader.ShowZDataDelim();
            for (int i = 0; i < 64; i++) {
                datareader.ShowZDataSection(i);
            }

            // this section may be showing what blocks are described in this file
            Debug.WriteLine("");
            datareader.ShowByteCount();
            uint datasize = datareader.ReadUInt16AtPosition(datareader.offset);
            datareader.ShowBytes(2);

            for (int i = 0; i < datasize; i++) {
                datareader.ShowBytesNoLineBreak(2);
                datareader.TabPrintComment($"{i}");
            }


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");


            // int datasize = datareader.ReadIntAtPosition(datareader.offset);
            // datareader.ShowBytes(4);
            // datareader.ShowBytes(datasize * 2);
            // Debug.WriteLine("");




            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount("glsl source offsets");
            datareader.PrintIntWithValue();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment("always 3");
            datareader.PrintIntWithValue();

            int sourceOffset = datareader.offset + datareader.ReadIntAtPosition(datareader.offset - 4);


            Debug.WriteLine("");
            datareader.ShowByteCount("SOURCE[0]");
            datareader.ShowBytes(100);
            Debug.WriteLine($"// ... ({sourceOffset - datareader.offset} bytes of data not shown)");
            datareader.offset = sourceOffset;
            Debug.WriteLine("");


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(1);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(2);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");


            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(3);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"nr end blocks ({datareader.ReadIntAtPosition(datareader.offset - 4)})");

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);



            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 1000);
            Debug.WriteLine("");




        }





        // ZFRAME0
        static void Trial1ZFrame00() {
            // new ShaderReader(ANALYSIS_DIR1 + @"\glow_output_pcgl_30_ps.vcs");
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");

            DataReader datareader = shaderReader.getZframeDataReader(0);
            datareader.ShowByteCount();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            Debug.WriteLine("");


            // uint count = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowByteCount();
            for (int i = 0; i < 8; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 2; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 4; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 4; i++) {
                datareader.ShowBytes(4);
            }


            Debug.WriteLine("");
            datareader.ShowByteCount(); // [88] 06 00 pattern
            datareader.ShowBytes(2);

            Debug.WriteLine("");
            datareader.ShowMurmurString();


            uint someval = datareader.ReadUInt16AtPosition(datareader.offset);
            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(3);
            datareader.TabPrintComment($"{someval} (?)    (14,74)"); // [116] 0E 4A 00

            Debug.WriteLine("");
            datareader.ShowMurmurString(); // SupportsMappingDimensions

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3); // [149] 09 FF FF
            datareader.ShowBytes(4);
            datareader.ShowBytes(1);

            Debug.WriteLine("");
            datareader.ShowMurmurString(); // DoNotReflect

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3);

            int dynExpLen0 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen0}");

            string dynExpStr = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen0);
            string dynExp0 = getDynamicExpression(dynExpStr);
            Debug.WriteLine($"// {dynExp0}");
            datareader.ShowBytes(dynExpLen0);


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowMurmurString(); // DoNotCastShadows
            Debug.WriteLine("");


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(3); // 09 FF FF control pattern?
            Debug.WriteLine("");


            int dynExpLen1 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen1}");
            string dynExpStr1 = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen1);
            string dynExp1 = getDynamicExpression(dynExpStr1);
            Debug.WriteLine($"// {dynExp1}");
            datareader.ShowBytes(dynExpLen1);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowMurmurString(); // ShadowsOnly

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3); // [267] 09 FF FF


            int dynExpLen2 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen2}");
            string dynExpStr2 = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen2);
            string dynExp2 = getDynamicExpression(dynExpStr2);
            Debug.WriteLine($"// {dynExp2}");
            datareader.ShowBytes(dynExpLen2);


            Debug.WriteLine("");
            datareader.ShowMurmurString(); // DepthPassBatchID


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(8); // [316] 05 FF FF 00 00 00 00 01
            datareader.ShowBytes(5);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            // datareader.ShowBytes(12, 4);
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(18 * 4, 4);


            Debug.WriteLine("");
            datareader.ShowByteCount(); // 756 null bytes follow
            datareader.ShowBytes(756);


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(128);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.PrintUInt16WithValue();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();

            Debug.WriteLine("");
            datareader.ShowByteCount();
            Debug.WriteLine("// source file");
            datareader.ShowBytes(500);
            Debug.WriteLine("// ...");

            datareader.offset = 9361;


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(16);


            Debug.WriteLine("");
            datareader.ShowByteCount();


            datareader.ShowBytes(2);


            Debug.WriteLine("");
            datareader.ShowBytes(96, 8);



            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 10000);
            Debug.WriteLine("");



        }


        static ParseDynamicExpressionShader myDynParser = null;


        static string getDynamicExpression(string bytesAsString) {
            byte[] databytes = ParseString(bytesAsString);
            if (myDynParser == null) {
                myDynParser = new ParseDynamicExpressionShader();
            }
            myDynParser.ParseExpression(databytes);
            if (myDynParser.errorWhileParsing) {
                // throw new ShaderParserException("error in dynamic expression!");

                Debug.WriteLine(myDynParser.errorMessage);

            }
            return myDynParser.dynamicExpressionResult;
        }




        static byte[] ParseString(String bytestring) {
            string[] tokens = bytestring.Trim().Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }









        static List<string> getAllVsFiles() {
            List<string> vsFiles = new();
            string[] coreFileNames = Directory.GetFiles(ANALYSIS_DIR_CORE);
            foreach (string filepath in coreFileNames) {
                if (filepath.Substring(filepath.Length - 6, 2) == "vs") {
                    vsFiles.Add(filepath);
                }
            }
            string[] nonCoreFileNames = Directory.GetFiles(ANALYSIS_DIR_NOT_CORE);
            foreach (string filepath in nonCoreFileNames) {
                if (filepath.Substring(filepath.Length - 6, 2) == "vs") {
                    vsFiles.Add(filepath);
                }
            }
            return vsFiles;
        }


        static List<string> getAllPsFiles() {
            List<string> psFiles = new();
            string[] coreFileNames = Directory.GetFiles(ANALYSIS_DIR_CORE);
            foreach (string filepath in coreFileNames) {
                if (filepath.Substring(filepath.Length - 6, 2) == "ps") {
                    psFiles.Add(filepath);
                }
            }
            string[] nonCoreFileNames = Directory.GetFiles(ANALYSIS_DIR_NOT_CORE);
            foreach (string filepath in nonCoreFileNames) {
                if (filepath.Substring(filepath.Length - 6, 2) == "ps") {
                    psFiles.Add(filepath);
                }
            }
            return psFiles;
        }



        static List<string> getAllZFrameFiles() {
            List<string> vcsFiles = new();
            string[] coreFileNames = Directory.GetFiles(ANALYSIS_DIR_CORE);
            foreach (string filepath in coreFileNames) {
                if (filepath.Substring(filepath.Length - 12, 8) == "features") {
                    continue;
                }
                if (filepath.Substring(filepath.Length - 3, 3) == "vcs") {
                    vcsFiles.Add(filepath);
                }
            }
            string[] nonCoreFileNames = Directory.GetFiles(ANALYSIS_DIR_NOT_CORE);
            foreach (string filepath in nonCoreFileNames) {
                if (filepath.Substring(filepath.Length - 12, 8) == "features") {
                    continue;
                }
                if (filepath.Substring(filepath.Length - 3, 3) == "vcs") {
                    vcsFiles.Add(filepath);
                }
            }
            return vcsFiles;

        }




        static void PrintReport() {

            List<int> intvalues = new();
            foreach (int i in collectValuesInt.Keys) {
                intvalues.Add(i);
            }
            intvalues.Sort();
            foreach (int i in intvalues) {
                if (requestCount[0]) {
                    collectValuesInt.TryGetValue(i, out int instanceCount);
                    Debug.WriteLine($"{i,5}        {instanceCount,3}");
                } else {
                    Debug.WriteLine($"{i}");
                }
            }


            List<string> stringvalues = new();
            foreach (string s in collectValuesString.Keys) {
                stringvalues.Add(s);
            }
            stringvalues.Sort();

            foreach (string s in stringvalues) {
                if (requestCount[0]) {
                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Debug.WriteLine($"{s}        {instanceCount,3}");
                    // Debug.WriteLine("");
                } else {
                    Debug.WriteLine($"{s}");
                }
            }
        }





    }


}