using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;

/*
 * 14 March 2022
 * Adding this file to do some additional analysis on the Vulkan files
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis
{
    public class PrintoutVulkanCode
    {
        public const string HLALYX_CORE_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-core";
        public const string HLALYX_HLVR_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-hlvr";


        public static void RunTrials()
        {
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE+"/cables_vulkan_50_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);

            // Console.WriteLine(shaderFile.GetZFrameCount());
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(3);

            // Console.WriteLine(zframeFile.leadingData.h0);
            // Console.WriteLine(zframeFile.gpuSources.Capacity);



            // there is one Vulkan source file present
            // Console.WriteLine(zframeFile.gpuSources.Count);


            // this doesn't print anything ..
            // try to convert the sourcebytes to text ..
            // Console.WriteLine(zframeFile.gpuSources[0].sourcebytes);

            // Well -> regarding printing the sourcebytes as text, I'm pretty sure that won't have a good result




        }




    }
}



