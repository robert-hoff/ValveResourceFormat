using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.serverhtml;
using MyShaderAnalysis.util;
using MyShaderAnalysis.vulkanreflect;
using System;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

/*
 * 8 April 2022
 * Also some methods added in TestSingleFileParsing, switching back to here
 *
 * 14 March 2022
 * Adding this file to do some additional analysis on the Vulkan files
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.parsing
{
    public class PrintoutVulkanCode
    {
        public static void RunTrials()
        {
            // ShowSourceCodeSpirvReflection();
            ShowSourceCode();
            // PrintAZframeToConsole();
            // Trial2();
            // Trial1();
        }

        static void ShowSourceCodeSpirvReflection()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs"); int gpuIndex = 1;
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, "blur_pcgl_40_ps.vcs"); int gpuIndex = 0;
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pc_v64, "3dskyboxstencil_pc_40_vs.vcs"); int gpuIndex = 0;

            ShaderFile shaderFile = fileTokens.GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            VulkanSource vulkanSource = (VulkanSource)zframeFile.GpuSources[gpuIndex];
            string reflectedSpirv = DecompileSpirvDll.DecompileVulkan(vulkanSource.GetSpirvBytes());

            FileWriter fw = new FileWriter(ServerNames.GetServerTestFile());
            fw.WriteHtmlHeader("Spirv reflection", "Spirv reflection");
            fw.WriteLine(vulkanSource.GetSourceDetails());
            fw.WriteLine($"// Spirv source ({vulkanSource.MetadataOffset}), reflection performed with SPIRV-Cross, KhronosGroup\n");
            fw.WriteLine($"{reflectedSpirv}");
            fw.WriteLine($"// Source metadata (unknown encoding) ({vulkanSource.MetadataLength})");
            fw.WriteLine($"[{vulkanSource.MetadataOffset}]");
            fw.WriteLine($"{BytesToString(vulkanSource.GetMetadataBytes())}");
            fw.CloseStreamWriter();
        }

        static void ShowSourceCode()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs"); int gpuIndex = 1;
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, "blur_pcgl_40_ps.vcs"); int gpuIndex = 0;
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pc_v64, "3dskyboxstencil_pc_40_vs.vcs"); int gpuIndex = 0;

            ShaderFile shaderFile = fileTokens.GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            FileWriter fw = new FileWriter(ServerNames.GetServerTestFile());
            fw.WriteHtmlHeader("Zframe testing", "Zframe testing");
            zframeFile.PrintGpuSource(gpuIndex, fw.GetOutputWriter());
            fw.CloseStreamWriter();
        }

        static void PrintAZframeToConsole()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs");
            ShaderFile shaderFile = fileTokens.GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            new PrintZFrameSummary(shaderFile, zframeFile);
        }

        /*
         * saves bytecode that can be decompiled as SPIR-V
         *
         */
        static void Trial2()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs");
            ShaderFile shaderFile = fileTokens.GetShaderFile();

            // Console.WriteLine(shaderFile.GetZFrameCount());
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            // -- save to file, the spirv source ends 96 bytes before the end for this file (when gpuIndex = 0)
            // byte[] sourceBytes = zframeFile.gpuSources[0].sourcebytes[0..^96];
            // File.WriteAllBytes("X:/checkouts/SPIRV-Cross/Debug/source0.spv", sourceBytes);

            // -- spriv source ends 97 bytes before the end for this file (when gpuIndex = 1)
            // sourceBytes = zframeFile.gpuSources[1].sourcebytes[0..^97];
            // File.WriteAllBytes("X:/checkouts/SPIRV-Cross/Debug/source1.spv", sourceBytes);
        }

        static void Trial1()
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_vs.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs");
            ShaderFile shaderFile = fileTokens.GetShaderFile();

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

            VulkanSource vulkanSource = zframeFile.GpuSources[0] as VulkanSource;
            byte[] sourceBytes = vulkanSource.Sourcebytes;
            string vulkanBytesAsString = BytesToString(sourceBytes);
            Console.WriteLine(vulkanBytesAsString); // prints the Vulkan source as a byte string
        }
    }
}

