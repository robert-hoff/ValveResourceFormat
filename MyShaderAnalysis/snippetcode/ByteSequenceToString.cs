using System.Diagnostics;
using System.Globalization;

namespace MyShaderAnalysis.snippetcode
{
    internal class ByteSequenceToString
    {
        public static void Run()
        {
            ConvertByteSequenceToString("30 50 51 52 53 5D"); // 0PQRS
        }

        public static void ConvertByteSequenceToString(string byteSequence)
        {
            string[] byteTokens = byteSequence.Split();
            string resultStr = "";
            foreach (string byteToken in byteTokens)
            {
                int intValue = int.Parse(byteToken, NumberStyles.HexNumber);
                resultStr += (char) intValue;
            }
            Debug.WriteLine($"{resultStr}");
        }
    }
}

