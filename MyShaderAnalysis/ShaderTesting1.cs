using MyShaderAnalysis.readers01;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis {
    class ShaderTesting1 {


        // PCGL dirs
        const string EXPORT_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string EXPORT_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        // PC dirs
        const string EXPORT_DIR_PC_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string EXPORT_DIR_PC_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";





        /*
         *
         * I wrote this methods to test the zframe decomopression.
         * If decompressing the frames in a large file (hero_pcgl_30_ps.vcs) the memory usage
         * will grow to over 10 GB.
         * I disabled afterwards the autmatic decompression when instantiating the ShaderReader class
         *
         *
         *
         *
         */
        public static void RunTrials() {
            // Trial1();
            // Trial2();
        }





        // read one file to look at memory usage
        static void Trial2() {
            // reading this uses 10 GB of data, doesn't seem to be released - is this maybe a Debug thing?
            string filepath = EXPORT_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            ShaderReader01 shaderReader = new(filepath);
            Debug.WriteLine(filepath);
        }




        static void Trial1() {
            string[] files = Directory.GetFiles(EXPORT_DIR_NOT_CORE);


            // This works but the memory usage quickly grows upwards of 25 GB + (for the non-core dirs)
            // must be some memory leak or something going on! Resources are not being released
            // that was without disposing the decompressor! This has been fixed and the results are very different!!
            // It still isn't perfect though (far from it)

            foreach (string filepath in files) {
                if (!filepath.EndsWith("vcs")) {
                    continue;
                }
                ShaderReader01 shaderReader = new(filepath);
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
