using System;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.filearchive.ReadShaderFile;

namespace MyShaderAnalysis.parsing
{
    public class TestMobileShaderFiles
    {
        public static void RunTrials()
        {
            // Trial1();
            // Trial2();
            Trial3();
        }

        static void Trial3()
        {
            // string filenamepath = $"{DOTA_DAC_ANDROID_VULKAN_SOURCE}/spritecard_android_vulkan_40_ps.vcs";
            string filenamepath = $"{DOTA_DAC_IOS_VULKAN_SOURCE}/spritecard_ios_vulkan_40_ps.vcs";
            // string filenamepath = $"{DOTA_DAC_ANDROID_VULKAN_SOURCE}/ui_twotexture_android_vulkan_40_ps.vcs";
            // string filenamepath = $"{DOTA_DAC_IOS_VULKAN_SOURCE}/ui_twotexture_ios_vulkan_40_ps.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);
            zframeFile.PrintByteDetail();
            // shaderFile.PrintByteDetail();
        }

        static void Trial2()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);
            zframeFile.PrintByteDetail();
        }

        // this works well
        static void Trial1()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteDetail(shortenOutput: false);
        }

        static void DirListings()
        {
            string dir = DOTA_CORE_MOBILE_GLES_SOURCE;
            // string dir = DOTA_DAC_MOBILE_GLES_SOURCE;
            // string dir = DOTA_CORE_ANDROID_VULKAN_SOURCE;
            // string dir = DOTA_DAC_ANDROID_VULKAN_SOURCE;
            // string dir = DOTA_CORE_IOS_VULKAN_SOURCE;
            // string dir = DOTA_DAC_IOS_VULKAN_SOURCE;

            foreach (var d in Directory.GetFiles(dir))
            {
                Console.WriteLine($"{d}");
            }
        }
    }
}
