using MyValveResourceFormat.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace MyShaderAnalysis.readers {
    public class DataReader {

        public byte[] databytes;
        public readonly int start;
        public int offset;
        StreamWriter sw = null;
        public bool DisableOutput = false;
        public string filepath = "";
        public bool[] requestCount = { false };
        public bool[] requestShowFile = { false };


        public Dictionary<int, int> collectValuesInt = new();
        public Dictionary<string, int> collectValuesString = new();
        private void AddCollectValueInt(int val) {
            int currIterator = collectValuesInt.GetValueOrDefault(val, 0);
            collectValuesInt[val] = currIterator + 1;
        }
        private void AddCollectValueString(string val) {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;


            //if (collectValuesString.ContainsKey(val)) {
            //    return;
            //} else {
            //    collectValuesString.Add(val, 1);
            //}
        }



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

        public uint ReadUInt16AtPosition(int ind) {
            uint b0 = databytes[start + ind];
            uint b1 = databytes[start + ind + 1];
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


        // WARN WARN - this has the offset as the second argument
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


        public string ReadBytesAsStringAtPosition(int ind, int len) {

            //int currentoffset = offset;
            //offset = ind;
            //string bytestring = ReadBytesAsString(len, 999999);
            //offset = currentoffset;
            //return bytestring;

            if (ind + len >= databytes.Length) {
                len = databytes.Length - ind;
                OutputWriteLine("WARN - request a bit too long");
            }
            byte[] b = ReadBytesAtPosition(len, offset);
            string bytestring = "";
            for (int i = ind; i < b.Length + ind; i++) {
                if (i == databytes.Length) {
                    bytestring += "EOF";
                    return bytestring;
                }
                bytestring += $"{b[i - ind]:X02} ";
            }
            return bytestring.Trim();

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



        //public void ShowIntValue() {
        //    int intval = ReadIntAtPosition(offset);
        //    for (int i = 0; i < 4; i++) {
        //        OutputWrite($"{ ReadByte():X02} ");
        //    }
        //    OutputWriteLine($"        // {intval}");
        //}


        public void PrintUInt16WithValue() {
            uint intval = ReadUInt16AtPosition(offset);
            ShowBytesNoLineBreak(2);
            TabPrintComment($"{intval}");
        }

        public void PrintIntWithValue() {
            int intval = ReadIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{intval}");
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

            uint unk1 = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment("values seen are 0 and 1");

            // Debug.WriteLine($"{unk1}");

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
            // Debug.WriteLine($"{unk0}");

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

            // Debug.WriteLine($"{count}");

            if (oneOrZeroValue == 1) {
                // do this again
                // NOTE the count is overwritten
                count = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"count = {count}");
                // Debug.WriteLine($"{count}");
                // AddCollectValueInt((int) count);
            }

            OutputWriteLine("");
            ShowByteCount();

            for (int i = 0; i < count; i++) {
                string default_name = readNullTermStringAtPosition(offset);
                OutputWriteLine($"// {default_name}");
                ShowBytes(128);
                uint sSymbols = ReadUIntAtPosition(offset);
                ShowBytes(4);
                // Debug.WriteLine($"{sSymbols}");
                for (int j = 0; j < sSymbols; j++) {
                    uint sSymbolArgValue = ReadUIntAtPosition(offset + 64);
                    // Debug.WriteLine($"{sSymbolArgValue}");
                    string sSymbolName = readNullTermStringAtPosition(offset);
                    OutputWriteLine($"// {sSymbolName}");
                    ShowBytes(68);
                }
            }


            //AddCollectValueString(ReadBytesAsStringAtPosition(offset, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+16, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+32, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+48, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+64, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+80, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+96, 16));
            //AddCollectValueString(ReadBytesAsStringAtPosition(offset+112, 16));
            //if (oneOrZeroValue == 1) {
            //    AddCollectValueString(ReadBytesAsStringAtPosition(offset+128, 16));
            //}



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


        // int sfargument = 0;


        public void PrintSFBlock() {

            //int int0 = ReadIntAtPosition(offset + 136);
            //int int1 = ReadIntAtPosition(offset + 140);
            //string mystr = $"{int0},{int1}";
            //AddCollectValueString(mystr);

            // sfargument = ReadIntAtPosition(offset+144);
            // int int0 = ReadIntAtPosition(offset+144);
            //int int1 = ReadIntAtPosition(offset+92);
            //int int2 = ReadIntAtPosition(offset+96);
            //int int3 = ReadIntAtPosition(offset+100);
            //string mystr = $"{int0},{int1},{int2},{int3}";
            // string mystr = $"{int0}";
            // AddCollectValueString(mystr);


            ShowByteCount();
            for (int i = 0; i < 2; i++) {
                string name1 = readNullTermStringAtPosition(offset);
                if (name1.Length > 0) {
                    OutputWriteLine($"// {name1}");
                }
                // Debug.WriteLine($"{name1}");
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
            TabPrintComment($"({arg4}) known values [-1,28]");

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

        public void PrintCompatibilitiesBlock(int compatBlockId) {
            // requestCount[0] = true;

            //int int0 = ReadIntAtPosition(offset + 36);
            //int int1 = ReadIntAtPosition(offset + 40);
            //string mystr = $"({int0},{int1})";
            //AddCollectValueString(mystr);

            //int int0 = ReadIntAtPosition(offset + 24);
            //int int1 = ReadIntAtPosition(offset + 28);
            //int int2 = ReadIntAtPosition(offset + 32);
            //int int3 = ReadIntAtPosition(offset + 36);
            //int int4 = ReadIntAtPosition(offset + 40);
            //string mystr = $"{int0},{int1},{int2},{int3},{int4}";
            //AddCollectValueString(mystr);


            //int int0 = ReadIntAtPosition(offset+88);
            //int int1 = ReadIntAtPosition(offset + 92);
            //int int2 = ReadIntAtPosition(offset + 96);
            //int int3 = ReadIntAtPosition(offset + 100);
            //string mystr = $"{int0},{int1},{int2},{int3}";
            //string mystr = $"{int1},{int2},{int3}";
            //AddCollectValueString(mystr);




            // int arg1 = ReadIntAtPosition(offset+28);
            // string mystr = $"{sfargument}, {arg1}";
            // AddCollectValueString(mystr);

            // for (int i = 0; i < 14; i++) {
            // int arg1 = ReadIntAtPosition(offset+160+i*4);
            // AddCollectValueInt(arg1);
            // if (arg1 != -1) {
            // Debug.WriteLine(36 + i * 4);
            // Debug.WriteLine(arg1);
            // }

            // }

            // int arg1 = ReadIntAtPosition(offset+44);
            // AddCollectValueInt(arg1);
            // Debug.WriteLine(arg1);

            // string bytestring = ReadBytesAsStringAtPosition(offset+24, 4);
            // Debug.WriteLine(bytestring);
            // AddCollectValueString(bytestring);

            OutputWriteLine($"[{offset}] COMPAT-BLOCK[{compatBlockId}]");
            ShowBytes(216);
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            OutputWriteLine("");
        }

        public void PrintDBlock() {
            //int int0 = ReadIntAtPosition(offset + 136);
            //int int1 = ReadIntAtPosition(offset + 140);
            //int int2 = ReadIntAtPosition(offset + 144);
            // string mystr = $"{int0}";
            //string mystr = $"{int0},{int1},{int2}";
            //AddCollectValueString(mystr);


            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(128);
            ShowBytes(12, 4);
            ShowBytes(12);
            OutputWriteLine("");
        }

        public void PrintUnknownBlockType1(int unknownBlockId) {
            // int int0 = ReadIntAtPosition(offset + 36);
            // int int1 = ReadIntAtPosition(offset + 40);
            // int int2 = ReadIntAtPosition(offset + 40);
            // string mystr = $"{int0}";
            // string mystr = $"({int0},{int1})";
            // string mystr = $"({int0},{int1},{int2})";
            // string mystr = $"{int0},{int1},{int2}";
            // AddCollectValueString(mystr);
            // AddCollectValueInt(int0);

            // string val = ReadBytesAsStringAtPosition(offset + 108, 44);


            //int int0 = ReadIntAtPosition(offset + 92);
            //int int1 = ReadIntAtPosition(offset + 96);
            //int int2 = ReadIntAtPosition(offset + 100);
            //int int3 = ReadIntAtPosition(offset + 104);

            // AddCollectValueString($"{val}");
            // AddCollectValueString($"{int0}  {int1}  {val}");
            // AddCollectValueString($"{int0},{int1},{int2},{int3}");




            ShowByteCount($"TYPE1_UNKBLOCK[{unknownBlockId}]");
            ShowBytes(472);
            OutputWriteLine("");
        }


        public void PrintParamAssignmentBlock(int paramId) {

            // requestCount[0] = true;
            // int save_offset = offset;

            // Debug.WriteLine(offset);

            // int int0 = ReadIntAtPosition(offset + 200);
            // int int1 = ReadIntAtPosition(offset + 132);
            // float float1 = ReadFloatAtPosition(offset + 132);
            // string mystr = $"({int0},{int1})";
            // string mystr = $"({int0},{float1})";
            // AddCollectValueString(mystr);
            // AddCollectValueInt(int0);



            ShowByteCount($"PARAM-BLOCK[{paramId}]");
            string name1 = readNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name1}");
            ShowBytes(64);
            string name2 = readNullTermStringAtPosition(offset);
            if (name2.Length > 0) {
                OutputWriteLine($"// {name2}");
            }
            ShowBytes(64);
            ShowBytes(8);
            string name3 = readNullTermStringAtPosition(offset);
            if (name3.Length > 0) {
                OutputWriteLine($"// {name3}");
            }
            ShowBytes(64);

            uint paramType = ReadUIntAtPosition(offset);
            OutputWriteLine($"// param-type, 6 or 7 lead dynamic-exp. Known values: 0,1,5,6,7,8,10,11,13");
            ShowBytes(4);

            if (paramType == 6 || paramType == 7) {
                int dynLength = ReadIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment("dyn-exp len", 0);


                // INSPECTIONS
                // requestCount[0] = true;
                // string dyn_expression = ReadBytesAsStringAtPosition(offset, dynLength);
                // AddCollectValueString(dyn_expression);


                ShowBytesNoLineBreak(dynLength);
                TabPrintComment("dynamic expression", 0);

            } else {
                // ShowBytes(4);
            }


            // INSPECTIONS
            // int int0 = ReadIntAtPosition(offset + 20);
            // int int1 = ReadIntAtPosition(offset + 132);
            // float float1 = ReadFloatAtPosition(offset + 132);

            // string mystr = ReadBytesAsStringAtPosition(offset+216, 64);
            // string mystr = $"({int0},{int1})";
            // string mystr = $"({int0},{float1})";
            // AddCollectValueString(mystr);
            // AddCollectValueInt(int0);
            // if (int0 > 0) {
            // Debug.WriteLine($"{filepath} {paramId}");
            // }




            // 6 parameters that may follow the dynamic expression
            ShowBytes(24, 4);


            // a rarely seen file reference
            string name4 = readNullTermStringAtPosition(offset);
            if (name4.Length > 0) {
                OutputWriteLine($"// {name4}");
            }
            ShowBytes(64);





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


            // a command word, or pair of these
            string name5 = readNullTermStringAtPosition(offset);
            if (name5.Length > 0) {
                OutputWriteLine($"// {name5}");
            }
            ShowBytes(32);
            string name6 = readNullTermStringAtPosition(offset);
            if (name6.Length > 0) {
                OutputWriteLine($"// {name6}");
            }
            ShowBytes(32);


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




        public void ShowMurmurString() {
            string nulltermstr = readNullTermStringAtPosition(offset);
            uint murmur32 = ReadUIntAtPosition(offset + nulltermstr.Length + 1);

            uint MURMUR2SEED = 0x31415926; // It's pi!
            uint murmurCheck = MurmurHash2.Hash(nulltermstr.ToLower(), MURMUR2SEED);
            if (murmur32 != murmurCheck) {
                throw new ShaderParserException("not a murmur string!");
            }

            Debug.WriteLine($"// {nulltermstr} | 0x{murmur32:x08}");
            ShowBytes(nulltermstr.Length + 1 + 4);


        }


        // return the header-size in bytes


        private bool prevBlockWasZero = false;

        public void ShowZDataSection() {
            ShowZDataSection(-1);
        }

        public void ShowZDataSection(int blockId) {
            int blockSize = ShowZBlockDataHeader(blockId);
            ShowZBlockDataBody(blockSize);

        }
        public int ShowZBlockDataHeader(int blockId) {
            int arg0 = ReadIntAtPosition(offset);
            int arg1 = ReadIntAtPosition(offset+4);
            int arg2 = ReadIntAtPosition(offset+8);

            if (arg0==0 && arg1==0 && arg2==0) {
                ShowBytesNoLineBreak(12);
                TabPrintComment($"block[{blockId}]");
                return 0;
            }
            string comment = "";
            if (blockId >= 0) {
                comment = $"block[{blockId}]";
            }
            int blockSize = ReadIntAtPosition(offset);
            if (prevBlockWasZero) {
                Debug.WriteLine("");
            }
            ShowByteCount(comment);
            PrintIntWithValue();
            PrintIntWithValue();
            PrintIntWithValue();
            return blockSize * 4;
        }
        public void ShowZBlockDataBody(int byteSize) {
            if (byteSize == 0) {
                prevBlockWasZero = true;
                return;
            } else {
                prevBlockWasZero = false;
            }
            // ShowByteCount($"{byteSize/4}*4 bytes");
            Debug.WriteLine($"// {byteSize/4}*4 bytes");
            ShowBytes(byteSize);
            Debug.WriteLine("");
        }


        public void ShowDynamicExpression(int dynExpLen) {

            string dynExpByteStr = ReadBytesAsStringAtPosition(offset, dynExpLen);
            string dynExp = getDynamicExpression(dynExpByteStr);
            Debug.WriteLine($"// {dynExp}");
            ShowBytes(dynExpLen);
        }


        private ParseDynamicExpressionShader myDynParser = new ParseDynamicExpressionShader();
        string getDynamicExpression(string bytesAsString) {
            byte[] databytes = ParseString(bytesAsString);
            if (myDynParser == null) {
                myDynParser = new ParseDynamicExpressionShader();
            }
            myDynParser.ParseExpression(databytes);
            if (myDynParser.errorWhileParsing) {
                // throw new ShaderParserException("error in dynamic expression!");

                Debug.WriteLine(myDynParser.errorMessage);

            }
            return myDynParser.dynamicExpressionResult;
        }
        static byte[] ParseString(String bytestring) {
            string[] tokens = bytestring.Trim().Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }


        public void ShowZFrameHeader() {
            ShowByteCount("ZFrame header");
            ShowBytes(2);
            ShowMurmurString();
            ShowBytes(3);
            ShowMurmurString();
            ShowBytes(8);
            ShowMurmurString();
            int dynExpLen = ReadIntAtPosition(offset + 3);
            ShowBytes(7);
            ShowDynamicExpression(dynExpLen);
            ShowMurmurString();
            dynExpLen = ReadIntAtPosition(offset + 3);
            ShowBytes(7);
            ShowDynamicExpression(dynExpLen);
            ShowMurmurString();
            dynExpLen = ReadIntAtPosition(offset + 3);
            ShowBytes(7);
            ShowDynamicExpression(dynExpLen);
            ShowMurmurString();
            Debug.WriteLine("");
        }

        public void ShowZDataDelim() {
            ShowByteCount("Data delim");
            ShowBytes(3);
            ShowBytes(8);
            ShowBytes(2);
            Debug.WriteLine("");
        }

        public void ShowZSourceOffsets() {
            ShowByteCount("glsl source offsets");
            PrintIntWithValue();
            ShowBytesNoLineBreak(4);
            TabPrintComment("always 3");
            PrintIntWithValue();
            Debug.WriteLine("");
        }

        public void ShowZGlslSourceSummary(int sourceId) {
            int endOfSource = offset + ReadIntAtPosition(offset-4);
            Debug.WriteLine("");
            ShowByteCount($"SOURCE[{sourceId}]");
            ShowBytes(100);
            offset = endOfSource;
            Debug.WriteLine("...\n");
        }




        public void ConfigureWriteToFile(StreamWriter sw) {
            this.sw = sw;
        }
        private void OutputWrite(string text) {
            if (DisableOutput) {
                return;
            }

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








