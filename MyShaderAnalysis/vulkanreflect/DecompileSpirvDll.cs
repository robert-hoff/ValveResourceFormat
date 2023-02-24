using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MyShaderAnalysis.vulkanreflect
{
    public class DecompileSpirvDll
    {
#pragma warning disable CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes

        [DllImport("SpirvCrossDll.dll")]
        private static extern IntPtr CreateSpirvDecompiler();

        [DllImport("SpirvCrossDll.dll")]
        private static extern int PushUInt32(IntPtr decompiler, uint val);

        [DllImport("SpirvCrossDll.dll")]
        private static extern char Parse(IntPtr decompiler);

        [DllImport("SpirvCrossDll.dll")]
        private static extern int GetDataLength(IntPtr decompiler);

        [DllImport("SpirvCrossDll.dll")]
        private static extern char GetChar(IntPtr decompiler, int i);

#pragma warning restore CA5392

        public static string DecompileVulkan(byte[] databytes)
        {
            IntPtr decompiler = CreateSpirvDecompiler();
            for (int i = 0; i < databytes.Length; i += 4)
            {
                uint b0 = databytes[i + 0];
                uint b1 = databytes[i + 1];
                uint b2 = databytes[i + 2];
                uint b3 = databytes[i + 3];
                uint nextUInt32 = b3 + (b2 << 8) + (b1 << 16) + (b0 << 24);
                PushUInt32(decompiler, nextUInt32);
            }
            Parse(decompiler);
            int len = GetDataLength(decompiler);
            StringBuilder sb = new();
            for (int i = 0; i < len; i++)
            {
                char c = GetChar(decompiler, i);
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}

