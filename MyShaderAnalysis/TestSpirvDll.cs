using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis
{
    public class TestSpirvDll
    {

        public static void RunTrials()
        {
            DecompileVulkan();
        }


        [DllImport("SpirvCrossDll.dll")]
        public static extern IntPtr CreateSpirvDecompiler();

        [DllImport("SpirvCrossDll.dll")]
        public static extern int PushUInt32(IntPtr decompiler, UInt32 val);

        [DllImport("SpirvCrossDll.dll")]
        public static extern char Parse(IntPtr decompiler);

        [DllImport("SpirvCrossDll.dll")]
        public static extern int GetDataLength(IntPtr decompiler);

        [DllImport("SpirvCrossDll.dll")]
        public static extern char GetChar(IntPtr decompiler, int i);

        static void DecompileVulkan()
        {
            IntPtr decompiler = CreateSpirvDecompiler();
            // byte[] databytes = File.ReadAllBytes(@"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples\source0.spv");
            byte[] databytes = File.ReadAllBytes(@"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples\source1.spv");
            for (int i = 0; i < databytes.Length; i += 4)
            {
                uint b0 = (uint)databytes[i + 0];
                uint b1 = (uint)databytes[i + 1];
                uint b2 = (uint)databytes[i + 2];
                uint b3 = (uint)databytes[i + 3];
                uint nextUInt32 = b3 + (b2 << 8) + (b1 << 16) + (b0 << 24);
                PushUInt32(decompiler, nextUInt32);
            }
            Parse(decompiler);
            int len = GetDataLength(decompiler);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                char c = GetChar(decompiler, i);
                sb.Append(c);
            }
            Console.WriteLine($"{sb}");
        }


        public static void ShowBytes()
        {
            byte[] databytes = File.ReadAllBytes(@"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples\source0.spv");
            Console.WriteLine($"{databytes.Length}");
        }


    }
}


