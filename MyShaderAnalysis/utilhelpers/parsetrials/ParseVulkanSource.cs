using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

/*
 * Jan 2023 recap
 * Example Vulkan bytecode
 *
 * 03 02 23 07 00 00 01 00 0A 00 08 00
 * AC 62 00 00 00 00 00 00 11 00 02 00 01 00 00 00 0B 00 06 00
 *
 * SPIR-V magic number is 0x07230203
 * Version = 0x00010000 = 0.1.0.0
 * Generator 0x0008000A
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers.parsetrials
{
    public class ParseVulkanSource : ShaderDataReader
    {
        VulkanSource vulkanSource;

        public ParseVulkanSource(VulkanSource vulkanSource, HandleOutputWrite outputWriter = null) :
            base(new MemoryStream(vulkanSource.sourcebytes), outputWriter)
        {
            this.vulkanSource = vulkanSource;
        }


        public void PrintByteDetail()
        {
            BaseStream.Position = 0;
            outputWriter($"{vulkanSource.arg0,-4} arg0\n");
            outputWriter($"{vulkanSource.offset,-4} offset\n");
            outputWriter($"{vulkanSource.metadataOffset,-4} offset2\n");

            // -- use this to write to file
            // byte[] databytes = ReadBytes(vulkanSource.offset2);
            // File.WriteAllBytes("X:/checkouts/SPIRV-Cross/vcs_vulkan_samples/source4.spv", databytes);

            Comment("spirv part");
            ShowBytes(vulkanSource.metadataOffset);
            BreakLine();
            ShowBytes(4000);
        }



        public void PrintByteDetailSpirvReflection()
        {
            BaseStream.Position = 0;
            outputWriter($"{vulkanSource.arg0,-4} arg0\n");
            outputWriter($"{vulkanSource.offset,-4} offset\n");
            outputWriter($"{vulkanSource.metadataOffset,-4} offset2\n");

            if (vulkanSource.offset == 0)
            {
                outputWriter("empty source");
                return;
            }

            byte[] databytes = ReadBytes(vulkanSource.metadataOffset);
            string source = DecompileSpirvDll.DecompileVulkan(databytes);
            BreakLine();
            outputWriter(source);
            BreakLine();
            ShowBytes(4000);
        }



    }
}