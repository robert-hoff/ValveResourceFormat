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


            // ShowRelatedFilesInVcsCollection(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30");
            ShowRelatedFilesInVcsCollection(ARCHIVE.dotagame_pcgl, "hero_pcgl_30");
            // TestShaderInstantiation();

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



