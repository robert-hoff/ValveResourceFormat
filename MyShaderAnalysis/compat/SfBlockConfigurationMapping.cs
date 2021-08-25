using MyShaderAnalysis.vcsparsing;
using System;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;

namespace MyShaderAnalysis.compat
{

    public class SfBlockConfigurationMapping
    {

        ShaderFile shaderfile;

        public SfBlockConfigurationMapping(ShaderFile shaderfile)
        {
            this.shaderfile = shaderfile;
            GenerateOffsetAndStateLookups(shaderfile);
        }


        /*
         *
         * for example for water_dota_pcgl_30_ps.vcs
         *
         * sf-index =   [0    1    2    3    4    5    6    7    8    9   10   11]
         * offsets =    [1    1    2    4    8   16   32   64  128  384  768 1536]
         * nr_states =  [1    2    2    2    2    2    2    2    3    2    2    2]
         *
         * Note S_TOOLS_ENABLED only has one state (which is off). It appears to be disabled (possibly
         * because it is a dev parameter), it's also possibility that it is controlled by an external arguments.
         *
         *
         * for blur_pcgl_30_ps.vcs (core)
         * offsets   = [1    5]
         * nr_states = [5    2]
         *
         */
        private void GenerateOffsetAndStateLookups(ShaderFile shaderFile)
        {
            if (shaderFile.sfBlocks.Count == 0)
            {
                offsets = Array.Empty<int>();
                nr_states = Array.Empty<int>();
                return;
            }

            offsets = new int[shaderFile.sfBlocks.Count];
            nr_states = new int[shaderFile.sfBlocks.Count];

            offsets[0] = 1;
            nr_states[0] = shaderFile.sfBlocks[0].arg2 + 1;

            for (int i = 1; i < shaderFile.sfBlocks.Count; i++)
            {
                nr_states[i] = shaderFile.sfBlocks[i].arg2 + 1;
                offsets[i] = offsets[i - 1] * (nr_states[i - 1]);
            }
        }


        /*
         * getting the config state is not dependend on processing the configuration constraints (but is useful for verification)
         * It is much more efficient to move from a known zframeId to a configuration state
         */
        public int[] GetConfigState(long zframeId)
        {
            int[] state = new int[nr_states.Length];
            for (int i = 0; i < nr_states.Length; i++)
            {
                state[i] = (int)(zframeId / offsets[i] % nr_states[i]);
            }
            return state;
        }

        int[] offsets;
        int[] nr_states;
        bool[,] exclusions = new bool[100, 100];
        bool[,] inclusions = new bool[100, 100];

        void AddExclusionRule(int s1, int s2, int s3)
        {
            AddExclusionRule(s1, s2);
            AddExclusionRule(s1, s3);
            AddExclusionRule(s2, s3);
        }

        void AddExclusionRule(int s1, int s2)
        {
            exclusions[s1, s2] = true;
            exclusions[s2, s1] = true;
        }
        void AddInclusionRule(int s1, int s2)
        {
            inclusions[s1, s2] = true;
        }

        /*
         * possible zframe values are upto this value,
         * but not equal or exceeding
         *
         */
        public int MaxEnumeration()
        {
            return nr_states[^1] * offsets[^1];
        }
        bool CheckZFrame(int zframe)
        {
            int[] state = GetConfigState(zframe);
            // checking exclusion rules
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
            // checking inclusion rules
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

        public void ShowOffsetAndLayersArrays()
        {
            ShowIntArray(offsets, 8, "offsets", hex: true);
            ShowIntArray(nr_states, 8, "layers");

        }


    }
}

