using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;


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

            PrintZframeToHtml();

        }



        static void ShowSourceCode()
        {
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE + "/solidcolor_vulkan_50_vs.vcs";
            // string vcsTestFile = DOTA_GAME_PCGL_SOURCE + "/blur_pcgl_40_ps.vcs";
            // string vcsTestFile = DOTA_GAME_PC_SOURCE + "/3dskyboxstencil_pc_40_vs.vcs";

            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            StreamWriter sw = new StreamWriter("Z:/dev/www/vcs.codecreation.dev/testfile.html");
            sw.WriteLine(GetHtmlHeader("Zframe testing", "Zframe testing"));
            zframeFile.PrintGpuSource(0, (x) => { sw.Write(x); });
            sw.WriteLine(GetHtmlFooter());
            sw.Close();
        }


        /*
         * very useful method for working on presentation
         *
         */
        static void PrintZframeToHtml()
        {
            StreamWriter sw = new StreamWriter("Z:/dev/www/vcs.codecreation.dev/testfile.html");
            sw.WriteLine(GetHtmlHeader("Zframe testing", "Zframe testing"));
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE + "/solidcolor_vulkan_50_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            new PrintZFrameSummary(shaderFile, zframeFile, (x) => { sw.Write(x); }, showRichTextBoxLinks: true);
            sw.WriteLine(GetHtmlFooter());
            sw.Close();
        }


        static void PrintAZframeToConsole()
        {
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE + "/solidcolor_vulkan_50_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            new PrintZFrameSummary(shaderFile, zframeFile);
        }


        /*
         * saves bytecode that can be decompiled as SPIR-V
         *
         */
        static void Trial2()
        {
            // string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE+"/cables_vulkan_50_vs.vcs";
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE + "/solidcolor_vulkan_50_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);

            // Console.WriteLine(shaderFile.GetZFrameCount());
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);


            byte[] sourceBytes = zframeFile.gpuSources[0].sourcebytes[0..^96];
            File.WriteAllBytes("X:/checkouts/SPIRV-Cross/Debug/source0.spv", sourceBytes);

            sourceBytes = zframeFile.gpuSources[1].sourcebytes[0..^97];
            File.WriteAllBytes("X:/checkouts/SPIRV-Cross/Debug/source1.spv", sourceBytes);
        }



        static void Trial1()
        {
            // string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE+"/cables_vulkan_50_vs.vcs";
            string vcsTestFile = HLALYX_HLVR_VULKAN_SOURCE + "/solidcolor_vulkan_50_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);

            // Console.WriteLine(shaderFile.GetZFrameCount());
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            // Console.WriteLine(zframeFile.leadingData.h0);
            // Console.WriteLine(zframeFile.gpuSources.Capacity);



            // there is one Vulkan source file present
            // Console.WriteLine(zframeFile.gpuSources.Count);


            // this doesn't print anything ..
            // try to convert the sourcebytes to text ..
            // Console.WriteLine(zframeFile.gpuSources[0].sourcebytes);

            // Well -> regarding printing the sourcebytes as text, I'm pretty sure that won't have a good result


            VulkanSource vulkanSource = zframeFile.gpuSources[1] as VulkanSource;
            byte[] sourceBytes = vulkanSource.sourcebytes;
            string vulkanBytesAsString = BytesToString(sourceBytes);
            Console.WriteLine(vulkanBytesAsString); // prints the Vulkan source as a byte string
        }







    }
}



