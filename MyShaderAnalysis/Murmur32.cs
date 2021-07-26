using MyValveResourceFormat.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis {
    class Murmur32 {


        public static void RunTrials() {
            uint MURMUR2SEED = 0x31415926; // It's pi!
            uint murmur32 = MurmurHash2.Hash("flGlobalLightAmbientShadowAmount", MURMUR2SEED);

            Debug.WriteLine($"{murmur32:X08}");


        }



    }
}
