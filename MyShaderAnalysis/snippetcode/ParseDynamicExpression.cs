using System;
using ValveResourceFormat.Serialization.VfxEval;

namespace MyShaderAnalysis.snippetcode
{
    class ParseDynamicExpression
    {
        public static void RunTrials()
        {
            //var testBytes = ParseString(
            //    "1D 37 2B 32 AB 07 DB 0F 49 40 15 07 00 00 34 43 16 08 00 09 00 06 01 00 1D D2 F6 9A C7 16 08 01 09 " +
            //    "00 06 00 00 1D D2 F6 9A C7 16 08 02 1D 16 82 0D 28 1D 16 82 0D 28 06 0B 00 07 AC C5 27 37 0F 04 45 " +
            //    "00 51 00 1D 16 82 0D 28 06 1B 00 15 02 56 00 07 00 00 00 00 1D CF 75 4A D4 13 07 00 00 00 3F 09 01 " +
            //    "09 02 14 09 02 09 01 13 06 1A 00 15 14 07 00 00 00 3F 13 08 03 09 02 09 01 09 03 1E 55 06 19 00 00");


            var testBytes = ParseString("1A 04 00");


            // -- not a dynamic expressions (it's a common header pattern in the zframes)
            // var testBytes = ParseString("0E 71 00");



            // string dynExpResult = new VfxEval(testBytes, omitReturnStatement: true).DynamicExpressionResult;
            string dynExpResult = new VfxEval(testBytes).DynamicExpressionResult;
            Console.WriteLine(dynExpResult);

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

