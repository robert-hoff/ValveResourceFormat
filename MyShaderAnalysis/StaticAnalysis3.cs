using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.compat;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;

namespace MyShaderAnalysis
{
    public class StaticAnalysis3
    {
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";
        static OutputWriter output = new();


        public static void RunTrials()
        {
            Trial1();
            output.CloseStreamWriter();
        }


        static void Trial1()
        {
            PrintSingleFileAnalysis($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            // TestWriteSystem($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
        }






        static void PrintSingleFileAnalysis(string outputFilenamepath = null, bool writeFile = false)
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml("title", $"{ShortHandName(filenamepath)}");
            }
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3", "arg4" });
            foreach (var item in shaderFile.sfBlocks)
            {
                output.AddTabulatedRow(new string[] {$"[{item.blockIndex,2}]", $"{item.name0}", $"{item.arg2}",
                    $"{item.arg3}", $"{item.arg4,2}"});
            }
            output.printTabulatedValues();
            output.BreakLine();

            // parameters
            output.WriteLine("PARAMETERS");
            output.DefineHeaders(new string[] { "index", "name", "type", "res", "ld0", "arg0", "arg1", "arg2", "arg3", "arg4", "arg5", "" });
            foreach (var param in shaderFile.paramBlocks)
            {
                string nameCondensed = param.name0;
                if (param.name1.Length > 0)
                {
                    nameCondensed += $" | {param.name1}";
                }
                if (param.name2.Length > 0)
                {
                    nameCondensed += $" | {param.name2}(2)";
                }

                int indexPad = shaderFile.paramBlocks.Count > 100 ? 3 : 2;
                string dynExpstring = new VfxEval().ParseExpression(param.dynExp);

                output.AddTabulatedRow(new string[] {$"[{(""+param.blockIndex).PadLeft(indexPad)}]", $"{nameCondensed}", $"{param.type}",
                    $"{param.res0}", $"{param.lead0}", $"{BlankMOne(param.arg0),2}", $"{param.arg1,2}", $"{param.arg2}",
                    $"{Pow2Rep(param.arg3),4}", $"{param.arg4,2}", $"{BlankMOne(param.arg5),2}", $"{dynExpstring}"});

            }
            output.printTabulatedValues();
            output.BreakLine();


            output.WriteLine("PARAMETERS - Default values and ranges");
            output.DefineHeaders(new string[] { "index", "name", "ints0", "ints1", "ints2", "floats0", "floats1", "floats2",
                "ints3", "ints4", "command", "fileref"});
            foreach (var param in shaderFile.paramBlocks)
            {
                int indexPad = shaderFile.paramBlocks.Count > 100 ? 3 : 2;
                string fileref = param.fileref;
                int[] r0 = param.ranges0;
                int[] r1 = param.ranges1;
                int[] r2 = param.ranges2;
                float[] r3 = param.ranges3;
                float[] r4 = param.ranges4;
                float[] r5 = param.ranges5;
                int[] r6 = param.ranges6;
                int[] r7 = param.ranges7;
                string c0 = param.command0;
                string c1 = param.command1;
                if (c1.Length>0)
                {
                    c0 += $" | {c1}";
                }

                output.AddTabulatedRow(new string[] { $"[{("" + param.blockIndex).PadLeft(indexPad)}]"
                    ,$"{param.name0}", $"{comb(r0)}", $"{comb(r1)}", $"{comb(r2)}", $"{comb(r3)}", $"{comb(r4)}", $"{comb(r5)}",
                    $"{comb(r6)}", $"{comb(r7)}", $"{c0}", $"{param.fileref}"});
            }
            output.printTabulatedValues();



        }


        static string BlankMOne(int val)
        {
            if (val == -1)
            {
                return "_";
            }

            return "" + val;
        }


        static string Pow2Rep(int val)
        {
            int orig = val;
            //if (val==6144) {
            //    return "(3)(2^11)";
            //}
            int pow = 0;
            while (val > 1 && (val & 1) == 0)
            {
                val >>= 1;
                pow++;
            }
            if (val != 1)
            {
                return "" + orig;
            }
            return $"2^{pow}";
        }

        private static string comb2(int[] ints0)
        {
            return $"({f(ints0[0]),2},{f(ints0[1]),2},{f(ints0[2]),2},{f(ints0[3]),2})";
        }

        private static string comb(int[] ints0)
        {
            return $"({f(ints0[0])},{f(ints0[1])},{f(ints0[2])},{f(ints0[3])})";
        }

        private static string comb(float[] floats0)
        {
            return $"({f(floats0[0])},{f(floats0[1])},{f(floats0[2])},{f(floats0[3])})";
        }

        private static string f(float val)
        {
            if (val == -1e9) return "-i";
            if (val == 1e9) return "+i";
            return $"{val}";
        }

        private static string f(int val)
        {
            if (val == -999999999) return "-i";
            if (val == 999999999) return "+i";
            return "" + val; ;
        }



        static void TestWriteSystem(string outputFilenamepath = null, bool writeFile = false)
        {
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml("title", "header");
            }

            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3" });
            output.AddTabulatedRow(new string[] { $"sdf", "1", "2" });
            output.printTabulatedValues();


        }




    }
}





