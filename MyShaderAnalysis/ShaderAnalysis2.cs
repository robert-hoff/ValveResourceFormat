using MyShaderAnalysis.readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis {
    class ShaderAnalysis2 {

        const string ANALYSIS_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string ANALYSIS_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";

        const string GLOW_OUPUT_PCGL_30_FEATURES = ANALYSIS_DIR_NOT_CORE + @"\glow_output_pcgl_30_features.vcs";
        const string GLOW_OUPUT_PCGL_30_PS = ANALYSIS_DIR_NOT_CORE + @"\glow_output_pcgl_30_ps.vcs";
        const string GLOW_OUPUT_PCGL_30_VS = ANALYSIS_DIR_NOT_CORE + @"\glow_output_pcgl_30_vs.vcs";
        const string GLOW_OUPUT_PCGL_40_PS = ANALYSIS_DIR_NOT_CORE + @"\glow_output_pcgl_40_ps.vcs";
        const string GLOW_OUPUT_PCGL_40_VS = ANALYSIS_DIR_NOT_CORE + @"\glow_output_pcgl_40_vs.vcs";
        const string MULTIBLEND_PCGL_30_PS = ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
        const string MULTIBLEND_PCGL_30_VS = ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
        const string REFRACT_PCGL_30_FEATURES = ANALYSIS_DIR_NOT_CORE + @"\refract_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_FEATURES = ANALYSIS_DIR_NOT_CORE + @"\ui_twotexture_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_PS = ANALYSIS_DIR_NOT_CORE + @"\ui_twotexture_pcgl_30_ps.vcs";
        const string UI_TWOTEXTURE_PCGL_30_VS = ANALYSIS_DIR_NOT_CORE + @"\ui_twotexture_pcgl_30_vs.vcs";


        public static void RunTrials() {
            // Trial3();
            // Trial2();
            // Trial1ScanHeaderData();


            // WriteZframeAnalysisToFile();
            // ScanSizes();

            // Trial1ZVsGrasstileFile();
            ParseAllPsFiles();
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





        static void Trial1ScanHeaderData() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            for (int i = 0; i < 3335; i++) {
                doScan(shaderReader, i);
            }


            // doScan(shaderReader, 3);

        }



        static void doScan(ShaderReader shaderReader, int zframeID) {


            DataReader datareader = shaderReader.getZframeDataReader(zframeID);

            datareader.DisableOutput = true;


            datareader.ShowZDataSection(-1);
            datareader.ShowByteCount("ZFrame header");
            datareader.ShowBytes(2);
            int nr_arguments = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            //if (nr_arguments != 6) {
            //    Debug.WriteLine($"{nr_arguments}    {zframeID}");
            //}

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

            // Debug.WriteLine(nr_of_blocks);
            if (nr_of_blocks != 64) {
                Debug.WriteLine($"{nr_of_blocks}    {zframeID}");
            }



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



        static void ScanSizes() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            for (int i = 0; i < 3335; i++) {
                Trial1ZGen(shaderReader, i);
            }

            // Trial1ZGen(shaderReader, 216);

        }

        static void Trial1ZGen(ShaderReader shaderReader, int zframeId) {
            Debug.WriteLine($"ZFRAME = {zframeId}");
            // return;

            DataReader datareader = shaderReader.getZframeDataReader(zframeId);
            // Debug.WriteLine($"{zframeId}    {datareader.databytes.Length}");
            datareader.DisableOutput = true;
            datareader.ParseAndShowZFrame();
        }







        static void Trial1ZVsGrasstileFile() {

            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR_NOT_CORE + @"\grasstile_pcgl_30_vs.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(0);


            Debug.WriteLine(shaderReader.zFrames.Count);


            datareader.ShowZDataSection(-1);

            Debug.WriteLine("");
            datareader.ShowBytes(2);
            datareader.ShowBytes(2);
            datareader.ShowBytes(24);


            datareader.ShowBytes(2);
            //datareader.ShowBytes(528, 44);
            //datareader.ShowBytes(72, 24);

            datareader.ShowBytes(528, 88);
            datareader.ShowBytes(72, 12);

            datareader.ShowBytes(2);
            datareader.ShowBytes(24);



            datareader.ShowBytes(2);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);



            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 400);
            Debug.WriteLine("");



            datareader.ShowByteCount();

        }




        static void parseAFile() {
            // string filename = ANALYSIS_DIR1 + @"\3dskyboxstencil_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\3dskyboxstencil_pcgl_40_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\bloom_dota_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\blur_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\cables_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\crystal_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\global_lit_simple_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR2 + @"\depth_only_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR2 + @"\error_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\dof_pcgl_30_vs.vcs";
            // string filename = ANALYSIS_DIR1 + @"\hero_pcgl_30_vs.vcs";


            // string filename = ANALYSIS_DIR1 + @"\hero_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR1 + @"\deferred_unlit_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR1 + @"\projected_dota_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR1 + @"\refract_pcgl_40_ps.vcs";
            // string filename = ANALYSIS_DIR1 + @"\crystal_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR2 + @"\error_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR2 + @"\generic_light_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR2 + @"\grasstile_pcgl_41_ps.vcs";
            // string filename = ANALYSIS_DIR2 + @"\panorama_fancyquad_pcgl_50_ps.vcs"; // one massive zframe in this one
            // string filename = ANALYSIS_DIR2 + @"\reconstruct_normals_pcgl_40_ps.vcs";
            string filename = ANALYSIS_DIR_CORE + @"\sky_model_pcgl_40_ps.vcs";

            // RunTrial1ZVsFrame01(filename);

            List<string> psFiles = getAllPsFiles();
            RunTrial1ZVsFrame01(psFiles[7], false);

        }




        static void ParseAllPsFiles() {

            List<string> psFiles = getAllPsFiles();
            foreach (string filenamepath in psFiles) {
                RunTrial1ZVsFrame01(filenamepath, true);
            }
            // RunTrial1ZVsFrame01(psFiles[0]);



            // string filename = ANALYSIS_DIR_NOT_CORE + @"\crystal_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\depth_only_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\grasstile_preview_pcgl_41_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\solidcolor_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filename = ANALYSIS_DIR_CORE + @"\physics_wireframe_pcgl_30_ps.vcs"; // frame 16
            // string filename = ANALYSIS_DIR_CORE + @"\tools_solid_pcgl_30_ps.vcs"; // frame 7
            // string filename = ANALYSIS_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs"; // frame 5
            // string filename = ANALYSIS_DIR_CORE + @"\visualize_nav_pcgl_40_ps.vcs"; // frame 10
            // RunTrial1ZVsFrame01(filename, false);



        }




        static void RunTrial1ZVsFrame01(string filenamepath, bool runningTest) {
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

            if (filetype == -1) {
                throw new ShaderParserException($"unknown file type! {filenamepath}");
            }


            ShaderReader shaderReader = new(filenamepath);


            int min = 0;
            int zcount = shaderReader.zFrames.Count;
            // int max = 11;
            int max = zcount;

            int numberToParse = zcount > max ? max : zcount;

            Debug.WriteLine($"parsing {shaderReader.filepath} frames [{min},{numberToParse})");
            for (int i = min; i < (numberToParse); i++) {
                Trial1ZVsFrame01(shaderReader, i, filetype, runningTest);
            }
        }






        const int PS_FILE = 0;
        const int VS_FILE = 1;
        const int PSRS_FILE = 2;


        static void Trial1ZVsFrame01(ShaderReader shaderReader, int zframeId, int filetype, bool runningTest) {

            DataReader datareader = shaderReader.getZframeDataReader(zframeId);

            if (!runningTest) {
                Debug.WriteLine($"parsing {shaderReader.filepath} ZFRAME{zframeId:d03}");
            }
            if (runningTest) {
                datareader.DisableOutput = true;
            }

            datareader.ShowZDataSection(-1);

            //datareader.ShowBytes(2);
            //datareader.ShowMurmurString();
            //datareader.ShowBytes(8);
            //datareader.ShowMurmurString();
            //datareader.ShowBytes(3);
            //datareader.ShowMurmurString();
            //datareader.ShowBytes(600);
            //return;
            datareader.ShowZFrameHeaderUpdated();





            // only for vs files (ps files don't have this section)
            if (filetype == VS_FILE) {
                int blockSummaries = datareader.ReadInt16AtPosition(datareader.offset);
                if (blockSummaries == 0) {
                    datareader.ShowBytes(3);
                } else {
                    datareader.ShowBytes(2);
                    datareader.ShowBytes(blockSummaries * 2);
                }
            }



            // return;
            datareader.OutputWriteLine("");
            int blockCount = datareader.ReadInt16AtPosition(datareader.offset);
            datareader.ShowByteCount($"nr of blocks ({blockCount})");
            datareader.ShowBytes(2);
            for (int i = 0; i < blockCount; i++) {
                datareader.ShowZDataSection(i);
            }



            // return;
            int blockSummaries2 = datareader.ReadInt16AtPosition(datareader.offset);
            datareader.ShowBytes(2);
            datareader.ShowBytes(blockSummaries2 * 2);
            datareader.OutputWriteLine("");




            // Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("always 00 00", 7);
            datareader.ShowBytesNoLineBreak(1);
            datareader.TabPrintComment("values seen 0,1", 10);

            datareader.ShowBytesNoLineBreak(2); // this may be an integer, but reading it as a short
            uint glslSourceCount = datareader.ReadUInt16AtPosition(datareader.offset - 2);
            datareader.TabPrintComment($"glsl source files ({glslSourceCount})", 7);

            datareader.ShowBytesNoLineBreak(3); // always 00 00 01 ?
            datareader.TabPrintComment("always 00 00 01 ?");
            datareader.OutputWriteLine("");


            for (int i = 0; i < glslSourceCount; i++) {
                datareader.ShowZSourceSection(i);
            }

            if (filetype == VS_FILE) {
                datareader.ShowZAllEndBlocksTypeVs();
            }



            if (filetype == PS_FILE) {

                //  END BLOCKS
                datareader.OutputWriteLine("");
                datareader.ShowByteCount();
                datareader.ShowBytesNoLineBreak(4);
                int nrEndBlocks = datareader.ReadIntAtPosition(datareader.offset - 4);
                datareader.TabPrintComment($"nr of end blocks ({nrEndBlocks})");
                datareader.OutputWriteLine("");

                // datareader.ShowZAllEndBlocks();

                // 18 00 00 00 00 00 00 00
                // 03 00 00 00
                // BB 6B
                // 00 00 01 01
                // 00 00 00 XX          <-- if XX == 01 there is an extra row of 16
                // 00 00 00 00 00 00 00 00

                // 04 04 04 04 04 04 04 04 05 05 05 05 05 05 05 05
                // 00 00 00 00 00 00 00 00 04 04 04 04 04 04 04 04
                // 05 05 05 05 05 05 05 05 00 00 00 00 00 00 00 00
                // 0F 0F 0F 0F 0F 0F 0F 0F 00 00 00 00 00 00 00 00

                for (int i = 0; i < nrEndBlocks; i++) {
                    datareader.OutputWriteLine("");
                    datareader.ShowByteCount($"END-BLOCK[{i}]");
                    datareader.ShowBytes(8);
                    datareader.ShowBytes(4);
                    datareader.ShowBytes(2);
                    // datareader.ShowBytes(4);
                    // datareader.ShowBytes(4);
                    // datareader.ShowBytes(4);
                    // datareader.ShowBytes(4);


                    int checkExtraExtraShortBlock = datareader.ReadByteAtPosition(datareader.offset + 2);
                    int checkExtraShortBlock = datareader.ReadByteAtPosition(datareader.offset + 3);
                    int checkShortBlock = datareader.ReadByteAtPosition(datareader.offset + 4);
                    if (checkExtraExtraShortBlock > 0 && checkExtraShortBlock > 0 && checkShortBlock > 0) {
                        datareader.ShowBytes(5);
                        continue;
                    }
                    if (checkExtraShortBlock > 0 && checkShortBlock > 0) {
                        datareader.ShowBytes(16);
                        datareader.ShowBytes(4);
                        datareader.ShowBytes(1);
                        continue;
                    }
                    if (checkExtraExtraShortBlock > 0 && checkShortBlock > 0) {
                        datareader.ShowBytes(16);
                        datareader.ShowBytes(8);
                        datareader.ShowBytes(1);
                        continue;
                    }
                    if (checkShortBlock > 0) {
                        datareader.ShowBytes(16);
                        datareader.ShowBytes(24);
                        datareader.ShowBytes(1);
                        continue;
                    }




                    int checkLongStartingRow = datareader.ReadByteAtPosition(datareader.offset + 7);
                    if (checkLongStartingRow == 3) {
                        datareader.ShowBytes(36, 36);
                        datareader.ShowBytes(64, 16);
                        continue;
                    }
                    datareader.ShowBytes(16);



                    int checkExtraRow = datareader.ReadByteAtPosition(datareader.offset - 8);
                    if (checkExtraRow > 0) {
                        int checkLongRow = datareader.ReadByteAtPosition(datareader.offset + 7);
                        if (checkLongRow == 2 || checkLongRow == 3 || checkLongRow == 4 || checkLongRow == 7) {
                            datareader.ShowBytes(36, 36);
                        } else {
                            datareader.ShowBytes(16);
                        }
                    }


                    datareader.ShowBytes(64, 16);

                    // variable length depending on the file
                    // datareader.ShowBytes(16);
                    // datareader.ShowBytes(20);
                    // datareader.ShowBytes(32);
                    // datareader.ShowBytes(36, 36);


                    // these bits always the same

                }
            }



            if (runningTest) {
                datareader.EndOfFile();
                return;
            }



            // datareader.ShowByteCount();


            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 6000);
            Debug.WriteLine("");




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






    }


}