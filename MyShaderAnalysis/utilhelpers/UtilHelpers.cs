using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.readers;


namespace MyShaderAnalysis.utilhelpers {


    public class UtilHelpers {



        public static void RunTrials() {
            PrintByteCounter();
        }




        static void PrintByteCounter() {

            for (int i = 0; i < 8; i++) {
                Debug.WriteLine($"({i * 32 + 216}) 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            }

        }





    }


}






