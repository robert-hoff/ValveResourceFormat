using System;
using System.Diagnostics;
using System.IO;

namespace MyShaderAnalysis.vcsparsing {

    public class ShaderDataReader {

        public int offset { get; set; }
        public byte[] databytes { get; }
        public ShaderDataReader(byte[] data) {
            this.databytes = data;
            this.offset = 0;
        }

        private bool disableOutput = false;
        public void SetDisableOutput(bool disableOutput) {
            this.disableOutput = disableOutput;
        }
        private StreamWriter sw = null;
        public void ConfigureWriteToFile(StreamWriter sw, bool disableOutput) {
            this.sw = sw;
            this.disableOutput = disableOutput;
        }
        public void OutputWrite(string text) {
            if (!disableOutput) {
                Debug.Write(text);
            }
            if (sw != null) {
                sw.Write(text);
            }
        }
        public void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }



        public byte ReadByte() {
            return databytes[offset++];
        }

        public byte ReadByteAtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            return databytes[fromInd];
        }

        public uint ReadUInt16() {
            uint uint0 = ReadUInt16AtPosition();
            offset += 2;
            return uint0;
        }

        public uint ReadUInt16AtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            uint b0 = databytes[fromInd];
            uint b1 = databytes[fromInd + 1];
            uint int0 = (b1 << 8) + b0;
            return int0;
        }

        public int ReadInt16() {
            int int0 = ReadInt16AtPosition();
            offset += 2;
            return int0;
        }

        // need to do it like this for signed values to work
        public int ReadInt16AtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            short b1 = databytes[fromInd + 1];
            b1 <<= 8;
            b1 += databytes[fromInd];
            return b1;
        }

        public uint ReadUInt() {
            uint int0 = ReadUIntAtPosition();
            offset += 4;
            return int0;
        }

        public uint ReadUIntAtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            uint b0 = databytes[fromInd];
            uint b1 = databytes[fromInd + 1];
            uint b2 = databytes[fromInd + 2];
            uint b3 = databytes[fromInd + 3];
            uint int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }

        public int ReadInt() {
            int int0 = ReadIntAtPosition();
            offset += 4;
            return int0;
        }

        public int ReadIntAtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            int b0 = databytes[fromInd];
            int b1 = databytes[fromInd + 1];
            int b2 = databytes[fromInd + 2];
            int b3 = databytes[fromInd + 3];
            int int0 = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return int0;
        }

        public long ReadLong() {
            long long0 = 0;
            for (int i = 7; i >= 0; i--) {
                long0 <<= 8;
                long0 |= databytes[offset + i];
            }
            offset += 8;
            return long0;
        }

        public float ReadFloat() {
            float float0 = ReadFloatAtPosition();
            offset += 4;
            return float0;
        }

        public float ReadFloatAtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            float float0 = BitConverter.ToSingle(databytes, fromInd);
            return float0;
        }

        public byte[] ReadBytes(int len) {
            byte[] bytes0 = ReadBytesAtPosition(0, len);
            offset += len;
            return bytes0;
        }

        public string ReadBytesAsString(int len) {
            byte[] bytes0 = ReadBytesAtPosition(0, len);
            offset += len;
            return BytesToString(bytes0);
        }

        public byte[] ReadBytesAtPosition(int ind, int len, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            byte[] bytes0 = new byte[len];
            for (int i = 0; i < len; i++) {
                bytes0[i] = databytes[fromInd + i];
            }
            return bytes0;
        }

        public string ReadNullTermString() {
            string str = ReadNullTermStringAtPosition();
            offset += str.Length + 1;
            return str;
        }

        public string ReadNullTermStringAtPosition(int ind = 0, bool rel = true) {
            int fromInd = rel ? offset + ind : ind;
            string str = "";
            while (databytes[fromInd] > 0) {
                str += (char)databytes[fromInd++];
            }
            return str;
        }

        public void PrintIntWithValue() {
            int intval = ReadIntAtPosition();
            ShowBytes(4, breakLine: false);
            TabComment($"{intval}");
        }

        public void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
            ShowByteCount();
            OutputWriteLine("EOF");
            BreakLine();
        }

        public void ShowByteCount(string message = null) {
            OutputWrite($"[{offset}]{(message != null ? " "+message : "")}\n");
        }

        public void ShowBytes(int len, string message = null, int tabLen = 4, bool use_slashes = true, bool breakLine = true) {
            ShowBytes(len, 32, message, tabLen, use_slashes, breakLine);
        }

        public void ShowBytes(int len, int breakLen, string message = null, int tabLen = 4, bool use_slashes = true, bool breakLine = true) {
            byte[] bytes0 = ReadBytes(len);
            string byteString = BytesToString(bytes0, breakLen);
            OutputWrite(byteString);
            if (message != null) {
                TabComment(message, tabLen, use_slashes);
            }
            if (message == null && breakLine) {
                BreakLine();
            }
        }

        public void ShowBytesAtPosition(int ind, int len, int breakLen = 32, bool rel = true) {
            byte[] bytes0 = ReadBytesAtPosition(ind, len, rel);
            string bytestr = BytesToString(bytes0, breakLen);
            OutputWriteLine($"{bytestr}");
        }

        public void BreakLine() {
            OutputWrite("\n");
        }

        public void Comment(string message) {
            TabComment(message, 0, true);
        }

        public void TabComment(string message, int tabLen = 4, bool useSlashes = true) {
            OutputWrite($"{"".PadLeft(tabLen)}{(useSlashes ? "// " : "")}{message}\n");
        }

        public static string BytesToString(byte[] databytes, int breakLen = 32) {
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









