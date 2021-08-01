using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace MyShaderAnalysis.readers {

    public class DataReader {

        public byte[] databytes;
        public readonly int start;
        public int offset;


        public DataReader(byte[] data, int start) {
            this.databytes = data;
            this.start = start;
            this.offset = start;
        }

        public DataReader(byte[] data) : this(data, 0) { }

        public void ResetOffset() {
            offset = start;
        }

        public byte ReadByte() {
            return databytes[offset++];
        }

        public byte ReadByteAtPosition(int fromInd) {
            return databytes[start + fromInd];
        }

        public uint ReadUInt16() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint uint0 = (b1 << 8) + b0;
            return uint0;
        }

        public uint ReadUInt16AtPosition(int fromInd) {
            uint b0 = databytes[start + fromInd];
            uint b1 = databytes[start + fromInd + 1];
            uint int0 = (b1 << 8) + b0;
            return int0;
        }

        // need to do it like this for signed values to work
        public int ReadInt16() {
            short b0 = ReadByte();
            short b1 = ReadByte();
            b1 <<= 8;
            b1 += b0;
            return b1;
        }

        public int ReadInt16AtPosition(int fromInd) {
            short b1 = databytes[start + fromInd + 1];
            b1 <<= 8;
            b1 += databytes[start + fromInd];
            return b1;
        }

        public uint ReadUInt() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint b2 = ReadByte();
            uint b3 = ReadByte();
            uint int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }

        public uint ReadUIntAtPosition(int fromInd) {
            uint b0 = databytes[start + fromInd];
            uint b1 = databytes[start + fromInd + 1];
            uint b2 = databytes[start + fromInd + 2];
            uint b3 = databytes[start + fromInd + 3];
            uint int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }
        public int ReadInt() {
            int b0 = ReadByte();
            int b1 = ReadByte();
            int b2 = ReadByte();
            int b3 = ReadByte();
            int int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }

        public int ReadIntAtPosition() {
            return ReadIntAtPosition(offset);
        }

        public int ReadIntAtPosition(int fromInd) {
            int b0 = databytes[start + fromInd];
            int b1 = databytes[start + fromInd + 1];
            int b2 = databytes[start + fromInd + 2];
            int b3 = databytes[start + fromInd + 3];
            int int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }

        public float ReadFloat() {
            float float0 = BitConverter.ToSingle(databytes, offset);
            offset += 4;
            return float0;
        }

        public float ReadFloatAtPosition(int fromInd) {
            float float0 = BitConverter.ToSingle(databytes, fromInd);
            return float0;
        }
        public byte[] ReadBytes(int len) {
            byte[] bytes0 = new byte[len];
            for (int i = 0; i < len; i++) {
                bytes0[i] = ReadByte();
            }
            return bytes0;
        }

        public byte[] ReadBytesAtPosition(int fromInd, int len) {
            byte[] bytes0 = new byte[len];
            for (int i = 0; i < len; i++) {
                bytes0[i] = databytes[start + fromInd + i];
            }
            return bytes0;
        }

        public string ReadNullTermStringAtPosition() {
            return ReadNullTermStringAtPosition(offset);
        }

        public string ReadNullTermStringAtPosition(int ind) {
            string str = "";
            while (databytes[ind] > 0) {
                str += (char)databytes[ind++];
            }
            return str;
        }




        public void ShowByteCount() {
            Debug.Write($"[{offset}]\n");
        }

        public void ShowByteCount(string message) {
            Debug.Write($"[{offset}] {message}\n");
        }

        public void ShowBytes(int len) {
            ShowBytes(len, 32, true);
        }

        public void ShowBytes(int len, bool breakLine) {
            ShowBytes(len, 32, breakLine);
        }

        public void ShowBytes(int len, int breakLen) {
            ShowBytes(len, breakLen, true);
        }

        public void ShowBytes(int len, int breakLen, bool breakLine) {
            byte[] bytes0 = ReadBytes(len);
            // ShowBytesByteArray(b, breakLen);
            string byteString = BytesToString(bytes0);
            Debug.Write(byteString+(breakLine ? "\n":""));
        }

        public void BreakLine() {
            Debug.Write("\n");
        }

        public void Comment(string message) {
            TabComment(message, 0, true);
        }

        public void Comment(string message, bool useSlashes) {
            TabComment(message, 0, useSlashes);
        }

        public void TabComment(string message) {
            TabComment(message, 4, true);
        }

        public void TabComment(string message, int tabLength) {
            TabComment(message, tabLength, true);
        }

        public void TabComment(string message, int tabLength, bool useSlashes) {
            Debug.Write($"{"".PadLeft(tabLength)}"+(useSlashes?"// ":"")+$"{message}\n");
        }




        public static string BytesToString(byte[] databytes) {
            return BytesToString(databytes, 32);
        }

        public static string BytesToString(byte[] databytes, int breakLen) {
            if (breakLen == -1) {
                breakLen = 999999;
            }
            int count = 0;
            string bytestring = "";
            for (int i = 0; i < databytes.Length; i++) {
                bytestring += $"{databytes[i]:X02} ";
                if (++count % breakLen == 0) {
                    bytestring += "\n";
                }
            }
            return bytestring.Trim();
        }



    }




}







