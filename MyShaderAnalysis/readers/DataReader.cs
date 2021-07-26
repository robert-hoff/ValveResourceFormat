using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis.readers {
    public class DataReader {

        public byte[] databytes;
        public readonly int start;
        public int offset;


        public DataReader(byte[] data) {
            this.databytes = data;
            this.start = 0;
            this.offset = 0;
        }
        public DataReader(byte[] data, int start) {
            this.databytes = data;
            this.start = start;
            this.offset = start;
        }

        public void ResetOffset() {
            offset = start;
        }


        public byte ReadByte() {
            return databytes[offset++];
        }

        public byte ReadByteAtPosition(int ind) {
            return databytes[start+ind];
        }

        public uint ReadUInt16() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint intval = (b1<<8) + b0;
            return intval;
        }
        public uint ReadUInt() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint b2 = ReadByte();
            uint b3 = ReadByte();
            uint intval = (b3<<24) + (b2<<16) + (b1<<8) + b0;
            return intval;
        }

        // read uint32 without changing the offset
        public uint ReadUIntAtPosition(int ind) {
            uint b0 = databytes[start+ind];
            uint b1 = databytes[start+ind+1];
            uint b2 = databytes[start+ind+2];
            uint b3 = databytes[start+ind+3];
            uint intval = (b3<<24) + (b2<<16) + (b1<<8) + b0;
            return intval;
        }

        public int ReadInt() {
            int b0 = ReadByte();
            int b1 = ReadByte();
            int b2 = ReadByte();
            int b3 = ReadByte();
            int intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return intval;
        }

        // read uint32 without changing the offset
        public int ReadIntAtPosition(int ind) {
            int b0 = databytes[start+ind];
            int b1 = databytes[start+ind+1];
            int b2 = databytes[start+ind+2];
            int b3 = databytes[start+ind+3];
            int intval = (b3<<24) + (b2<<16) + (b1<<8) + b0;
            return intval;
        }

        public float ReadFloat() {
            float floatval = BitConverter.ToSingle(databytes, offset);
            offset += 4;
            return floatval;
        }

        public byte[] ReadBytes(int len) {
            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++) {
                bytes[i] = ReadByte();
            }
            return bytes;
        }

        public byte[] ReadBytesAtPosition(int len, int ind) {
            byte[] segmentbytes = new byte[len];
            for (int i = 0; i < len; i++) {
                segmentbytes[i] = databytes[start+ind+i];
            }
            return segmentbytes;
        }



        public string readNullTermStringAtPosition(int ind) {
            string str = "";
            while (databytes[ind] > 0) {
                str += (char) databytes[ind++];
            }
            return str;
        }



        public bool HasData() {
            return offset < databytes.Length;
        }




        // this does not change the offset
        public List<int> SearchForByteSequence(byte[] bytes) {
            List<int> indexes = new();
            for (int i = 0; i < databytes.Length-bytes.Length; i++) {
                bool match = true;
                for (int j = 0; j < bytes.Length; j++) {
                    if (databytes[i+j] != bytes[j]) {
                        match = false;
                        break;
                    }
                }
                if (match) {
                    indexes.Add(i);
                }
            }
            return indexes;
        }



        /*
         *
         * show bytes around the current offset
         *
         */
        public void ShowBytesSurrounding() {
            ShowBytesSurrounding(offset);
        }

        /*
         * does not change the offset
         *
         *
         */
        public void ShowBytesSurrounding(int ind) {
            for (int i = -20; i < 40; i++) {
                if (ind+i<0) {
                    Debug.Write($"   ");
                    continue;
                }
                if (ind+i > databytes.Length - 1) {
                    Debug.WriteLine($"EOF");
                    return;
                }
                string bytestr = string.Format($"{databytes[ind+i]:X02}");
                Debug.Write($"{bytestr} ");
            }
            Debug.WriteLine("");
        }

        public void ShowAllBytes() {
            ShowBytes(0, databytes.Length, 32);
        }


        public void ShowBytes(int fromInd, int len) {
            ShowBytes(fromInd, len, 32);
        }


        // FIXME - the method doesn't use the start stuff

        public void ShowBytes(int fromInd, int len, int breakLen) {
            int count = 0;
            for (int i = fromInd; i < fromInd+len; i++) {
                if (i==databytes.Length) {
                    Debug.WriteLine("EOF");
                    return;
                }
                Debug.Write($"{databytes[i]:X02} ");
                if (++count%breakLen== 0) {
                    Debug.WriteLine("");
                }
            }
            if (count%breakLen!=0) {
                Debug.WriteLine("");
            }
        }



        public void Print484BlockPrev(int ind) {
            ShowBytes(ind,64);
            ShowBytes(ind+64,64);
            ShowBytes(ind+128,4);
            ShowBytes(ind+132,64);
            ShowBytes(ind+196,96);
            ShowBytes(ind+292,128,16);
            ShowBytes(ind+420,64);
        }

        public void Print484Block(int ind) {
            string name1 = readNullTermStringAtPosition(ind);
            Debug.WriteLine($"[{ind}] {name1}");
            ShowBytes(ind,64);
            string name2 = readNullTermStringAtPosition(ind+64);
            if (name2.Length>0) {
                Debug.WriteLine($"[{ind+64}] {name2}");
            }
            ShowBytes(ind+64,64);
            ShowBytes(ind+128,8);
            string name3 = readNullTermStringAtPosition(ind+136);
            if (name3.Length>0) {
                Debug.WriteLine($"[{ind+136}] {name3}");
            }
            ShowBytes(ind+136,64);
            ShowBytes(ind+200,28);
            ShowBytes(ind+228,64);
            ShowBytes(ind+292,128,16);
            ShowBytes(ind+420,64);
        }



    }
}









