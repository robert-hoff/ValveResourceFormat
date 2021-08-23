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
            // PrintSingleFileAnalysisParameterView($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            PrintSingleFileAnalysisBufferViews($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            // TestWriteSystem($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
        }



        static void PrintSingleFileAnalysisBufferViews(string outputFilenamepath = null, bool writeFile = false)
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs"; // doesn't have any mipmaps
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
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

            output.WriteLine("MIPMAP BLOCKS");
            if (shaderFile.mipmapBlocks.Count > 0)
            {
                output.DefineHeaders(new string[] { "index", "name", "arg0", "arg1", "arg2", "arg3", "arg4", "arg5" });
            } else
            {
                output.DefineHeaders(Array.Empty<string>());
                output.WriteLine("[none]");
            }
            foreach (var mipmap in shaderFile.mipmapBlocks)
            {
                output.AddTabulatedRow(new string[] { $"[{mipmap.blockIndex,2}]", $"{mipmap.name}",
                    $"{ShaderDataReader.BytesToString(mipmap.arg0),-14}", $"{mipmap.arg1,2}", $"{BlankMOne(mipmap.arg2),2}",
                    $"{BlankMOne(mipmap.arg3),2}", $"{BlankMOne(mipmap.arg4),2}", $"{mipmap.arg5,2}" });
            }
            output.printTabulatedValues();
            output.BreakLine();


            foreach (var bufferBlock in shaderFile.bufferBlocks)
            {
                output.WriteLine($"BUFFER-BLOCK[{bufferBlock.blockIndex}]");
                output.WriteLine($"{bufferBlock.name} size={bufferBlock.bufferSize} param-count={bufferBlock.paramCount}" +
                    $" arg0={bufferBlock.arg0} crc32={bufferBlock.blockCrc:x08}");
                output.DefineHeaders(new string[] { "       ", "name", "offset", "vertex-size", "attrib-count", "data-count" });
                foreach (var bufferParams in bufferBlock.bufferParams)
                {
                    string name = bufferParams.Item1;
                    int bOffset = bufferParams.Item2;
                    int nrVertices = bufferParams.Item3;
                    int nrAttribs = bufferParams.Item4;
                    int length = bufferParams.Item5;
                    output.AddTabulatedRow(new string[] { "", $"{name}", $"{bOffset,3}", $"{nrVertices,3}", $"{nrAttribs,3}", $"{length,3}" });

                }
                output.printTabulatedValues();
                output.BreakLine();
            }


            output.WriteLine($"VERTEX-BUFFER-SYMBOLS({shaderFile.symbolBlocks.Count})");
            // find best passing
            int namePad = 0;
            int typePad = 0;
            int optionPad = 0;
            foreach (var symbolBlock in shaderFile.symbolBlocks)
            {
                foreach (var symbolsDef in symbolBlock.symbolsDefinition)
                {
                    if (symbolsDef.Item1.Length > namePad)
                    {
                        namePad = symbolsDef.Item1.Length;
                    }
                    if (symbolsDef.Item2.Length > typePad)
                    {
                        typePad = symbolsDef.Item2.Length;
                    }
                    if (symbolsDef.Item3.Length > optionPad)
                    {
                        optionPad = symbolsDef.Item3.Length;
                    }
                }
            }


            foreach (var symbolBlock in shaderFile.symbolBlocks)
            {
                output.WriteLine($"VERTEX-SYMBOLS[{symbolBlock.blockIndex}] definitions={symbolBlock.symbolsCount}");
                output.DefineHeaders(new string[] { "       ", "name".PadRight(namePad), "type".PadRight(typePad),
                    $"option".PadRight(optionPad), "semantic-index" });
                foreach (var symbolsDef in symbolBlock.symbolsDefinition)
                {
                    string name = symbolsDef.Item1;
                    string type = symbolsDef.Item2;
                    string option = symbolsDef.Item3;
                    int semanticIndex = symbolsDef.Item4;
                    output.AddTabulatedRow(new string[] { "", $"{name}", $"{type}", $"{option}", $"{semanticIndex,2}" });
                }
                output.printTabulatedValues();
                output.BreakLine();
            }


        }




        static void PrintSingleFileAnalysisParameterView(string outputFilenamepath = null, bool writeFile = false)
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


            output.WriteLine("PARAMETERS - Default values and ranges (as well as command and fileref arguments if these exist)");
            output.WriteLine("(- indicates -infinity, + indicates +infinity)");
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
                if (c1.Length > 0)
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
            if (val == -1e9) return "-";
            if (val == 1e9) return "+";
            return $"{val}";
        }

        private static string f(int val)
        {
            if (val == -999999999) return "-";
            if (val == 999999999) return "+";
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





