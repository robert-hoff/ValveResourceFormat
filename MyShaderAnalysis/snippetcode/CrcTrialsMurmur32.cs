using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyShaderAnalysis.util;
using MyShaderFile.ThirdParty;

namespace MyShaderAnalysis.snippetcode
{
    public class CrcTrialsMurmur32
    {
        private const uint MURMUR2SEED = 0x31415926; // It's pi!

        public static void RunTrials()
        {
            // Trial5();
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial4();

            SaveMurmurStringsForParams();
            // ShowMurmurStringsForParams();
            // ReadParameterNames();

            // Debug.WriteLine($"{Murmur32("$TRANS_OFFSET_V"):x08}");
        }

        public static void SaveMurmurStringsForParams()
        {
            List<string> murmurs = GetMurmurStringsForParams();
            SaveDataToFile.SaveSingleColumnStringData("paramname-murmurs.txt", murmurs);
        }

        public static void ShowMurmurStringsForParams()
        {
            List<string> murmurs = GetMurmurStringsForParams();
            foreach (string murmurString in murmurs)
            {
                Debug.WriteLine($"{murmurString}");
            }
        }

        public static List<string> GetMurmurStringsForParams()
        {
            List<string> murmurs = new();
            foreach (string paramname in ReadParameterNames())
            {
                uint murmur32 = Murmur32(paramname);
                murmurs.Add($"{murmur32:x08} {paramname}");
            }
            murmurs.Sort();
            return murmurs;
        }

        public static void Trial5()
        {
            // string myStr = "g_flRimLightScale";
            string myStr = "g_flAmbientScale";
            uint murmur32 = MurmurHash2.Hash(myStr.ToLower(), MURMUR2SEED);
            Console.WriteLine($"{murmur32:X08}");
        }

        public static void Trial1()
        {
            string databytes = "53 68 61 64 6F 77 73 4F 6E 6C 79"; // ShadowsOnly
            string theword = ByteStringtoString(databytes);
            uint murmur32 = MurmurHash2.Hash(theword.ToLower(), MURMUR2SEED);

            Console.WriteLine(theword);
            Console.WriteLine($"{murmur32:X08}");
        }

        /*
         * Testing some predictable string for comparison with Java
         *
         */
        public static void Trial2()
        {
            // uint murmur32 = MurmurHash2.Hash("r", MURMUR2SEED);
            // uint murmur32 = MurmurHash2.Hash("representativetexture", MURMUR2SEED);
            uint murmur32 = MurmurHash2.Hash("g_flDetailBlendToFull".ToLower(), MURMUR2SEED);
            Console.WriteLine($"{murmur32:X08}");
        }

        /*
         * testing the h*=M multiplication part to see how it compares with Java
         * It looks Java produces the same results for this operation when using signed ints
         *
         */
        public static void Trial3()
        {
            uint seed = 0x31415926;
            // uint seed = 0xF1415926;
            uint M = 0x5bd1e995;
            uint length = 1;
            uint h = seed ^ length;
            h ^= 0xff;
            h *= M;
            Console.WriteLine("{0:X08}", h);
        }

        public static void Trial4()
        {
            byte[] databytes = GetDatabytesExample1();
            uint murmur32 = MurmurHash2.Hash(databytes, MURMUR2SEED);
            Console.WriteLine($"{murmur32:X08}");
        }

        public static string ByteStringtoString(string databytes)
        {
            string mystr = "";
            foreach (byte b in ParseString(databytes))
            {
                mystr += (char) b;
            }

            return mystr;
        }

        public static byte[] ParseString(string bytestring)
        {
            string[] tokens = bytestring.Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }

        /*
         * This is a buffer entry from generic_light_pcgl_30_vs.vcs
         * [5508] BUFFER-BLOCK[1] IrradTextureActiveRectCB
         *
         * Tried to reproduce the buffer block crc, but couldn't get it (also tried crc32)
         *
         */
        public static byte[] GetDatabytesExample1()
        {
            string bytestring = "" +
                "49 72 72 61 64 54 65 78 74 75 72 65 41 63 74 69 76 65 52 65 63 74 43 42 00 00 00 00 00 00 00 00 " +
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
                "10 00 00 00 " +
                "00 00 00 00 " +
                "01 00 00 00 " +
                "67 5F 76 49 6E 76 49 72 72 61 64 54 65 78 44 69 6D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
                "00 00 00 00 " +
                "04 00 00 00 01 00 00 00 01 00 00 00";
            return ParseString(bytestring);
        }

        public static uint Murmur32(string input)
        {
            return MurmurHash2.Hash(input.ToLower(), MURMUR2SEED);
        }

        public static List<string> ReadParameterNames()
        {
            List<string> data = ReadDataFromFile.ReadSingleColumnStringData("param-names.txt");
            return data;
        }
    }
}


