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


        public bool HasData() {
            return offset < databytes.Length;
        }



        // this does not change the offset
        public List<int> searchForByteSequence(byte[] bytes) {
            List<int> indexes = new();
            for (int i = 0; i < databytes.Length-bytes.Length; i++) {
                Boolean match = true;
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
        public void showBytesSurrounding() {
            showBytesSurrounding(offset);
        }

        /*
         * does not change the offset
         *
         *
         */
        public void showBytesSurrounding(int ind) {
            for (int i = -20; i < 40; i++) {
                if (ind+i<0) {
                    Debug.Write($"   ");
                    continue;
                }
                if (ind+i > databytes.Length - 1) {
                    Debug.WriteLine($"EOF");
                    return;
                }
                string bytestr = String.Format($"{databytes[ind+i]:x02}");
                Debug.Write($"{bytestr} ");
            }
            Debug.WriteLine("");
        }






    }
}








