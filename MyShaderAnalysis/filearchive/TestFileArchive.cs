using System;
using System.Collections.Generic;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.filearchive
{
    public class TestFileArchive
    {
        public static void RunTrials()
        {
            // ShowFileArchiveDirectory();
            // ShowFileTokenDetail1();
            // ShowRelatedFilesInVcsCollection(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30");
            // ShowRelatedFilesInVcsCollection(ARCHIVE.dota_game_pcgl_v64, "hero_pcgl_30");
            // TestShaderInstantiation();
            ShowFileListing();
            // ShowAllFilesForArchive();
        }

        static void ShowFileArchiveDirectory()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_ps.vcs");
            Console.WriteLine($"{fileTokens.filedir}");
        }

        static void ShowFileTokenDetail1()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_ps.vcs");
            ShaderFile shaderFile = fileTokens.GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);

            Console.WriteLine($" * GetServerFileDir()                  {fileTokens.GetServerFileDir()}");
            Console.WriteLine($" * GetServerFilenamepath(label)        {fileTokens.GetServerFilenamepath("label")}");
            Console.WriteLine($" * GetServerFilePath()                 {fileTokens.GetServerFilePath()}");
            Console.WriteLine($" * GetServerFileUrl(label)             {fileTokens.GetServerFileUrl("label")}");
            Console.WriteLine($" * GetGpuServerDir()                   {fileTokens.GetGpuServerDir()}");
            Console.WriteLine($" * GetGpuServerUrl()                   {fileTokens.GetGpuServerUrl()}");
            Console.WriteLine($" * GetGpuHtmlFilename(refid) " +
                $"{fileTokens.GetGpuHtmlFilename(zframeFile.GpuSources[0].GetEditorRefIdAsString())}");
            Console.WriteLine($" * GetGpuHtmlUrl(refid)      " +
                $"{fileTokens.GetGpuHtmlUrl(zframeFile.GpuSources[0].GetEditorRefIdAsString())}");
            Console.WriteLine($" * GetZFramesServerDir()               {fileTokens.GetZFramesServerDir()}");
            Console.WriteLine($" * GetZFramesServerPath()              {fileTokens.GetZFramesServerPath()}");
            Console.WriteLine($" * GetZFrameHtmlFilenamepath(id,label) {fileTokens.GetZFrameHtmlFilenamepath(0, "label")}");
            Console.WriteLine($" * GetZFrameHtmlFilename(id,label)     {fileTokens.GetZFrameHtmlFilename(0, "label")}");
            Console.WriteLine($" * GetZFrameUrl(id,label)              {fileTokens.GetZFrameUrl(0, "label")}");
            Console.WriteLine($" * GetShortName()                      {fileTokens.GetShortName()}");
            Console.WriteLine($" * GetBaseName()                       {fileTokens.GetBaseName()}");
        }

        static void ShowRelatedFilesInVcsCollection(ARCHIVE archive, string vcsCollectionName)
        {
            List<string> relatedFiles = FileVcsCollection.GetRelatedFiles(archive, vcsCollectionName);
            foreach (var filename in relatedFiles)
            {
                Console.WriteLine($"{filename}");
            }
        }

        static void TestShaderInstantiation()
        {
            FileVcsTokens spritecard_ps = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, "spritecard_pcgl_30_ps.vcs");
            ShaderFile shaderFile = spritecard_ps.GetShaderFile();
            // shows the spritecard_ps file has 22560 zframes (quite a lot)
            Console.WriteLine($"spritecard_ps has {shaderFile.GetZFrameCount()} zframes");
        }

        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;
        public static void ShowFileListing()
        {
            FileArchive fileArchive = new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS);
            foreach (var fileTokens in fileArchive.GetFileVcsTokens())
            {
                Debug.WriteLine($"{fileTokens}");
            }
        }

        public static void ShowAllFilesForArchive()
        {
            FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (FileVcsTokens vcsTokens in vcsArchive.GetFileVcsTokens())
            {
                Console.WriteLine($"{vcsTokens}");
            }
        }
    }
}

