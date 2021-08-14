using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
#pragma warning disable IDE1006 // Naming Styles



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
        const string SERVER_BASEDIR = @"Z:\dev\www\vcs.codecreation.dev";
        const string OUTPUT_SUB_DIR = @"\GEN-output";


        // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
        // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
        // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
        // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
        // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs";
        // string filenamepath = PCGL_DIR_NOT_CORE + @"\spring_meteor_pcgl_30_vs.vcs";
        // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_features.vcs";
        // string filenamepath = PCGL_DIR_NOT_CORE + @"\spritecard_pcgl_30_features.vcs";


        public static void RunTrials() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";

            // -- writes a useful summary for every file (pretty long process)
            // Don't overwrite multiblend! (the zframes are stored in a different directory)
            // FileSummaryAllFiles();

            // - prints a single page summary and links to all the files produced with FileSummaryAllFiles()
            // BlockCountSurvery($@"{SERVER_OUTPUT_DIR}\file-overview.html", writeFile: true);
            // FileBlockCount(filenamepath);
            // WriteSfArgumentsAllFiles($"{SERVER_OUTPUT_DIR}/testrun.html", writeFile: true);



            // FileSummarySingleFile();
            // ZFramePrintout();




            // -- these methods aren't particularly valuable (should upgrade and remove)
            // Trial1();
            // FeaturesHeaderFirstFour();
            // MainParamsInFeaturesFiles();
            // SfBlockInspections();
            // AllFiles();
            // ShowFileTriples();
            // SfBlockInspections2();
            // CompatBlockDetailsConcise(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");



            // -- setting up comprehensive summary for particular file (NEEDS UPDATE)
            FileSummaryMultiblendPs();




            // FullFileSummary(@$"{PCGL_DIR_NOT_CORE}\water_dota_pcgl_30_features.vcs", "water", $@"{SERVER_OUTPUT_DIR}\summary-water.html", writeFile: false);


            // -- useful methods
            // CompareTriplesMainParams($@"{SERVER_OUTPUT_DIR}\SF-names.html", writeFile: true);
            // CompatBlockPresence($@"{SERVER_OUTPUT_DIR}\Compat-block-count.html", writeFile: false);

            // CompatibilityBlocksValuesSeenAtOffset(24);
            // CompatibilityBlocksSurvery($@"{SERVER_OUTPUT_DIR}\CBlock-value-survey.html", writeFile: false);
            // CompatBlockDetails($@"{SERVER_OUTPUT_DIR}\Compat-block-details.html", writeFile: true);
            // CompatBlockDetails($@"{SERVER_OUTPUT_DIR}\Compat-block-details-all.html", writeFile: true);

            // CompatBlockDetailsConcise2(PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_ps.vcs", showLink: false);
            // CompatBlockConciseAll($@"{SERVER_OUTPUT_DIR}\Compat-blocks-summary.html", writeFile: true);
            // CompatibilityBlocksSurveyIntRange(92, 3);
            // CompatBlockRelationships();

            // slight changes here
            // CompatBlockResolveSfReferences($@"{SERVER_OUTPUT_DIR}\Compat-blocks-resolved-names.html", writeFile: true);

            // multiblend
            // CompatBlockResolveSfReferences($@"{SERVER_OUTPUT_DIR}\Compat-blocks-sf-names-multiblend.html", writeFile: true);

            // CompatBlockResolveSfReferences();
            // ShowSfArgumentList(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            // ShowDBlock(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            // DBlockSurvey();
            // ShowUnknownBlock();
            // UnknownBlocksSurvey();
            // ShowUnknownBlockConcise();
            // ScanForValueSfBlocks();
            // TestHelperMethods();


            // TestRun();
            // CompatRuleKeyDescriptionSurvey();
            // CompatRuleKeyValuesAnalysis();
            // DBlockRuleKeyDescriptionSurvey();
            // DBlockRuleKeyValuesAnalysis();

            PrintReport();
            CloseStreamWriter();
        }





        static void DBlockRuleKeyValuesAnalysis() {
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.any, 30);
            foreach (string vcsFilenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                foreach (UnknownBlock uknBlock in shaderFile.unknownBlocks) {

                    // check on the rules that have range2 = (0,1)
                    //if (uknBlock.range2[0] == 0) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] { 8, 4, 7, 5 })}" +
                    //        $"{shaderFile.dBlocks[0].name0,-33}");
                    //}

                    //if (uknBlock.relRule == 2 && uknBlock.AllFlagsAre3()) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] {8, 4, 7, 5})}" +
                    //        $"{shaderFile.dBlocks[0].name0,-33} {shaderFile.dBlocks[1].name0}");
                    //}

                    //if (uknBlock.relRule == 3 && uknBlock.AllFlagsAre3() && uknBlock.flags.Length>=2) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-44} {uknBlock.GetConciseDescription(new int[] {8, 4, 12, 5})} " +
                    //        $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    //}

                    //if (uknBlock.relRule == 2 && uknBlock.flags.Length==2 && uknBlock.flags[1]==2 && uknBlock.range1.Length==1) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] {8, 4, 12, 5})} " +
                    //        $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    //}

                    //if (uknBlock.range1.Length == 1 && uknBlock.range1[0] == 0) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] {8, 4, 12, 5})} " +
                    //        $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    //}

                    //if (uknBlock.range1.Length > 1) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] {8, 4, 12, 5})} " +
                    //        $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    //}

                    //if (uknBlock.relRule == 2 && uknBlock.range1.Length == 0 && uknBlock.flags.Length >= 3) {
                    //    Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] {8, 4, 12, 5})} " +
                    //        $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    //}

                    if (uknBlock.arg1 > -1) {
                        Debug.WriteLine($"{ShortHandName(vcsFilenamepath)[1..],-55} {uknBlock.GetConciseDescription(new int[] { 8, 4, 12, 5 })} " +
                            $"{uknBlock.GetResolvedNames(shaderFile.sfBlocks, shaderFile.dBlocks)}");
                    }


                }
            }
        }



        static void DBlockRuleKeyDescriptionSurvey() {
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string vcsFilenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                foreach (UnknownBlock unkBlock in shaderFile.unknownBlocks) {
                    string relRuleKeyDesciption = $"{unkBlock.RelRuleDescribe(),-10} {CombineValues2(unkBlock.range1),-8} " +
                        $"{CombineValues2(unkBlock.flags, includeParenth: true),-15} {CombineValues2(unkBlock.range2)}";
                    CollectStringValue(relRuleKeyDesciption);
                }
            }
        }


        static void CompatRuleKeyValuesAnalysis() {
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.any, 30);
            foreach (string vcsFilenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    if (cBlock.range2[0] == 0) {
                        Debug.Write($"{ShortHandName(vcsFilenamepath),-50}");
                        Debug.WriteLine($"nr arguments {cBlock.range0.Length} {shaderFile.sfBlocks[cBlock.range0[0]].name0}");
                    }
                }
            }
        }


        static void CompatRuleKeyDescriptionSurvey() {
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string vcsFilenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {
                    string relRuleKeyDesciption = $"{cBlock.RelRuleDescribe(),-10} {CombineValues2(cBlock.range1),-7} {CombineValues2(cBlock.range2)}";
                    CollectStringValue(relRuleKeyDesciption);
                }
            }
        }






        static void TestRun() {
            // string targetFile = @$"{PCGL_DIR_CORE}\depth_only_pcgl_40_vs.vcs";
            // UnknownBlockConcise(targetFile, showLink: false);
            FileSummarySingleFile();
        }



        /*
         * This method just prints the zframe IDs for a given file,
         * I wrote it mainly to help investigating how the zframes are generated.
         *
         *
         */
        static void ZFramePrintout() {
            string filenamepath = @$"{PCGL_DIR_NOT_CORE}\water_dota_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);

            foreach (var item in shaderFile.zframesLookup) {
                // Debug.WriteLine($"{item.Key:x04}");

                string binaryString = Convert.ToString(item.Key, 2).PadLeft(12, '0');
                // Debug.WriteLine($"{binaryString}");

                string spacedOutBinary = "";
                foreach (var item2 in binaryString.ToCharArray()) {
                    spacedOutBinary += $"{item2}    ";
                }
                spacedOutBinary = spacedOutBinary.Trim();


                // Debug.WriteLine($"{Convert.ToString(item.Key, 2).PadLeft(12, '0')} {item.Key,5}      {item.Key:x04}       {spacedOutBinary}");
                Debug.WriteLine($"{Convert.ToString(item.Key, 2).PadLeft(12, '0')}");


            }


        }



        static string GetSFSummaryLink(string filenamepath) {
            // FileTripe fileTriple = FileTriple.GetTripleIfExists()
            FILETYPE vcsFiletype = GetVcsFileType(filenamepath);
            if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.ps_file) {
                ARCHIVE archive = DetermineArchiveType(filenamepath);
                FileTriple fileTriple = FileTriple.GetTripleIfExists(archive, $"{filenamepath[0..^6]}features.vcs");
                if (fileTriple == null) {
                    return "";
                }
                if (vcsFiletype == FILETYPE.vs_file) {
                    return $"<a href='{fileTriple.vsFile.GetSummariesPath()}'>det.</a>";
                }
                if (vcsFiletype == FILETYPE.ps_file) {
                    return $"<a href='{fileTriple.psFile.GetSummariesPath()}'>det.</a>";
                }
            }
            return "";
        }




        static void FileSummarySingleFile() {
            // List<(string, string, string)> triples = new();
            // FileTriple triple = new(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotacore_pcgl, "visualize_cloth_pcgl_40_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotacore_pcgl, "depth_only_pcgl_40_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotacore_pcgl, "convolve_environment_map_pcgl_41_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotacore_pcgl, "apply_fog_pcgl_40_features.vcs");
            FileTriple triple = new(ARCHIVE.dotacore_pcgl, "blur_pcgl_30_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotagame_pcgl, "water_dota_pcgl_30_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_features.vcs");
            // FileTriple triple = new(ARCHIVE.dotagame_pcgl, "spritecard_pcgl_30_features.vcs");


            WriteVsPsFileSummary(triple, FILETYPE.ps_file);
        }



        /*
         *
         */
        static void FileSummaryAllFiles() {
            List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, -1);
            foreach (var triple in triples) {
                if (triple.psFile.filename.Equals("multiblend_pcgl_30_ps.vcs")) {
                    continue;
                }
                WriteVsPsFileSummary(triple, FILETYPE.vs_file, disableOutput: true);
                CloseStreamWriter();
                WriteVsPsFileSummary(triple, FILETYPE.ps_file, disableOutput: true);
                CloseStreamWriter();
            }
            swWriterAlreadyClosed = true;
        }


        static void WriteVsPsFileSummary(FileTriple fileTriple, FILETYPE targetFileType, bool disableOutput = false) {
            if (targetFileType != FILETYPE.vs_file && targetFileType != FILETYPE.ps_file) {
                throw new ShaderParserException("need to target either vs or ps file");
            }
            FileTokens targetFile = targetFileType == FILETYPE.vs_file ? fileTriple.vsFile : fileTriple.psFile;
            string htmlTitle = $"{targetFile.namelabel}({targetFile.vcstoken})";
            string outputNamepath = targetFile.GetServerFilePath("summary", createDirs: true);
            FileSummaryVsPSFile(fileTriple, targetFileType, htmlTitle, outputNamepath, writeFile: true, disableOutput);
        }



        static void ScanForValueSfBlocks() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\water_dota_pcgl_30_ps.vcs";

            List<string> result = new();

            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, 30);
            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (var sfBlock in shaderFile.sfBlocks) {
                    // Debug.WriteLine($"{sfBlock.arg3}");
                    // if (sfBlock.arg2 != 1 && sfBlock.arg3 != 0) {


                    // if (sfBlock.arg2 == 2) {
                    // if (sfBlock.arg2 == 3) {
                    // if (sfBlock.arg2 == 0) {
                    // if (sfBlock.arg2 == 5) {
                    // if (sfBlock.arg3 == 2) {
                    // if (sfBlock.arg3 == 4) {
                    if (sfBlock.arg3 == 1) {
                        result.Add($"{sfBlock.arg0,3} {sfBlock.arg2,3} {sfBlock.arg3,3} {sfBlock.arg4,3}     " +
                            $"{sfBlock.name0,-25} {ShortHandName(filenamepath)}");
                    }



                    //if (sfBlock.arg0 == 1) {
                    //    Debug.Write($"{sfBlock.arg0} {sfBlock.arg1} {sfBlock.arg2} {sfBlock.arg3}    ");
                    //    Debug.WriteLine($"{sfBlock.name0,-25}    {RemoveBaseDir(filenamepath)}");
                    //}


                    // Debug.WriteLine($"{sfBlock.arg0,3} {sfBlock.arg2,3} {sfBlock.arg3,3} {sfBlock.arg4,3}        {sfBlock.name0}");

                }
            }

            // result.Sort();
            foreach (string item in result) {
                Debug.WriteLine($"{item}");
            }

        }



        static void BlockCountSurvery(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("File", "File summary");
            }
            string fH = "File";
            string sfH = "SF blocks";
            string link = "";
            string cH = "compat";
            string dH = "Dblocks";
            string uH = "DRules";
            string pH = "params";
            string mH = "mmaps";
            string bH = "buffer";
            string sH = "symbols";
            string zH = "zframes";
            const int pad = 8;
            string header = $"{fH,-55}{link,3}{sfH,pad}{cH,pad}{dH,pad}{uH,pad}{pH,pad}{mH,pad}{bH,pad}{sH,pad}{zH,pad}";
            OutputWriteLine($"{header}");
            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in allVcsFiles) {
                FileBlockCount(filenamepath);
            }
        }

        static void FileBlockCount(string filenamepath) {
            ShaderFile shaderFile = new(filenamepath);
            int sfCount = shaderFile.sfBlocks.Count;
            int cCount = shaderFile.compatibilityBlocks.Count;
            int dCount = shaderFile.dBlocks.Count;
            int uCount = shaderFile.unknownBlocks.Count;
            int pCount = shaderFile.paramBlocks.Count;
            int mCount = shaderFile.mipmapBlocks.Count;
            int bCount = shaderFile.bufferBlocks.Count;
            int sCount = shaderFile.symbolBlocks.Count;
            int zCount = shaderFile.zframesLookup.Count;
            const int pad = 8;
            //OutputWriteLine($"{RemoveBaseDir(shaderFile.filenamepath),-70}{sfCount,pad}{cCount,pad}{dCount,pad}" +
            //    $"{uCount,pad}{pCount,pad}{mCount,pad}{bCount,pad}{sCount,pad}{zCount,pad}");

            int filepadlength = 55 - Path.GetFileName(filenamepath).Length;
            if (filepadlength < 0) filepadlength = 0;
            // string empty = "";
            string detLink = GetSFSummaryLink(filenamepath);
            const int linkLength = 4; // "det."

            OutputWriteLine($"{GetHtmlLink(filenamepath)}{new string(' ', filepadlength)}{detLink,linkLength}" +
                $"{sfCount,pad}{cCount,pad}{dCount,pad}" +
                $"{uCount,pad}{pCount,pad}{mCount,pad}{bCount,pad}{sCount,pad}{zCount,pad}");
        }



        static void ShowUnknownBlockConcise() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";

            // string filenamepath = PCGL_DIR_NOT_CORE + @"\global_lit_simple_pcgl_30_features.vcs"; // 0 d-blocks
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\global_lit_simple_pcgl_30_vs.vcs";
            string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";

            UnknownBlockConcise(filenamepath);

        }


        static void UnknownBlockConcise(string filenamepath, bool showLink = true) {
            ShaderFile shaderFile = new(filenamepath);
            bool newFile = true;

            foreach (UnknownBlock uBlock in shaderFile.unknownBlocks) {

                if (newFile) {
                    OutputWriteLine("D-BLOCK RULES");
                    if (showLink) {
                        OutputWriteLine(GetHtmlLink(filenamepath));
                    }
                    // OutputWriteLine(RemoveBaseDir(filenamepath));
                    // OutputWriteLine("");
                }
                newFile = false;

                //string[] sfNames = new string[uBlock.range0.Length];
                //for (int i = 0; i < sfNames.Length; i++) {
                //    sfNames[i] = shaderFile.sfBlocks[uBlock.range0[i]].name0;
                //}

                string[] uknNames = new string[uBlock.flags.Length];
                for (int i = 0; i < uknNames.Length; i++) {
                    if (uBlock.flags[i] == 3) {
                        uknNames[i] = shaderFile.dBlocks[uBlock.range0[i]].name0;
                        continue;
                    }
                    if (uBlock.flags[i] == 2) {
                        uknNames[i] = shaderFile.sfBlocks[uBlock.range0[i]].name0;
                        continue;
                    }
                    throw new ShaderParserException($"unknown flag value {uBlock.flags[i]}");
                }

                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(uknNames, BL);
                string s0 = $"[{uBlock.blockIndex,2}]";
                string s1 = (uBlock.relRule == 1 || uBlock.relRule == 2) ? $"INC({uBlock.relRule})" : $"EXC({uBlock.relRule})";
                // string s2 = $"{cBlock.arg0}";
                string s3 = $"{uBlock.ReadByteFlags()}";
                // string s4 = $"{CombineValues(uknNames)}";
                string s4 = $"{breakNames[0]}";
                // string s4 = "NAMES HERE";
                string s5 = $"{CombineValues2(uBlock.range0)}";
                string s6 = $"{CombineValues2(uBlock.range1)}";
                string s7 = $"{CombineValues2(uBlock.range2)}";
                // string blockSummary = $"{s0.PadRight(10)} {s1.PadRight(6)} {s2.PadRight(6)} {s3.PadRight(18)} s4.PadRight(20)} {s5.PadRight(8)} {s6.PadRight(8)}";
                string blockSummary = $"{s0,-7}{s1,-10}{s3,-15}{s5,-16}{s4,-BL}{s6,-10}{s7,-8}";
                for (int i = 1; i < breakNames.Length; i++) {
                    blockSummary += $"\n{(""),-7}{(""),-10}{(""),-15}{(""),-16}{breakNames[i],-BL}";
                }
                OutputWrite(blockSummary);
                OutputWriteLine("");
            }

            OutputWriteLine("");
        }





        static void UnknownBlocksSurvey(string outputFilenamepath = null, bool writeFile = true) {
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
                foreach (UnknownBlock uBlock in shaderFile.unknownBlocks) {
                    for (int i = 0; i < 8; i += 4) {
                        int val = uBlock.ReadIntegerAtPosition(i);
                        int curVal = values[i / 4].GetValueOrDefault(val, 0);
                        values[i / 4][val] = curVal + 1;
                    }
                    {
                        // 16 bytes long, so these are skipped in the next part
                        string val = uBlock.ReadByteFlags();
                        int curVal = byteflags.GetValueOrDefault(val, 0);
                        byteflags[val] = curVal + 1;
                    }
                    for (int i = 24; i < 216; i += 4) {
                        int val = uBlock.ReadIntegerAtPosition(i);
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





        static void ShowUnknownBlock() {
            // ShaderFile shaderFile = new(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");
            ShaderFile shaderFile = new(PCGL_DIR_CORE + @"\depth_only_pcgl_40_vs.vcs");
            Debug.WriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");

            foreach (UnknownBlock uBlock in shaderFile.unknownBlocks) {
                OutputWriteLine($"COMPAT-BLOCK[{uBlock.blockIndex}]");
                OutputWriteLine($"rule = {uBlock.relRule}");
                OutputWriteLine($"arg1 = {uBlock.arg0}");
                // OutputWriteLine($"{CombineValues(cBlock.flags)}");
                OutputWriteLine($"      {CombineValues(uBlock.flags)}");
                OutputWriteLine($"{CombineValues(uBlock.range0)}");
                OutputWriteLine($"{CombineValues(uBlock.range1)}");
                OutputWriteLine($"{CombineValues(uBlock.range2)}");
                OutputWriteLine($"{uBlock.description}");
                OutputWriteLine($"");
            }

        }





        static void DBlockSurvey(string outputFilenamepath = null, bool writeFile = true) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("DBlock survery", "DBlocks - values seen");
            }

            int ARG_OFFSET = 128;

            List<SortedDictionary<int, int>> values = new();
            for (int i = 0; i < 6; i++) {
                values.Add(new SortedDictionary<int, int>());
            }

            List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            // List<string> allVcsFiles = new();
            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");

            foreach (string filenamepath in allVcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (DBlock dBlock in shaderFile.dBlocks) {
                    for (int i = ARG_OFFSET; i < ARG_OFFSET + 24; i += 4) {
                        int val = dBlock.ReadIntegerAtPosition(i);
                        int curVal = values[(i - ARG_OFFSET) / 4].GetValueOrDefault(val, 0);
                        values[(i - ARG_OFFSET) / 4][val] = curVal + 1;

                    }
                }
            }

            OutputWriteLine($"  BYTE    VALUES SEEN");
            OutputWriteLine($"  -----   -----------");
            for (int i = 0; i < 6; i++) {
                OutputWriteLine($"  ({i * 4,3})       {CombineValues(values[i])}");
            }
            //OutputWriteLine($"  ({8,3})      {CombineValues(byteflags)}");
            //for (int i = 6; i < 54; i++) {
            //    string tab = "";
            //    if (!values[i].ContainsKey(-1)) {
            //        tab = "    ";
            //    }
            //    OutputWriteLine($"  ({i * 4,3})   {tab}{CombineValues(values[i])}");
            //}

        }



        /*
         * dBlock.arg0 is always 2
         * dBlock.arg1 and dBlock.arg5 are always 0
         */
        static void ShowDBlockArgumentList(string filenamepath, bool showHtmlLink = true) {
            ShaderFile shaderFile = new(filenamepath);
            OutputWriteLine($"DYNAMIC PARAMS");
            if (showHtmlLink) {
                OutputWriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");
            }
            int[] pad = { 7, 40, 7, 7, 7 };
            string h0 = "index";
            string h1 = "name";
            string h2 = "arg2";
            string h3 = "arg3";
            string h4 = "arg4";
            string blockHeader = $"{h0.PadRight(pad[0])} {h1.PadRight(pad[1])} {h2.PadRight(pad[2])} {h3.PadRight(pad[3])} {h4.PadRight(pad[4])}";
            OutputWriteLine(blockHeader);
            foreach (DBlock dBlock in shaderFile.dBlocks) {
                string v0 = $"[{dBlock.blockIndex,2}]";
                string v1 = dBlock.name0;
                string v2 = "" + dBlock.arg2;
                string v3 = "" + dBlock.arg3;
                string v4 = $"{dBlock.arg4,2}";
                string blockSummary = $"{v0.PadRight(pad[0])} {v1.PadRight(pad[1])} {v2.PadRight(pad[2])} {v3.PadRight(pad[3])} {v4.PadRight(pad[4])}";
                OutputWriteLine(blockSummary);
            }
            if (shaderFile.dBlocks.Count == 0) {
                OutputWrite("[empty list]");
            }
            OutputWriteLine("");
        }


        static SortedDictionary<string, int> abbreviationsUsed = new();



        static void ShowSfArgumentList(string filenamepath, bool showLink = true) {
            abbreviationsUsed = new();
            ShaderFile shaderFile = new(filenamepath);
            if (showLink) {
                OutputWriteLine($"SF params for {GetHtmlLink(filenamepath)}");
            } else {
                OutputWriteLine($"SF params for {ShortHandName(filenamepath)}");
            }
            int[] pad = { 7, 40, 9, 9 };
            string h0 = "index";
            string h1 = "name";
            string h2 = "layers";
            string h3 = "arg3";
            string blockHeader = $"{h0.PadRight(pad[0])} {h1.PadRight(pad[1])} {h2.PadRight(pad[2])} {h3.PadRight(pad[3])}";
            OutputWriteLine(blockHeader);
            foreach (var sfBlock in shaderFile.sfBlocks) {
                string v0 = $"[{sfBlock.blockId,2}]";
                string v1 = sfBlock.name0;

                // R: the abbreviations are only used later if printing the zframes (instantited to new() at start of method)
                string abbreviation = $"{sfBlock.name0}({ShortenShaderParam(sfBlock.name0).ToLower()})";
                abbreviationsUsed[abbreviation] = 1;


                string v2 = "" + sfBlock.arg2;
                string v3 = "" + sfBlock.arg3;
                string blockSummary = $"{v0.PadRight(pad[0])} {v1.PadRight(pad[1])} {v2.PadRight(pad[2])} {v3.PadRight(pad[3])}";
                OutputWriteLine(blockSummary);
            }
            if (shaderFile.sfBlocks.Count == 0) {
                OutputWrite("[empty list]");
            }

            OutputWriteLine("");
        }




        //static void FileSummaryVsPSFile((string, string, string) triple, FILETYPE targetFileType, string title = "summary",
        //        string outputFilenamepath = null, bool writeFile = false) {

        static void FileSummaryVsPSFile(FileTriple triple, FILETYPE targetFileType, string title = "summary",
                string outputFilenamepath = null, bool writeFile = false, bool disableOutput = false) {
            if (targetFileType != FILETYPE.vs_file && targetFileType != FILETYPE.ps_file) {
                throw new ShaderParserException("need to target either vs or ps file");
            }

            FileTokens ftFile = triple.ftFile;
            FileTokens targetFile = targetFileType == FILETYPE.vs_file ? triple.vsFile : triple.psFile;
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath, disableOutput);
                WriteHtmlFile(title, $"SF SUMMARY for {targetFile.GetShortHandName()}");
            }
            List<(string, string, string)> triples = new();



            SfSummaryOfFileTriple(triple);
            ShowSfArgumentList(targetFile.filenamepath);
            CompatBlockDetailsConcise2(targetFile.filenamepath, showLink: false);
            ShowDBlockArgumentList(targetFile.filenamepath, showHtmlLink: false);
            UnknownBlockConcise(targetFile.filenamepath, showLink: false);


            // print the config headers every 100 frames
            int zframeCount = 0;

            // print the zframes
            string zFrameBaseDir = $"/vcs-all/{GetCoreOrDotaString(targetFile.filenamepath)}/zsource/";
            ShaderFile shaderFile = new(targetFile.filenamepath);

            // prepare the lookup to determine configuration state
            CompatRulesGeneration configGen = new(shaderFile);



            OutputWriteLine("");
            string zframesHeader = $"ZFRAMES ({shaderFile.GetZFrameCount()})";
            OutputWriteLine(zframesHeader);
            OutputWriteLine(new string('-', zframesHeader.Length));


            // collect names in the order they appear
            List<string> sfNames = new();
            foreach (DataBlockSfBlock sfBlock in shaderFile.sfBlocks) {
                sfNames.Add(ShortenShaderParam(sfBlock.name0).ToLower());
            }


            string configHeader = CombineStringsSpaceSep(sfNames.ToArray(), 6);
            configHeader = $"{new string(' ', 14)}{configHeader}";
            // OutputWriteLine(configHeader);

            foreach (var item in shaderFile.zframesLookup) {
                if (zframeCount%100==0) {
                    OutputWriteLine($"{configHeader}");
                }
                int[] configState = configGen.GetConfigState(item.Key);

                // string zframeLink = $"{GetZframeHtmlLinkCheckExists((uint)item.Key, targetFile.filenamepath, SERVER_BASEDIR, zFrameBaseDir)}";

                string zframeLink = targetFile.GetBestZframesLink(item.Key);
                OutputWriteLine($"{zframeLink} {CombineIntsSpaceSep(configState, 6)}");
                zframeCount++;

            }

        }



        /*
         * FIXME WARN - this has been overwritten to output data for multiblend ONLY!!
         * (in general this method has been retired by the ones above)
         *
         *
         */
        // static void FileSummaryMultiblendPs(string featuresfile, string outputFilenamepath = null, bool writeFile = false) {

        static void FileSummaryMultiblendPs() {
            string featuresfile = @$"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_features.vcs";
            string outputFilenamepath = $@"{SERVER_BASEDIR}\dota-game\pcgl\multiblend_pcgl_30\multiblend_pcgl_30_ps-summary.html";
            bool writeFile = true;
            FileTriple triple = FileTriple.GetTripleIfExists(ARCHIVE.dotagame_pcgl, featuresfile);


            string multiBlendPsFile = featuresfile[0..^12] + "ps.vcs";
            string htmlTitle = GetShortName(multiBlendPsFile);

            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile(htmlTitle, $"SF SUMMARY for {Path.GetFileName(multiBlendPsFile)} ({GetCoreOrDotaString(multiBlendPsFile)})");
            }
            SfSummaryOfFileTriple(triple);
            ShowSfArgumentList(multiBlendPsFile);
            CompatBlockDetailsConcise2(multiBlendPsFile, showLink: false);
            ShowDBlockArgumentList(multiBlendPsFile, showHtmlLink: false);
            UnknownBlockConcise(multiBlendPsFile, showLink: false);


            // R: zframes specific to multiblend_pcgl_30_ps.vcs only
            OutputWriteLine("");
            OutputWriteLine("ZFRAMES (3335)");
            OutputWriteLine("--------------");
            string[] abbreviations = new string[abbreviationsUsed.Count];
            int ind = 0;
            foreach (var item in abbreviationsUsed) {
                abbreviations[ind++] = item.Key;
            }
            string[] breakabbreviations = CombineValuesBreakString(abbreviations, 120);
            foreach (string abbr in breakabbreviations) {
                OutputWriteLine(abbr.Replace("(", "<span style='color: blue'>(").Replace(")", "</span>)"));
            }
            OutputWriteLine("");


            // I need to use the multiblend directory to get all the files


            string zFrameBaseDir = $"/multiblend_pcgl_30/";
            ShaderFile shaderFile = new(multiBlendPsFile);
            CompatRulesGeneration configGen = new(shaderFile);
            int zframeCount = 0;

            // collect names in the order they appear
            List<string> sfNames = new();
            foreach (DataBlockSfBlock sfBlock in shaderFile.sfBlocks) {
                sfNames.Add(ShortenShaderParam(sfBlock.name0).ToLower());
            }


            string configHeader = CombineStringsSpaceSep(sfNames.ToArray(), 6);
            configHeader = $"{new string(' ', 14)}{configHeader}";
            // OutputWriteLine(configHeader);

            foreach (var item in shaderFile.zframesLookup) {
                if (zframeCount%100==0) {
                    OutputWriteLine($"{configHeader}");
                }
                int[] configState = configGen.GetConfigState(item.Key);
                string zframeLink = $"{GetZframeHtmlLinkCheckExists((uint)item.Key, multiBlendPsFile, SERVER_BASEDIR, zFrameBaseDir)}";

                if(File.Exists(triple.psFile.GetZFrameHtmlFilenamepath(item.Key))) {
                    zframeLink = $"* <a href='{triple.psFile.GetZFrameLink(item.Key)}'>Z[0x{item.Key:x08}]</a>";
                }

                OutputWriteLine($"{zframeLink} {CombineIntsSpaceSep(configState, 6)}");
                zframeCount++;

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
        static void CompatBlockResolveSfReferences(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("Cblocks", "Compatibility blocks, with resolved symbol names");
            }
            // List<string> allVcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            List<string> allVcsFiles = new();

            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_features.vcs");
            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs");
            // allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs");

            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs");
            //allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs");

            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\global_lit_simple_pcgl_30_features.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\global_lit_simple_pcgl_30_vs.vcs");
            allVcsFiles.Add(PCGL_DIR_NOT_CORE + @"\global_lit_simple_pcgl_30_ps.vcs");

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

        static void CompatBlockDetailsConcise2(string filenamepath, bool showLink = true) {

            ShaderFile shaderFile = new(filenamepath);
            bool newFile = true;

            foreach (CompatibilityBlock cBlock in shaderFile.compatibilityBlocks) {

                if (newFile && showLink) {
                    OutputWriteLine($"Compatibility rules for {GetHtmlLink(filenamepath)}");
                    // OutputWriteLine(RemoveBaseDir(filenamepath));
                    OutputWriteLine("");
                } else if (newFile) {
                    OutputWriteLine("Compatibility rules");
                }
                newFile = false;
                string[] sfNames = new string[cBlock.range0.Length];
                for (int i = 0; i < sfNames.Length; i++) {
                    sfNames[i] = shaderFile.sfBlocks[cBlock.range0[i]].name0;
                }

                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(sfNames, BL);

                string s0 = $"[{cBlock.blockIndex,2}]";
                string s1 = (cBlock.relRule == 1 || cBlock.relRule == 2) ? $"INC({cBlock.relRule})" : $"EXC({cBlock.relRule})";
                // string s2 = $"{cBlock.arg0}";
                string s3 = $"{cBlock.ReadByteFlags()}";
                // string s4 = $"{CombineValues(sfNames)}";
                string s4 = $"{breakNames[0]}";
                string s5 = $"{CombineValues2(cBlock.range0)}";
                string s6 = $"{CombineValues2(cBlock.range1)}";
                string s7 = $"{CombineValues2(cBlock.range2)}";
                // string blockSummary = $"{s0.PadRight(10)} {s1.PadRight(6)} {s2.PadRight(6)} {s3.PadRight(18)} s4.PadRight(20)} {s5.PadRight(8)} {s6.PadRight(8)}";
                string blockSummary = $"{s0.PadRight(7)}{s1.PadRight(10)}{s5.PadRight(16)}{s4.PadRight(BL)}{s6.PadRight(8)}{s7.PadRight(8)}";
                for (int i = 1; i < breakNames.Length; i++) {
                    blockSummary += $"\n{(""),7}{(""),10}{(""),16}{breakNames[i],-BL}";
                }
                OutputWrite(blockSummary);
                OutputWriteLine("");
            }
            OutputWriteLine("");
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



        static void CompatBlockConciseAll(string outputFilenamepath = null, bool writeFile = false) {
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



        static void CompatBlockDetails(string outputFilenamepath = null, bool writeFile = false) {
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




        static void CompatibilityBlocksSurveyIntRange(int offset, int count, string outputFilenamepath = null, bool writeFile = false) {
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




        static void CompatibilityBlocksSurvey(string outputFilenamepath = null, bool writeFile = false) {
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




        static void CompatibilityBlocksValuesSeenAtOffset(int offset, string outputFilenamepath = null, bool writeFile = false) {
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



        static void CompatibilityBlocksValuesSeenAtOffset0(string outputFilenamepath = null, bool writeFile = false) {
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



        static void TestHelperMethods() {
            string[] @string = { "D_SPECULAR_GBUFFER_DISABLED", "S_MODE_FORWARD", "S_MODE_DEPTH", "S_MODE_TOOLS_VIS", "S_MODE_TOOLS_WIREFRAME" };
            string[] result = CombineValuesBreakString(@string, 70);
            foreach (var str in result) {
                Debug.WriteLine($"{str}");
            }
        }




        static string CombineValues2(int[] ints0, bool includeParenth = false) {
            if (ints0.Length == 0) return $"_";
            string valueString = "";
            foreach (int i in ints0) {
                valueString += $"{i},";
            }
            valueString = valueString[0..^1];
            return includeParenth ? $"({valueString})" : $"{valueString}";
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




        static void CompatibilityBlockValuesExample(string outputFilenamepath = null, bool writeFile = false) {
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




        static void CompatBlockPresence(string outputFilenamepath = null, bool writeFile = false) {
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







        static void WriteSfArgumentsAllFiles(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("SF names", "SF names for vcs, vs, ps triples");
            }
            List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, -1);
            SfSummaryOfFileTriples(triples);

        }


        static void SfSummaryOfFileTriple(FileTriple triple) {
            List<FileTriple> triples = new();
            triples.Add(triple);
            SfSummaryOfFileTriples(triples);
        }



        // static void SfSummaryOfFileTriple(List<(string, string, string)> triples) {
        static void SfSummaryOfFileTriples(List<FileTriple> triples) {
            foreach (var triple in triples) {

                string title = $"{triple.ftFile.RemoveBaseDir()} + vs, ps files";
                OutputWriteLine($"{title}");
                OutputWriteLine(new string('-', title.Length));
                ShaderFile ftFile = new(triple.ftFile.filenamepath);
                OutputWriteLine($"{ftFile.featuresHeader.file_description}");
                Dictionary<int, string[]> p = new();
                for (int i = 0; i < 30; i++) {
                    p.Add(i, new string[] { "", "", "" });
                }
                Dictionary<string, int> n = new();
                ShaderFile vsFile = new(triple.vsFile.filenamepath);
                ShaderFile psFile = new(triple.psFile.filenamepath);
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
                OutputWriteLine($"<span style='color: #3783ed'>Arguments in {triple.ftFile.filename} header ({ftHeaderNrArguments})</span>");
                // print the features main args
                int max_len = 0;
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    max_len = mp.Item1.Length > max_len ? mp.Item1.Length : max_len;
                }
                foreach (var mp in ftFile.featuresHeader.mainParams) {
                    OutputWriteLine($"<span style='color: #3783ed'>{mp.Item1.PadRight(max_len)} {mp.Item2}</span>");
                }
                OutputWriteLine("");


                string headerText = $"{"[FEATURES-FILE]".PadRight(52)} ";
                OutputWrite(headerText.Replace("[", $"<a href='{triple.ftFile.GetBytePath()}'>").Replace("]", "</a>"));
                headerText = $"{"[VS-FILE]".PadRight(52)} ";
                OutputWrite(headerText.Replace("[", $"<a href='{triple.vsFile.GetSummariesPath()}'>").Replace("]", "</a>"));
                headerText = $"{"[PS-FILE]".PadRight(52)}\n";
                OutputWrite(headerText.Replace("[", $"<a href='{triple.psFile.GetSummariesPath()}'>").Replace("]", "</a>"));
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


        /*
         * Some arguments are specific only to only vs/ps files, while others are shared.
         *
         * E.g S_TRANSFORM_CONSTANT_BUFFER is only seen in vs files
         *
         *
         *
         */
        static void SfBlockInspections() {
            List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in featuresFiles) {
                ShaderFile shaderFile = new(filenamepath);

                string filetype = "";
                if (shaderFile.vcsFiletype == FILETYPE.features_file) {
                    filetype = "           FEAT";
                }
                if (shaderFile.vcsFiletype == FILETYPE.vs_file) {
                    filetype = "VS             ";
                }
                if (shaderFile.vcsFiletype == FILETYPE.ps_file) {
                    filetype = "      PS       ";
                }

                foreach (var sfBlock in shaderFile.sfBlocks) {
                    CollectStringValue($"{sfBlock.name0.PadRight(35)[2..]} {filetype}    {sfBlock.name1.PadRight(35)}");
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




        /*
         * Note only the features files have a features header!
         * arg4,arg5,arg6,arg7 are always 0
         *
         */
        static void FeaturesHeaderFirstFour() {
            List<string> featuresFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
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


        private static bool swWriterAlreadyClosed = false;


        // This basestream != null is nonsense, it doesn't check if the file is open
        private static void CloseStreamWriter() {
            if (WriteAsHtml && !swWriterAlreadyClosed) {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed) {
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
#pragma warning restore IDE1006 // Naming Styles








