using MyShaderAnalysis.vcsparsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyShaderAnalysis.compat {


    public class CompatRulesNewApproach {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";

        public static void RunTrials() {


            Trials1();

        }
        static void Trials1() {

            ShaderFile shaderFile = new(@$"{PCGL_DIR_NOT_CORE}\water_dota_pcgl_30_ps.vcs");
            foreach (var sfBlock in shaderFile.sfBlocks) {

                Debug.WriteLine($"{sfBlock.name0}");
            }


        }


        



        static bool CheckZFrame(int zframeId) {



            return false;
        }








    }



}






