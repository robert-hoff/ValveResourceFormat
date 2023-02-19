using System;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;

/*
 *
 * Printouts formatted vcs files - replaced by production file PrintVcsFileSummary
 *
 *
 */
namespace MyShaderAnalysis.codestash
{
    public class PrintoutSingleFile
    {
        private OutputFormatterTabulatedData output;
        private string filenamepath;
        private FileTokensOld fileTokens;
        private ShaderFile shaderFile;

        /* todo - I'm not doing anything currently with the printHtmlLinks variable
         *
         *
         * (i.e. it output to HTML is always the case and hardcoded in)
         * NOTE - this file is almost the same as MyShaderFile.CompiledShader.PrintVcsFileSummary
         * only difference is the output is conditioned for HTML versus Windows GUI (using richbox links)
         *
         * NOTE - this file was previously named StaticAnalysis3
         *
         *
         *
         */
        public PrintoutSingleFile(string filenamepath, HandleOutputWrite OutputWriter = null, bool printHtmlLinks = true)
        {
            output = new OutputFormatterTabulatedData(OutputWriter);
            this.filenamepath = filenamepath;
            fileTokens = new FileTokensOld(filenamepath);
            try
            {
                shaderFile = InstantiateShaderFile(filenamepath);
            } catch (Exception e)
            {
                Console.WriteLine($"ERROR! couldn't parse this file {filenamepath} {e.Message}");
                return;
            }
            if (shaderFile.VcsProgramType == VcsProgramType.Features)
            {
                PrintFeaturesHeader();
                PrintFBlocks();
            } else
            {
                PrintPsVsHeader();
                PrintSBlocks();
            }
            PrintStaticConstraints();
            PrintDynamicConfigurations();
            PrintDynamicConstraints();
            PrintParameters();
            PrintMipmapBlocks();
            PrintBufferBlocks();
            PrintVertexSymbolBuffers();
            PrintZFrames();
        }

        private void PrintFeaturesHeader()
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.FeaturesHeader.VcsFileVersion}");
            output.BreakLine();
            output.Write($"Features Detail ({Path.GetFileName(shaderFile.FilenamePath)})");
            output.WriteLine($" (byte version <a href='{fileTokens.GetServerFileUrl("bytes")}'>{fileTokens.filename}</a>)");
            string[] relatedFiles = GetRelatedFiles(fileTokens.filenamepath);
            if (relatedFiles.Length > 0)
            {
                output.Write("Related files:");
                foreach (var f in relatedFiles)
                {
                    FileTokensOld fT = new FileTokensOld(f);
                    output.Write($" <a href='{fT.GetServerFileUrl("summary2")}'>{fT.filename}</a>");
                }
                output.BreakLine();
            }
            output.WriteLine($"VFX File Desc: {shaderFile.FeaturesHeader.FileDescription}");
            output.BreakLine();
            output.WriteLine($"has_psrs_file = {shaderFile.FeaturesHeader.HasPsrsFile}");
            output.WriteLine($"unknown_val = {shaderFile.FeaturesHeader.Version} values seen (0,1) (likely editor related)");
            var ftHeader = shaderFile.FeaturesHeader;
            output.WriteLine($"bool flags = ({ftHeader.DevShader},{ftHeader.Arg1},{ftHeader.Arg2},{ftHeader.Arg3}," +
                $"{ftHeader.Arg4},{ftHeader.Arg5},{ftHeader.Arg6},{ftHeader.Arg7}) (related to editor dependencies)");
            output.WriteLine($"probable minor version = {shaderFile.PossibleEditorDescription}");
            output.BreakLine();
            output.WriteLine("Editor/Shader compiler stack");
            for (int i = 0; i < ftHeader.EditorIDs.Count - 1; i++)
            {
                output.WriteLine($"{ftHeader.EditorIDs[i].Item1}    {ftHeader.EditorIDs[i].Item2}");
            }
            output.WriteLine($"{ftHeader.EditorIDs[^1].Item1}    // Editor ref. ID{ftHeader.EditorIDs.Count - 1} " +
                $"- this ID is shared across archives for vcs files with minor-version = {shaderFile.PossibleEditorDescription}");
            output.BreakLine();
            if (ftHeader.MainParams.Count == 0)
            {
                output.WriteLine("Primary modes");
                output.WriteLine("[default only]");
                return;
            }
            if (ftHeader.MainParams.Count > 1)
            {
                output.WriteLine($"Primary static modes (one of these should be selected)");
            } else
            {
                output.WriteLine($"Primary static modes (this file has only one default mode)");
            }
            output.DefineHeaders(new string[] { "name", "mode", "config-states" });
            output.AddTabulatedRow(new string[] { "----", "----", "-------------" });
            foreach (var mainParam in ftHeader.MainParams)
            {
                string arg2 = mainParam.Item2.Length == 0 ? "(default)" : mainParam.Item2;
                string configs = mainParam.Item2.Length == 0 ? "(implicit)" : "0,1";
                output.AddTabulatedRow(new string[] { $"{mainParam.Item1}", $"{arg2}", $"{configs}" });
            }
            output.PrintTabulatedValues();
            output.BreakLine();
        }

        private void PrintPsVsHeader()
        {
            output.WriteLine($"Valve Compiled Shader 2 (vcs2), version {shaderFile.VspsHeader.VcsFileVersion}");
            output.BreakLine();
            output.Write($"{shaderFile.VcsProgramType} ({Path.GetFileName(shaderFile.FilenamePath)})");
            output.WriteLine($" (byte version <a href='{fileTokens.GetServerFileUrl("bytes")}'>{fileTokens.filename}</a>)");
            string[] relatedFiles = GetRelatedFiles(fileTokens.filenamepath);
            if (relatedFiles.Length > 0)
            {
                output.Write("Related files:");
                foreach (var f in relatedFiles)
                {
                    FileTokensOld fT = new FileTokensOld(f);
                    output.Write($" <a href='{fT.GetServerFileUrl("summary2")}'>{fT.filename}</a>");
                }
                output.BreakLine();
            }
            output.WriteLine($"probable minor version = {shaderFile.PossibleEditorDescription}");
            output.BreakLine();
            output.WriteLine("Editor/Shader compiler stack");
            output.WriteLine($"{shaderFile.VspsHeader.FileID0}    // Editor ref. ID0 (produces this file)");
            output.WriteLine($"{shaderFile.VspsHeader.FileID1}    // Editor ref. ID1 " +
                $"- this ID is shared across archives for vcs files with minor-version = {shaderFile.PossibleEditorDescription}");
            output.BreakLine();
        }

        private void PrintFBlocks()
        {
            output.WriteLine($"FEATURE/STATIC-CONFIGURATIONS({shaderFile.SfBlocks.Count})");
            if (shaderFile.SfBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            output.DefineHeaders(new string[] { "index", "name", "nr-configs", "config-states", "" });
            foreach (var item in shaderFile.SfBlocks)
            {
                string configStates = "_";
                if (item.Arg2 > 0)
                {
                    configStates = "0";
                }
                for (int i = 1; i <= item.Arg2; i++)
                {
                    configStates += $",{i}";
                }
                string configStates2 = "";
                if (item.Arg2 > 1)
                {
                    configStates2 = $"{CombineStringArray(item.AdditionalParams.ToArray())}";
                }

                output.AddTabulatedRow(new string[] {$"[{item.BlockIndex,2}]", $"{item.Name0}", $"{item.Arg2+1}",
                    $"{configStates}", $"{configStates2}"});
            }
            output.PrintTabulatedValues();
            output.BreakLine();
        }

        private void PrintSBlocks()
        {
            output.WriteLine($"STATIC-CONFIGURATIONS({shaderFile.SfBlocks.Count})");
            if (shaderFile.SfBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3", "arg4" });
            foreach (var item in shaderFile.SfBlocks)
            {
                output.AddTabulatedRow(new string[] {$"[{item.BlockIndex,2}]", $"{item.Name0}", $"{item.Arg2}",
                    $"{item.Arg3}", $"{item.Arg4,2}"});
            }
            output.PrintTabulatedValues();
            output.BreakLine();
        }

        // todo - bring these in-line with the other printouts
        private void PrintStaticConstraints()
        {
            output.WriteLine("STATIC-CONFIGS INCLUSION/EXCLUSION RULES");
            if (shaderFile.SfConstraintsBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            foreach (SfConstraintsBlock cBlock in shaderFile.SfConstraintsBlocks)
            {
                string[] sfNames = new string[cBlock.Range0.Length];
                for (int i = 0; i < sfNames.Length; i++)
                {
                    sfNames[i] = shaderFile.SfBlocks[cBlock.Range0[i]].Name0;
                }
                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(sfNames, BL);
                string s0 = $"[{cBlock.BlockIndex,2}]";
                string s1 = (cBlock.RelRule == 1 || cBlock.RelRule == 2) ? $"INC({cBlock.RelRule})" : $"EXC({cBlock.RelRule})";
                string s3 = $"{cBlock.GetByteFlagsAsString()}";
                string s4 = $"{breakNames[0]}";
                string s5 = $"{CombineIntArray(cBlock.Range0)}";
                string s6 = $"{CombineIntArray(cBlock.Range1)}";
                string s7 = $"{CombineIntArray(cBlock.Range2)}";
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
        private void PrintDynamicConfigurations()
        {
            output.WriteLine($"DYNAMIC-CONFIGURATIONS({shaderFile.DBlocks.Count})");
            if (shaderFile.DBlocks.Count == 0)
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
            foreach (DBlock dBlock in shaderFile.DBlocks)
            {
                string v0 = $"[{dBlock.BlockIndex,2}]";
                string v1 = dBlock.Name0;
                string v2 = "" + dBlock.Arg2;
                string v3 = "" + dBlock.Arg3;
                string v4 = $"{dBlock.Arg4,2}";
                string blockSummary = $"{v0.PadRight(pad[0])} {v1.PadRight(pad[1])} {v2.PadRight(pad[2])} {v3.PadRight(pad[3])} {v4.PadRight(pad[4])}";
                output.WriteLine(blockSummary);
            }
            if (shaderFile.DBlocks.Count == 0)
            {
                output.WriteLine("[empty list]");
            }
            output.BreakLine();
        }

        // todo - bring these in-line with the other printouts
        private void PrintDynamicConstraints()
        {
            output.WriteLine("DYNAMIC-CONFIGS INCLUSION/EXCLUSION RULES");
            if (shaderFile.DConstraintsBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            foreach (DConstraintsBlock uBlock in shaderFile.DConstraintsBlocks)
            {
                string[] uknNames = new string[uBlock.Flags.Length];
                for (int i = 0; i < uknNames.Length; i++)
                {
                    if (uBlock.Flags[i] == 3)
                    {
                        uknNames[i] = shaderFile.DBlocks[uBlock.Range0[i]].Name0;
                        continue;
                    }
                    if (uBlock.Flags[i] == 2)
                    {
                        uknNames[i] = shaderFile.SfBlocks[uBlock.Range0[i]].Name0;
                        continue;
                    }
                    throw new ShaderParserException($"unknown flag value {uBlock.Flags[i]}");
                }
                const int BL = 70;
                string[] breakNames = CombineValuesBreakString(uknNames, BL);
                string s0 = $"[{uBlock.BlockIndex,2}]";
                string s1 = (uBlock.RelRule == 1 || uBlock.RelRule == 2) ? $"INC({uBlock.RelRule})" : $"EXC({uBlock.RelRule})";
                string s3 = $"{uBlock.ReadByteFlagsAsString()}";
                string s4 = $"{breakNames[0]}";
                string s5 = $"{CombineIntArray(uBlock.Range0)}";
                string s6 = $"{CombineIntArray(uBlock.Range1)}";
                string s7 = $"{CombineIntArray(uBlock.Range2)}";
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

        private void PrintParameters()
        {
            if (shaderFile.ParamBlocks.Count == 0)
            {
                output.WriteLine($"PARAMETERS(0)");
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }

            int dynExpCount = 0;
            int indexPad = shaderFile.ParamBlocks.Count > 100 ? 3 : 2;
            // parameters
            output.WriteLine($"PARAMETERS({shaderFile.ParamBlocks.Count})    *dyn-expressions shown separately");
            output.DefineHeaders(new string[] { "index", "name0 | name1 | name2", "type0", "type1", "res", "arg0", "arg1",
                "arg2", "arg3", "arg4", "arg5", "dyn-exp*", "command 0|1", "fileref"});
            foreach (var param in shaderFile.ParamBlocks)
            {
                string nameCondensed = param.Name0;
                if (param.Name1.Length > 0)
                {
                    nameCondensed += $" | {param.Name1}";
                }
                if (param.Name2.Length > 0)
                {
                    nameCondensed += $" | {param.Name2}(2)";
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
                string dynExpExists = param.Lead0 == 6 || param.Lead0 == 7 ? "true" : "";
                if (dynExpExists.Length > 0)
                {
                    dynExpCount++;
                }
                string c0 = param.Command0;
                string c1 = param.Command1;
                if (c1.Length > 0)
                {
                    c0 += $" | {c1}";
                }
                output.AddTabulatedRow(new string[] {$"[{(""+param.BlockIndex).PadLeft(indexPad)}]", $"{nameCondensed}", $"{param.Type}",
                    $"{param.Lead0}", $"{param.Res0}", $"{BlankNegOne(param.Arg0),2}", $"{param.Arg1,2}", $"{param.Arg2}",
                    $"{Pow2Rep(param.Arg3),4}", $"{param.Arg4,2}", $"{BlankNegOne(param.Arg5),2}",
                    $"{dynExpExists}", $"{c0}", $"{param.FileRef}"});
            }
            output.PrintTabulatedValues(spacing: 1);
            output.BreakLine();
            if (dynExpCount == 0)
            {
                output.WriteLine($"DYNAMIC EXPRESSIONS({dynExpCount})");
                output.WriteLine("[none defined]");
            } else
            {
                output.WriteLine($"DYNAMIC EXPRESSIONS({dynExpCount})    (name0,type0,type1,arg0,arg1,arg2,arg4,arg5 reprinted)");
                output.DefineHeaders(new string[] { "param-index", "name0", "t0,t1,a0,a1,a2,a4,a5  ", "dyn-exp" });
                foreach (var param in shaderFile.ParamBlocks)
                {
                    if (param.Lead0 != 6 && param.Lead0 != 7)
                    {
                        continue;
                    }
                    string dynExpstring = ParseDynamicExpression(param.DynExp);
                    output.AddTabulatedRow(new string[] { $"[{(""+param.BlockIndex).PadLeft(indexPad)}]",
                        $"{param.Name0}",
                        $"{param.Type,2},{param.Lead0,2},{BlankNegOne(param.Arg0),2},{param.Arg1,2},{param.Arg2,2},{param.Arg4,2},{BlankNegOne(param.Arg5),2}",
                        $"{dynExpstring}" });
                }
                output.PrintTabulatedValues();
            }
            output.BreakLine();
            output.WriteLine("PARAMETERS - Default values and limits    (type0,type1,arg0,arg1,arg2,arg4,arg5,command0 reprinted)");
            output.WriteLine("(- indicates -infinity, + indicates +infinity, def. = default)");
            output.DefineHeaders(new string[] { "index", "name0", "t0,t1,a0,a1,a2,a4,a5  ", "ints-def.", "ints-min", "ints-max",
                "floats-def.", "floats-min", "floats-max", "int-args0", "int-args1", "command0", "fileref", "dyn-exp"});
            foreach (var param in shaderFile.ParamBlocks)
            {
                string fileref = param.FileRef;
                int[] r0 = param.Ranges0;
                int[] r1 = param.Ranges1;
                int[] r2 = param.Ranges2;
                float[] r3 = param.Ranges3;
                float[] r4 = param.Ranges4;
                float[] r5 = param.Ranges5;
                int[] r6 = param.Ranges6;
                int[] r7 = param.Ranges7;
                string hasFileRef = param.FileRef.Length > 0 ? "true" : "";
                string hasDynExp = param.Lead0 == 6 || param.Lead0 == 7 ? "true" : "";
                output.AddTabulatedRow(new string[] { $"[{("" + param.BlockIndex).PadLeft(indexPad)}]", $"{param.Name0}",
                    $"{param.Type,2},{param.Lead0,2},{BlankNegOne(param.Arg0),2},{param.Arg1,2},{param.Arg2,2},{param.Arg4,2},{BlankNegOne(param.Arg5),2}",
                    $"{Comb(r0)}", $"{Comb(r1)}", $"{Comb(r2)}", $"{Comb(r3)}", $"{Comb(r4)}",
                    $"{Comb(r5)}", $"{Comb(r6)}", $"{Comb(r7)}", $"{param.Command0}", $"{hasFileRef}", $"{hasDynExp}"});
            }
            output.PrintTabulatedValues(spacing: 1);
            output.BreakLine();
        }

        private void PrintMipmapBlocks()
        {
            output.WriteLine($"MIPMAP BLOCKS({shaderFile.MipmapBlocks.Count})");
            if (shaderFile.MipmapBlocks.Count > 0)
            {
                output.DefineHeaders(new string[] { "index", "name", "arg0", "arg1", "arg2", "arg3", "arg4", "arg5" });
            } else
            {
                output.DefineHeaders(Array.Empty<string>());
                output.WriteLine("[none defined]");
            }
            foreach (var mipmap in shaderFile.MipmapBlocks)
            {
                output.AddTabulatedRow(new string[] { $"[{mipmap.BlockIndex,2}]", $"{mipmap.Name}",
                    $"{BytesToString(mipmap.Arg0),-14}", $"{mipmap.Arg1,2}", $"{BlankNegOne(mipmap.Arg2),2}",
                    $"{BlankNegOne(mipmap.Arg3),2}", $"{BlankNegOne(mipmap.Arg4),2}", $"{mipmap.Arg5,2}" });
            }
            output.PrintTabulatedValues();
            output.BreakLine();
        }

        private void PrintBufferBlocks()
        {
            if (shaderFile.BufferBlocks.Count == 0)
            {
                output.WriteLine("BUFFER-BLOCKS(0)");
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            foreach (var bufferBlock in shaderFile.BufferBlocks)
            {
                output.WriteLine($"BUFFER-BLOCK[{bufferBlock.BlockIndex}]");
                output.WriteLine($"{bufferBlock.Name} size={bufferBlock.BufferSize} param-count={bufferBlock.ParamCount}" +
                    $" arg0={bufferBlock.Arg0} crc32={bufferBlock.BlockCrc:x08}");
                output.DefineHeaders(new string[] { "       ", "name", "offset", "vertex-size", "attrib-count", "data-count" });
                foreach (var bufferParams in bufferBlock.BufferParams)
                {
                    string name = bufferParams.Item1;
                    int bOffset = bufferParams.Item2;
                    int nrVertices = bufferParams.Item3;
                    int nrAttribs = bufferParams.Item4;
                    int length = bufferParams.Item5;
                    output.AddTabulatedRow(new string[] { "", $"{name}", $"{bOffset,3}", $"{nrVertices,3}", $"{nrAttribs,3}", $"{length,3}" });
                }
                output.PrintTabulatedValues();
                output.BreakLine();
            }
        }

        private void PrintVertexSymbolBuffers()
        {
            output.WriteLine($"VERTEX-BUFFER-SYMBOLS({shaderFile.SymbolBlocks.Count})");
            if (shaderFile.SymbolBlocks.Count == 0)
            {
                output.WriteLine("[none defined]");
                output.BreakLine();
                return;
            }
            // find best padding
            int namePad = 0;
            int typePad = 0;
            int optionPad = 0;
            foreach (var symbolBlock in shaderFile.SymbolBlocks)
            {
                foreach (var symbolsDef in symbolBlock.SymbolsDefinition)
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
            foreach (var symbolBlock in shaderFile.SymbolBlocks)
            {
                output.WriteLine($"VERTEX-SYMBOLS[{symbolBlock.BlockIndex}] definitions={symbolBlock.SymbolsCount}");
                output.DefineHeaders(new string[] { "       ", "name".PadRight(namePad), "type".PadRight(typePad),
                    $"option".PadRight(optionPad), "semantic-index" });
                foreach (var symbolsDef in symbolBlock.SymbolsDefinition)
                {
                    string name = symbolsDef.Item1;
                    string type = symbolsDef.Item2;
                    string option = symbolsDef.Item3;
                    int semanticIndex = symbolsDef.Item4;
                    output.AddTabulatedRow(new string[] { "", $"{name}", $"{type}", $"{option}", $"{semanticIndex,2}" });
                }
                output.PrintTabulatedValues();
                output.BreakLine();
            }
            output.BreakLine();
        }

        private void PrintZFrames()
        {
            string zframesHeader = $"ZFRAMES({shaderFile.GetZFrameCount()})";
            output.WriteLine(zframesHeader);
            if (shaderFile.GetZFrameCount() == 0)
            {
                string infoText = "";
                if (shaderFile.VcsProgramType == VcsProgramType.Features)
                {
                    infoText = "(Features files in general don't contain zframes)";
                }
                output.WriteLine($"[none defined] {infoText}");
                output.BreakLine();
                return;
            }
            FileTokensOld fileTokens = new FileTokensOld(shaderFile.FilenamePath);
            // print the config headers every 100 frames
            int zframeCount = 0;
            // print the zframes
            string zFrameBaseDir = $"/vcs-all/{GetCoreOrDotaString(shaderFile.FilenamePath)}/zsource/";
            // prepare the lookup to determine configuration state
            ConfigMappingSParams configGen = new(shaderFile);
            output.WriteLine(new string('-', zframesHeader.Length));
            // collect names in the order they appear
            List<string> sfNames = new();
            List<string> abbreviations = new();
            foreach (var sfBlock in shaderFile.SfBlocks)
            {
                string sfShortName = ShortenShaderParam(sfBlock.Name0).ToLower();
                abbreviations.Add($"{sfBlock.Name0}({sfShortName})");
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
            foreach (var item in shaderFile.ZframesLookup)
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

        private static string Comb(int[] ints0)
        {
            return $"({Fmt(ints0[0])},{Fmt(ints0[1])},{Fmt(ints0[2])},{Fmt(ints0[3])})";
        }

        private static string Comb(float[] floats0)
        {
            return $"({Fmt(floats0[0])},{Fmt(floats0[1])},{Fmt(floats0[2])},{Fmt(floats0[3])})";
        }

        private static string Fmt(float val)
        {
            if (val == -1e9) return "-";
            if (val == 1e9) return "+";
            return $"{val}";
        }

        private static string Fmt(int val)
        {
            if (val == -999999999) return "-";
            if (val == 999999999) return "+";
            return "" + val; ;
        }
    }
}
