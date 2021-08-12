using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValveResourceFormat.Serialization.VfxEval;

namespace TestVRF
{
    public class RunVfxEval
    {

        public static void RunTrials()
        {

            Trial1();

        }

        static void Trial1()
        {

            byte[] databytes = example1();
            VfxEval vfxEval = new VfxEval(databytes);


            // Debug.WriteLine($"{vfxEval.ErrorMessage}");


            Debug.WriteLine($"{vfxEval.DynamicExpressionResult}");


        }




        /*
         *
         * v0 = rotation2d(12);
         * v1 = rotate2d(12,12);
         * v2 = sincos(10);
         * return v0;
         *
         *
         *
         */
        static byte[] example1()
        {
            String exampleStr = "07 00 00 40 41 06 25 00 08 00 07 00 00 40 41 07 00 00 40 41 06 26 00 08 01 07 00 00 20 41 06 27 00 08 02 09 00 00";
            return parseString(exampleStr);
        }






        static byte[] parseString(String bytestring)
        {
            string[] tokens = bytestring.Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }



    }
}
