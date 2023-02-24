using System;
using System.Collections.Generic;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;
using static MyShaderAnalysis.filearchive.FileArchive;
using static MyShaderAnalysis.filearchive.ReadShaderFile;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.codestash
{
    public class TestFileUtilFunctionsOld
    {
        public static void RunTrials()
        {
            ShowFileTokenDetail();
            // TestGenerateTargetFilenames();
            // TestCreateDirectoryStructures();
            // TestFileTokensAbbreviatedName();
            // TestGetRelatedFiles();
            // TestSourceTypes();
            // TestWriteSystem();
            // TestGpuSourceFileTokens();
            // TestFileTokensForNewGlesArchive();
            // TestFileAndPathNamesNewArchive();
            // TestFileSystem();
        }

        static void ShowFileTokenDetail()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);
            FileTokensOld fileToken = new FileTokensOld(filenamepath);

            Console.WriteLine($"GetServerFileDir()                  {fileToken.GetServerFileDir()}");
            Console.WriteLine($"GetServerFilenamepath(label)        {fileToken.GetServerFilenamepath("label")}");
            Console.WriteLine($"GetServerFilePath()                 {fileToken.GetServerFilePath()}");
            Console.WriteLine($"GetServerFileUrl(label)             {fileToken.GetServerFileUrl("label")}");
            Console.WriteLine($"GetGlslServerDir()                  {fileToken.GetGlslServerDir()}");
            Console.WriteLine($"GetGlslHtmlFilename(gpuSource) {fileToken.GetGlslHtmlFilename(zframeFile.GpuSources[0].GetEditorRefIdAsString())}");
            Console.WriteLine($"GetGlslHtmlUrl(gpusource)      {fileToken.GetGlslHtmlUrl(zframeFile.GpuSources[0].GetEditorRefIdAsString())}");

            Console.WriteLine($"GetZFramesServerDir()               {fileToken.GetZFramesServerDir()}");
            // Console.WriteLine($"CreateZFramesDirectory()         {fileToken.CreateZFramesDirectory()}"); // returns void
            Console.WriteLine($"GetZFramesServerPath()              {fileToken.GetZFramesServerPath()}");
            Console.WriteLine($"GetZFrameHtmlFilenamepath(id,label) {fileToken.GetZFrameHtmlFilenamepath(0, "label")}");
            Console.WriteLine($"GetZFrameHtmlFilename(id,label)     {fileToken.GetZFrameHtmlFilename(0, "label")}");
            Console.WriteLine($"GetZFrameLink(id,label)             {fileToken.GetZFrameLink(0, "label")}");
            Console.WriteLine($"GetShortHandName()                  {fileToken.GetShortHandName()}");
        }

        /*
         * Use this loop to generate names that I want, stick with the label 'summary2' for now
         *
         *
         */
        static void TestGenerateTargetFilenames()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsProgramType.Undetermined, -1);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (string filenamepath in vcsFiles)
            {
                FileTokensOld fileTokens = new FileTokensOld(filenamepath);
                string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: false)}";

                // I really don't remember where these method names came from
                // what is clear is that I need both a method for generating the desired output and a method
                // for printing the output to file
                // PrintSingleFileFullSummary(filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
                // output.CloseStreamWriter();
            }
        }

        /*
         * creates the base folder for each file, in the form
         *
         * Z:\dev\www\vcs.codecreation.dev\dota-core\pcgl\apply_fog_pcgl_40\
         *
         * NOTE - all files starting as 'apply_fog_pcgl_40' go here, which whould include
         * {apply_fog_pcgl_40_features.vcs, apply_fog_pcgl_40_vs.vcs, apply_fog_pcgl_40_ps.vcs}
         *
         * I've previously created three types of printouts 'summary', 'summary2' and 'bytes'
         *
         *
         * The main overview should be saved as
         *
         * apply_fog_pcgl_40_features-summary2.html
         *
         * 'summary2' was the name I gave to the latest single file formatted outputs
         *
         * 'summary' files were earlier printouts where I choose to compare the static variables between related
         * files (features, vs, ps). These previous 'summary' files do not include data from parameter, mipmap,
         * buffer, and vertex-symbol blocks
         *
         *
         *
         */
        static void TestCreateDirectoryStructures()
        {
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, null, VcsProgramType.Features, 40);
            foreach (string filenamepath in vcsFiles)
            {
                FileTokensOld fileTokens = new FileTokensOld(filenamepath);
                string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";

                Console.WriteLine($"{filenamepath}");
                Console.WriteLine($"{outputFilenamepath}");
                return;
            }
        }

        /*
         * hero(pcgl-ps)
         * hero(dxbc-ps)
         *
         */
        static void TestFileTokensAbbreviatedName()
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_40_ps.vcs";
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            Console.WriteLine($"{fileTokens.GetAbbreviatedName()}");
        }

        /*
         * Prints the full filenamepath of all related files. E.g. any file from the following collection
         * should print all others
         * {hero_pcgl_30_features.vcs, hero_pcgl_30_vs.vcs, hero_pcgl_30_ps.vcs, hero_pcgl_30_psrs.vcs}
         *
         */
        static void TestGetRelatedFiles()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_psrs.vcs";
            foreach (string f in GetRelatedFiles(filenamepath))
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

        /*
         * shows the server pathname for the first source file in spritecard_mobile_gles_30_vs.vcs
         * (this file doesn't need to be present on disk, it is the name we want to use by convention)
         *
         * /dota-game/gles/spritecard_mobile_gles_30/gles/gles-4345fc45dbdc2fca73bbd6172fc79ad6.html
         *
         * This method also works for glsl sources, e.g.
         *
         * /dota-game/pcgl/hero_pcgl_30/glsl/glsl-6450bd7b85ba0c0b472265ebea6376f6.html
         *
         */
        static void TestGpuSourceFileTokens()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            // NOTE - the gles sources are assigned as the class GlslSource
            GpuSource gpuSource = zframeFile.GpuSources[0];
            if (gpuSource is not GlslSource)
            {
                Console.WriteLine($"this source is not considered a GlslSource");
                return;
            }

            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            Console.WriteLine($"{fileTokens.GetGlslHtmlUrl(gpuSource.GetEditorRefIdAsString())}");
        }

        static void TestFileTokensForNewGlesArchive()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
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
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);

            Console.WriteLine($"{fileTokens.GetBestPath().Length == 0}"); // empty string if no path found
            Console.WriteLine($"{fileTokens.GetServerFilePath()}"); // /dota-game/pc/hero_pc_30
            Console.WriteLine($"{fileTokens.GetServerFileDir()}"); // Z:/dev/www/vcs.codecreation.dev/dota-game/pc/hero_pc_30
            Console.WriteLine($"{fileTokens.GetBestZframesLink(0)}"); //   Z[00000000] (plaintext if no zframe found)
        }

        /*
         * should they exist on disk, prints the zframes belonging to a given vcs files
         * E.g.
         * Z:/dev/www/vcs.codecreation/dota-core/pcgl/spritecard_pcgl_30/zframes\spritecard_pcgl_30_ps-ZFRAME00000000-bytes.html
         * ...
         *
         * Note the mix of / and \ present, I couldn't find any easy fix for this
         * (the path separator is a read only property of the Path class)
         *
         */
        static void TestFileSystem()
        {
            FileTokensOld spritecard = new(ARCHIVE_OLD.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");
            List<string> filenames = spritecard.GetZFrameListing();
            foreach (string f in filenames)
            {
                Console.WriteLine($"{f}");
            }
        }
    }
}


