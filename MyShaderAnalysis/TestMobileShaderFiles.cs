using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MyShaderAnalysis.compat;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;

namespace MyShaderAnalysis
{
    public class TestMobileShaderFiles
    {


        public static void RunTrials()
        {
            // Trial1();
            Trial2();
        }



        static void Trial2()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0);

            zframeFile.PrintByteAnalysis();
        }



        // this works well
        static void Trial1()
        {
            string filenamepath = $"{DOTA_DAC_MOBILE_GLES_SOURCE}/spritecard_mobile_gles_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteAnalysis(shortenOutput: false);
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
                Debug.WriteLine($"{d}");
            }

        }



    }
}
