using MyShaderAnalysis.vcsparsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.compat {
    public class CompatRulesGeneric {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";

        public static void RunTrials() {
            Trial1();
        }



        static void Trial1() {
            // string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}\water_dota_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new(vcsFilenamepath);
            GenerateZFrames(shaderFile);
        }




        static void GenerateZFrames(ShaderFile shaderFile) {
            GenerateOffsetAndLayers(shaderFile);
        }


        static int[] offset;
        static int[] layers;

        static void GenerateOffsetAndLayers(ShaderFile shaderFile) {

            foreach (DataBlockSfBlock sfBlock in shaderFile.sfBlocks) {
                Debug.WriteLine($"{sfBlock.name0}");
            }


        }




    }
}











