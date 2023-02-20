using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderFileKristiker.MyHelperClasses.ProgEntries
{
    internal class RunBatchTests
    {


        public static void RunTrials()
        {
            TestV62Set();
            // TestSingleFile();
        }


        public static void TestV62Set()
        {
            int LIMIT_ZFRAMES = 20;
            int LIMIT_GPU_SOURCES = 20;

            // FileArchive vcsArchive = new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, maxFiles: 20000);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62, maxFiles: 20000);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_vulkan_v65, maxFiles: 20000);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_vulkan_v66, maxFiles: 20000);
            for (int i = 0; i < vcsArchive.GetFileCount(); i++)
            {
                try
                {
                    ShaderFile shaderFile = vcsArchive.GetShaderFile(i);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"problem in file {i}");
                }
            }
        }

        public static void TestSingleFile()
        {
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62, maxFiles: 20000);

            FileVcsTokens vcsTokens = vcsArchive.GetFileVcsTokens(0);
            // Debug.WriteLine($"{vcsTokens.filename}");

            ShaderFile shaderFiles = vcsTokens.GetShaderFile();


        }


    }
}




