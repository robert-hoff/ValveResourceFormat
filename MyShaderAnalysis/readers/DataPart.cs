using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers {


    public class DataPart {

        public int length;
        public DataReader datareader;


        public DataPart(byte[] databytes) {
            this.length = databytes.Length;
            this.datareader = new DataReader(databytes, 0);
        }

        public DataPart(byte[] databytes, int offset) {
            this.length = databytes.Length;
            this.datareader = new DataReader(databytes, offset);
        }

        public DataPart(byte[] databytes, int offset, int length) {
            if (length > databytes.Length - offset) {
                throw new Exception("this doesn't make any sense");
            }
            this.length = length;
            this.datareader = new DataReader(databytes, offset);
        }



        public void PrintFirstNrOfByte(int count) {
            byte[] segmentbytes = datareader.ReadBytesAtPosition(0, length);
            int i = 0;
            while (i < count) {
                if (i == segmentbytes.Length) {
                    Debug.WriteLine("EOF");
                    return;
                }

                Debug.Write($"{segmentbytes[i]:X02} ");
                if (++i % 32 == 0) {
                    Debug.WriteLine("");
                }
            }
        }


        public void PrintAllBytes() {
            byte[] segmentbytes = datareader.ReadBytesAtPosition(0, length);
            int i = 0;
            while (i < segmentbytes.Length) {
                Debug.Write($"{segmentbytes[i]:X02} ");
                if (++i % 32 == 0) {
                    Debug.WriteLine("");
                }
            }
            Debug.WriteLine("");
        }



    }



}







