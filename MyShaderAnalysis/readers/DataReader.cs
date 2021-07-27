using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis.readers {
    public class DataReader {

        public byte[] databytes;
        public readonly int start;
        public int offset;
        StreamWriter sw = null;


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
            return databytes[start + ind];
        }

        public uint ReadUInt16() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint intval = (b1 << 8) + b0;
            return intval;
        }
        public uint ReadUInt() {
            uint b0 = ReadByte();
            uint b1 = ReadByte();
            uint b2 = ReadByte();
            uint b3 = ReadByte();
            uint intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return intval;
        }

        // read uint32 without changing the offset
        public uint ReadUIntAtPosition(int ind) {
            uint b0 = databytes[start + ind];
            uint b1 = databytes[start + ind + 1];
            uint b2 = databytes[start + ind + 2];
            uint b3 = databytes[start + ind + 3];
            uint intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
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
            int b0 = databytes[start + ind];
            int b1 = databytes[start + ind + 1];
            int b2 = databytes[start + ind + 2];
            int b3 = databytes[start + ind + 3];
            int intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
            return intval;
        }

        public float ReadFloat() {
            float floatval = BitConverter.ToSingle(databytes, offset);
            offset += 4;
            return floatval;
        }
        public float ReadFloatAtPosition(int ind) {
            float floatval = BitConverter.ToSingle(databytes, ind);
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
                segmentbytes[i] = databytes[start + ind + i];
            }
            return segmentbytes;
        }



        public string readNullTermStringAtPosition(int ind) {
            string str = "";
            while (databytes[ind] > 0) {
                str += (char)databytes[ind++];
            }
            return str;
        }



        public bool HasData() {
            return offset < databytes.Length;
        }




        // this does not change the offset
        public List<int> SearchForByteSequence(byte[] bytes) {
            List<int> indexes = new();
            for (int i = 0; i < databytes.Length - bytes.Length; i++) {
                bool match = true;
                for (int j = 0; j < bytes.Length; j++) {
                    if (databytes[i + j] != bytes[j]) {
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
                if (ind + i < 0) {
                    OutputWrite($"   ");
                    continue;
                }
                if (ind + i > databytes.Length - 1) {
                    OutputWriteLine($"EOF");
                    return;
                }
                string bytestr = string.Format($"{databytes[ind + i]:X02}");
                OutputWrite($"{bytestr} ");
            }
            OutputWriteLine("");
        }

        public void ShowAllBytes() {
            ShowBytesAtPosition(0, databytes.Length, 32);
        }


        public string ReadBytesAsString(int len, int breakLen) {
            if (offset == databytes.Length) {
                OutputWriteLine("EOF reached");
            }
            if (offset + len >= databytes.Length) {
                len = databytes.Length - offset;
                OutputWriteLine("WARN - request a bit too long");
            }

            byte[] b = ReadBytes(len);
            int count = 0;
            string bytestring = "";
            for (int i = 0; i < b.Length; i++) {
                if (i == databytes.Length) {
                    bytestring += "EOF";
                    return bytestring;
                }
                bytestring += $"{b[i]:X02} ";
                if (++count % breakLen == 0) {
                    bytestring += "\n";
                }
            }
            if (count % breakLen != 0) {
                OutputWriteLine("");
            }
            return bytestring;
        }



        public void ShowBytes(int len) {
            ShowBytes(len, 32);
        }


        public void ShowBytes(int len, int breakLen) {
            byte[] b = ReadBytes(len);
            ShowBytesByteArray(b, breakLen);
        }



        public void ShowBytesAtPosition(int fromInd, int len) {
            ShowBytesAtPosition(fromInd, len, 32);
        }


        // FIXME - the method doesn't use the start stuff

        public void ShowBytesAtPosition(int fromInd, int len, int breakLen) {
            int count = 0;
            for (int i = fromInd; i < fromInd + len; i++) {
                if (i == databytes.Length) {
                    OutputWriteLine("EOF");
                    return;
                }
                OutputWrite($"{databytes[i]:X02} ");
                if (++count % breakLen == 0) {
                    OutputWriteLine("");
                }
            }
            if (count % breakLen != 0) {
                OutputWriteLine("");
            }
        }

        public void ShowBytesByteArray(byte[] b) {
            ShowBytesByteArray(b, 32);
        }

        public void ShowBytesByteArray(byte[] b, int breakLen) {
            int count = 0;
            for (int i = 0; i < b.Length; i++) {
                OutputWrite($"{b[i]:X02} ");
                if (++count % breakLen == 0) {
                    OutputWriteLine("");
                }
            }
            if (count % breakLen != 0) {
                OutputWriteLine("");
            }
        }

        public void ShowByteCount() {
            OutputWriteLine($"[{offset}]");
        }

        public void ShowByteCount(string comment) {
            OutputWriteLine($"[{offset}] {comment}");
        }


        public void ShowIntValue() {
            int intval = ReadIntAtPosition(offset);
            for (int i = 0; i < 4; i++) {
                OutputWrite($"{ ReadByte():x02} ");
            }
            OutputWriteLine($"        // {intval}");
        }


        public void ShowBytesAtPositionNoLineBreak(int offset, int len) {
            OutputWrite($"{databytes[offset]:X02}");
            for (int i = 1; i < len; i++) {
                OutputWrite($" {databytes[offset + i]:X02}");
            }
        }
        public void ShowBytesNoLineBreak(int len) {
            OutputWrite($"{ReadByte():X02}");
            for (int i = 1; i < len; i++) {
                OutputWrite($" {ReadByte():X02}");
            }
        }



        public void TabPrintComment(string message) {
            TabPrintComment(message, 4);
        }

        public void TabPrintComment(string message, int tabLength) {
            string space = "";
            OutputWriteLine($"{space.PadLeft(tabLength)} // {message}");
        }



        uint oneOrZeroValue;


        public void PrintVcsFileHeader() {
            if (offset > 0) {
                throw new ShaderParserException("the offset is not at zero!");
            }
            ShowByteCount("vcs file header");

            uint magicId = ReadUIntAtPosition(offset);
            if (magicId != 0x32736376) {
                throw new ShaderParserException("wrong magic!");
            }
            ShowBytesNoLineBreak(4);
            TabPrintComment("vcs2");
            uint version = ReadUIntAtPosition(offset);
            if (version != 0x40) {
                throw new ShaderParserException("wrong version!");
            }
            ShowBytesNoLineBreak(4);
            TabPrintComment("version 64");
            oneOrZeroValue = ReadUIntAtPosition(offset);
            if (oneOrZeroValue != 0 && oneOrZeroValue != 1) {
                throw new ShaderParserException("unexpected value!");
            }
            ShowBytesNoLineBreak(4);
            TabPrintComment("values seen are 0 and 1");
        }



        public void PrintPcVsHeader() {
            ShowByteCount("ps/vs header");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID0");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID1");
            OutputWriteLine("");
        }


        public void PrintFeaturesHeader() {
            OutputWriteLine("");
            ShowByteCount("features header");
            uint unk0 = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{unk0} ?");

            uint name_len = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{name_len} len of name");
            OutputWriteLine("");

            string name = readNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name}");
            ShowBytes(name.Length + 1);

            OutputWriteLine("");
            ShowByteCount();

            uint arg1 = ReadUIntAtPosition(offset);
            uint arg2 = ReadUIntAtPosition(offset + 4);
            uint arg3 = ReadUIntAtPosition(offset + 8);
            uint arg4 = ReadUIntAtPosition(offset + 12);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"({arg1},{arg2},{arg3},{arg4})");
            uint arg5 = ReadUIntAtPosition(offset);
            uint arg6 = ReadUIntAtPosition(offset + 4);
            uint arg7 = ReadUIntAtPosition(offset + 8);
            uint arg8 = ReadUIntAtPosition(offset + 12);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"({arg5},{arg6},{arg7},{arg8})");
            OutputWriteLine("");

            ShowByteCount();
            uint count = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"count = {count}");

            if (oneOrZeroValue == 1) {
                // do this again
                count = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"count = {count}");
            }

            OutputWriteLine("");
            ShowByteCount();

            for (int i = 0; i < count; i++) {
                string default_name = readNullTermStringAtPosition(offset);
                OutputWriteLine($"// {default_name}");
                ShowBytes(128);
                uint sSymbols = ReadUIntAtPosition(offset);
                ShowBytes(4);
                for (int j = 0; j < sSymbols; j++) {
                    string sSymbolName = readNullTermStringAtPosition(offset);
                    OutputWriteLine($"// {sSymbolName}");
                    ShowBytes(68);
                }
            }

            OutputWriteLine("");
            ShowByteCount();
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID0");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID1");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID2");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID3");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID4");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID5");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID6");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID7");
            // affects the number of ids
            if (oneOrZeroValue == 1) {
                ShowBytesNoLineBreak(16);
                TabPrintComment("file ID8");
            }
            OutputWriteLine("");

        }


        private void PrintStringList(List<string> names) {
            if (names.Count == 0) {
                return;
            }
            OutputWrite($"// {names[0]}");
            for (int i = 1; i < names.Count; i++) {
                OutputWrite($", {names[i]}");
            }
            OutputWriteLine("");
        }


        public void PrintSFBlock() {
            ShowByteCount();
            for (int i = 0; i < 2; i++) {
                string name1 = readNullTermStringAtPosition(offset);
                if (name1.Length > 0) {
                    OutputWriteLine($"// {name1}");
                }
                ShowBytes(64);
            }

            int arg0 = ReadIntAtPosition(offset);
            int arg1 = ReadIntAtPosition(offset + 4);
            int arg2 = ReadIntAtPosition(offset + 8);
            int arg3 = ReadIntAtPosition(offset + 12);
            int arg4 = ReadIntAtPosition(offset + 16);
            int arg5 = ReadIntAtPosition(offset + 20);

            ShowBytes(12, 4);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"({arg0},{arg1},{arg2},{arg3})");

            ShowBytesNoLineBreak(4);
            TabPrintComment($"({arg4}) values seen: [-1,13]");

            ShowBytesNoLineBreak(4);
            TabPrintComment($"{arg5} additional string params");
            int string_offset = offset;
            List<string> names = new();
            for (int i = 0; i < arg5; i++) {
                string paramname = readNullTermStringAtPosition(string_offset);
                names.Add(paramname);
                string_offset += paramname.Length + 1;
            }
            PrintStringList(names);
            ShowBytes(string_offset - offset);
            OutputWriteLine("");
        }


        //public void PrintStringSequence() {
        //    List<string> names = new();
        //    int string_offset = offset;
        //    string name1 = readNullTermStringAtPosition(string_offset);
        //    string_offset += name1.Length + 1;
        //    if (name1.Length > 0) {
        //        names.Add(name1);
        //    }
        //    while (name1.Length > 0) {
        //        name1 = readNullTermStringAtPosition(string_offset);
        //        if (name1.Length > 0) {
        //            names.Add(name1);
        //            string_offset += name1.Length + 1;
        //        }
        //    };
        //    PrintStringList(names);
        //    ShowBytes(string_offset - offset);
        //}



        public void PrintCompatibilitiesBlock(int compatBlockId) {
            OutputWriteLine($"[{offset}] COMPAT-BLOCK[{compatBlockId}]");
            ShowBytes(216);
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            OutputWriteLine("");
        }

        public void PrintDBlock() {
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(128);
            ShowBytes(12, 4);
            ShowBytes(12);
            OutputWriteLine("");
        }

        public void PrintUnknownBlockType1(int unknownBlockId) {
            ShowByteCount($"TYPE1_UNKBLOCK[{unknownBlockId}]");
            ShowBytes(472);
            OutputWriteLine("");
        }


        public void PrintParamAssignmentBlock(int paramId) {
            ShowByteCount($"PARAM-BLOCK[{paramId}]");
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name1}");
            ShowBytesAtPosition(offset, 64);
            offset += 64;
            string name2 = readNullTermStringAtPosition(offset);
            if (name2.Length > 0) {
                OutputWriteLine($"// {name2}");
            }
            ShowBytesAtPosition(offset, 64);
            offset += 64;
            ShowBytesAtPosition(offset, 8);
            offset += 8;
            string name3 = readNullTermStringAtPosition(offset);
            if (name3.Length > 0) {
                OutputWriteLine($"// {name3}");
            }
            ShowBytesAtPosition(offset, 64);
            offset += 64;

            uint paramType = ReadUIntAtPosition(offset);
            OutputWriteLine($"// param-type, 6 or 7 lead dynamic-exp. Known values: 0,1,5,6,7,8,10,11,13");
            ShowBytes(4);

            if (paramType == 6 || paramType == 7) {
                int dynLength = ReadIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment("dyn-exp len", 0);
                ShowBytesNoLineBreak(dynLength);
                TabPrintComment("dynamic expression", 0);

            } else {
                // ShowBytes(4);
            }

            // parameters
            ShowBytes(24);
            ShowBytes(64); // 64 byte 0 padding


            // float or int arguments
            int a0 = ReadIntAtPosition(offset);
            int a1 = ReadIntAtPosition(offset + 4);
            int a2 = ReadIntAtPosition(offset + 8);
            int a3 = ReadIntAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"ints   ({show(a0)},{show(a1)},{show(a2)},{show(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"ints   ({show(a0)},{show(a1)},{show(a2)},{show(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"ints   ({show(a0)},{show(a1)},{show(a2)},{show(a3)})", 10);

            float f0 = ReadFloatAtPosition(offset);
            float f1 = ReadFloatAtPosition(offset + 4);
            float f2 = ReadFloatAtPosition(offset + 8);
            float f3 = ReadFloatAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"floats ({show(f0)},{show(f1)},{show(f2)},{show(f3)})", 10);

            f0 = ReadFloatAtPosition(offset);
            f1 = ReadFloatAtPosition(offset + 4);
            f2 = ReadFloatAtPosition(offset + 8);
            f3 = ReadFloatAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"floats ({show(f0)},{show(f1)},{show(f2)},{show(f3)})", 10);


            f0 = ReadFloatAtPosition(offset);
            f1 = ReadFloatAtPosition(offset + 4);
            f2 = ReadFloatAtPosition(offset + 8);
            f3 = ReadFloatAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"floats ({show(f0)},{show(f1)},{show(f2)},{show(f3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"ints   ({show(a0)},{show(a1)},{show(a2)},{show(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytesNoLineBreak(16);
            TabPrintComment($"ints   ({show(a0)},{show(a1)},{show(a2)},{show(a3)})", 10);


            ShowBytes(64); // 64 byte 0 padding
            OutputWriteLine("");
        }


        private string show(float val) {
            if (val == -1e9) return "-inf";
            if (val == 1e9) return "inf";
            return $"{val}";
        }

        private string show(int val) {
            if (val == -999999999) return "-inf";
            if (val == 999999999) return "inf";
            return "" + val; ;
        }



        public void PrintMipmapBlock(int mipmapId) {
            OutputWriteLine($"[{offset}] MIPMAP-BLOCK[{mipmapId}]");
            ShowBytes(24, 4);
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name1}");
            ShowBytes(256);
            OutputWriteLine("");
        }


        public void PrintBufferBlock(int bufferBlockId) {
            string blockname = readNullTermStringAtPosition(offset);
            ShowByteCount($"BUFFER-BLOCK[{bufferBlockId}] {blockname}");
            ShowBytes(64);
            uint bufferSize = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{bufferSize} buffer-size");
            ShowBytes(4);
            uint paramCount = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{paramCount} param-count");

            for (int i = 0; i < paramCount; i++) {
                string paramname = readNullTermStringAtPosition(offset);
                OutputWriteLine($"// {paramname}");
                ShowBytes(64);
                uint paramIndex = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"{paramIndex} buffer-index", 28);


                uint vertexSize = ReadUIntAtPosition(offset);
                uint attributeCount = ReadUIntAtPosition(offset + 4);
                uint size = ReadUIntAtPosition(offset + 8);

                ShowBytesNoLineBreak(12);
                TabPrintComment($"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, size)");
            }

            OutputWriteLine("");
            ShowBytesNoLineBreak(4);
            TabPrintComment("check/crc/confirmation for the block");
            OutputWriteLine("");
            OutputWriteLine("");
        }


        public void PrintNamesBlock(int block_id) {
            ShowByteCount($"SYMBOL-NAMES-BLOCK[{block_id}]");
            uint symbolGroupCount = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{symbolGroupCount} string groups in this block");
            for (int i = 0; i < symbolGroupCount; i++) {
                OutputWriteLine("");
                for (int j = 0; j < 3; j++) {
                    string symbolname = readNullTermStringAtPosition(offset);
                    OutputWriteLine($"// {symbolname}");
                    ShowBytes(symbolname.Length + 1);
                }
                ShowBytes(4);
            }
            OutputWriteLine("");
        }


        public void ParseZFramesSection() {
            OutputWriteLine("");
            ShowByteCount();
            uint zframe_count = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{zframe_count} zframes");
            OutputWriteLine("");

            if (zframe_count == 0) {
                return;
            }

            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame ID's");
            for (int i = 0; i < zframe_count; i++) {
                uint zframe_index = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(8);
                TabPrintComment($"zframe[{zframe_index}]");
                zFrameIndexes.Add(zframe_index);
            }
            OutputWriteLine("");

            ShowByteCount("zFrame file offsets");
            foreach (int zframeIndex in zFrameIndexes) {
                uint zframe_offset = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"{zframe_offset,-10} index of zframe[{zframeIndex}]");
            }

            uint total_size = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{total_size} - end of file");
            OutputWriteLine("");

            foreach (int zframeIndex in zFrameIndexes) {
                PrintCompressedZFrame(zframeIndex);
            }
        }


        public void PrintCompressedZFrame(int zframeId) {
            OutputWriteLine($"[{offset}] zframe[{zframeId}]");
            ShowBytesNoLineBreak(4);
            TabPrintComment("DELIM (0xfffffffd)");
            ShowBytesAtPositionNoLineBreak(offset, 4);
            int uncompressed_length = ReadInt();
            TabPrintComment($"{uncompressed_length,-8} uncompressed length");
            // TabPrintComment(uncompressed_length.ToString().PadRight(8));
            ShowBytesAtPositionNoLineBreak(offset, 4);
            int compressed_length = ReadInt();
            TabPrintComment($"{compressed_length,-8} compressed length");
            ShowBytesAtPosition(offset, 96);
            OutputWriteLine("// ...");
            offset += compressed_length;
            OutputWriteLine("");
        }



        public void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
            ShowByteCount();
            OutputWriteLine("EOF");
        }





        public void ConfigureWriteToFile(StreamWriter sw) {
            this.sw = sw;
        }
        private void OutputWrite(string text) {
            if (sw != null) {
                sw.Write(text);
            } else {
                Debug.Write(text);
            }
        }
        private void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }
}









