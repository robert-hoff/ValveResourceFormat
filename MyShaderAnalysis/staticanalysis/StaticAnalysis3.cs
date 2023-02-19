using MyShaderAnalysis.filearchive;
using System;
using System.Diagnostics;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.util.DataCollectAcrossQueries;

namespace MyShaderAnalysis.staticanalysis
{
    internal class StaticAnalysis3
    {
        const VcsProgramType FEAT = VcsProgramType.Features;
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            Parameters();
            // ConfigurationHeaders();
            // MipMapCountFileSets();
            // CheckDBlockCountForFeaturesFiles();
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
