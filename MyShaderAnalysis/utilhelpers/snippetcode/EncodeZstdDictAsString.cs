using System;
using System.IO;

namespace MyShaderAnalysis.utilhelpers.snippetcode
{
    public class EncodeZstdDictAsString
    {

        public static void RunTrials()
        {
            EncodeDictionary();
            // Trials2();

        }



        static void EncodeDictionary()
        {
            byte[] zstdDict = File.ReadAllBytes("../../zstdictionary_2bc2fa87.dat");
            EncodeBytes(zstdDict);
        }


        static void Trials2()
        {
            byte[] hello2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            EncodeBytes(hello2);
        }


        static void Trials1()
        {
            byte[] hello2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            // byte[] hello2 = { 155, 27, 66, 4, 5, 6, 7, 8, 9, 10 };
            // int[] val = GetPiece(hello, 3);
            // int val = GetPiece2(hello, 1);
            GetPiece3(hello2, 0);
            GetPiece3(hello2, 1);
            GetPiece3(hello2, 2);
            GetPiece3(hello2, 3);
            // Decode("0@83");
            // Decode("Va]2");
        }


        /*
         * Note - encoder always assumes databytes requires 21846 * 4 characters to encode
         *
         */

        static void EncodeBytes(byte[] databytes)
        {

            string dict = "";
            for (int i = 0; i < 21846; i++)
            {
                dict += GetPiece3(databytes, i);
            }

            // use p as a placeholder in place of '\'
            dict = dict.Replace('\\', 'p');


            for (int i = 0; i < dict.Length; i += 100)
            {
                int end = i + 100;
                if (end > dict.Length)
                {
                    end = dict.Length;
                }
                Console.WriteLine($"\"{dict[i..end]}\"+");
            }
        }


        static byte[] Decode(string enc)
        {
            byte[] res = new byte[3];
            int val = ctv(enc[0], 18) + ctv(enc[1], 12) + ctv(enc[2], 6) + ctv(enc[3], 0);
            res[0] = (byte)(val >> 16);
            res[1] = (byte)(0xFF & (val >> 8));
            res[2] = (byte)(0xFF & val);
            return res;
        }

        static int ctv(char c, int shift)
        {
            return (c - 0x30) << shift;
        }



        static string GetPiece3(byte[] dict, int ind)
        {
            int val = 0;
            for (int i = ind * 3; i < ind * 3 + 3; i++)
            {
                val <<= 8;
                val += i < dict.Length ? dict[i] : 0;
            }

            string strval = $"{ValToChar(val, 18)}{ValToChar(val, 12)}{ValToChar(val, 6)}{ValToChar(val, 0)}";
            // string strval = $"{ValToChar(val>>18)}";

            // Debug.WriteLine($"{strval}");
            return strval;
        }

        static char ValToChar(int val, int shift)
        {
            return (char)((0x3F & (val >> shift)) + 0x30);
        }
        static char ValToChar(int val)
        {
            return (char)(val + 0x30);
        }


        static int GetPiece2(byte[] dict, int ind)
        {
            int val = 0;
            for (int i = ind * 3; i < ind * 3 + 3; i++)
            {
                val <<= 8;
                val += i < dict.Length ? dict[i] : 0;
            }
            return val;

        }

        static int[] GetPiece(byte[] dict, int ind)
        {
            int[] returnArr = new int[3];
            int start = ind * 3;
            for (int i = start; i < dict.Length && i < start + 3; i++)
            {
                returnArr[i - start] = dict[i];
            }
            return returnArr;

        }


    }
}



