using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;


namespace MyShaderAnalysis.utilhelpers
{
    public class TestFileUtilFunctions
    {

        public static void RunTrials()
        {


            // ShowFileTokenDetail2();
            ShowFileTokenDetail1();
            // ShowRelatedFilesInVcsCollection(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30");
            // ShowRelatedFilesInVcsCollection(ARCHIVE.dotagame_pcgl, "hero_pcgl_30");
            // TestShaderInstantiation();

        }

        static void ShowFileTokenDetail2()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs");
            // Console.WriteLine($"{fileTokens.sourcedir}");
        }


        static void ShowFileTokenDetail1()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs");
            ShaderFile shaderFile = fileTokens.GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);

            Console.WriteLine($" * GetServerFileDir()                  {fileTokens.GetServerFileDir()}");
            Console.WriteLine($" * GetServerFilenamepath(label)        {fileTokens.GetServerFilenamepath("label")}");
            Console.WriteLine($" * GetServerFilePath()                 {fileTokens.GetServerFilePath()}");
            Console.WriteLine($" * GetServerFileUrl(label)             {fileTokens.GetServerFileUrl("label")}");
            Console.WriteLine($" * GetGpuServerDir()                   {fileTokens.GetGpuServerDir()}");
            Console.WriteLine($" * GetGpuServerUrl()                   {fileTokens.GetGpuServerUrl()}");
            Console.WriteLine($" * GetGpuHtmlFilename(gpuSource)       {fileTokens.GetGpuHtmlFilename(zframeFile.gpuSources[0])}");
            Console.WriteLine($" * GetGpuHtmlUrl(gpusource)            {fileTokens.GetGpuHtmlUrl(zframeFile.gpuSources[0])}");

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
            FileVcsTokens spritecard_ps = new(ARCHIVE.dotagame_pcgl, "spritecard_pcgl_30_ps.vcs");
            ShaderFile shaderFile = spritecard_ps.GetShaderFile();
            // shows the spritecard_ps file has 22560 zframes (quite a lot)
            Console.WriteLine($"spritecard_ps has {shaderFile.GetZFrameCount()} zframes");
        }



    }




}



