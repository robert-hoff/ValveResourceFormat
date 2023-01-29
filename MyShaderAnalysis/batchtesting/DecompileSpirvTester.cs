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
    public class DecompileSpirvTester
    {
        private ZFrameFile zFrameFile;

        public DecompileSpirvTester(ZFrameFile zFrameFile, int MAX_SOURCES = 10)
        {
            this.zFrameFile = zFrameFile;
            int sourceCount = zFrameFile.gpuSourceCount;
            if (sourceCount > MAX_SOURCES)
            {
                sourceCount = MAX_SOURCES;
            }

            for (int i = 0; i < zFrameFile.gpuSourceCount; i++)
            {
                VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[i];
                if (vulkanSource.arg0 == -1)
                {
                    // nothing to do
                    continue;
                }
                byte[] spirvBytes = vulkanSource.GetSpirvBytes();

                try
                {
                    string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
                } catch (Exception)
                {
                    Debug.WriteLine($"spirv failed for {zFrameFile.filenamepath} zframeId[{zFrameFile.zframeId:x}] " +
                        $"source[{i}] source-size={spirvBytes.Length}");
                }


            }
        }
    }
}

