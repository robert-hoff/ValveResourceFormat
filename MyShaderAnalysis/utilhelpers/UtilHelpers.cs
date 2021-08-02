using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis.utilhelpers {


    public class UtilHelpers {





        public static string RemoveBaseDir(string filenamepath) {
            string dirname = Path.GetDirectoryName(filenamepath);
            string filename = Path.GetFileName(filenamepath);
            if (dirname.EndsWith(@"\shaders\vfx")) {
                return @"\shaders\vfx\" + filename;
            }
            else if (dirname.EndsWith(@"\shaders-core\vfx")) {
                return @"\shaders-core\vfx\" + filename;
            }
            else {

                return filenamepath;
            }
        }



        public static uint MurmurHashPiSeed(byte[] data) {
            uint PI_SEED = 0x31415926;
            return MurmurHash(data, PI_SEED);
        }
        public static uint MurmurHashPiSeed(string data) {
            uint PI_SEED = 0x31415926;
            return MurmurHash(data, PI_SEED);
        }


        public static uint MurmurHash(string data, uint seed) => MurmurHash(Encoding.ASCII.GetBytes(data), seed);

        public static uint MurmurHash(byte[] data, uint seed) {
            const uint M = 0x5bd1e995;
            const int R = 24;
            int length = data.Length;
            if (length == 0) {
                return 0;
            }
            uint h = seed ^ (uint)length;
            int ind = 0;
            while (length >= 4) {
                uint k = (uint)(data[ind++] | data[ind++] << 8 | data[ind++] << 16 | data[ind++] << 24);
                k *= M;
                k ^= k >> R;
                k *= M;
                h *= M;
                h ^= k;
                length -= 4;
            }
            switch (length) {
                case 3:
                    h ^= (ushort)(data[ind++] | data[ind++] << 8);
                    h ^= (uint)(data[ind] << 16);
                    h *= M;
                    break;
                case 2:
                    h ^= (ushort)(data[ind++] | data[ind] << 8);
                    h *= M;
                    break;
                case 1:
                    h ^= data[ind];
                    h *= M;
                    break;
                default:
                    break;
            }
            h ^= h >> 13;
            h *= M;
            h ^= h >> 15;
            return h;
        }





    }


}






