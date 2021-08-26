using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.Serialization.VfxEval;

namespace ShaderAnalysis.utilhelpers
{
    public class TestVfxEvalShaderExpressions
    {

        public static void RunTrials()
        {
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial4();
            Trial5();
        }



        static void Trial5()
        {
            var exampleStr =
            "07 00 00 20 41 19 51 A2 54 EA 15 08 00 1F 51 A2 54 EA 06 00 00 04 1A 00 4F 00 " +
            "07 00 00 80 3F 04 24 00 47 00 07 00 00 00 00 04 2E 00 36 00 07 00 00 20 41 02 " +
            "44 00 07 00 00 20 41 07 00 00 20 41 15 06 03 00 02 4C 00 07 00 00 80 3F 02 54 " +
            "00 07 00 00 80 3F 00";
            byte[] databytes = ParseString(exampleStr);
            VfxEval vfxEval = new VfxEval(databytes, omitReturnStatement: true);
            Console.WriteLine($"{vfxEval.DynamicExpressionResult}");
        }



        // v0 = (EVAL[ab322b37]*3.1415927)/180;
        // v1 = cos(v0)/EVAL[c79af6d2];
        // v2 = sin(v0)/EVAL[c79af6d2];
        // v3 = ((((dot2(EVAL[280d8216],EVAL[280d8216])>1e-05) ? (EVAL[280d8216]*time()) : 0)+EVAL[d44a75cf])-(.5*float2(v1-v2,v2+v1)))+.5;
        // float3(v2,v1,v3.y)

        static void Trial4()
        {
            string byteString =
                "1D 37 2B 32 AB 07 DB 0F 49 40 15 07 00 00 34 43 16 08 00 09 00 06 01 00 1D D2 F6 9A C7 16 08 01 09 " +
                "00 06 00 00 1D D2 F6 9A C7 16 08 02 1D 16 82 0D 28 1D 16 82 0D 28 06 0B 00 07 AC C5 27 37 0F 04 45 " +
                "00 51 00 1D 16 82 0D 28 06 1B 00 15 02 56 00 07 00 00 00 00 1D CF 75 4A D4 13 07 00 00 00 3F 09 01 " +
                "09 02 14 09 02 09 01 13 06 1A 00 15 14 07 00 00 00 3F 13 08 03 09 02 09 01 09 03 1E 55 06 19 00 00";
            byte[] databytes = ParseString(byteString);
            VfxEval vfxEval = new VfxEval(databytes, omitReturnStatement: true);
            Console.WriteLine($"{vfxEval.DynamicExpressionResult}");
        }

        // (COND[19] && exists(UNKNOWN[7090a628])) ? UNKNOWN[1e52d0a1] : EVAL[b829896f]
        static void Trial3()
        {
            string byteString = "1A 13 04 0F 00 07 00 07 00 00 00 00 02 14 00 1F 28 A6 90 70 04 19 00 21 00 19 A1 D0 52 1E 02 26 00 1D 6F 89 29 B8 00";
            byte[] databytes = ParseString(byteString);
            VfxEval vfxEval = new VfxEval(databytes, omitReturnStatement: true);
            Console.WriteLine($"{vfxEval.DynamicExpressionResult}");
        }

        // SrgbGammaToLinear(EVAL[a392133c].xyz)
        static void Trial2()
        {
            string byteString = "1D 3C 13 92 A3 1E A4 06 1F 00 00";
            byte[] databytes = ParseString(byteString);
            VfxEval vfxEval = new VfxEval(databytes, omitReturnStatement: true);
            Console.WriteLine($"{vfxEval.DynamicExpressionResult}");
        }


        // COND[17] || 0
        static void Trial1()
        {
            string byteString = "1A 11 04 07 00 0F 00 07 00 00 80 3F 02 14 00 07 00 00 00 00 00";
            byte[] databytes = ParseString(byteString);
            Console.WriteLine($"{new VfxEval(databytes, omitReturnStatement: true).DynamicExpressionResult}");
        }



        private static byte[] ParseString(string bytestring)
        {
            var tokens = bytestring.Split(" ");
            var databytes = new byte[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }


    }
}
