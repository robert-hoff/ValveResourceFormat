using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using System;




/*
 * Cables website - looks interesting
 * https://cables.gl/docs/5_writing_ops/shader/shader
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis {


    public class StaticAnalysis {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        // const string OUTPUT_DIR = @"..\..\..\GEN-OUTPUT";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";


        public static void RunTrials() {

            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spring_meteor_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spritecard_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\blur_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\physics_wireframe_pcgl_30_ps.vcs"; // frame 16
            // string filenamepath = PCGL_DIR_CORE + @"\tools_solid_pcgl_30_ps.vcs"; // frame 7
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs"; // frame 5 has an empty glsl file reference
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_nav_pcgl_40_ps.vcs"; // frame 10
            // string filenamepath = PCGL_DIR_CORE + @"\tools_sprite_pcgl_40_gs.vcs"; // gs file
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_psrs.vcs"; // psrs file
            // string filenamepath = PCGL_DIR_CORE + @"\deferred_shading_pcgl_41_ps.vcs"; // interesting zframe content
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\grasstile_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\3dskyboxstencil_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs"; // this file has some very short zframes
            string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";



            // Trial1();
            // FeaturesHeaderFirstFour();
            // MainParamsInFeaturesFiles();
            // SfBlockInspections();
            // AllFiles();
            // ShowFileTriples();
            // CompareTriplesMainParams();
            // SfBlockInspections2();
            // CompareTriplesMainParams2();
            // CompareTriplesMainParams3();





            CompareTriplesMainParams();



            PrintReport();
        }



        static void CompareTriplesMainParams() {
            List<(string, string, string)> triples = GetFeaturesVsPsFileTriples();
            // List<(string, string, string)> triples = new();
            // triples.Add(GetTriple(@$"{PCGL_DIR_CORE}\depth_only_pcgl_30_features.vcs"));
            // triples.Add(GetTriple(@$"{PCGL_DIR_CORE}\visualize_cloth_pcgl_40_features.vcs"));

            foreach (var triple in triples) {

                string title = $"{RemoveBaseDir(triple.Item1)} + vs, ps files";
                Debug.WriteLine($"{title}");
                // new String('-', 5);
                Debug.WriteLine(new string('-', title.Length));
                Dictionary<int, string[]> p = new();
                for (int i = 0; i < 30; i++) {
                    p.Add(i, new string[] { "", "", "" });
                }
                Dictionary<string, int> n = new();
                ShaderFile ftFile = new(triple.Item1);
                ShaderFile vsFile = new(triple.Item2);
                ShaderFile psFile = new(triple.Item3);
                List<string> vs_items = new();
                List<string> ps_items = new();

                int count = 0;
                foreach (var sfBlock in ftFile.sfBlocks) {
                    p[count++][0] = $"{sfBlock.name0}({sfBlock.arg4})";
                }
                foreach (var sfBlock in vsFile.sfBlocks) {
                    vs_items.Add(sfBlock.name0);
                    if (sfBlock.arg4 >= 0) {
                        if (p[sfBlock.arg4][1].Length > 0) throw new ShaderParserException("id not unique");
                        p[sfBlock.arg4][1] = $"{sfBlock.name0}({sfBlock.arg4})";
                    } else {
                        if (sfBlock.arg4 != -1) throw new ShaderParserException("unexpected value");
                        n[sfBlock.name0] = 1;
                    }
                }
                foreach (var sfBlock in psFile.sfBlocks) {
                    ps_items.Add(sfBlock.name0);
                    if (sfBlock.arg4 >= 0) {
                        if (p[sfBlock.arg4][2].Length > 0) throw new ShaderParserException("error!");
                        p[sfBlock.arg4][2] = $"{sfBlock.name0}({sfBlock.arg4})";
                    } else {
                        if (sfBlock.arg4 != -1) throw new ShaderParserException("unexpected value");
                        int val = n.GetValueOrDefault(sfBlock.name0, 0);
                        n[sfBlock.name0] = val + 2;
                    }
                }



                // print the features main args
                int max_len = 0;
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    max_len = mp.Item1.Length > max_len ? mp.Item1.Length : max_len;
                }
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    Debug.WriteLine($"<span style='color: #3783ed'>{mp.Item1.PadRight(max_len)} {mp.Item2}</span>");
                }


                // print the negative values
                foreach (var item in n) {
                    string e1 = "";
                    string e2 = "";
                    if ((item.Value & 1) > 0) {
                        e2 = $"{item.Key}(-1)";
                    }
                    string e3 = "";
                    if ((item.Value & 2) > 0) {
                        e3 = $"{item.Key}(-1)";
                    }

                    // string e2 = (item.Value & 1) > 0 ? $"{item.Key}(-1)" : "";
                    // string e3 = (item.Value & 2) > 0 ? $"{item.Key}(-1)" : "";
                    string reportStr = $"{e1.PadRight(50)} {e2.PadRight(50)} {e3.PadRight(50)}";
                    Debug.WriteLine($"{reportStr}");
                }
                for (int i = 0; i < 30; i++) {
                    if (!string.IsNullOrEmpty(p[i][0]) || !string.IsNullOrEmpty(p[i][1]) || !string.IsNullOrEmpty(p[i][2])) {
                        string e1 = p[i][0];
                        string e2 = p[i][1];
                        string e3 = p[i][2];
                        int e1len = 0;
                        int e2len = 0;
                        int e3len = 0;
                        if (e1 == "") {
                            e1 = $"<span style='color:#999'>({i})</span>";
                            e1len = e1.Length - 2 - nd(i);
                        }
                        if (e2 == "") {
                            e2 = $"<span style='color:#999'>({i})</span>";
                            e2len = e2.Length - 2 - nd(i);
                        } else {
                        }
                        if (e3 == "") {
                            e3 = $"<span style='color:#999'>({i})</span>";
                            e3len = e3.Length - 2 - nd(i);
                        } else {
                        }
                        string reportStr = $"{e1.PadRight(50 + e1len)} {e2.PadRight(50 + e2len)} {e3.PadRight(50 + e3len)}";
                        Debug.WriteLine($"{reportStr}");
                    }
                }

                int vsmaxIndex = vsFile.GetZFrameCount() - 1;
                long vsMaxId = vsFile.GetZFrameIdByIndex(vsmaxIndex);
                int psmaxIndex = psFile.GetZFrameCount() - 1;
                long psMaxId = psFile.GetZFrameIdByIndex(psmaxIndex);
                // int vsMaxD = Convert.ToString(vsMaxId, 2).Length;
                int vsMaxD = nBinDigits((int) vsMaxId);
                // int psMaxD = Convert.ToString(psMaxId, 2).Length;
                int psMaxD = nBinDigits((int) psMaxId);
                string vsStyle = $"<span style='color:#999; font-weight:bold'>({Convert.ToString(vsMaxId, 2)})</span>";
                string psStyle = $"<span style='color:#999; font-weight:bold'>({Convert.ToString(psMaxId, 2)})</span>";
                string f0 = "";
                string f1 = $"MaxZ-digits = {vsMaxD}  {vsStyle}";
                string f2 = $"MaxZ-digits = {psMaxD}  {psStyle}";
                string reportStr2 = $"{f0.PadRight(50)} " +
                    $"{f1.PadRight(50 + vsStyle.Length - 2 - (vsMaxD==0 ? 1 : vsMaxD))} " +
                    $"{f2.PadRight(50 + psStyle.Length - 2 - (psMaxD==0 ? 1 : psMaxD))}";
                Debug.WriteLine($"{reportStr2}");

                // print the argument count for vs/ps
                int vsArgCount = CountActiveModes(vs_items);
                int psArgCount = CountActiveModes(ps_items);
                string vsArgCountStr = $"VSARGS = {vsArgCount}";
                string psArgCountStr = $"PSARGS = {psArgCount}";
                Debug.WriteLine($"{"",-50} {vsArgCountStr,-50} {psArgCountStr,-50}");


                Debug.WriteLine($"");
                Debug.WriteLine($"");
                Debug.WriteLine($"");
                Debug.WriteLine($"");
            }

        }


        private static int CountActiveModes(List<string> items) {
            int count = 0;
            foreach (string item in items) {
                if (!item.Equals("S_TOOLS_ENABLED")) {
                    count++;
                }
            }

            return count;
        }


        public static int nd(int i) {
            if (i == 0) {
                return 1;
            }
            return (int)(Math.Floor(Math.Log10(i) + 1));
        }

        public static int nBinDigits(int num) {
            if (num == 0) {
                return 0;
            }
            return Convert.ToString(num, 2).Length;
        }


        public static void AddStringValue(SortedDictionary<string, int> register, string val, int fid) {
            int currIterator = register.GetValueOrDefault(val, 0);
            register[val] = currIterator + fid;
        }



        static void PrintStringList(List<string> strList) {
            foreach (var s in strList) {
                Debug.Write($"{s.PadRight(20)}");
            }
            Debug.WriteLine("");
        }



        static void ShowFileTriples() {
            List<(string, string, string)> shadFiles = GetFeaturesVsPsFileTriples();
            foreach (var sFiles in shadFiles) {
                string directory = Path.GetDirectoryName(RemoveBaseDir(sFiles.Item1));
                string file1 = Path.GetFileName(sFiles.Item1);
                string file2 = Path.GetFileName(sFiles.Item2);
                string file3 = Path.GetFileName(sFiles.Item3);
                Debug.WriteLine($"{directory.PadRight(25)} {file1.PadRight(60)} {file2.PadRight(60)} {file3}");
            }


        }


        static void Trial1() {
            string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}\n");
            // DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            // shaderByteAnalysis.SetShortenOutput(false);
            // shaderByteAnalysis.PrintByteAnalysis();


            ShaderFile shaderFile = new(filenamepath);
            DataBlockFeaturesHeader featuresHeader = shaderFile.featuresHeader;
            // ShowMainParams()
        }






        static void AllFiles() {
            List<string> allVcs = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in allVcs) {
                CollectStringValue(RemoveBaseDir(filenamepath));
            }
            PrintReport(showCount: false);
        }


        static void SfBlockInspections2() {
            List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in files) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (var sfBlock in shaderFile.sfBlocks) {
                    CollectStringValue($"{sfBlock.name0} ({sfBlock.arg4})");
                }


            }

        }


        static void SfBlockInspections() {
            List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, 30);

            //List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.vs_file, -1);
            //featuresFiles.AddRange(GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.ps_file, -1));
            //featuresFiles.AddRange(GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.psrs_file, -1));
            //featuresFiles.AddRange(GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.gs_file, -1));

            // List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in featuresFiles) {
                ShaderFile shaderFile = new(filenamepath);
                // Debug.WriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");
                foreach (var sfBlock in shaderFile.sfBlocks) {
                    // Debug.WriteLine($"            {sfBlock.name0.PadRight(35)} {sfBlock.name1.PadRight(35)}");
                    // CollectStringValue($"{sfBlock.name0.Substring(2)}({sfBlock.name0.Substring(0,1)})");


                    CollectStringValue($"{sfBlock.name0.PadRight(35)} {sfBlock.name1.PadRight(35)}");
                }


            }


        }



        static void MainParamsInFeaturesFiles() {
            List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            foreach (string filenamepath in featuresFiles) {
                ShaderFile shaderFile = new(filenamepath);
                DataBlockFeaturesHeader featuresHeader = shaderFile.featuresHeader;
                Debug.WriteLine($"{RemoveBaseDir(filenamepath)}");
                // featuresHeader.ShowMainParams();
                foreach (var parampair in featuresHeader.mainParams) {

                    Debug.WriteLine($"         {parampair.Item1.PadRight(35)} {parampair.Item2.PadRight(35)}");
                    // CollectStringValue($"{parampair.Item1.PadRight(35)} {parampair.Item2.PadRight(35)}");
                    // CollectStringValue($"{parampair.Item2.PadRight(35)}");
                    // CollectStringValue($"{parampair.Item1.PadRight(35)}");
                }

                Debug.WriteLine($"");
            }


            Debug.WriteLine($"{featuresFiles.Count}");


        }



        static void FeaturesHeaderFirstFour() {
            List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, 30);
            foreach (string filenamepath in featuresFiles) {
                ShaderFile shaderFile = new(filenamepath);
                DataBlockFeaturesHeader featuresHeader = shaderFile.featuresHeader;
                //string featuresArgs = $"{RemoveBaseDir(filenamepath).PadRight(80)} {featuresHeader.arg0} " +
                //    $"{featuresHeader.arg1} {featuresHeader.arg2} {featuresHeader.arg3}";

                // string featuresArgs = $"({featuresHeader.arg0} {featuresHeader.arg1} {featuresHeader.arg2} {featuresHeader.arg3})";
                string featuresArgs = $"({featuresHeader.arg4} {featuresHeader.arg5} {featuresHeader.arg6} {featuresHeader.arg7})";
                CollectStringValue(featuresArgs);
            }

            PrintReport();

        }



        static List<(string, string, string)> GetFeaturesVsPsFileTriples() {
            return GetFeaturesVsPsFileTriple(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE);
        }





    }







}









