using System;

namespace MyShaderAnalysis.compat
{
    public class BitTestGeneration
    {
        public static void RunTrials()
        {
            Trial1();
        }

        // 1           0        0
        // 2           2        2
        // 3           4        2*2
        // 4           8        2*2*2
        // 5          16        2*2*2*2
        // 6          32        2*2*2*2*2
        // 7          64        2*2*2*2*2*2
        // 8         128        2*2*2*2*2*2*2
        // 9         384        2*2*2*2*2*2*2*3
        //10         768        2*2*2*2*2*2*2*3*2
        //11        1536        2*2*2*2*2*2*2*3*2*2

        /*
         * The bit system from water_dota_pcgl_30_ps.vcs
         *
         *
         */
        static void Trial1()
        {
            int[] testnums = { 128, 256, 384, 512, 640, 768, 896, 1024, 1152, 1280, 1408, 1536, 1664, 1792, 1920, 2048, 2176 };
            for (int i = 0; i < 17; i++)
            {
                Console.WriteLine($"{GetBitPattern(testnums[i])}");
            }
        }

        //          00000xxxxxxx         11(off), 10(off), 9(off), 8(off)
        //  128     00001xxxxxxx         11(off), 10(off), 9(off), 8(1on)
        //  256     00010xxxxxxx         11(off), 10(off), 9(off), 8(2on)
        //  384     00011xxxxxxx         11(off), 10(off), 9(on),  8(off)
        //  512     00100xxxxxxx         11(off), 10(off), 9(on),  8(1on)
        //  640     00101xxxxxxx         11(off), 10(off), 9(on),  8(2on)
        //  768     00110xxxxxxx         11(off), 10(on),  9(off), 8(off)
        //  896     00111xxxxxxx         11(off), 10(on),  9(off), 8(1on)
        // 1024     01000xxxxxxx         11(off), 10(on),  9(off), 8(2on)
        // 1152     01001xxxxxxx         11(off), 10(on),  9(on),  8(off)
        // 1280     01010xxxxxxx         11(off), 10(on),  9(on),  8(1on)
        // 1408     01011xxxxxxx         11(off), 10(on),  9(on),  8(2on)
        // 1536     01100xxxxxxx         11(on),  10(off), 9(off), 8(off)
        // 1664     01101xxxxxxx         11(on),  10(off), 9(off), 8(1on)
        // 1792     01110xxxxxxx         11(on),  10(off), 9(off), 8(2on)
        // 1920     01111xxxxxxx         11(on),  10(off), 9(on),  8(off)
        // 2048     10000xxxxxxx         11(on),  10(off), 9(on),  8(1on)
        // 2176     10001xxxxxxx         11(on),  10(off), 9(on),  8(2on)
        static int[] offset = { -1, 0, 2, 4, 8, 16, 32, 64, 128, 384, 768, 1536 };
        static int[] layers = { -1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1 };
        static string GetBitPattern(int testNum)
        {
            string result = "";
            for (int i = 8; i <= 11; i++)
            {
                int res = (testNum / offset[i]) % (layers[i] + 1);
                result = $"{res} {result}";
            }
            return result.Trim();
        }
    }
}
