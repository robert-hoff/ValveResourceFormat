using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
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
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";


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

            // CompareTriplesMainParams($@"{SERVER_OUTPUT_DIR}\SF-names.html", writeFile: true);
            // CompatBlockPresence($@"{SERVER_OUTPUT_DIR}\Compat-block-count.html", writeFile: false);



            // CompatibilityBlocksValuesSeenAtOffset(24);
            // CompatibilityBlocksSurvery($@"{SERVER_OUTPUT_DIR}\CBlock-value-survey.html", writeFile: false);
            // CompatBlockDetails($@"{SERVER_OUTPUT_DIR}\Compat-block-details.html", writeFile: true);
            // CompatBlockDetails($@"{SERVER_OUTPUT_DIR}\Compat-block-details-all.html", writeFile: true);


            // CompatBlockDetailsConcise(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            // CompatBlockConciseAll($@"{SERVER_OUTPUT_DIR}\Compat-blocks-summary.html", writeFile: true);

            // CompatibilityBlocksSurveyIntRange(92, 3);
            // CompatBlockRelationships();


            // slight changes here
            // CompatBlockResolveSfReferences($@"{SERVER_OUTPUT_DIR}\Compat-blocks-resolved-names.html", writeFile: true);

            // multiblend
            CompatBlockResolveSfReferences($@"{SERVER_OUTPUT_DIR}\Compat-blocks-sf-names-multiblend.html", writeFile: true);



            // ShowSfArgumentList();


            PrintReport();
            CloseStreamWriter();
        }



        static void ShowSfArgumentList() {
            ShaderFile shaderFile = new(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");


            for (int i = 0; i < shaderFile.sfBlocks.Count; i++) {
                Debug.WriteLine($"{i}    {shaderFile.sfBlocks[i].name0}");
            }

        }



        /*
         *
         *
         * I believe refer to the SF arguments in the compat-blocks based on the index of their appearance in the vcs files
         * let's try this
         *
         * to build better understanding about the data, I should build up more complete printouts of the files
         * the way to achieve this is to write it into the parser
         *
         *
         * But I'm already building the tables the way they should be!
         *
         */
        static void CompatBlockResolveSfReferences(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("Cblocks", "Compatibility blocks, with resolved symbol names");
            }
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            List<string> allVcsFiles = new();
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs");

            string h0 = "Block";
            string h5 = "params";
            string h1 = "Rule";
            // string h2 = "arg0";
            string h3 = "flags";
            string h4 = "symbols";
            string h6 = "range1";
            string h7 = "range2";
            // string header = $"{h0,-7}{h5,-16}{h1,-6}{h2,-6}{h4,-70}{h6,-8}{h7,-8}";
            string header = $"{h0,-7}{h1,-10}{h5,-16}{h4,-70}{h6,-8}{h7,-8}";
            OutputWriteLine(header);
            OutputWriteLine(new string('-', header.Length));
            foreach (string filenamepath in allVcsFiles) {
                CompatBlockDetailsConcise2(filenamepath);
            }
            // Debug.WriteLine($"{blockCount}");
        }

        static void CompatBlockDetailsConcise2(string filenamepath) {

            ShaderFile shaderFile = new(filenamepath);
            bool newFile = true;

            foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {

                if (newFile) {
                    OutputWriteLine("");
                    OutputWriteLine(GetHtmlLink(filenamepath));
                    // OutputWriteLine(RemoveBaseDir(filenamepath));
                    OutputWriteLine("");
                }
                newFile = false;


                string[] sfNames = new string[cBlock.range0.Length];
                for (int i = 0; i < sfNames.Length; i++) {
                    sfNames[i] = shaderFile.sfBlocks[cBlock.range0[i]].name0;
                }


                string s0 = $"[{cBlock.blockIndex,2}]";
                string s1 = (cBlock.relRule == 1 || cBlock.relRule == 2) ? $"INC({cBlock.relRule})" : $"EXC({cBlock.relRule})";
                // string s2 = $"{cBlock.arg0}";
                string s3 = $"{cBlock.ReadByteFlags()}";
                string s4 = $"{CombineValues(sfNames)}";
                string s5 = $"{CombineValues2(cBlock.range0)}";
                string s6 = $"{CombineValues2(cBlock.range1)}";
                string s7 = $"{CombineValues2(cBlock.range2)}";
                // string blockSummary = $"{s0.PadRight(10)} {s1.PadRight(6)} {s2.PadRight(6)} {s3.PadRight(18)} s4.PadRight(20)} {s5.PadRight(8)} {s6.PadRight(8)}";
                string blockSummary = $"{s0.PadRight(7)}{s1.PadRight(10)}{s5.PadRight(16)}{s4.PadRight(70)}{s6.PadRight(8)}{s7.PadRight(8)}";


                OutputWrite(blockSummary);
                OutputWriteLine("");






            }
        }








        static void CompatBlockRelationships() {
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (var cBlock in shaderFile.compatibilityBlocks) {
                    if (cBlock.range2[1] != cBlock.range0.Length) {
                        Debug.WriteLine($"ERROR!");
                    }

                    if (cBlock.arg0 == 1 && shaderFile.vcsFiletype != FILETYPE.features_file) {
                        Debug.WriteLine($"error!");
                    }

                    if (cBlock.arg0 == 2) {
                        Debug.WriteLine($"{filenamepath}");
                    }


                }
            }
        }



        static void CompatBlockConciseAll(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("Cblock details", "Compatibility blocks summary");
            }
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            //List<string> allVcsFiles = new();
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs");

            string h0 = "Block";
            string h1 = "Rule";
            string h2 = "arg0";
            string h3 = "flags";
            string h4 = "range0";
            string h5 = "range1";
            string h6 = "range2";
            // string header = $"{h0.PadRight(10)} {h1.PadRight(6)} {h2.PadRight(6)} {h3.PadRight(18)} {h4.PadRight(20)} {h5.PadRight(8)} {h6.PadRight(8)}";
            string header = $"{h0.PadRight(7)}{h1.PadRight(6)}{h2.PadRight(6)}{h4.PadRight(16)}{h5.PadRight(8)}{h6.PadRight(8)}";
            OutputWriteLine(header);
            OutputWriteLine(new string('-', header.Length));

            //OutputWrite($"{s0.PadRight(10)} {s1.PadRight(6)} {s2.PadRight(6)} {s3.PadRight(14)} " +
            //    $"{s4.PadRight(20)} {s5.PadRight(8)} {s6.PadRight(8)}");


            foreach (string filenamepath in allVcsFiles) {
                CompatBlockDetailsConcise(filenamepath);
            }
            // Debug.WriteLine($"{blockCount}");
        }




        static int blockCount = 0;

        static void CompatBlockDetailsConcise(string filenamepath) {

            ShaderFile shaderFile = new(filenamepath);

            // OutputWriteLine("");
            // OutputWriteLine($"<a href='../vcs-all/{Path.GetFileName(filenamepath)[0..^4]}-analysis.html'>{RemoveBaseDir(filenamepath)}</a>");
            // OutputWriteLine(new string('-', 100));

            // OutputWriteLine($"{RemoveBaseDir(filenamepath)}");

            bool newFile = true;


            foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                string s0 = $"[{cBlock.blockIndex,2}]";
                string s1 = $"{cBlock.relRule}";
                string s2 = $"{cBlock.arg0}";
                string s3 = $"{cBlock.ReadByteFlags()}";
                string s4 = $"{CombineValues2(cBlock.range0)}";
                string s5 = $"{CombineValues2(cBlock.range1)}";
                string s6 = $"{CombineValues2(cBlock.range2)}";
                // string blockSummary = $"{s0.PadRight(10)} {s1.PadRight(6)} {s2.PadRight(6)} {s3.PadRight(18)} s4.PadRight(20)} {s5.PadRight(8)} {s6.PadRight(8)}";
                string blockSummary = $"{s0.PadRight(7)}{s1.PadRight(6)}{s2.PadRight(6)}{s4.PadRight(16)}{s5.PadRight(8)}{s6.PadRight(8)}";

                OutputWrite($"{blockSummary}");

                if (newFile) {
                    OutputWrite(GetHtmlLink(filenamepath) + " ");
                } else {
                    OutputWrite("");
                }

                if (cBlock.description.Length > 0) {
                    // OutputWriteLine($"{blockSummary}  {cBlock.description.PadRight(70)} {GetHtmlLink(filenamepath)}");
                    OutputWriteLine($"{cBlock.description}");
                } else {
                    OutputWriteLine("");
                }

                newFile = false;
            }
        }



        static void CompatBlockDetails(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("Cblock details", "Cblock details selected files");
            }
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            List<string> allVcsFiles = new();
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs");

            foreach (string filenamepath in allVcsFiles) {
                CompatBlockDetailsSelectedFile(filenamepath);
            }
        }






        static void CompatBlockDetailsSelectedFile(string filenamepath) {

            ShaderFile shaderFile = new(filenamepath);

            OutputWriteLine("");
            OutputWriteLine($"<a href='../vcs-all/{Path.GetFileName(filenamepath)[0..^4]}-analysis.html'>{RemoveBaseDir(filenamepath)}</a>");
            OutputWriteLine(new string('-', 100));
            foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {

                OutputWriteLine($"COMPAT-BLOCK[{cBlock.blockIndex}]");
                OutputWriteLine($"rule = {cBlock.relRule}");
                OutputWriteLine($"arg1 = {cBlock.arg0}");
                // OutputWriteLine($"{CombineValues(cBlock.flags)}");
                OutputWriteLine($"      {cBlock.ReadByteFlags()}");
                OutputWriteLine($"{CombineValues(cBlock.range0)}");
                OutputWriteLine($"{CombineValues(cBlock.range1)}");
                OutputWriteLine($"{CombineValues(cBlock.range2)}");
                OutputWriteLine($"{cBlock.description}");
                OutputWriteLine($"");

            }

        }




        static void CompatibilityBlocksSurveyIntRange(int offset, int count, string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("", "");
            }
            SortedDictionary<string, int> intrange = new();
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            List<string> allVcsFiles = new();
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs");
            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs");
            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");


            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    int[] ints0 = new int[count];
                    for (int i = 0; i < count; i++) {
                        ints0[i] = cBlock.ReadIntegerAtPosition(offset + i * 4);
                    }
                    string intsStr = CombineValues(ints0);
                    int parmCount = intrange.GetValueOrDefault(intsStr, 0);
                    intrange[intsStr] = parmCount + 1;
                }
            }

            foreach (var item in intrange) {
                Debug.WriteLine($"{item.Key,5}                 {item.Value}");
            }

        }




        static void CompatibilityBlocksSurvey(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("CBlock values", "Compatibility block value ranges, bytes 0 to 212");
            }
            List<SortedDictionary<int, int>> values = new();
            SortedDictionary<string, int> byteflags = new();
            for (int i = 0; i < 54; i++) {
                values.Add(new SortedDictionary<int, int>());
            }

            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    for (int i = 0; i < 8; i += 4) {
                        int val = cBlock.ReadIntegerAtPosition(i);
                        int curVal = values[i / 4].GetValueOrDefault(val, 0);
                        values[i / 4][val] = curVal + 1;
                    }
                    {
                        // 16 bytes long, so these are skipped in the next part
                        string val = cBlock.ReadByteFlags();
                        int curVal = byteflags.GetValueOrDefault(val, 0);
                        byteflags[val] = curVal + 1;
                    }
                    for (int i = 24; i < 216; i += 4) {
                        int val = cBlock.ReadIntegerAtPosition(i);
                        int curVal = values[i / 4].GetValueOrDefault(val, 0);
                        values[i / 4][val] = curVal + 1;
                    }
                }
            }
            OutputWriteLine($"  BYTE    VALUES SEEN");
            OutputWriteLine($"  -----   -----------");
            for (int i = 0; i < 2; i++) {
                OutputWriteLine($"  ({i * 4,3})       {CombineValues(values[i])}");
            }
            OutputWriteLine($"  ({8,3})      {CombineValues(byteflags)}");
            for (int i = 6; i < 54; i++) {
                string tab = "";
                if (!values[i].ContainsKey(-1)) {
                    tab = "    ";
                }
                OutputWriteLine($"  ({i * 4,3})   {tab}{CombineValues(values[i])}");
            }
        }




        static void CompatibilityBlocksValuesSeenAtOffset(int offset, string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("", "");
            }
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            SortedDictionary<int, int> values = new();
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    int val = cBlock.ReadIntegerAtPosition(offset);
                    int curVal = values.GetValueOrDefault(val, offset);
                    values[val] = curVal + 1;
                }
            }
            Debug.WriteLine($"{CombineValues(values)}");
        }



        static void CompatibilityBlocksValuesSeenAtOffset0(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("", "");
            }
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            SortedDictionary<int, int> values = new();
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    int val = cBlock.ReadIntegerAtPosition(0);
                    int curVal = values.GetValueOrDefault(val, 0);
                    values[val] = curVal + 1;
                }
            }
            Debug.WriteLine($"{CombineValues(values)}");
        }


        static string CombineValues2(int[] ints0) {
            if (ints0.Length == 0) return $"_";
            string valueString = "";
            foreach (int i in ints0) {
                valueString += $"{i},";
            }
            return $"{valueString[0..^1]}";
        }


        static string CombineValues(int[] ints0) {
            if (ints0.Length == 0) return $"{"()",3}";
            string valueString = "";
            foreach (int i in ints0) {
                valueString += $"{i,3}, ";
            }
            return $"{valueString[0..^2]}";
        }


        static string CombineValues(SortedDictionary<int, int> values) {
            string valueString = "";
            foreach (var item in values) {
                valueString += $"{item.Key}, ";
            }
            return valueString[0..^2];
        }

        static string CombineValues(SortedDictionary<string, int> values) {
            string valueString = "";
            foreach (var item in values) {
                valueString += $"{item.Key}, ";
            }
            return valueString[0..^2];
        }

        static string CombineValues(string[] values) {
            string valueString = "";
            foreach (var item in values) {
                valueString += $"{item}, ";
            }
            return valueString[0..^2];
        }




        static void CompatibilityBlockValuesExample(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("", "");
            }
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);

            CompatibilityBlock block0 = shaderFile.compatibilityBlocks[0];
            Debug.WriteLine($"{block0.ReadIntegerAtPosition(0)}");
            Debug.WriteLine($"{block0.ReadIntegerAtPosition(4)}");
            Debug.WriteLine($"{block0.ReadByteFlags()}");

            for (int i = 24; i <= 215; i += 4) {
                Debug.WriteLine($"{block0.ReadIntegerAtPosition(i)}");
            }

            // if present, the bytes following contain a string decription of the block

        }




        static void CompatBlockPresence(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("CBlock count", "Compatibility block count, all files");
            }
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                OutputWriteLine($"{RemoveBaseDir(filenamepath).PadRight(80)} " +
                    $"nr of compat blocks = {shaderFile.compatibilityBlocks.Count}");
            }
        }





        static void CompareTriplesMainParams(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("SF names", "SF names for vcs, vs, ps triples");
            }


            List<(string, string, string)> triples = GetFeaturesVsPsFileTriples();
            // List<(string, string, string)> triples = new();
            // triples.Add(GetTriple(@$"{PCGL_DIR_CORE}\depth_only_pcgl_30_features.vcs"));
            // triples.Add(GetTriple(@$"{PCGL_DIR_CORE}\visualize_cloth_pcgl_40_features.vcs"));

            foreach (var triple in triples) {

                string title = $"{RemoveBaseDir(triple.Item1)} + vs, ps files";
                OutputWriteLine($"{title}");
                // new String('-', 5);
                OutputWriteLine(new string('-', title.Length));
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


                // features-header
                int ftHeaderNrArguments = ftFile.featuresHeader.mainParams.Count;
                OutputWriteLine($"<span style='color: #3783ed'>Arguments in {Path.GetFileName(triple.Item1)} header ({ftHeaderNrArguments})</span>");
                // print the features main args
                int max_len = 0;
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    max_len = mp.Item1.Length > max_len ? mp.Item1.Length : max_len;
                }
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    OutputWriteLine($"<span style='color: #3783ed'>{mp.Item1.PadRight(max_len)} {mp.Item2}</span>");
                }
                OutputWriteLine("");


                OutputWriteLine($"{"FEATURES-FILE".PadRight(50)} {"VS_FILE".PadRight(50)} {"PS-FILE".PadRight(50)}");
                OutputWriteLine($"{"-------------".PadRight(50)} {"-------".PadRight(50)} {"-------".PadRight(50)}");


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
                    OutputWriteLine($"{reportStr}");
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
                        OutputWriteLine($"{reportStr}");
                    }
                }

                int vsmaxIndex = vsFile.GetZFrameCount() - 1;
                long vsMaxId = vsFile.GetZFrameIdByIndex(vsmaxIndex);
                int psmaxIndex = psFile.GetZFrameCount() - 1;
                long psMaxId = psFile.GetZFrameIdByIndex(psmaxIndex);
                // int vsMaxD = Convert.ToString(vsMaxId, 2).Length;
                int vsMaxD = nBinDigits((int)vsMaxId);
                // int psMaxD = Convert.ToString(psMaxId, 2).Length;
                int psMaxD = nBinDigits((int)psMaxId);
                string vsStyle = $"<span style='color:#999; font-weight:bold'>({Convert.ToString(vsMaxId, 2)})</span>";
                string psStyle = $"<span style='color:#999; font-weight:bold'>({Convert.ToString(psMaxId, 2)})</span>";
                string f0 = "";
                string f1 = $"MaxZ-digits = {vsMaxD}  {vsStyle}";
                string f2 = $"MaxZ-digits = {psMaxD}  {psStyle}";
                string reportStr2 = $"{f0.PadRight(50)} " +
                    $"{f1.PadRight(50 + vsStyle.Length - 2 - (vsMaxD == 0 ? 1 : vsMaxD))} " +
                    $"{f2.PadRight(50 + psStyle.Length - 2 - (psMaxD == 0 ? 1 : psMaxD))}";
                OutputWriteLine($"{reportStr2}");

                // print the argument count for vs/ps
                // int vsArgCount = CountActiveModes(vs_items);
                // int psArgCount = CountActiveModes(ps_items);
                int vsArgCount = vs_items.Count;
                int psArgCount = ps_items.Count;
                string vsArgCountStr = $"VSARGS = {vsArgCount}";
                string psArgCountStr = $"PSARGS = {psArgCount}";
                OutputWriteLine($"{"",-50} {vsArgCountStr,-50} {psArgCountStr,-50}");


                OutputWriteLine($"");
                OutputWriteLine($"");
                OutputWriteLine($"");
                OutputWriteLine($"");
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





        private static StreamWriter sw = null;
        private static bool DisableOutput = false;
        private static bool WriteAsHtml = false;

        private static void WriteHtmlFile(string htmlTitle, string htmlHeader) {
            if (sw == null) {
                throw new ShaderParserException("StreamWriter needs to be setup before calling this");
            }
            WriteAsHtml = true;
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
        }


        private static void ConfigureOutputFile(string filepathname, bool disableOutput = false) {
            DisableOutput = disableOutput;
            Debug.WriteLine($"writing to {filepathname}");
            sw = new StreamWriter(filepathname);
        }
        private static void CloseStreamWriter() {
            if (WriteAsHtml) {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null) {
                sw.Close();
            }
        }

        public static void OutputWrite(string text) {
            if (!DisableOutput) {
                Debug.Write(text);
            }
            if (sw != null) {
                sw.Write(text);
            }
        }
        public static void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }







}









