using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;


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
            outputWriter($"{vulkanSource.offset2,-4} offset2\n");

            // -- use this to write to file
            // byte[] databytes = ReadBytes(vulkanSource.offset2);
            // File.WriteAllBytes("X:/checkouts/SPIRV-Cross/vcs_vulkan_samples/source4.spv", databytes);

            Comment("spirv part");
            ShowBytes(vulkanSource.offset2);
            BreakLine();
            ShowBytes(4000);
        }



        public void PrintByteDetailSpirvReflection()
        {
            BaseStream.Position = 0;
            outputWriter($"{vulkanSource.arg0,-4} arg0\n");
            outputWriter($"{vulkanSource.offset,-4} offset\n");
            outputWriter($"{vulkanSource.offset2,-4} offset2\n");

            if (vulkanSource.offset == 0)
            {
                outputWriter("empty source");
                return;
            }

            byte[] databytes = ReadBytes(vulkanSource.offset2);
            string source = DecompileSpirvDll.DecompileVulkan(databytes);
            BreakLine();
            outputWriter(source);
            BreakLine();
            ShowBytes(4000);
        }



    }
}
