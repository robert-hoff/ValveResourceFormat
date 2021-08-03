using System;
using System.Diagnostics;
using System.IO;
// using MyShaderAnalysis.utilhelpers;

/*
 *
 * I don't think I need this start thing, because I don't think I ever need to use relative offsets
 *
 *
 */
namespace MyShaderAnalysis.vcsparsing01 {

    public class DataReader01 {

        public byte[] databytes;
        public readonly int start;
        protected int offset;

        protected StreamWriter sw = null;
        public bool DisableOutput = false;


        public DataReader01(byte[] data, int start) {
            this.databytes = data;
            this.start = start;
            this.offset = start;
        }

        public DataReader01(byte[] data) : this(data, 0) { }

        public void ResetOffset() {
            offset = start;
        }

        public int GetFileOffset() {
            return offset;
        }

        public byte ReadByte() {
            return databytes[start + offset++];
        }

        public byte ReadByteAtPosition() {
            return ReadByteAtPosition(offset);
        }

        public byte ReadByteAtPosition(int fromInd) {
            return databytes[start + fromInd];
        }

        public uint ReadUInt16() {
            uint uint0 = ReadUInt16AtPosition(offset);
            offset += 2;
            return uint0;
        }

        public uint ReadUInt16AtPosition() {
            return ReadUInt16AtPosition(offset);
        }

        public uint ReadUInt16AtPosition(int fromInd) {
            uint b0 = databytes[start + fromInd];
            uint b1 = databytes[start + fromInd + 1];
            uint int0 = (b1 << 8) + b0;
            return int0;
        }

        public int ReadInt16() {
            int int0 = ReadInt16AtPosition(offset);
            offset += 2;
            return int0;
        }

        public int ReadInt16AtPosition() {
            return ReadInt16AtPosition(offset);
        }

        // need to do it like this for signed values to work
        public int ReadInt16AtPosition(int fromInd) {
            short b1 = databytes[start + fromInd + 1];
            b1 <<= 8;
            b1 += databytes[start + fromInd];
            return b1;
        }

        public uint ReadUInt() {
            uint int0 = ReadUIntAtPosition(offset);
            offset += 4;
            return int0;
        }

        public uint ReadUIntAtPosition() {
            return ReadUIntAtPosition(offset);
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
            int int0 = ReadIntAtPosition(offset);
            offset += 4;
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
            float float0 = ReadFloatAtPosition(offset);
            offset += 4;
            return float0;
        }

        public float ReadFloatAtPosition() {
            return ReadFloatAtPosition(offset);
        }

        public float ReadFloatAtPosition(int fromInd) {
            float float0 = BitConverter.ToSingle(databytes, start+fromInd);
            return float0;
        }

        public byte[] ReadBytes(int len) {
            byte[] bytes0 = ReadBytesAtPosition(offset, len);
            offset += len;
            return bytes0;
        }

        public byte[] ReadBytesAtPosition(int fromInd, int len) {
            byte[] bytes0 = new byte[len];
            for (int i = 0; i < len; i++) {
                bytes0[i] = databytes[start + fromInd + i];
            }
            return bytes0;
        }

        public string ReadNullTermString() {
            string str = ReadNullTermStringAtPosition(offset);
            return null;
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
            OutputWrite($"[{offset}]\n");
        }

        public void ShowByteCount(string message) {
            OutputWrite($"[{offset}] {message}\n");
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
            string byteString = BytesToString(bytes0, breakLen);
            OutputWrite(byteString+(breakLine ? "\n":""));
        }

        public void ShowBytesAtPosition(int fromInd, int len) {
            ShowBytesAtPosition(fromInd, len, 32);
        }

        public void ShowBytesAtPosition(int fromInd, int len, int breakLen) {
            byte[] bytes0 = ReadBytesAtPosition(fromInd, len);
            string bytestr = BytesToString(bytes0, breakLen);
            OutputWriteLine($"{bytestr}");
        }

        public void BreakLine() {
            OutputWrite("\n");
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
            OutputWrite($"{"".PadLeft(tabLength)}"+(useSlashes?"// ":"")+$"{message}\n");
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
        public void ConfigureWriteToFile(StreamWriter sw, bool disableOutput) {
            this.sw = sw;
            this.DisableOutput = disableOutput;
        }
        public void OutputWrite(string text) {
            if (!DisableOutput)  {
                Debug.Write(text);
            }
            if (sw != null) {
                sw.Write(text);
            }
        }

        public void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }




}









