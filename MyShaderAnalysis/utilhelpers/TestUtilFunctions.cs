using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.compat;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;


namespace MyShaderAnalysis.utilhelpers
{
    public class TestUtilFunctions
    {



        public static void RunTrials()
        {
            // TestFileSystem();
            // TestWriteSystem();
            // TestSourceTypes();
            // TestFileTokensForNewGlesArchive();
            // TestGpuSourceFileTokens();
            TestGetRelatedFiles();
        }



        static void TestFileTokensAbbreviatedName()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_40_ps.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);
            Console.WriteLine($"{fileTokens.GetAbbreviatedName()}");
        }


        static void TestGetRelatedFiles()
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


        static void TestSourceTypes()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_ANDROID_VULKAN_SOURCE}/copytexture_android_vulkan_40_features.vcs";
            // string filenamepath = $"{DOTA_DAC_IOS_VULKAN_SOURCE}/spritecard_ios_vulkan_40_features.vcs";
            // string filenamepath = $"{HLALYX_HLVR_VULKAN_SOURCE}/cables_vulkan_50_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_40_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_40_vs.vcs";
            VcsPlatformType vcsSourceType = ComputeVCSFileName(filenamepath).Item2;
            Console.WriteLine($"{vcsSourceType}");
        }


        static void TestWriteSystem(string outputFilenamepath = null, bool writeFile = false)
        {
            /*
            OutputWriter output = new(WriteToConsole: false, WriteToDebug: true);
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml("title", "header");
            }
            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3" });
            output.AddTabulatedRow(new string[] { $"sdf", "1", "2" });
            output.printTabulatedValues();
            */
        }



        static void TestGpuSourceFileTokens()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            // NOTE - the gles sources are assigned as the class GlslSource
            GpuSource gpuSource = zframeFile.gpuSources[0];
            if (gpuSource is not GlslSource)
            {
                Console.WriteLine($"this source is considered a GlslSource");
            }

            FileTokens fileTokens = new FileTokens(filenamepath);
            Console.WriteLine($"{fileTokens.GetGlslHtmlUrl((GlslSource) gpuSource)}");
        }



        static void TestFileTokensForNewGlesArchive()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);
            // Console.WriteLine($"{fileTokens}");
            Console.WriteLine($"{fileTokens.GetServerFilePath()}"); // /dota-game/gles/spritecard_mobile_gles_30
            Console.WriteLine($"{fileTokens.GetServerFileDir()}"); // Z:/dev/www/vcs.codecreation.dev/dota-game/gles/spritecard_mobile_gles_30
            Console.WriteLine($"{fileTokens.GetZFramesServerPath()}"); // /dota-game/gles/spritecard_mobile_gles_30/zframes
            Console.WriteLine($"{fileTokens.GetZFramesServerDir()}"); // Z:/dev/www/vcs.codecreation.dev/dota-game/gles/spritecard_mobile_gles_30/zframes
            Console.WriteLine($"{fileTokens.GetGlslServerDir()}"); //
        }

        static void TestFileAndPathNamesNewArchive()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);

            Console.WriteLine($"{fileTokens.GetBestPath().Length == 0}"); // empty string if no path found
            Console.WriteLine($"{fileTokens.GetServerFilePath()}"); // /dota-game/pc/hero_pc_30
            Console.WriteLine($"{fileTokens.GetServerFileDir()}"); // Z:/dev/www/vcs.codecreation.dev/dota-game/pc/hero_pc_30
            Console.WriteLine($"{fileTokens.GetBestZframesLink(0)}"); //   Z[00000000] (plaintext if no zframe found)
        }



        static void TestFileSystem()
        {
            FileTokens spritecard = new(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");
            spritecard.GetZFrameListing();
        }


    }
}





