using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis
{
    public class PrintoutsSingleFile
    {
        const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";
        static OutputWriter output = new();


        public static void RunTrials()
        {

            // PrintAllFiles();
            TestBatchPrinting(); // (works for single file)
            // Trial1();
            // Trial2();
            // Trial3();
            output.CloseStreamWriter();
        }


        static void Trial3()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_40_ps.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);

            Console.WriteLine($"{fileTokens.GetAbbreviatedName()}");
        }


        static void Trial2()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            foreach (var f in GetRelatedFiles(filenamepath))
            {
                Console.WriteLine($"{f}");
            }
        }

        static void Trial1()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_psrs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/msaa_resolve_cs_pcgl_50_features.vcs"; // strange file that doesn't contain any data
            PrintSingleFileFullSummary(filenamepath, $"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
        }



        static void PrintAllFiles()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsProgramType.Undetermined, -1);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
                string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
                PrintSingleFileFullSummary(filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
                output.CloseStreamWriter();
            }

        }


        static void TestBatchPrinting()
        {

            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, null, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, null, VcsFileType.Any, -1);

            List<string> vcsFiles = new();
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/water_dota_pcgl_30_ps.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/visualize_physics_pcgl_40_gs.vcs");
            vcsFiles.Add($"{DOTA_CORE_PCGL_SOURCE}/visualize_physics_pcgl_40_gs.vcs");
            // vcsFiles.Add($"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/depth_only_pc_40_vs.vcs");


            FileTokens fileTokens = new FileTokens(vcsFiles[0]);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            PrintSingleFileFullSummary(fileTokens.filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
        }




        static void PrintSingleFileFullSummary(string filenamepath, string outputFilenamepath = null,
            bool writeFile = false, bool disableOutput = false)
        {
            FileTokens fileTokens = new FileTokens(filenamepath);
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml(fileTokens.GetAbbreviatedName(), $"{ShortHandName(filenamepath)}");
                if (disableOutput)
                {
                    output.DisableOutput();
                }
            }
            ShaderFile shaderFile = null;
            try
            {
                shaderFile = InstantiateShaderFile(filenamepath);
            } catch (Exception)
            {
                Console.WriteLine($"ERROR! couldn't parse this file {filenamepath}");
                return;
            }
            if (shaderFile.vcsProgramType == VcsProgramType.Features)
            {
                PrintFeaturesHeader(shaderFile, fileTokens);
                PrintFBlocks(shaderFile);
            } else
            {
                PrintPsVsHeader(shaderFile, fileTokens);
                PrintSBlocks(shaderFile);
            }

            if (shaderFile.vcsProgramType == VcsProgramType.PixelShader)
            {

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

        private static void PrintFeaturesHeader(ShaderFile shaderFile, FileTokens fileTokens)
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.featuresHeader.vcsFileVersion}");
            output.BreakLine();
            output.Write($"Features Detail ({Path.GetFileName(shaderFile.filenamepath)})");

            output.WriteLine($" (byte version <a href='{fileTokens.GetServerFileUrl("bytes")}'>{fileTokens.filename}</a>)");
            string[] relatedFiles = GetRelatedFiles(fileTokens.filenamepath);
            if (relatedFiles.Length > 0)
            {
                output.Write("Related files:");
                foreach (var f in relatedFiles)
                {
                    FileTokens fT = new FileTokens(f);
                    output.Write($" <a href='{fT.GetServerFileUrl("summary2")}'>{fT.filename}</a>");
                }
                output.BreakLine();
            }


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
                output.WriteLine($"Primary static modes (this file has only one default mode)");
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

        private static void PrintPsVsHeader(ShaderFile shaderFile, FileTokens fileTokens)
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.vspsHeader.vcsFileVersion}");
            output.BreakLine();
            output.Write($"{shaderFile.vcsProgramType} ({Path.GetFileName(shaderFile.filenamepath)})");
            output.WriteLine($" (byte version <a href='{fileTokens.GetServerFileUrl("bytes")}'>{fileTokens.filename}</a>)");
            string[] relatedFiles = GetRelatedFiles(fileTokens.filenamepath);
            if (relatedFiles.Length > 0)
            {
                output.Write("Related files:");
                foreach (var f in relatedFiles)
                {
                    FileTokens fT = new FileTokens(f);
                    output.Write($" <a href='{fT.GetServerFileUrl("summary2")}'>{fT.filename}</a>");
                }
                output.BreakLine();
            }

            output.WriteLine($"probable minor version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
            output.WriteLine("Editor/Shader compiler stack");
            output.WriteLine($"{shaderFile.vspsHeader.fileID0}    // Editor ref. ID0 (produces this file)");
            output.WriteLine($"{shaderFile.vspsHeader.fileID1}    // Editor ref. ID1 " +
                $"- this ID is shared across archives for vcs files with minor-version = {shaderFile.possibleMinorVersion}");
            output.BreakLine();
        }

        private static void PrintFBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"FEATURE/STATIC-CONFIGURATIONS({shaderFile.sfBlocks.Count})");
            if (shaderFile.sfBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
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

        private static void PrintSBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"STATIC-CONFIGURATIONS({shaderFile.sfBlocks.Count})");
            if (shaderFile.sfBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
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
        private static void PrintStaticConstraints(ShaderFile shaderFile)
        {
            output.WriteLine("STATIC-CONFIGS INCLUSION/EXCLUSION RULES");
            if (shaderFile.sfConstraintsBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
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
                string s5 = $"{CombineIntArray(cBlock.range0)}";
                string s6 = $"{CombineIntArray(cBlock.range1)}";
                string s7 = $"{CombineIntArray(cBlock.range2)}";
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

        // todo - bring these in-line with the other printouts
        private static void PrintDynamicConfigurations(ShaderFile shaderFile)
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
        private static void PrintDynamicConstraints(ShaderFile shaderFile)
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
                string s5 = $"{CombineIntArray(uBlock.range0)}";
                string s6 = $"{CombineIntArray(uBlock.range1)}";
                string s7 = $"{CombineIntArray(uBlock.range2)}";
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

        private static void PrintParameters(ShaderFile shaderFile)
        {
            if (shaderFile.paramBlocks.Count == 0)
            {
                output.WriteLine($"PARAMETERS(0)");
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }

            int dynExpCount = 0;
            int indexPad = shaderFile.paramBlocks.Count > 100 ? 3 : 2;
            // parameters
            output.WriteLine($"PARAMETERS({shaderFile.paramBlocks.Count})    *dyn-expressions shown separately");
            output.DefineHeaders(new string[] { "index", "name0 | name1 | name2", "type0", "type1", "res", "arg0", "arg1",
                "arg2", "arg3", "arg4", "arg5", "dyn-exp*", "command 0|1", "fileref"});
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
                if (nameCondensed.Length > 65)
                {
                    string[] tokens = nameCondensed.Split("|");
                    nameCondensed = tokens[0].Trim();
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        nameCondensed += $"\n{tokens[i].Trim()}";
                    }
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
                    $"{param.lead0}", $"{param.res0}", $"{BlankNegOne(param.arg0),2}", $"{param.arg1,2}", $"{param.arg2}",
                    $"{Pow2Rep(param.arg3),4}", $"{param.arg4,2}", $"{BlankNegOne(param.arg5),2}",
                    $"{dynExpExists}", $"{c0}", $"{param.fileref}"});
            }
            output.printTabulatedValues(spacing: 1);
            output.BreakLine();
            if (dynExpCount == 0)
            {
                output.WriteLine($"DYNAMIC EXPRESSIONS({dynExpCount})");
                output.WriteLine("[none defined]");
            } else
            {
                output.WriteLine($"DYNAMIC EXPRESSIONS({dynExpCount})    (name0,type0,type1,arg0,arg1,arg2,arg4,arg5 reprinted)");
                output.DefineHeaders(new string[] { "param-index", "name0", "t0,t1,a0,a1,a2,a4,a5  ", "dyn-exp" });
                foreach (var param in shaderFile.paramBlocks)
                {
                    if (param.lead0 != 6 && param.lead0 != 7)
                    {
                        continue;
                    }
                    string dynExpstring = ParseDynamicExpression(param.dynExp);
                    output.AddTabulatedRow(new string[] { $"[{(""+param.blockIndex).PadLeft(indexPad)}]",
                        $"{param.name0}",
                        $"{param.type,2},{param.lead0,2},{BlankNegOne(param.arg0),2},{param.arg1,2},{param.arg2,2},{param.arg4,2},{BlankNegOne(param.arg5),2}",
                        $"{dynExpstring}" });
                }
                output.printTabulatedValues();
            }
            output.BreakLine();
            output.WriteLine("PARAMETERS - Default values and limits    (type0,type1,arg0,arg1,arg2,arg4,arg5,command0 reprinted)");
            output.WriteLine("(- indicates -infinity, + indicates +infinity, def. = default)");
            output.DefineHeaders(new string[] { "index", "name0", "t0,t1,a0,a1,a2,a4,a5  ", "ints-def.", "ints-min", "ints-max",
                "floats-def.", "floats-min", "floats-max", "int-args0", "int-args1", "command0", "fileref", "dyn-exp"});
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
                string hasFileRef = param.fileref.Length > 0 ? "true" : "";
                string hasDynExp = param.lead0 == 6 || param.lead0 == 7 ? "true" : "";
                output.AddTabulatedRow(new string[] { $"[{("" + param.blockIndex).PadLeft(indexPad)}]", $"{param.name0}",
                    $"{param.type,2},{param.lead0,2},{BlankNegOne(param.arg0),2},{param.arg1,2},{param.arg2,2},{param.arg4,2},{BlankNegOne(param.arg5),2}",
                    $"{comb(r0)}", $"{comb(r1)}", $"{comb(r2)}", $"{comb(r3)}", $"{comb(r4)}",
                    $"{comb(r5)}", $"{comb(r6)}", $"{comb(r7)}", $"{param.command0}", $"{hasFileRef}", $"{hasDynExp}"});
            }
            output.printTabulatedValues(spacing: 1);
            output.BreakLine();
        }

        private static void PrintMipmapBlocks(ShaderFile shaderFile)
        {
            output.WriteLine($"MIPMAP BLOCKS({shaderFile.mipmapBlocks.Count})");
            if (shaderFile.mipmapBlocks.Count > 0)
            {
                output.DefineHeaders(new string[] { "index", "name", "arg0", "arg1", "arg2", "arg3", "arg4", "arg5" });
            } else
            {
                output.DefineHeaders(Array.Empty<string>());
                output.WriteLine("[none defined]");
            }
            foreach (var mipmap in shaderFile.mipmapBlocks)
            {
                output.AddTabulatedRow(new string[] { $"[{mipmap.blockIndex,2}]", $"{mipmap.name}",
                    $"{BytesToString(mipmap.arg0),-14}", $"{mipmap.arg1,2}", $"{BlankNegOne(mipmap.arg2),2}",
                    $"{BlankNegOne(mipmap.arg3),2}", $"{BlankNegOne(mipmap.arg4),2}", $"{mipmap.arg5,2}" });
            }
            output.printTabulatedValues();
            output.BreakLine();
        }

        private static void PrintBufferBlocks(ShaderFile shaderFile)
        {
            if (shaderFile.bufferBlocks.Count == 0)
            {
                output.WriteLine("BUFFER-BLOCKS(0)");
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
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

        private static void PrintVertexSymbolBuffers(ShaderFile shaderFile)
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

        private static void PrintZFrames(ShaderFile shaderFile)
        {
            string zframesHeader = $"ZFRAMES({shaderFile.GetZFrameCount()})";
            output.WriteLine(zframesHeader);
            if (shaderFile.GetZFrameCount() == 0)
            {
                string infoText = "";
                if (shaderFile.vcsProgramType == VcsProgramType.Features)
                {
                    infoText = "(Features files in general don't contain zframes)";
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
            ConfigMappingSParams configGen = new(shaderFile);
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

        private static string BlankNegOne(int val)
        {
            if (val == -1)
            {
                return "_";
            }
            return "" + val;
        }

        private static string Pow2Rep(int val)
        {
            int orig = val;
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


    }
}





