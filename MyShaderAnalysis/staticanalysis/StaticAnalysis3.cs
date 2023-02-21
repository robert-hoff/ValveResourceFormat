using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyShaderAnalysis.filearchive;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.util.DataCollectAcrossQueries;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.staticanalysis
{
    internal class StaticAnalysis3
    {
        const VcsProgramType FEAT = VcsProgramType.Features;
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            // ShowDblocksForFilesOfInterest();
            // ShowDRulesForFile();
            // DRuleBlocksFindGivenPattern();
            // DRuleBlocks();
            CollectAllParameterNames();
            // Parameters();
            // ConfigurationHeaders();
            // MipMapCountFileSets();
            // CheckDBlockCountForFeaturesFiles();
        }

        public static void ShowDblocksForFilesOfInterest()
        {
            List<FileArchive> fileArchives = new List<FileArchive>();
            // fileArchives.Add(new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v66, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v65, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.the_lab_pc_v62, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_core_pcgl_v64, FEAT, VS, PS));
            OutputFormatterTabulatedData outputter = new OutputFormatterTabulatedData();
            outputter.DefineHeaders(new string[] { "BlockIndex", "Rule", "Arg1", "BlockType", "ConditionalTypes",
                "Indices", "Values", "Range2", "Desc." });
            foreach (FileArchive fileArchive in fileArchives)
            {
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    try
                    {
                        ShaderFile shaderFile = vcsFile.GetShaderFile();
                        foreach (ConstraintsBlock dRuleBlock in shaderFile.SfConstraintsBlocks)
                        {
                            // if (dRuleBlock.Values.Length > 0)
                            // if (dRuleBlock.Arg1 > -1)
                            if (dRuleBlock.BlockType == ConditionalType.Feature && dRuleBlock.Description.Length > 0)
                            // if (dRuleBlock.Description.Length == 0 && shaderFile.VcsProgramType == VcsProgramType.Features)
                            // if (shaderFile.VcsProgramType == VcsProgramType.Features)
                            {
                                Debug.WriteLine($"\n\n-- {shaderFile.FilenamePath}");
                                ShowSfBlockRules(shaderFile);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        public static void ShowDRulesForFile()
        {
            // FileVcsTokens vcsTokens = new(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_complex_vulkan_50_vs.vcs");
            // FileVcsTokens vcsTokens = new(ARCHIVE.the_lab_pc_v62, "vr_warp_pc_50_ps.vcs");
            FileVcsTokens vcsTokens = new(ARCHIVE.dota_game_pcgl_v64, "hero_pcgl_40_ps.vcs");
            ShaderFile shaderFile = vcsTokens.GetShaderFile();
            ShowDBlockRules(shaderFile);
        }

        public static void ShowDBlockRules(ShaderFile shaderFile)
        {
            ShowConstraintRules(shaderFile, ConditionalType.Dynamic);
        }

        public static void ShowSfBlockRules(ShaderFile shaderFile)
        {
            ShowConstraintRules(shaderFile, ConditionalType.Static);
        }

        public static void ShowConstraintRules(ShaderFile shaderFile, ConditionalType type)
        {
            List<ConstraintsBlock> rules = (type) switch
            {
                ConditionalType.Feature => shaderFile.SfConstraintsBlocks,
                ConditionalType.Static => shaderFile.SfConstraintsBlocks,
                ConditionalType.Dynamic => shaderFile.DConstraintsBlocks,
                _ => throw new Exception("can't happen")
            };
            OutputFormatterTabulatedData outputter = new OutputFormatterTabulatedData();
            outputter.DefineHeaders(new string[] { "BlockIndex", "Rule", "Arg1", "BlockType", "ConditionalTypes",
                "Indices", "Values", "Range2", "Desc." });
            foreach (ConstraintsBlock ruleBlock in rules)
            {
                string biText = $"{ruleBlock.BlockIndex:00}";
                string ruleIncExc = ruleBlock.Rule == ConditionalRule.Requires ? "inc" : "exc";
                string ruleText = $"{ruleIncExc}({(int) ruleBlock.Rule})";
                string a1Text = $"{ruleBlock.Arg1,2}";
                string btText = $"{ruleBlock.BlockType}";
                // string ctText = $"{CombineIntArray(ruleBlock.ConditionalTypes)}";
                string ctText = CombineIntArray(Array.ConvertAll(ruleBlock.ConditionalTypes, x => (int) x));
                string indText = $"{CombineIntArray(ruleBlock.Indices)}";
                string valText = $"{CombineIntArray(ruleBlock.Values)}";
                string ran2Text = $"{CombineIntArray(ruleBlock.Range2)}";
                string descText = $"{ruleBlock.Description}";
                outputter.AddTabulatedRow(new string[] { biText, ruleText,
                                a1Text, btText, ctText, indText, valText, ran2Text, descText });
            }
            outputter.PrintTabulatedValues();
        }

        public static void DRuleBlocksFindGivenPattern()
        {
            FileArchive fileArchive = new(ARCHIVE.the_lab_pc_v62, FEAT, VS, PS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                try
                {
                    ShaderFile shaderFile = vcsFile.GetShaderFile();
                    foreach (ConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
                    {
                        // if (dRuleBlock.Indices.Length > 6)
                        if (dRuleBlock.Arg1 == 2)
                        {
                            Debug.WriteLine($"{shaderFile.FilenamePath}");
                            break;
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        public static void DRuleBlocks()
        {
            List<FileArchive> fileArchives = new List<FileArchive>();
            // fileArchives.Add(new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v66, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v65, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.the_lab_pc_v62, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS));
            // fileArchives.Add(new FileArchive(ARCHIVE.dota_core_pcgl_v64, FEAT, VS, PS));

            OutputFormatterTabulatedData outputter = new OutputFormatterTabulatedData();
            outputter.DefineHeaders(new string[] { "BlockIndex", "Rule", "Arg1", "BlockType", "ConditionalTypes",
                "Indices", "Values", "Range2", "Desc." });
            foreach (FileArchive fileArchive in fileArchives)
            {
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    try
                    {
                        ShaderFile shaderFile = vcsFile.GetShaderFile();
                        foreach (ConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
                        {
                            //string reportLine = $"{StaticAnalysis1.GetConciseDescription(dRuleBlock)} {dRuleBlock.Arg1}  " +
                            //    $"{ShaderUtilHelpers.CombineIntArray(dRuleBlock.Indices)}";
                            // CollectStringValue(reportLine);
                            // string biText = $"{dRuleBlock.BlockIndex:00}";
                            string biText = $".";
                            string ruleIncExc = dRuleBlock.Rule == ConditionalRule.Requires ? "inc" : "exc";
                            string ruleText = $"{ruleIncExc}({(int) dRuleBlock.Rule})";
                            string a1Text = $"{dRuleBlock.Arg1,2}";
                            string btText = $"{dRuleBlock.BlockType}";
                            /// string ctText = $"{CombineIntArray(dRuleBlock.ConditionalTypes)}";
                            string ctText = CombineIntArray(Array.ConvertAll(dRuleBlock.ConditionalTypes, x => (int) x));
                            string indText = $"{CombineIntArray(dRuleBlock.Indices)}";
                            // string indText = $".";
                            string valText = $"{CombineIntArray(dRuleBlock.Values)}";
                            string ran2Text = $"{CombineIntArray(dRuleBlock.Range2)}";
                            string descText = $"{dRuleBlock.Description}";
                            outputter.AddTabulatedRow(new string[] { biText, ruleText,
                                a1Text, btText, ctText, indText, valText, ran2Text, descText });
                            //if (dRuleBlock.Arg1 > -1)
                            //{
                            //    Debug.WriteLine($"{shaderFile.FilenamePath}");
                            //}
                        }
                    }
                    catch (Exception) { }
                }
            }
            List<string> rows = outputter.BuildTabulatedRows();
            foreach (string row in rows)
            {
                CollectStringValue(row);
            }
            PrintReport(showCount: false);
        }

        public static void CollectAllParameterNames()
        {
            List<FileArchive> fileArchives = new List<FileArchive>();
            fileArchives.Add(new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v66, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.dota_game_vulkan_v65, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.the_lab_pc_v62, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS));
            fileArchives.Add(new FileArchive(ARCHIVE.dota_core_pcgl_v64, FEAT, VS, PS));

            foreach (FileArchive fileArchive in fileArchives)
            {
                Debug.WriteLine($"parsing {fileArchive.ArchiveName()}");
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    try
                    {
                        ShaderFile shaderFile = vcsFile.GetShaderFile();
                        foreach (ParamBlock pBlock in shaderFile.ParamBlocks)
                        {
                            CollectStringValue($"{pBlock.Name}");
                        }
                    }
                    catch (Exception) { }
                }
            }
            PrintReport(showCount: false);
        }

        public static void Parameters()
        {
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                foreach (ParamBlock pBlock in shaderFile.ParamBlocks)
                {
                    // same as (kristiker)
                    // lead0, UiType, VfxType, ParamType
                    string reportLine = $"{pBlock.Lead0:00} {pBlock.UiType} {pBlock.VfxType} {pBlock.ParamType}";
                    CollectStringValue(reportLine);
                }
            }
            PrintReport();
        }

        public static void ConfigurationHeaders()
        {
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                foreach (SfBlock sfBlock in shaderFile.SfBlocks)
                {
                    // string reportLine = $"{sfBlock.name0,-40} {sfBlock.name1,-40} {sfBlock.arg2}";
                    // string reportLine = $"{sfBlock.name0,-40} {sfBlock.arg2}";
                    string reportLine = $"{sfBlock.Name,-40} {sfBlock.Sys}";
                    CollectStringValue(reportLine);
                }
            }
            PrintReport();
        }

        public static void MipMapCountFileSets()
        {
            // FileArchive fileArchive = new(ARCHIVE.dota_game_vulkan_v65, FEAT, VS, PS);
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            string lastFile = "";
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                if (!vcsFile.foldername.Equals(lastFile))
                {
                    Debug.WriteLine($"");
                    lastFile = vcsFile.foldername;
                }
                Debug.WriteLine($"{vcsFile,-80} {vcsFile.GetShaderFile().ChannelBlocks.Count}");
            }
        }

        /*
         * D-blocks are always 0 for features files
         */
        public static void CheckDBlockCountForFeaturesFiles()
        {
            // FileArchive fileArchive = new(ARCHIVE.dota_game_vulkan_v65, FEAT);
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                Debug.WriteLine($"{vcsFile,-80} {shaderFile.DBlocks.Count}");
                if (shaderFile.DBlocks.Count != 0)
                {
                    throw new Exception("unecpexted value");
                }
            }
        }
    }
}
