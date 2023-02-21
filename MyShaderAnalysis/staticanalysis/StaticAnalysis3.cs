using MyShaderAnalysis.filearchive;
using System;
using System.Diagnostics;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.util.DataCollectAcrossQueries;
using MyShaderAnalysis.codestash;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace MyShaderAnalysis.staticanalysis
{
    internal class StaticAnalysis3
    {
        const VcsProgramType FEAT = VcsProgramType.Features;
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            ShowDblocksForFilesOfInterest();
            // ShowDRulesForFile();
            // DRuleBlocksFindGivenPattern();
            // DRuleBlocks();
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
            foreach (var fileArchive in fileArchives)
            {
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    try
                    {
                        ShaderFile shaderFile = vcsFile.GetShaderFile();
                        foreach (DConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
                        {
                            if (dRuleBlock.Values.Length > 0)
                            // if (dRuleBlock.Arg1 > -1)
                            {
                                Debug.WriteLine($"\n\n-- file {shaderFile.FilenamePath}");
                                ShowDBlockRules(shaderFile);
                            }
                        }
                    } catch (Exception) { }
                }
            }
        }


        public static void ShowDRulesForFile()
        {
            FileVcsTokens vcsTokens = new(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_complex_vulkan_50_vs.vcs");
            ShaderFile shaderFile = vcsTokens.GetShaderFile();
            ShowDBlockRules(shaderFile);
        }

        public static void ShowDBlockRules(ShaderFile shaderFile)
        {
            OutputFormatterTabulatedData outputter = new OutputFormatterTabulatedData();
            outputter.DefineHeaders(new string[] { "BlockIndex", "Rule", "Arg1", "BlockType", "ConditionalTypes",
                "Indices", "Values", "Range2", "Desc." });
            foreach (DConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
            {
                string biText = $"{dRuleBlock.BlockIndex:00}";
                string ruleText = $"{dRuleBlock.Rule}";
                string a1Text = $"{dRuleBlock.Arg1,2}";
                string btText = $"{dRuleBlock.BlockType}";
                string ctText = $"{CombineIntArray(dRuleBlock.ConditionalTypes)}";
                string indText = $"{CombineIntArray(dRuleBlock.Indices)}";
                string valText = $"{CombineIntArray(dRuleBlock.Values)}";
                string ran2Text = $"{CombineIntArray(dRuleBlock.Range2)}";
                string descText = $"{dRuleBlock.Description}";
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
                    foreach (DConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
                    {
                        if (dRuleBlock.Indices.Length > 6)
                        {
                            Debug.WriteLine($"{shaderFile.FilenamePath}");
                            break;
                        }
                    }
                } catch (Exception) { }
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
            foreach (var fileArchive in fileArchives)
            {
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    try
                    {
                        ShaderFile shaderFile = vcsFile.GetShaderFile();
                        foreach (DConstraintsBlock dRuleBlock in shaderFile.DConstraintsBlocks)
                        {
                            //string reportLine = $"{StaticAnalysis1.GetConciseDescription(dRuleBlock)} {dRuleBlock.Arg1}  " +
                            //    $"{ShaderUtilHelpers.CombineIntArray(dRuleBlock.Indices)}";
                            // CollectStringValue(reportLine);
                            // string biText = $"{dRuleBlock.BlockIndex:00}";
                            string biText = $".";
                            string ruleText = $"{dRuleBlock.Rule}";
                            string a1Text = $"{dRuleBlock.Arg1,2}";
                            string btText = $"{dRuleBlock.BlockType}";
                            string ctText = $"{CombineIntArray(dRuleBlock.ConditionalTypes)}";
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
                    } catch (Exception) { }
                }
            }
            List<string> rows = outputter.BuildTabulatedRows();
            foreach (var row in rows)
            {
                CollectStringValue(row);
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
                    string reportLine = $"{pBlock.Lead0:00} {pBlock.Type} {pBlock.Arg1} {pBlock.Arg2}";
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
                    string reportLine = $"{sfBlock.Name0,-40} {sfBlock.Arg3}";
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
                Debug.WriteLine($"{vcsFile,-80} {vcsFile.GetShaderFile().MipmapBlocks.Count}");
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
