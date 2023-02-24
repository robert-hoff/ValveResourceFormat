using System;
using System.Diagnostics;
using MyShaderFile.CompiledShader;

namespace MyShaderAnalysis.vulkanreflect
{
    public class DecompileSpirvTester
    {
        private ZFrameFile zFrameFile;

        public DecompileSpirvTester(ZFrameFile zFrameFile, int MAX_SOURCES = 10)
        {
            this.zFrameFile = zFrameFile;
            int sourceCount = zFrameFile.GpuSourceCount;
            if (sourceCount > MAX_SOURCES)
            {
                sourceCount = MAX_SOURCES;
            }

            for (int i = 0; i < zFrameFile.GpuSourceCount; i++)
            {
                VulkanSource vulkanSource = (VulkanSource) zFrameFile.GpuSources[i];
                if (vulkanSource.Arg0 == -1)
                {
                    // nothing to do
                    continue;
                }
                byte[] spirvBytes = vulkanSource.GetSpirvBytes();

                try
                {
                    string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"spirv failed for {zFrameFile.FilenamePath} zframeId[{zFrameFile.ZframeId:x}] " +
                        $"source[{i}] source-size={spirvBytes.Length}");
                }
            }
        }
    }
}


