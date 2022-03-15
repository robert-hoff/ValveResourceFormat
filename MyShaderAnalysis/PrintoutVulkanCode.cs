using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

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


            VulkanSource vulkanSource = zframeFile.gpuSources[0] as VulkanSource;


            byte[] sourceBytes = vulkanSource.sourcebytes;


            string vulkanBytesAsString = BytesToString(sourceBytes);

            // prints the Vulkan source as a byte string
            Console.WriteLine(vulkanBytesAsString);

            // attempts to print the Vulkan source interpreted as an Ascii encoded byte array
            // for some reason this only outputs a few weird characters (4 visible strange characters)
            // string result = Encoding.UTF8.GetString(vulkanSource.sourcebytes);

            // this produces a bunch of garbage, there are some discernable strings though such as "GLSL.std.450"
            // for (int i = 0; i < 10000; i++)
            // {
            //    Console.WriteLine(result.Substring(i,i+1));
            // }



            // Console.WriteLine(result.Length);



        }




    }
}



