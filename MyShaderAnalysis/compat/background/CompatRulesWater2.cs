using System;

namespace MyShaderAnalysis.compat
{
    public class CompatRulesWater2
    {
        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";

        public static void RunTrials()
        {
            Trial1();
            // Trial2();
        }

        static void Trial2()
        {
            ShowState(3071);
        }

        static void Trial1()
        {
            // string filenamepath = @$"{PCGL_DIR_NOT_CORE}/water_dota_pcgl_30_ps.vcs";
            // ShaderFile shaderFile = new(filenamepath);
            // ShowBitGeneration();

            AddExclusion(1, 2);
            AddExclusion(1, 3);
            AddExclusion(2, 3);
            AddExclusion(2, 6);
            AddExclusion(2, 7);
            AddExclusion(2, 8);
            AddExclusion(2, 9);
            AddExclusion(3, 6);
            AddExclusion(3, 7);
            AddExclusion(3, 8);
            AddExclusion(3, 9);
            AddExclusion(10, 11);

            AddInclusion(8, 6);
            AddInclusion(7, 6);

            for (int i = 0; i < 3072; i++)
            {
                // for (int i = 0; i < 100; i++) {
                if (CheckZFrame(i))
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                    // ShowState(i);
                }
            }
        }

        static bool CheckZFrame(int zframe)
        {
            int[] state = GetBitPattern(zframe);

            for (int j = 2; j <= 11; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    int s1 = state[i];
                    int s2 = state[j];
                    if (s1 == 0 || s2 == 0)
                    {
                        continue;
                    }

                    // Console.WriteLine($"{i}:{s1} {j}:{s2}");

                    if (exclusions[i, j] == true)
                    {
                        return false;
                    }
                    if (inclusions[i, j] == true)
                    {
                        return false;
                    }
                }
            }

            for (int i = 1; i <= 11; i++)
            {
                int s1 = state[i];
                if (s1 == 0) continue;
                for (int j = 1; j <= 11; j++)
                {
                    //Console.WriteLine($"{i} {j}");

                    if (inclusions[i, j] && state[j] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static void ShowBitGeneration()
        {
            for (int i = 0; i < 500; i++)
            {
                int[] state = GetBitPattern(i);
                ShowState(state);
            }
        }

        static void ShowState(int i)
        {
            ShowState(GetBitPattern(i));
        }

        static void ShowState(int[] state)
        {
            string stateStr = "";
            for (int i = 1; i < state.Length; i++)
            {
                stateStr = $"{state[i]} {stateStr}";
            }
            Console.WriteLine($"{stateStr[0..^1]}");
        }

        static int[] GetBitPattern(int testNum)
        {
            int[] state = new int[12];
            for (int i = 1; i <= 11; i++)
            {
                int res = (testNum / offset[i]) % (layers[i] + 1);
                state[i] = res;
            }
            return state;
        }

        static int[] offset = { 1, 1, 2, 4, 8, 16, 32, 64, 128, 384, 768, 1536 };
        static int[] layers = { 0, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1 };

        static bool[,] exclusions = new bool[100, 100];
        static bool[,] inclusions = new bool[100, 100];

        static void AddExclusion(int s1, int s2)
        {
            exclusions[s1, s2] = true;
            exclusions[s2, s1] = true;
        }

        static void AddInclusion(int s1, int s2)
        {
            inclusions[s1, s2] = true;
        }
    }
}


