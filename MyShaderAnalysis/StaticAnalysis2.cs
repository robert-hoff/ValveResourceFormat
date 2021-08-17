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


namespace MyShaderAnalysis {

    public class StaticAnalysis2 {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";


        public static void RunTrials() {
            Trial1();
            PrintReport(showCount: false);
            CloseStreamWriter();
        }



        static void Trial1() {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\hero_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\global_lit_simple_pcgl_30_vs.vcs";


            // SfBlocksTest();
            // ShowSfBlocksWithAdditionalArguments(TestFilenamepath("F-params-multiple-properties.html"), true);
            // ShowFeaturesDetails();
            // SurveyParamBlocks(TestFilenamepath("param-printout.html"), false);
            // PrintingAndSortingParams(TestFilenamepath("TESTFILE.html"), true);
            // PrintingParamsWithDynamicExpressions(TestFilenamepath("TESTFILE.html"), true);




            DoSomething();

        }


        static void DoSomething() {

            Debug.WriteLine($"do do it");
        }






        static string Pow2Rep(int val) {
            int orig = val;
            //if (val==6144) {
            //    return "(3)(2^11)";
            //}
            int pow = 0;
            while (val > 1 && (val & 1) == 0) {
                val >>= 1;
                pow++;
            }
            if (val != 1) {
                return "" + orig;
            }
            return $"(2^{pow})";
        }




        static void PrintingParamsWithDynamicExpressions(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("all params", "Shader params");
            }
            int CUTLEN = 0;

            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            foreach (string filenamepath in vcsFiles) {
                ShaderFile shaderFile = new(filenamepath);

                foreach (ParamBlock paramBlock in shaderFile.paramBlocks) {
                    string name0 = paramBlock.name0;
                    string name1 = paramBlock.name1;
                    string name2 = paramBlock.name2;
                    int tp = paramBlock.pt0;
                    float res0 = paramBlock.res0;
                    int main0 = paramBlock.main0;
                    int arg0 = paramBlock.arg0;
                    int arg1 = paramBlock.arg1;
                    int arg2 = paramBlock.arg2;
                    int arg3 = paramBlock.arg3;
                    int arg4 = paramBlock.arg4;
                    int arg5 = paramBlock.arg5;
                    string fileref = paramBlock.fileref;
                    int[] r0 = paramBlock.ranges0;
                    int[] r1 = paramBlock.ranges1;
                    int[] r2 = paramBlock.ranges2;
                    float[] r3 = paramBlock.ranges3;
                    float[] r4 = paramBlock.ranges4;
                    float[] r5 = paramBlock.ranges5;
                    int[] r6 = paramBlock.ranges6;
                    int[] r7 = paramBlock.ranges7;
                    string c0 = paramBlock.command0;
                    string c1 = paramBlock.command1;

                    byte[] dynExp = paramBlock.dynExp;
                    string dynExpstring = new ParseDynamicExpressionShader().ParseExpression(dynExp);


                    CUTLEN = 16;
                    string sortOn = $"{r7[0]+100:d04}{r7[1]+100:d04}{r7[2]+100:d04}{r7[3]+100:d04}";
                    string c0c1 = c0;
                    if (c1.Length > 0) {
                        c0c1 += $", {c1}";
                    }
                    string reportString = $"{sortOn}{name0,-40} {tp,2}  {res0,8} {main0,3} " +
                    $"{arg0,3} {arg1,3} {arg2,3} {Pow2Rep(arg3),8} {arg4,3} {arg5,3}  " +
                    $"{comb(r0),12} {comb(r6),14} {comb(r7),12} | ";


                    string spaceString = new string(' ', reportString.Length - CUTLEN + 3);
                    string[] dynExpLines = dynExpstring.Split("\n");

                    if (dynExpLines.Length>1 || dynExpLines[0].Length>80) {
                        spaceString = new string(' ', 4);
                        reportString += $"\n{spaceString}{dynExpLines[0]}";
                        for (int i = 1; i < dynExpLines.Length; i++) {
                            reportString += $"\n{spaceString}{dynExpLines[i]}";
                        }
                        reportString += "\n";
                    } else {
                        reportString += $"{dynExpLines[0]}";
                    }

                    CollectStringValue(reportString);

                }
            }
            ShowStringsCollected(CUTLEN);
        }




        static void PrintingAndSortingParams(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("params", "Parameters, i7 sorted");
            }
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            int CUTLEN = 0;

            foreach (string filenamepath in vcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                foreach (ParamBlock paramBlock in shaderFile.paramBlocks) {
                    string name0 = paramBlock.name0;
                    string name1 = paramBlock.name1;
                    string name2 = paramBlock.name2;
                    int tp = paramBlock.pt0;
                    float res0 = paramBlock.res0;
                    int main0 = paramBlock.main0;
                    int arg0 = paramBlock.arg0;
                    int arg1 = paramBlock.arg1;
                    int arg2 = paramBlock.arg2;
                    int arg3 = paramBlock.arg3;
                    int arg4 = paramBlock.arg4;
                    int arg5 = paramBlock.arg5;
                    string fileref = paramBlock.fileref;
                    int[] r0 = paramBlock.ranges0;
                    int[] r1 = paramBlock.ranges1;
                    int[] r2 = paramBlock.ranges2;
                    float[] r3 = paramBlock.ranges3;
                    float[] r4 = paramBlock.ranges4;
                    float[] r5 = paramBlock.ranges5;
                    int[] r6 = paramBlock.ranges6;
                    int[] r7 = paramBlock.ranges7;
                    string c0 = paramBlock.command0;
                    string c1 = paramBlock.command1;

                    // CollectStringValue($"{name0,-40} {name1,-40}  {lead0},{lead1,3}     {name2,-40}");
                    //CollectStringValue($"{name0,-40} {lead0},{lead1,3} " +
                    //    $"{arg0,-3} {arg1,-3} {arg2,-3} {arg3,-3} {arg4,-5} {arg5,-3}  " +
                    //    $"{comb(r0)} {comb(r1)} {comb(r2)} {comb(r3)} {comb(r4)} {comb(r5)} {comb(r6)} {comb(r7)} " +
                    //    $"{c0} {c1}   |    {name1} {fileref}");


                    CUTLEN = 16;
                    string sortOn = $"{r7[0]+100:d04}{r7[1]+100:d04}{r7[2]+100:d04}{r7[3]+100:d04}";

                    string c0c1 = c0;
                    if (c1.Length > 0) {
                        c0c1 += $", {c1}";
                    }
                    CollectStringValue($"{sortOn}{name0,-40} {tp,2}  {res0,8} {main0,3} " +
                    $"{arg0,3} {arg1,3} {arg2,3} {Pow2Rep(arg3),8} {arg4,3} {arg5,3}  " +
                    $"{comb(r0),12} {comb(r6),14} {comb(r7),12} " +
                    $"{c0c1,-14}   |    {name1} {fileref}");


                    // there exists one parameter that has three names
                    //
                    //     g_flIsSelected
                    //
                    //
                    if (name1.Length > 0 && name2.Length > 0) {
                        //Debug.WriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");
                        //paramBlock.ShowBlock();
                        //Debug.WriteLine($"");
                    }

                }
            }

            ShowStringsCollected(CUTLEN);

        }



        private static void ShowStringsCollected(int cutstring = 0) {
            OutputWriteLine("                                       type       res  m0  a0  a1  a2      " +
                "a3   a4  a5           i0             i6           i7      command(0,1)");
            List<string> strings0 = new();
            foreach (var item in collectValuesString) {
                OutputWriteLine(item.Key[cutstring..]);
            }

        }



        private static string comb2(int[] ints0) {
            return $"({f(ints0[0]),2},{f(ints0[1]),2},{f(ints0[2]),2},{f(ints0[3]),2})";
        }

        private static string comb(int[] ints0) {
            return $"({f(ints0[0])},{f(ints0[1])},{f(ints0[2])},{f(ints0[3])})";
        }

        private static string comb(float[] floats0) {
            return $"({f(floats0[0])},{f(floats0[1])},{f(floats0[2])},{f(floats0[3])})";
        }


        private static string f(float val) {
            if (val == -1e9) return "-M";
            if (val == 1e9) return "M";
            return $"{val}";
        }

        private static string f(int val) {
            if (val == -999999999) return "-M";
            if (val == 999999999) return "M";
            return "" + val; ;
        }




        static void SfBlocksTest(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("test", "test");
            }
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            foreach (string filenamepath in vcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                // FileTokens tokens = new(filenamepath);
                foreach (var sfBlocks in shaderFile.sfBlocks) {
                    CollectStringValue($"{sfBlocks.arg0}  {sfBlocks.arg1}  {sfBlocks.arg2}  {sfBlocks.arg3}  {sfBlocks.arg4}  {sfBlocks.arg5}");
                }
            }
        }




        static void ShowSfBlocksWithAdditionalArguments(string outputFilenamepath = null, bool writeFile = false) {
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile("mul-params", "Params with multiple configurations (only F-params have this)");
            }
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in vcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                FileTokens tokens = new(filenamepath);
                foreach (var sfBlocks in shaderFile.sfBlocks) {
                    if (sfBlocks.additionalParams.Count > 0) {
                        OutputWriteLine($"<b>{RemoveBaseDir(shaderFile.filenamepath),-70}</b>        " +
                            $"<a href='{tokens.GetServerFilePath()}/{tokens.foldername}_ps-summary.html'>det.</a>");
                        OutputWriteLine($"{sfBlocks.name0} ({sfBlocks.additionalParams.Count})     {sfBlocks.name1}");
                        OutputWriteLine($"{CombineStringArray(sfBlocks.additionalParams.ToArray())}");
                        OutputWriteLine($"");
                    }
                }
            }
        }



        static void ShowFeaturesDetails() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, -1);
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.features_file, -1);
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_NOT_CORE, null, FILETYPE.features_file, -1);
            foreach (string filenamepath in vcsFiles) {
                ShaderFile shaderFile = new(filenamepath);
                // - show all descriptions
                // Debug.WriteLine($"{shaderFile.featuresHeader.file_description}");
                // Debug.WriteLine($"{shader}");

                int a0 = shaderFile.featuresHeader.arg0;
                int a1 = shaderFile.featuresHeader.arg1;
                int a2 = shaderFile.featuresHeader.arg2;
                int a3 = shaderFile.featuresHeader.arg3;
                //if (a2 == 0 && a3 == 0) {
                //    Debug.WriteLine($"{GetShortName(shaderFile.filenamepath)}");
                //}

                //if (a0 == 0) {
                //CollectStringValue($"{CombineIntArray(new int[] { a0, a1, a2, a3 })}");
                //    Debug.WriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");
                //}

                if (a0 == 1 && a3 == 1) {
                    CollectStringValue($"{CombineIntArray(new int[] { a0, a1, a2, a3 })}");
                    Debug.WriteLine($"{RemoveBaseDir(shaderFile.filenamepath)}");
                }
            }
        }






        private static string TestFilenamepath(string filename) {
            return $"{SERVER_OUTPUT_DIR}/{filename}";
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







