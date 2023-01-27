using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.batchtesting
{
    internal class TestSpirvReflection
    {
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            TestSpirVReflection();
        }

        public static void TestSpirVReflection()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);

            ZFrameFile zFrame = fileArchive.GetZFrameFile(10, 100);
            // ZFrameFile zFrame = fileArchive.GetZFrameFile(1, 1);

            // zFrame.PrintByteDetail();


            VulkanSource vulkanSource = (VulkanSource)zFrame.gpuSources[0];
            Debug.WriteLine($"ARG0");
            Debug.WriteLine($"{vulkanSource.arg0}");
            Debug.WriteLine($"---\n\n");
            // byte[] sourceBytes = vulkanSource.sourcebytes;
            byte[] sourceBytes = vulkanSource.GetSpirvBytes();
            string spirvSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            Debug.WriteLine($"{spirvSource}");

        }



    }
}


