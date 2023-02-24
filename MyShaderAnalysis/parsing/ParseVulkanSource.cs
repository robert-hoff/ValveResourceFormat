using System.IO;
using MyShaderAnalysis.vulkanreflect;
using MyShaderFile.CompiledShader;

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
 */
#pragma warning disable IDE0044 // Add readonly modifier
namespace MyShaderAnalysis.parsing
{
    public class ParseVulkanSource : ShaderDataReader
    {
        VulkanSource vulkanSource;

        public ParseVulkanSource(VulkanSource vulkanSource, HandleOutputWrite outputWriter = null) :
            base(new MemoryStream(vulkanSource.Sourcebytes), outputWriter)
        {
            this.vulkanSource = vulkanSource;
        }

        public void PrintByteDetail()
        {
            BaseStream.Position = 0;
            OutputWriter($"{vulkanSource.Arg0,-4} arg0\n");
            OutputWriter($"{vulkanSource.Offset,-4} offset\n");
            OutputWriter($"{vulkanSource.MetadataOffset,-4} offset2\n");

            // -- use this to write to file
            // byte[] databytes = ReadBytes(vulkanSource.offset2);
            // File.WriteAllBytes("X:/checkouts/SPIRV-Cross/vcs_vulkan_samples/source4.spv", databytes);

            Comment("spirv part");
            ShowBytes(vulkanSource.MetadataOffset);
            BreakLine();
            ShowBytes(4000);
        }

        public void PrintByteDetailSpirvReflection()
        {
            BaseStream.Position = 0;
            OutputWriter($"{vulkanSource.Arg0,-4} arg0\n");
            OutputWriter($"{vulkanSource.Offset,-4} offset\n");
            OutputWriter($"{vulkanSource.MetadataOffset,-4} offset2\n");

            if (vulkanSource.Offset == 0)
            {
                OutputWriter("empty source");
                return;
            }

            byte[] databytes = ReadBytes(vulkanSource.MetadataOffset);
            string source = DecompileSpirvDll.DecompileVulkan(databytes);
            BreakLine();
            OutputWriter(source);
            BreakLine();
            ShowBytes(4000);
        }
    }
}

