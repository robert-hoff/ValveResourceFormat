using System;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace ValveResourceFormat.CompiledShader
{
    /*
     * ZFrameId to static-configuration mapping
     * ----------------------------------------
     *
     * BACKGROUND
     * ----------
     * We can define 'static configuration' as a particular state that the 'static shader variables' (defined by names given in the vcs files)
     * are allowed to take. A given state is described by assigning integers (that count from 0) to all static variables belonging to
     * the current context. Most of the static variables are by nature binary, accepting the values 0 or 1 (usually indicating some effect being
     * disabled or enabled).
     * For example F_HORZ_BLUR and F_MUL_COLOR_BY_ALPHA are two such variables, a possible state would be
     *
     * (F_HORZ_BLUR, F_MUL_COLOR_BY_ALPHA) = (1,0)
     *
     * If we reason that F_HORZ_BLUR and F_MUL_COLOR_BY_ALPHA may each be assigned the values 0 or 1 we can write all possible configurations as
     *
     * (F_HORZ_BLUR, F_MUL_COLOR_BY_ALPHA)[ID] = {(0,0),(0,1),(1,0),(1,1)}
     *
     * Defined like this the specific state (F_HORZ_BLUR, F_MUL_COLOR_BY_ALPHA)[2] = (1,0) can be considered as a member of a set. It is practical to
     * use an id enumerated from 0 to 3 to distinguish between members (see the next section for more details on this).
     * Knowledge of any variables' functionality is not strictly required to achieve the rendering. It is only their assigned state which is
     * essential; the calling application must therefore always present a legal configuration.
     *
     * Because the static variables are determined externally by the application's runtime,
     * no advanced assumptions are made for specific requests. However, if the number of states are small
     * (say less than 100,000) it's possible to fully describe and precompile assets for all possible responses. Designed in this
     * way, it means an important responsibility of the graphics system is to identify and retrieve such precompiled GPU assets.
     * This is computationally fast but involves a lot of redundancy on the disk; the vcs archives
     * tend to be large and in some cases includes many thousands of definitions for tasks that appear simple. Another design
     * decision present allieviating disk space somewhat is maintaining the GPU assets
     * under compression; GPU assets are decompressed at runtime as required.
     *
     * For our purposess we've called the GPU dataloads for *zframes*; that are containers of precompiled GPU code in the
     * form of GLSL source code, DXIL or VULKAN bytecode, and various header data. (This is not a Valve name, the name is drawn from the
     * code being organised as compressed 'ZStd frames'. There is a 1:1 correspondence between the compressed frames and
     * uncompressed dataloads needed to fulfill requests - hence the adoption of the name.)
     *
     * The configuration mapping performed in this file will match any valid static configuration with its predefined zframe. All legal
     * static configurations have a matching zframe. If the size of the state space is 4 it implies the existence of exactly
     * 4 predefined zframes. To complete steps in the rendering pipeline (e.g. vertex or pixel shading) only the
     * matching zframe is considered; all other zframes are ignored (and left in their compressed state).
     *
     *
     *
     * APPROACH
     * --------
     * The basic idea for mapping zframe-Ids to static configurations is by enumerating all possible
     * legal states and writing them (in order) next to the zframes.
     *
     * For example if there are 3 static-params (S1,S2,S3) that can each be in one of two states (on/off or 0/1)
     * they will combine to give 8 possible configurations, the zframe mapping will be
     *
     * zframeId S1 S2 S3
     *  0        0  0  0
     *  1        1  0  0
     *  2        0  1  0
     *  3        1  1  0
     *  4        0  0  1
     *  5        1  0  1
     *  6        0  1  1
     *  7        1  1  1
     *
     * Sometimes static-params have more than two states, for example S_DETAIL_2 from the Dota2 file
     * hero_pcgl_30_vs.vcs can be assigned to one of three states (identified as 'None', 'Add', 'Add Self Illum'). In our example,
     * if S2 is expanded to take the values (0,1,2) the number of possible configurations becomes 12 and a new
     * mapping would be written as
     *
     * zframeId S1 S2 S3
     *  0        0  0  0
     *  1        1  0  0
     *  2        0  1  0
     *  3        1  1  0
     *  4        0  2  0
     *  5        1  2  0
     *  6        0  0  1
     *  7        1  0  1
     *  8        0  1  1
     *  9        1  1  1
     * 10        0  2  1
     * 11        1  2  1
     *
     * In most shader files some static-combinations are not allowed. These are described by constraints specified
     * in the the Sf-constraints blocks. The most common types of constraints are mutual-exclusion and dependencies
     * between pairs of parameters.
     *
     * EXC(S1,S2) means S1 and S2 are mutually exclusive and cannot appear together
     * INC(S2,S3) means S2 is dependent on S3 and cannot appear by itself (but S3 can still appear without S2).
     *
     * To determine the configuration mapping where constraints are defined; the constraints are applied to the
     * mapping by deleting the rows that are disallowed. Importantly, the values of the zframeId's are left
     * unaltered. Applying this idea below, rows where S1 and S2 appeared together have been removed
     * and rows where S2 appeared without S3 have been removed.
     *
     *  zframeId S1 S2 S3
     *  0        0  0  0
     *  1        1  0  0
     *  6        0  0  1
     *  7        1  0  1
     *  8        0  1  1
     * 10        0  2  1
     *
     *
     * To calculate a configuration state from a zframeId observe (before any constraints are applied) that
     * S1 changes every 1 frame, S2 changes every 2 frames and S3 changes every 6 frames. The values (1,2,6) are the
     * number of successive frames that a state's digit is held constant, it is also equivalent to the offset where
     * a given state changes for the first time. (S1 first changes from 0 to 1 at offset=1, S2 first changes
     * from 0 to 1 at offset=2, and S3 first changes from 0 to 1 at offset=6). We collect these offsets together with
     * the number of states that each configuration can assume.
     *
     *            S1[0]       S2[1]       S3[2]
     * offset        1           2           6
     * nr_states     2           3           2
     *
     * It is possible to derive from this that a given zframeId can then be found as
     *
     *       state[i] = (zframeId / offset[i]) % nr_states[i]
     *
     * where zframeId / offset[i] is an integer division (the remainder is discarded)
     *
     *
     * For example, substituting zframeId = 10
     * S1 = 10 / offset[0] % nr_states[0] = (10 / 1) % 2 = 10 % 2 = 0
     * S2 = 10 / offset[1] % nr_states[1] = (10 / 2) % 3 =  5 % 3 = 2
     * S3 = 10 / offset[2] % nr_states[2] = (10 / 6) % 2 =  1 % 2 = 1
     *
     * Produces the last row in the table above (S1,S2,S3) = (0,2,1)
     *
     *
     * Dynamic-configurations
     * ----------------------
     * The same approach is also used to map from the dynamic-configuration to glsl (or given platform) source.
     * That is, the source-ids within the zframes enumerates and maps in the same way to dynamic-configurations
     * (these have their own constraints described by the 'DConstraintsBlocks'). The data that matches
     * dynamic configurations in the zframe files are called 'data-blocks'.
     *
     *
     */
    public class ConfigMappingSParams
    {
        private ShaderFile shaderfile;

        public ConfigMappingSParams(ShaderFile shaderfile)
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
         * because it is a dev parameter), it's also possible that it is controlled by an external arguments.
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
         * getting the config state is not dependent on processing the configuration constraints (but is useful for verification)
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
        public void ShowOffsetAndNrStatesArrays()
        {
            ShowIntArray(offsets, 8, "offsets", hex: true);
            ShowIntArray(nr_states, 8, "nr_states");

        }

    }
}
