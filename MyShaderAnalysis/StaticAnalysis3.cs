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
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
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
            output.DefineHeaders(new string[] {"index", "name", "type", "res", "ld0", "arg0", "arg1", "arg2", "arg3", "arg4", "arg5", ""});
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

                // output.AddTabulatedRow(new string[] {$"", $"{param.name1}", $"", $""});


                // $"{param.name1}"

            }
            output.printTabulatedValues();


        }


        static string BlankMOne(int val)
        {
            if (val==-1)
            {
                return "_";
            }

            return ""+val;
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





