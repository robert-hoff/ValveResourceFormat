using MyShaderAnalysis.readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis {
    class ShaderTesting {


        public static void RunTrials() {

            // Trial1();

        }





        // const string DECOMPILED_SHADERS_DIR = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        // const string DECOMPILED_SHADERS_DIR = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string DECOMPILED_SHADERS_DIR = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        // const string DECOMPILED_SHADERS_DIR = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";


        static void Trial1() {
            string[] files = Directory.GetFiles(DECOMPILED_SHADERS_DIR);


            // This works but the memory usage quickly grows upwards of 25 GB + (for the non-core dirs)
            // must be some memory leak or something going on! Resources are not being released

            foreach (string filepath in files) {
                if (filepath.Substring(filepath.Length - 3) != "vcs") {
                    continue;
                }
                ShaderReader shaderReader = new(filepath);
                Debug.WriteLine(filepath);
            }

            //for (int i = 240; i < files.Length; i++) {
            //    string filepath = files[i];
            //    if (filepath.Substring(filepath.Length - 3) != "vcs") {
            //        continue;
            //    }
            //    ShaderReader shaderReader = new(filepath);
            //    Debug.WriteLine(filepath);
            //}


        }




    }
}
