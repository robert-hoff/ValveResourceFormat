using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


/*
 *
 * Generating the zframes from the compat-rules is a verification-task. The method employed is slow.
 * But correctness is self-evident.
 * In a working system, and once things are verified, it is more desirable to move from the known zframes
 * to configuration status, bypassing the rule formulation entirerly.

 * In summary, the purpose of the zframe generation, by method of rule-elimnation is to to ensure
 * that the later working system is correct.
 *
 * There is one crucial shared component though, which is to go from a zframeId to a shader configuration.
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.compat
{
    public class CompatRulesGenericStaticClass
    {

        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";

        public static void RunTrials()
        {
            Trial1();
        }



        static void Trial1()
        {
            string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs";
            // string vcsFilenamepath = @$"{PCGL_DIR_CORE}/blur_pcgl_30_ps.vcs";
            // string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/water_dota_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new ReadShaderFile(vcsFilenamepath).GetShaderFile();
            GenerateZFrames(shaderFile);
        }




        static void GenerateZFrames(ShaderFile shaderFile)
        {
            GenerateOffsetAndLayers(shaderFile);
            // ShowIntArray(offsets);
            // ShowIntArray(layers);
            AddExclusion(1, 2, 3);         // rule[0]
            AddExclusion(2, 3, 4);         // rule[1]
            AddExclusion(2, 3, 5);         // rule[2]
            AddExclusion(2, 3, 7);         // rule[4]
            AddExclusion(2, 3, 0);         // rule[6]
            AddExclusion(2, 3, 10);        // rule[8]
            AddExclusion(2, 3, 11);        // rule[9]
            AddExclusion(13, 6, 2);        // rule[11]
            AddExclusion(14, 6, 2);        // rule[14]
            AddExclusion(15, 6, 2);        // rule[17]
            AddExclusion(16, 6, 2);        // rule[20]
            AddExclusion(2, 6, 17);        // rule[23]
            AddExclusion(17, 9);           // rule[24]
            AddExclusion(6, 17);           // rule[26]
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            AddInclusion(13, 5);           // compat[12]
            AddInclusion(13, 4);           // compat[13]
            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]


            for (int zframeId = 0; zframeId < MaxEnumeration(); zframeId++)
            {
                if (CheckZFrame(zframeId))
                {
                    int[] thisState = GetBitPattern(zframeId);
                    ShowIntArray(thisState, 5);
                }
            }


        }


        static int[] offsets;
        static int[] layers;


        static bool[,] exclusions = new bool[100, 100];
        static bool[,] inclusions = new bool[100, 100];
        static void AddExclusion(int s1, int s2, int s3)
        {
            AddExclusion(s1, s2);
            AddExclusion(s1, s3);
            AddExclusion(s2, s3);
        }

        static void AddExclusion(int s1, int s2)
        {
            exclusions[s1, s2] = true;
            exclusions[s2, s1] = true;
        }
        static void AddInclusion(int s1, int s2)
        {
            inclusions[s1, s2] = true;
        }

        static int MaxEnumeration()
        {
            return 2 * offsets[^1];
        }


        static bool CheckZFrame(int zframe)
        {
            int[] state = GetBitPattern(zframe);
            for (int j = 2; j < offsets.Length; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    int s1 = state[i];
                    int s2 = state[j];
                    if (s1 == 0 || s2 == 0)
                    {
                        continue;
                    }
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

            for (int i = 1; i < offsets.Length; i++)
            {
                int s1 = state[i];
                if (s1 == 0) continue;
                for (int j = 1; j < offsets.Length; j++)
                {
                    if (inclusions[i, j] && state[j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static int[] GetBitPattern(int testNum)
        {
            int[] state = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                int res = (testNum / offsets[i]) % (layers[i] + 1);
                state[i] = res;
            }
            return state;
        }



        /*
         *
         *
         * for example for water_dota_pcgl_30_ps.vcs
         *
         * sf-index = [0    1    2    3    4    5    6    7    8    9   10   11]
         * offsets =  [1    1    2    4    8   16   32   64  128  384  768 1536]
         * layers =   [0    1    1    1    1    1    1    1    2    1    1    1]
         *
         * for blur_pcgl_30_ps.vcs (core)
         * offsets = [1    5]
         * layers =  [4    1]
         *
         *
         *
         *
         */
        static void GenerateOffsetAndLayers(ShaderFile shaderFile)
        {

            offsets = new int[shaderFile.sfBlocks.Count];
            layers = new int[shaderFile.sfBlocks.Count];

            offsets[0] = 1;
            layers[0] = shaderFile.sfBlocks[0].arg2;

            for (int i = 1; i < shaderFile.sfBlocks.Count; i++)
            {
                int curLayer = shaderFile.sfBlocks[i].arg2;
                layers[i] = curLayer;
                offsets[i] = offsets[i - 1] * (layers[i - 1] + 1);
            }
        }






    }
}











