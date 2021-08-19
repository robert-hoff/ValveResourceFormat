using MyValveResourceFormat.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.utilhelpers.snippetcode {

    class CrcTrialsMurmur32 {

        public static void RunTrials() {

            Trial1();
            // Trial2();
            // Trial3();
            // Trial4();
        }




        static void Trial1() {
            string databytes = "53 68 61 64 6F 77 73 4F 6E 6C 79"; // ShadowsOnly
            string theword = ByteStringtoString(databytes);

            uint MURMUR2SEED = 0x31415926; // It's pi!
            uint murmur32 = MurmurHash2.Hash(theword.ToLower(), MURMUR2SEED);

            Debug.WriteLine(theword);
            Debug.WriteLine($"{murmur32:X08}");
        }


        /*
         * Testing some predictable string for comparison with Java
         *
         */
        static void Trial2() {
            uint MURMUR2SEED = 0x31415926; // It's pi!
            // uint murmur32 = MurmurHash2.Hash("r", MURMUR2SEED);
            uint murmur32 = MurmurHash2.Hash("representativetexture", MURMUR2SEED);
            Debug.WriteLine($"{murmur32:X08}");
        }


        /*
         * testing the h*=M multiplication part to see how it compares with Java
         * It looks Java produces the same results for this operation when using signed ints
         *
         */
        static void Trial3() {
            uint seed = 0x31415926;
            // uint seed = 0xF1415926;
            uint M = 0x5bd1e995;
            uint length = 1;
            uint h = seed ^ (uint)length;
            h ^= 0xff;
            h *= M;
            Debug.WriteLine("{0:X08}", h);

        }


        static void Trial4() {
            uint MURMUR2SEED = 0x31415926;
            byte[] databytes = getDatabytesExample1();
            uint murmur32 = MurmurHash2.Hash(databytes, MURMUR2SEED);
            Debug.WriteLine($"{murmur32:X08}");

        }


        static string ByteStringtoString(string databytes) {
            string mystr = "";
            foreach (byte b in ParseString(databytes)) {
                mystr += (char)b;
            }

            return mystr;
        }


        static byte[] ParseString(String bytestring) {
            string[] tokens = bytestring.Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
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
        static byte[] getDatabytesExample1() {
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


    }



}



