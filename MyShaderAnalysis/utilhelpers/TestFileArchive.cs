using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.utilhelpers
{
    public class TestFileArchive
    {

        public static void RunTrials()
        {
            TestShaderFiles();
        }

        public static void TestShaderFiles()
        {
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Undetermined, VcsShaderModelType._40);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (ShaderFile shaderFile in vcsArchive.ShaderFiles())
            {
                Console.WriteLine($"{Path.GetFileName(shaderFile.filenamepath)} zframe count = {shaderFile.GetZFrameCount()}");
            }
        }


    }
}
