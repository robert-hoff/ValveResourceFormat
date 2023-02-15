using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.staticanalysis
{
    internal class StaticAnalysis3
    {

        const VcsProgramType FEAT = VcsProgramType.Features;
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;


        public static void Run()
        {
            MipMapCountFileSets();
            // CheckDBlockCountForFeaturesFiles();
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
                Debug.WriteLine($"{vcsFile,-80} {vcsFile.GetShaderFile().mipmapBlocks.Count}");
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
                Debug.WriteLine($"{vcsFile,-80} {shaderFile.dBlocks.Count}");
                if (shaderFile.dBlocks.Count != 0)
                {
                    throw new Exception("unecpexted value");
                }

            }
        }



    }
}
