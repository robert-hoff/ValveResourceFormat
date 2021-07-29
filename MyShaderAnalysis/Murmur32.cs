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

            string databytes = "53 68 61 64 6F 77 73 4F 6E 6C 79";
            string theword = byteStringtoString(databytes);

            uint MURMUR2SEED = 0x31415926; // It's pi!
            uint murmur32 = MurmurHash2.Hash(theword.ToLower(), MURMUR2SEED);


            Debug.WriteLine(theword);
            Debug.WriteLine($"{murmur32:X08}");


        }



        static string byteStringtoString(string databytes) {
            string mystr = "";
            foreach (byte b in ParseString(databytes)) {
                mystr += (char) b;
            }

            return mystr;
        }


        static byte[] ParseString(String bytestring)
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
