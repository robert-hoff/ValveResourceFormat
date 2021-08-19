using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.compat {


    /*
     *
     * STILL TO DO - complete the reverse verification of zframe generation
     * Meanwhile, use this method to printout the presumed configuration for every zframe, for every source file
     *
     *
     *
     *
     *
     */
    public class CompatRulesGeneration {

        ShaderFile shaderfile;

        public CompatRulesGeneration(ShaderFile shaderfile) {
            this.shaderfile = shaderfile;
            GenerateOffsetAndLayers(shaderfile);
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
        private void GenerateOffsetAndLayers(ShaderFile shaderFile) {

            if (shaderFile.sfBlocks.Count == 0) {
                offsets = Array.Empty<int>();
                layers = Array.Empty<int>();
                return;
            }

            offsets = new int[shaderFile.sfBlocks.Count];
            layers = new int[shaderFile.sfBlocks.Count];

            offsets[0] = 1;
            layers[0] = shaderFile.sfBlocks[0].arg2;

            for (int i = 1; i < shaderFile.sfBlocks.Count; i++) {
                int curLayer = shaderFile.sfBlocks[i].arg2;
                layers[i] = curLayer;
                offsets[i] = offsets[i - 1] * (layers[i - 1] + 1);
            }
        }




        /*
         * getting the config state is not dependend on processing the rules
         * As long as the system is verified it is much more efficient to move
         * from a known zframeId to a configuration state
         *
         *
         */
        public int[] GetConfigState(long zframeId) {
            int[] state = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++) {
                long res = (zframeId / offsets[i]) % (layers[i] + 1);
                state[i] = (int)res;
            }
            return state;
        }




        static int[] offsets;
        static int[] layers;


        static bool[,] exclusions = new bool[100, 100];
        static bool[,] inclusions = new bool[100, 100];
        void AddExclusion(int s1, int s2, int s3) {
            AddExclusion(s1, s2);
            AddExclusion(s1, s3);
            AddExclusion(s2, s3);
        }

        void AddExclusion(int s1, int s2) {
            exclusions[s1, s2] = true;
            exclusions[s2, s1] = true;
        }
        void AddInclusion(int s1, int s2) {
            inclusions[s1, s2] = true;
        }

        /*
         * possible zframe values are upto this value,
         * but not equal or exceeding
         *
         */
        int MaxEnumeration() {
            return 2 * offsets[^1];
        }


        bool CheckZFrame(int zframe) {
            int[] state = GetConfigState(zframe);
            // checking exclusion rules
            for (int j = 2; j < offsets.Length; j++) {
                for (int i = 1; i < j; i++) {
                    int s1 = state[i];
                    int s2 = state[j];
                    if (s1 == 0 || s2 == 0) {
                        continue;
                    }
                    if (exclusions[i, j] == true) {
                        return false;
                    }
                    if (inclusions[i, j] == true) {
                        return false;
                    }
                }
            }
            // checking inclusion rules
            for (int i = 1; i < offsets.Length; i++) {
                int s1 = state[i];
                if (s1 == 0) continue;
                for (int j = 1; j < offsets.Length; j++) {
                    if (inclusions[i, j] && state[j] == 0) {
                        return false;
                    }
                }
            }
            return true;
        }



        public void ShowOffsetAndLayersArrays() {
            ShowIntArray(offsets, 8, "offsets", hex: true);
            ShowIntArray(layers, 8, "layers");

        }





    }

}




