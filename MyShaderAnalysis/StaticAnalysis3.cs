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
            // PrintSingleFileAnalysisBufferViews($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            // PrintSingleFileAnalysisZFrames($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            PrintSingleFileMainParamsAndConstaints($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            // TestWriteSystem($"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
        }



        static void PrintSingleFileMainParamsAndConstaints(string outputFilenamepath = null, bool writeFile = false)
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs"; // doesn't have any mipmaps
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml("title", $"{ShortHandName(filenamepath)}");
            }
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            if (shaderFile.vcsFileType == VcsFileType.Features)
            {
                PrintFeaturesHeader(shaderFile);
                PrintFBlocks(shaderFile);
            } else
            {
                PrintPsVsHeader(shaderFile);
                PrintSBlocks(shaderFile);
            }

            PrintStaticConstraints(shaderFile);
            PrintDynamicConfigurations(shaderFile);
            PrintDynamicConstraints(shaderFile);
            PrintParameters(shaderFile);
            PrintMipmapBlocks(shaderFile);
            PrintBufferBlocks(shaderFile);
            PrintVertexSymbolBuffers(shaderFile);
            PrintZFrames(shaderFile);
        }



        static void PrintFeaturesHeader(ShaderFile shaderFile)
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.featuresHeader.vcsFileVersion}");
            output.BreakLine();
            output.WriteLine($"Features Detail ({Path.GetFileName(shaderFile.filenamepath)})");
            output.WriteLine($"VFX File Desc: {shaderFile.featuresHeader.file_description}");
            output.BreakLine();
            output.WriteLine($"has_psrs_file = {shaderFile.featuresHeader.has_psrs_file}");
            output.WriteLine($"unknown_val = {shaderFile.featuresHeader.unknown_val} values seen (0,1) (likely editor related)");
            var ftHeader = shaderFile.featuresHeader;
            output.WriteLine($"bool flags = ({ftHeader.arg0},{ftHeader.arg1},{ftHeader.arg2},{ftHeader.arg3}," +
                $"{ftHeader.arg4},{ftHeader.arg5},{ftHeader.arg6},{ftHeader.arg7}) (related to editor dependencies)");
            output.WriteLine($"probable minor version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
            output.WriteLine("Editor/Shader compiler stack");
            for (int i = 0; i < ftHeader.editorIDs.Count - 1; i++)
            {
                output.WriteLine($"{ftHeader.editorIDs[i].Item1}    {ftHeader.editorIDs[i].Item2}");
            }
            output.WriteLine($"{ftHeader.editorIDs[^1].Item1}    // Editor ref. ID{ftHeader.editorIDs.Count - 1} " +
                $"- this ID is shared across archives for vcs files with minor-version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
            if (ftHeader.mainParams.Count == 0)
            {
                output.WriteLine("Primary modes");
                output.WriteLine("[default only]");
                return;
            }
            if (ftHeader.mainParams.Count > 1)
            {
                output.WriteLine($"Primary static modes (one of these should be selected)");
            } else
            {
                output.WriteLine($"Primary static modes (has only one default mode)");
            }
            output.DefineHeaders(new string[] { "name", "mode", "config-states" });
            output.AddTabulatedRow(new string[] { "----", "----", "-------------" });
            foreach (var mainParam in ftHeader.mainParams)
            {
                string arg2 = mainParam.Item2.Length == 0 ? "(default)" : mainParam.Item2;
                string configs = mainParam.Item2.Length == 0 ? "(implicit)" : "0,1";
                output.AddTabulatedRow(new string[] { $"{mainParam.Item1}", $"{arg2}", $"{configs}" });
            }
            output.printTabulatedValues();
            output.BreakLine();
        }

        static void PrintPsVsHeader(ShaderFile shaderFile)
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.vspsHeader.vcsFileVersion}");
            output.BreakLine();
            output.WriteLine($"{shaderFile.vcsFileType} ({Path.GetFileName(shaderFile.filenamepath)})");
            output.WriteLine($"probable minor version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
            output.WriteLine("Editor/Shader compiler stack");
            output.WriteLine($"{shaderFile.vspsHeader.fileID0}    // Editor ref. ID0 (produces this file)");
            output.WriteLine($"{shaderFile.vspsHeader.fileID1}    // Editor ref. ID1 " +
                $"- this ID is shared across archives for vcs files with minor-version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
        }

        static void PrintFBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"STATIC-CONFIGURATIONS({shaderFile.sfBlocks.Count})");
            if (shaderFile.sfBlocks.Count == 0)
            {
                output.WriteLine("[none]");
                output.BreakLine();
                return;
            }
            output.DefineHeaders(new string[] { "index", "name", "nr-configs", "config-states", "" });
            foreach (var item in shaderFile.sfBlocks)
            {
                string configStates = "_";
                if (item.arg2 > 0)
                {
                    configStates = "0";
                }
                for (int i = 1; i <= item.arg2; i++)
                {
                    configStates += $",{i}";
                }
                string configStates2 = "";
                if (item.arg2 > 1)
                {
                    configStates2 = $"{CombineStringArray(item.additionalParams.ToArray())}";
                }

                output.AddTabulatedRow(new string[] {$"[{item.blockIndex,2}]", $"{item.name0}", $"{item.arg2+1}",
                    $"{configStates}", $"{configStates2}"});
            }
            output.printTabulatedValues();
            output.BreakLine();
        }


        static void PrintSBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"STATIC-CONFIGURATIONS({shaderFile.sfBlocks.Count})");
            if (shaderFile.sfBlocks.Count == 0)
            {
                output.WriteLine("[none]");
                output.BreakLine();
                return;
            }
            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3", "arg4" });
            foreach (var item in shaderFile.sfBlocks)
            {
                output.AddTabulatedRow(new string[] {$"[{item.blockIndex,2}]", $"{item.name0}", $"{item.arg2}",
                    $"{item.arg3}", $"{item.arg4,2}"});
            }
            output.printTabulatedValues();
            output.BreakLine();
        }

        // todo - bring these in-line with the other printouts
        static void PrintStaticConstraints(ShaderFile shaderFile)
        {
            output.WriteLine("STATIC-CONFIGS INCLUSION/EXCLUSION RULES");

            foreach (SfConstraintsBlock cBlock in shaderFile.sfConstraintsBlocks)
            {
                string[] sfNames = new string[cBlock.range0.Length];
                for (int i = 0; i < sfNames.Length; i++)
                {
                    sfNames[i] = shaderFile.sfBlocks[cBlock.range0[i]].name0;
                }
                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(sfNames, BL);
                string s0 = $"[{cBlock.blockIndex,2}]";
                string s1 = (cBlock.relRule == 1 || cBlock.relRule == 2) ? $"INC({cBlock.relRule})" : $"EXC({cBlock.relRule})";
                string s3 = $"{cBlock.GetByteFlagsAsString()}";
                string s4 = $"{breakNames[0]}";
                string s5 = $"{CombineValues2(cBlock.range0)}";
                string s6 = $"{CombineValues2(cBlock.range1)}";
                string s7 = $"{CombineValues2(cBlock.range2)}";
                string blockSummary = $"{s0.PadRight(7)}{s1.PadRight(10)}{s5.PadRight(16)}{s4.PadRight(BL)}{s6.PadRight(8)}{s7.PadRight(8)}";
                for (int i = 1; i < breakNames.Length; i++)
                {
                    blockSummary += $"\n{(""),7}{(""),10}{(""),16}{breakNames[i],-BL}";
                }
                output.Write(blockSummary);
                output.BreakLine();
            }
            output.BreakLine();
        }

        static string CombineValues2(int[] ints0, bool includeParenth = false)
        {
            if (ints0.Length == 0) return $"_";
            string valueString = "";
            foreach (int i in ints0)
            {
                valueString += $"{i},";
            }
            valueString = valueString[0..^1];
            return includeParenth ? $"({valueString})" : $"{valueString}";
        }


        // todo - bring these in-line with the other printouts
        static void PrintDynamicConfigurations(ShaderFile shaderFile)
        {
            output.WriteLine($"DYNAMIC-CONFIGURATIONS({shaderFile.dBlocks.Count})");
            if (shaderFile.dBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }

            int[] pad = { 7, 40, 7, 7, 7 };
            string h0 = "index";
            string h1 = "name";
            string h2 = "arg2";
            string h3 = "arg3";
            string h4 = "arg4";
            string blockHeader = $"{h0.PadRight(pad[0])} {h1.PadRight(pad[1])} {h2.PadRight(pad[2])} {h3.PadRight(pad[3])} {h4.PadRight(pad[4])}";
            output.WriteLine(blockHeader);
            foreach (DBlock dBlock in shaderFile.dBlocks)
            {
                string v0 = $"[{dBlock.blockIndex,2}]";
                string v1 = dBlock.name0;
                string v2 = "" + dBlock.arg2;
                string v3 = "" + dBlock.arg3;
                string v4 = $"{dBlock.arg4,2}";
                string blockSummary = $"{v0.PadRight(pad[0])} {v1.PadRight(pad[1])} {v2.PadRight(pad[2])} {v3.PadRight(pad[3])} {v4.PadRight(pad[4])}";
                output.WriteLine(blockSummary);
            }
            if (shaderFile.dBlocks.Count == 0)
            {
                output.WriteLine("[empty list]");
            }
            output.BreakLine();
        }

        // todo - bring these in-line with the other printouts
        static void PrintDynamicConstraints(ShaderFile shaderFile)
        {
            output.WriteLine("DYNAMIC-CONFIGS INCLUSION/EXCLUSION RULES");
            if (shaderFile.dConstraintsBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }

            foreach (DConstraintsBlock uBlock in shaderFile.dConstraintsBlocks)
            {
                string[] uknNames = new string[uBlock.flags.Length];
                for (int i = 0; i < uknNames.Length; i++)
                {
                    if (uBlock.flags[i] == 3)
                    {
                        uknNames[i] = shaderFile.dBlocks[uBlock.range0[i]].name0;
                        continue;
                    }
                    if (uBlock.flags[i] == 2)
                    {
                        uknNames[i] = shaderFile.sfBlocks[uBlock.range0[i]].name0;
                        continue;
                    }
                    throw new ShaderParserException($"unknown flag value {uBlock.flags[i]}");
                }
                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(uknNames, BL);
                string s0 = $"[{uBlock.blockIndex,2}]";
                string s1 = (uBlock.relRule == 1 || uBlock.relRule == 2) ? $"INC({uBlock.relRule})" : $"EXC({uBlock.relRule})";
                string s3 = $"{uBlock.ReadByteFlagsAsString()}";
                string s4 = $"{breakNames[0]}";
                string s5 = $"{CombineValues2(uBlock.range0)}";
                string s6 = $"{CombineValues2(uBlock.range1)}";
                string s7 = $"{CombineValues2(uBlock.range2)}";
                string blockSummary = $"{s0,-7}{s1,-10}{s3,-15}{s5,-16}{s4,-BL}{s6,-10}{s7,-8}";
                for (int i = 1; i < breakNames.Length; i++)
                {
                    blockSummary += $"\n{(""),-7}{(""),-10}{(""),-15}{(""),-16}{breakNames[i],-BL}";
                }
                output.Write(blockSummary);
                output.BreakLine();
            }

            output.BreakLine();
        }



        static void PrintParameters(ShaderFile shaderFile)
        {
            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3", "arg4" });
            foreach (var item in shaderFile.sfBlocks)
            {
                output.AddTabulatedRow(new string[] {$"[{item.blockIndex,2}]", $"{item.name0}", $"{item.arg2}",
                    $"{item.arg3}", $"{item.arg4,2}"});
            }
            output.printTabulatedValues();
            output.BreakLine();


            int dynExpCount = 0;
            int indexPad = shaderFile.paramBlocks.Count > 100 ? 3 : 2;

            // parameters
            output.WriteLine($"PARAMETERS({shaderFile.paramBlocks.Count})    *dyn-expressions shown below");
            output.DefineHeaders(new string[] { "index", "name0 | name1 | name2", "type0", "type1", "res", "arg0", "arg1", "arg2", "arg3", "arg4",
                "arg5", "dyn-exp*", "command 0 | 1", "fileref"});
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

                string dynExpExists = param.lead0 == 6 || param.lead0 == 7 ? "true" : "";
                if (dynExpExists.Length > 0)
                {
                    dynExpCount++;
                }

                string c0 = param.command0;
                string c1 = param.command1;
                if (c1.Length > 0)
                {
                    c0 += $" | {c1}";
                }
                output.AddTabulatedRow(new string[] {$"[{(""+param.blockIndex).PadLeft(indexPad)}]", $"{nameCondensed}", $"{param.type}",
                    $"{param.lead0}", $"{param.res0}", $"{BlankMOne(param.arg0),2}", $"{param.arg1,2}", $"{param.arg2}",
                    $"{Pow2Rep(param.arg3),4}", $"{param.arg4,2}", $"{BlankMOne(param.arg5),2}",
                    $"{dynExpExists}", $"{c0}", $"{param.fileref}"});

            }
            output.printTabulatedValues();
            output.BreakLine();

            output.WriteLine($"DYNAMIC EXPRESSIONS({dynExpCount})    (name,type0,type1 reprinted)");
            if (dynExpCount == 0)
            {
                output.WriteLine("[none defined]");
            } else
            {
                output.DefineHeaders(new string[] { "parm-index", "name", "type0", "type1", "dyn-exp" });
                foreach (var param in shaderFile.paramBlocks)
                {
                    if (param.lead0 != 6 && param.lead0 != 7)
                    {
                        continue;
                    }
                    string dynExpstring = new VfxEval().ParseExpression(param.dynExp);
                    output.AddTabulatedRow(new string[] { $"[{(""+param.blockIndex).PadLeft(indexPad)}]",
                        $"{param.name0}", $"{param.type}", $"{param.lead0}", $"{dynExpstring}" });
                }
                output.printTabulatedValues();
            }
            output.BreakLine();

            output.WriteLine("PARAMETERS - Default values and limits    (command0 reprinted)");
            output.WriteLine("(- indicates -infinity, + indicates +infinity)");
            output.DefineHeaders(new string[] { "index", "name", "ints-default", "ints-min", "ints-max", "floats-default", "floats-min", "floats-max",
                "int-args0", "int-args1", "command0"});
            foreach (var param in shaderFile.paramBlocks)
            {
                string fileref = param.fileref;
                int[] r0 = param.ranges0;
                int[] r1 = param.ranges1;
                int[] r2 = param.ranges2;
                float[] r3 = param.ranges3;
                float[] r4 = param.ranges4;
                float[] r5 = param.ranges5;
                int[] r6 = param.ranges6;
                int[] r7 = param.ranges7;
                output.AddTabulatedRow(new string[] { $"[{("" + param.blockIndex).PadLeft(indexPad)}]"
                    ,$"{param.name0}", $"{comb(r0)}", $"{comb(r1)}", $"{comb(r2)}", $"{comb(r3)}", $"{comb(r4)}", $"{comb(r5)}",
                    $"{comb(r6)}", $"{comb(r7)}", $"{param.command0}"});
            }
            output.printTabulatedValues();
            output.BreakLine();
        }


        static void PrintMipmapBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"MIPMAP BLOCKS({shaderFile.mipmapBlocks.Count})");
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
        }


        static void PrintBufferBlocks(ShaderFile shaderFile)
        {
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
        }


        static void PrintVertexSymbolBuffers(ShaderFile shaderFile)
        {
            output.WriteLine($"VERTEX-BUFFER-SYMBOLS({shaderFile.symbolBlocks.Count})");
            if (shaderFile.symbolBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            // find best padding
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
            output.BreakLine();
        }


        static void PrintZFrames(ShaderFile shaderFile)
        {
            string zframesHeader = $"ZFRAMES({shaderFile.GetZFrameCount()})";
            output.WriteLine(zframesHeader);
            if (shaderFile.GetZFrameCount() == 0)
            {
                string infoText = "";
                if (shaderFile.vcsFileType == VcsFileType.Features)
                {
                    infoText = "(features files in general don't contain zframes)";
                }
                output.WriteLine($"[none defined] {infoText}");
                output.BreakLine();
                return;
            }

            FileTokens fileTokens = new FileTokens(shaderFile.filenamepath);
            // print the config headers every 100 frames
            int zframeCount = 0;
            // print the zframes
            string zFrameBaseDir = $"/vcs-all/{GetCoreOrDotaString(shaderFile.filenamepath)}/zsource/";
            // prepare the lookup to determine configuration state
            CompatRulesGeneration configGen = new(shaderFile);
            output.WriteLine(new string('-', zframesHeader.Length));

            // collect names in the order they appear
            List<string> sfNames = new();
            List<string> abbreviations = new();

            foreach (var sfBlock in shaderFile.sfBlocks)
            {
                string sfShortName = ShortenShaderParam(sfBlock.name0).ToLower();
                abbreviations.Add($"{sfBlock.name0}({sfShortName})");
                sfNames.Add(sfShortName);
            }

            string[] breakabbreviations = CombineValuesBreakString(abbreviations.ToArray(), 120);
            foreach (string abbr in breakabbreviations)
            {
                output.WriteLine(abbr.Replace("(", "<span style='color: blue'>(").Replace(")", "</span>)"));
            }
            output.BreakLine();

            string configHeader = CombineStringsSpaceSep(sfNames.ToArray(), 6);
            configHeader = $"{new string(' ', 14)}{configHeader}";
            foreach (var item in shaderFile.zframesLookup)
            {
                if (zframeCount % 100 == 0)
                {
                    output.WriteLine($"{configHeader}");
                }
                int[] configState = configGen.GetConfigState(item.Key);
                string zframeLink = fileTokens.GetBestZframesLink(item.Key);
                output.WriteLine($"{zframeLink} {CombineIntsSpaceSep(configState, 6)}");
                zframeCount++;
            }
        }





        static void PrintSingleFileAnalysisZFrames(string outputFilenamepath = null, bool writeFile = false)
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

            FileTokens fileTokens = new FileTokens(shaderFile.filenamepath);
            // print the config headers every 100 frames
            int zframeCount = 0;
            // print the zframes
            string zFrameBaseDir = $"/vcs-all/{GetCoreOrDotaString(shaderFile.filenamepath)}/zsource/";
            // prepare the lookup to determine configuration state
            CompatRulesGeneration configGen = new(shaderFile);

            string zframesHeader = $"ZFRAMES ({shaderFile.GetZFrameCount()})";
            output.WriteLine(zframesHeader);
            output.WriteLine(new string('-', zframesHeader.Length));

            // collect names in the order they appear
            List<string> sfNames = new();
            List<string> abbreviations = new();

            foreach (var sfBlock in shaderFile.sfBlocks)
            {
                string sfShortName = ShortenShaderParam(sfBlock.name0).ToLower();
                abbreviations.Add($"{sfBlock.name0}({sfShortName})");
                sfNames.Add(sfShortName);
            }

            string[] breakabbreviations = CombineValuesBreakString(abbreviations.ToArray(), 120);
            foreach (string abbr in breakabbreviations)
            {
                output.WriteLine(abbr.Replace("(", "<span style='color: blue'>(").Replace(")", "</span>)"));
            }
            output.BreakLine();

            string configHeader = CombineStringsSpaceSep(sfNames.ToArray(), 6);
            configHeader = $"{new string(' ', 14)}{configHeader}";
            foreach (var item in shaderFile.zframesLookup)
            {
                if (zframeCount % 100 == 0)
                {
                    output.WriteLine($"{configHeader}");
                }
                int[] configState = configGen.GetConfigState(item.Key);
                string zframeLink = fileTokens.GetBestZframesLink(item.Key);
                output.WriteLine($"{zframeLink} {CombineIntsSpaceSep(configState, 6)}");
                zframeCount++;
            }
        }







        static void PrintSingleFileAnalysisBufferViews(string outputFilenamepath = null, bool writeFile = false)
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs"; // doesn't have any mipmaps
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
            if (shaderFile.symbolBlocks.Count == 0)
            {
                output.WriteLine("[none]");
            }
            // find best padding
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
            output.BreakLine();


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





