using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MyValveResourceFormat.ThirdParty;


namespace MyShaderAnalysis.vcsparsing01 {
    public class DataReaderOld01 {

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
        public void AddCollectValueInt(int val) {
            int currIterator = collectValuesInt.GetValueOrDefault(val, 0);
            collectValuesInt[val] = currIterator + 1;
        }
        public void AddCollectValueString(string val) {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;


            //if (collectValuesString.ContainsKey(val)) {
            //    return;
            //} else {
            //    collectValuesString.Add(val, 1);
            //}
        }



        public DataReaderOld01(byte[] data) {
            this.databytes = data;
            this.start = 0;
            this.offset = 0;
        }
        public DataReaderOld01(byte[] data, int start) {
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

        public int ReadInt16() {
            short b0 = ReadByte();
            short b1 = ReadByte();
            b1 <<= 8;
            b1 += b0;
            // short intval = (b1 << 8) + b0;
            return b1;
        }

        public int ReadInt16AtPosition(int ind) {
            short b1 = databytes[ind + 1];
            b1 <<= 8;
            b1 += databytes[ind];
            return b1;
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


        public byte[] ReadBytesAtPosition(int ind, int len) {
            byte[] segmentbytes = new byte[len];
            for (int i = 0; i < len; i++) {
                segmentbytes[i] = databytes[start + ind + i];
            }
            return segmentbytes;
        }



        public string ReadNullTermStringAtPosition(int ind) {
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
                bytestring += "\n";
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
            byte[] b = ReadBytesAtPosition(ind, len);
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
            TabComment($"{intval}");
        }

        public void PrintIntWithValue() {
            int intval = ReadIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"{intval}");
        }




        public void TabComment(string message) {
            TabPrintComment(message, 4);
        }

        public void TabPrintComment(string message, int tabLength) {
            string space = "";
            OutputWriteLine($"{space.PadLeft(tabLength)} // {message}");
        }


        public void PrintVcsFileHeader() {
            if (offset > 0) {
                throw new ShaderParserException01("the offset is not at zero!");
            }
            ShowByteCount("vcs file header");

            uint magicId = ReadUIntAtPosition(offset);
            if (magicId != 0x32736376) {
                throw new ShaderParserException01("wrong magic!");
            }
            ShowBytesNoLineBreak(4);
            TabComment("\"vcs2\"");
            uint version = ReadUIntAtPosition(offset);
            if (version != 0x40) {
                throw new ShaderParserException01("wrong version!");
            }
            ShowBytesNoLineBreak(4);
            TabComment("version 64");
            OutputWriteLine("");
        }



        public void PrintFeaturesHeader() {
            ShowByteCount("features header");

            uint has_psrs_file = ReadUIntAtPosition(offset);
            if (has_psrs_file != 0 && has_psrs_file != 1) {
                throw new ShaderParserException01("unexpected value!");
            }
            ShowBytesNoLineBreak(4);
            TabComment($"has_psrs_file = {has_psrs_file>0}");

            ShowBytesNoLineBreak(4);
            uint unk0 = ReadUIntAtPosition(offset-4);
            TabComment($"unknown_val = {unk0} (usually 0)");

            ShowBytesNoLineBreak(4);
            uint name_len = ReadUIntAtPosition(offset-4);
            TabComment($"{name_len} len of name");
            OutputWriteLine("");

            string name = ReadNullTermStringAtPosition(offset);
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
            TabComment($"({arg1},{arg2},{arg3},{arg4})");
            uint arg5 = ReadUIntAtPosition(offset);
            uint arg6 = ReadUIntAtPosition(offset + 4);
            uint arg7 = ReadUIntAtPosition(offset + 8);
            uint arg8 = ReadUIntAtPosition(offset + 12);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytesNoLineBreak(4);
            TabComment($"({arg5},{arg6},{arg7},{arg8})");
            OutputWriteLine("");

            ShowByteCount();
            uint nrOfArguments = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"nr of arguments ({nrOfArguments})");

            if (has_psrs_file == 1) {
                // NOTE nrOfArguments is overwritten
                nrOfArguments = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabComment($"nr of arguments overriden ({nrOfArguments})");
            }

            OutputWriteLine("");
            ShowByteCount();

            for (int i = 0; i < nrOfArguments; i++) {
                string default_name = ReadNullTermStringAtPosition(offset);
                OutputWriteLine($"// {default_name}");
                ShowBytes(128);
                uint sSymbols = ReadUIntAtPosition(offset);
                ShowBytes(4);
                // Debug.WriteLine($"{sSymbols}");
                for (int j = 0; j < sSymbols; j++) {
                    uint sSymbolArgValue = ReadUIntAtPosition(offset + 64);
                    // Debug.WriteLine($"{sSymbolArgValue}");
                    string sSymbolName = ReadNullTermStringAtPosition(offset);
                    OutputWriteLine($"// {sSymbolName}");
                    ShowBytes(68);
                }
            }


            //string fileId0 = ReadBytesAsStringAtPosition(offset, 16);
            //string fileId1 = ReadBytesAsStringAtPosition(offset+16, 16);
            //string fileId2 = ReadBytesAsStringAtPosition(offset+32, 16);
            //string fileId3 = ReadBytesAsStringAtPosition(offset+48, 16);
            //string fileId4 = ReadBytesAsStringAtPosition(offset+64, 16);
            //string fileId5 = ReadBytesAsStringAtPosition(offset+80, 16);
            //string fileId6 = ReadBytesAsStringAtPosition(offset+96, 16);
            //string fileId7 = ReadBytesAsStringAtPosition(offset+112, 16);
            //AddCollectValueString($"{fileId0} fileId0 {filepath}");
            //AddCollectValueString($"{fileId1} fileId1 {filepath}");
            //AddCollectValueString($"{fileId2} fileId2 {filepath}");
            //AddCollectValueString($"{fileId3} fileId3 {filepath}");
            //AddCollectValueString($"{fileId4} fileId4 {filepath}");
            //AddCollectValueString($"{fileId5} fileId5 {filepath}");
            //AddCollectValueString($"{fileId6} fileId6 {filepath}");
            //AddCollectValueString($"{fileId7} fileId7 {filepath}");
            //if (additional_file == 1) {
            //    string fileId8 = ReadBytesAsStringAtPosition(offset+128, 16);
            //    AddCollectValueString($"{fileId8} fileId8 {filepath}");
            //}


            OutputWriteLine("");
            ShowByteCount("File IDs");
            ShowBytesNoLineBreak(16);
            TabComment("file ID0");
            ShowBytesNoLineBreak(16);
            TabComment("file ID1 - ref to vs file");
            ShowBytesNoLineBreak(16);
            TabComment("file ID2 - ref to ps file");
            ShowBytesNoLineBreak(16);
            TabComment("file ID3");
            ShowBytesNoLineBreak(16);
            TabComment("file ID4");
            ShowBytesNoLineBreak(16);
            TabComment("file ID5");
            ShowBytesNoLineBreak(16);
            TabComment("file ID6");
            if (has_psrs_file == 0) {
                ShowBytesNoLineBreak(16);
                TabComment("file ID7 - shared by all Dota2 vcs files");
            }
            if (has_psrs_file == 1) {
                ShowBytesNoLineBreak(16);
                TabComment("file ID7 - reference to psrs file");
                ShowBytesNoLineBreak(16);
                TabComment("file ID8 - shared by all Dota2 vcs files");
            }
            OutputWriteLine("");

        }


        public void PrintPcVsHeader() {
            ShowByteCount("ps/vs header");
            ShowBytesNoLineBreak(4);
            uint has_psrs_file = ReadUIntAtPosition(offset-4);
            TabComment($"has_psrs_file = {has_psrs_file>0}");

            //string fileId0 = ReadBytesAsStringAtPosition(offset, 16);
            //string fileId1 = ReadBytesAsStringAtPosition(offset+16, 16);
            //AddCollectValueString($"{fileId0} fileId0 {filepath}");
            //AddCollectValueString($"{fileId1} fileId1 {filepath}");

            ShowBytesNoLineBreak(16);
            TabComment("file ID0");
            ShowBytesNoLineBreak(16);
            TabComment("file ID1 - shared by all Dota2 vcs files");
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
                string name1 = ReadNullTermStringAtPosition(offset);
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
            TabComment($"({arg0},{arg1},{arg2},{arg3})");

            ShowBytesNoLineBreak(4);
            TabComment($"({arg4}) known values [-1,28]");

            ShowBytesNoLineBreak(4);
            TabComment($"{arg5} additional string params");
            int string_offset = offset;
            List<string> names = new();
            for (int i = 0; i < arg5; i++) {
                string paramname = ReadNullTermStringAtPosition(string_offset);
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
            string name1 = ReadNullTermStringAtPosition(offset);
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


            string name1 = ReadNullTermStringAtPosition(offset);
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


        public void PrintParameterBlock(int paramId) {

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
            string name1 = ReadNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name1}");
            ShowBytes(64);
            string name2 = ReadNullTermStringAtPosition(offset);
            if (name2.Length > 0) {
                OutputWriteLine($"// {name2}");
            }
            ShowBytes(64);
            ShowBytes(8);
            string name3 = ReadNullTermStringAtPosition(offset);
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
            string name4 = ReadNullTermStringAtPosition(offset);
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
            string name5 = ReadNullTermStringAtPosition(offset);
            if (name5.Length > 0) {
                OutputWriteLine($"// {name5}");
            }
            ShowBytes(32);
            string name6 = ReadNullTermStringAtPosition(offset);
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
            string name1 = ReadNullTermStringAtPosition(offset);
            OutputWriteLine($"// {name1}");
            ShowBytes(256);
            OutputWriteLine("");
        }


        public void PrintBufferBlock(int bufferBlockId) {
            string blockname = ReadNullTermStringAtPosition(offset);
            ShowByteCount($"BUFFER-BLOCK[{bufferBlockId}] {blockname}");
            ShowBytes(64);
            uint bufferSize = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"{bufferSize} buffer-size");
            ShowBytes(4);
            uint paramCount = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"{paramCount} param-count");

            for (int i = 0; i < paramCount; i++) {
                string paramname = ReadNullTermStringAtPosition(offset);
                OutputWriteLine($"// {paramname}");
                ShowBytes(64);
                uint paramIndex = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"{paramIndex} buffer-offset", 28);


                uint vertexSize = ReadUIntAtPosition(offset);
                uint attributeCount = ReadUIntAtPosition(offset + 4);
                uint size = ReadUIntAtPosition(offset + 8);

                ShowBytesNoLineBreak(12);
                TabComment($"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, size)");
            }

            OutputWriteLine("");
            ShowBytesNoLineBreak(4);
            TabComment("blockID (some kind of crc/check)");
            OutputWriteLine("");
            OutputWriteLine("");
        }


        public void PrintNamesBlock(int block_id) {
            ShowByteCount($"SYMBOL-NAMES-BLOCK[{block_id}]");
            uint symbolGroupCount = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"{symbolGroupCount} string groups in this block");
            for (int i = 0; i < symbolGroupCount; i++) {
                OutputWriteLine("");
                for (int j = 0; j < 3; j++) {
                    string symbolname = ReadNullTermStringAtPosition(offset);
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
            TabComment($"{zframe_count} zframes");
            OutputWriteLine("");

            if (zframe_count == 0) {
                return;
            }

            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame IDs");
            for (int i = 0; i < zframe_count; i++) {
                uint zframe_index = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(8);
                TabComment($"zframe[{zframe_index}]");
                zFrameIndexes.Add(zframe_index);
            }
            OutputWriteLine("");

            ShowByteCount("zFrame file offsets");
            foreach (int zframeIndex in zFrameIndexes) {
                uint zframe_offset = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabComment($"{zframe_offset,-10} index of zframe[{zframeIndex}]");
            }

            uint total_size = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabComment($"{total_size} - end of file");
            OutputWriteLine("");

            foreach (int zframeIndex in zFrameIndexes) {
                PrintCompressedZFrame(zframeIndex);
            }
        }


        public void PrintCompressedZFrame(int zframeId) {
            OutputWriteLine($"[{offset}] zframe[{zframeId}]");
            ShowBytesNoLineBreak(4);
            TabComment("DELIM (0xfffffffd)");
            ShowBytesAtPositionNoLineBreak(offset, 4);
            int uncompressed_length = ReadInt();
            TabComment($"{uncompressed_length,-8} uncompressed length");
            // TabPrintComment(uncompressed_length.ToString().PadRight(8));
            ShowBytesAtPositionNoLineBreak(offset, 4);
            int compressed_length = ReadInt();
            TabComment($"{compressed_length,-8} compressed length");
            ShowBytesAtPosition(offset, 96);
            OutputWriteLine("// ...");
            offset += compressed_length;
            OutputWriteLine("");
        }



        public void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException01("End of file not reached!");
            }
            // ShowByteCount("----------------------------------------------------------------------------------------");
            // ShowBytesAtPosition(offset, 1000);

            // DisableOutput = false;

            ShowByteCount();
            OutputWriteLine("EOF");
        }




        public void ShowMurmurString() {
            string nulltermstr = ReadNullTermStringAtPosition(offset);
            uint murmur32 = ReadUIntAtPosition(offset + nulltermstr.Length + 1);

            uint MURMUR2SEED = 0x31415926; // It's pi!
            uint murmurCheck = MurmurHash2.Hash(nulltermstr.ToLower(), MURMUR2SEED);
            if (murmur32 != murmurCheck) {
                throw new ShaderParserException01("not a murmur string!");
            }

            OutputWriteLine($"// {nulltermstr} | 0x{murmur32:x08}");
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

            //if (blockId==-1) {
            //    AddCollectValueInt(arg0);
            //}

            int arg1 = ReadIntAtPosition(offset + 4);
            int arg2 = ReadIntAtPosition(offset + 8);

            if (arg0 == 0 && arg1 == 0 && arg2 == 0) {
                ShowBytesNoLineBreak(12);
                TabComment($"data-block[{blockId}]");
                return 0;
            }
            string comment = "";
            if (blockId == -1) {
                comment = $"leading data";
            }
            if (blockId >= 0) {
                comment = $"data-block[{blockId}]";
            }
            int blockSize = ReadIntAtPosition(offset);
            if (prevBlockWasZero) {
                OutputWriteLine("");
            }
            ShowByteCount(comment);
            PrintIntWithValue();
            PrintIntWithValue();
            PrintIntWithValue();

            //DisableOutput = false;
            //ShowBytesNoLineBreak(12);
            //Debug.Write(" ");
            //DisableOutput = true;

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
            OutputWriteLine($"// {byteSize / 4}*4 bytes");

            ShowBytes(byteSize);

            //DisableOutput = false;
            //ShowBytesNoLineBreak(byteSize);
            //DisableOutput = true;

            OutputWriteLine("");
        }


        public void ShowDynamicExpression(int dynExpLen) {

            string dynExpByteStr = ReadBytesAsStringAtPosition(offset, dynExpLen);
            string dynExp = getDynamicExpression(dynExpByteStr);
            OutputWriteLine($"// {dynExp}");
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



        public void ParseAndShowZFrame() {

            ShowZDataSection(-1);
            ShowByteCount("ZFrame header");
            ShowBytes(2);
            int nr_arguments = (int)ReadUInt16AtPosition(offset - 2);
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

            if (nr_arguments == 6) {
                ShowMurmurString();
                ShowBytes(11);
            }

            ShowBytesNoLineBreak(2);
            int nr_of_blocks = (int)ReadUInt16AtPosition(offset - 2);
            TabComment($"nr of blocks ({nr_of_blocks})");
            for (int i = 0; i < nr_of_blocks; i++) {
                ShowZDataSection(i);
            }


            // we don't need this number here!
            // int blocksRegistered = ShowZBlocksRegisteredSection();

            ShowZBlocksRegisteredSection();
            // Debug.WriteLine($"blocks-registered: {blocksRegistered}");

            ShowByteCount();
            ShowBytesNoLineBreak(2);
            TabComment($"usually 1C 02, sometimes 00 00");

            // this is totally wrong!
            // TabPrintComment("if 00 00 we read one source, otherwise read one source per block");
            //uint controlVal = ReadUInt16AtPosition(offset - 2);
            //OutputWriteLine("");
            //if (controlVal == 0) {
            //    blocksRegistered = 1;
            //}

            // ShowZFlags();

            // number of sources is the fourth byte in what I thought was the flags
            ShowByteCount("flags");
            byte sourceEntries = databytes[offset + 3];
            // Debug.WriteLine(sourceEntries);
            ShowBytes(4);
            ShowBytes(4);
            OutputWriteLine("");




            for (int i = 0; i < sourceEntries; i++) {
                ShowZSourceSection(i);
            }
            ShowZAllEndBlocks();
            EndOfFile();


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
            OutputWriteLine("");
        }


        public void ShowZFrameHeaderUpdated() {
            // starts with number of arguments
            ShowByteCount("Frame header");
            ShowBytesNoLineBreak(2);
            uint nrArgs = ReadUInt16AtPosition(offset - 2);
            TabPrintComment($"nr of arguments ({nrArgs})", 0);
            OutputWriteLine("");

            for (int i = 0; i < nrArgs; i++) {
                ShowMurmurString();
                int headerOperator = databytes[offset];
                if (headerOperator == 0x0e) {
                    ShowBytes(3);
                    continue;
                }

                if (headerOperator == 1) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(8);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 9) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(8);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
                if (headerOperator == 5) {
                    int dynExpLen = ReadIntAtPosition(offset + 3);
                    if (dynExpLen == 0) {
                        ShowBytes(11);
                        continue;
                    } else {
                        ShowBytes(7);
                        ShowDynamicExpression(dynExpLen);
                        continue;
                    }
                }
            }
            if (nrArgs > 0) {
                OutputWriteLine("");
            }
        }


        //  This delimiter is totally wrong!
        public void ShowZDataDelim() {
            ShowByteCount("Data delim");
            ShowBytes(3);
            ShowBytes(8);
            ShowBytesNoLineBreak(2);
            TabComment($"nr of blocks ({ReadUInt16AtPosition(offset - 2)})");
            OutputWriteLine("");
        }

        public int ShowZBlocksRegisteredSection() {
            OutputWriteLine("");
            ShowByteCount();
            ShowBytes(2);
            int blockEntryCount = (int)ReadUInt16AtPosition(offset - 2);
            int blocksRegistered = ShowZBlocksRegistered(blockEntryCount);
            OutputWriteLine("");
            return blocksRegistered;
        }

        public int ShowZBlocksRegistered(int blockEntryCount) {
            int recordsFound = 0;
            for (int i = 0; i < blockEntryCount; i++) {
                int hasEntry = (int)ReadInt16AtPosition(offset + i * 2);
                if (hasEntry != 0 && hasEntry != -1) {
                    throw new ShaderParserException01("unexpeted value!");
                }
                recordsFound += hasEntry == 0 ? 1 : 0;
            }
            ShowBytes(blockEntryCount * 2, 32);
            return recordsFound;
        }


        public void ShowZFlags() {
            ShowByteCount("flags");
            byte b = databytes[offset + 3];
            Debug.WriteLine(b);
            DisableOutput = false;
            ShowBytes(4);
            DisableOutput = true;
            ShowBytes(4);
            OutputWriteLine("");
        }


        public (int, int, string) ShowZSourceSection(int blockId) {
            int sourceSize = ShowZSourceOffsets();
            int sourceOffset = offset;
            ShowZGlslSourceSummary(blockId);
            ShowByteCount();
            string fileId = ReadBytesAsString(16, 16).Trim();
            OutputWrite(fileId);
            TabComment($"File ID");
            OutputWriteLine("");
            // AddCollectValueString(fileId);
            return (sourceOffset, sourceSize, fileId);
        }

        public int ShowZSourceOffsets() {
            ShowByteCount("glsl source offsets");
            PrintIntWithValue();
            uint offset1 = ReadUIntAtPosition(offset - 4);
            if (offset1 == 0) {
                return 0;
            }
            ShowBytesNoLineBreak(4);
            TabComment("always 3");
            PrintIntWithValue();
            int sourceSize = ReadIntAtPosition(offset - 4) - 1; // one less because of null-term
            OutputWriteLine("");
            return sourceSize;
        }

        public void ShowZGlslSourceSummary(int sourceId) {
            int bytesToRead = ReadIntAtPosition(offset - 4);
            int endOfSource = offset + bytesToRead;
            ShowByteCount($"GLSL-SOURCE[{sourceId}]");
            if (bytesToRead == 0) {
                OutputWriteLine("// no source present");
            }
            if (bytesToRead > 100) {
                ShowBytes(100);
                ShowByteCount();
                OutputWriteLine($"// ... ({endOfSource - offset} bytes of data not shown)");
            } else {
                ShowBytes(bytesToRead);
            }

            offset = endOfSource;
            OutputWriteLine("");
        }




        public void ShowZAllEndBlocksTypeVs() {
            ShowByteCount();
            ShowBytesNoLineBreak(4);
            int nr_end_blocks = ReadIntAtPosition(offset - 4);
            TabComment($"nr end blocks ({nr_end_blocks})");
            OutputWriteLine("");

            for (int i = 0; i < nr_end_blocks; i++) {
                ShowBytes(16);
            }
        }



        public void ShowZAllEndBlocks() {
            ShowByteCount();
            ShowBytesNoLineBreak(4);
            int nr_end_blocks = ReadIntAtPosition(offset - 4);
            TabComment($"nr end blocks ({nr_end_blocks})");
            OutputWriteLine("");

            for (int i = 0; i < nr_end_blocks; i++) {
                //if (i==nr_end_blocks-1) {
                //    DisableOutput = false;
                //}
                ShowZEndBlock(i);
            }
        }


        public void ShowZEndBlock(int blockId) {
            ShowByteCount($"END-BLOCK({blockId})");
            ShowBytes(2);
            ShowBytes(8);
            ShowBytes(2);
            // ShowBytes(2);
            PrintUInt16WithValue();
            int extraRows = ReadIntAtPosition(offset + 4);
            if (extraRows > 0) {
                // Debug.WriteLine(extraRows);
                ShowBytes(16);
            }

            ShowBytes(16);
            ShowBytes(64, 16);
            OutputWriteLine("");

        }




        public void ConfigureWriteToFile(StreamWriter sw) {
            this.sw = sw;
        }
        public void OutputWrite(string text) {
            if (DisableOutput) {
                return;
            }

            if (sw != null) {
                sw.Write(text);
            } else {
                Debug.Write(text);
            }
        }
        public void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }
}








