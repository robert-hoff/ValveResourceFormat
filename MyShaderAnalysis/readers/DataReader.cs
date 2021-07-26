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
                    Debug.Write($"   ");
                    continue;
                }
                if (ind + i > databytes.Length - 1) {
                    Debug.WriteLine($"EOF");
                    return;
                }
                string bytestr = string.Format($"{databytes[ind + i]:X02}");
                Debug.Write($"{bytestr} ");
            }
            Debug.WriteLine("");
        }

        public void ShowAllBytes() {
            ShowBytesAtPosition(0, databytes.Length, 32);
        }


        public string ReadBytesAsString(int len, int breakLen) {
            if (offset == databytes.Length) {
                Debug.WriteLine("EOF reached");
            }
            if (offset + len >= databytes.Length) {
                len = databytes.Length - offset;
                Debug.WriteLine("WARN - request a bit too long");
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
                Debug.WriteLine("");
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
                    Debug.WriteLine("EOF");
                    return;
                }
                Debug.Write($"{databytes[i]:X02} ");
                if (++count % breakLen == 0) {
                    Debug.WriteLine("");
                }
            }
            if (count % breakLen != 0) {
                Debug.WriteLine("");
            }
        }

        public static void ShowBytesByteArray(byte[] b) {
            ShowBytesByteArray(b, 32);
        }

        public static void ShowBytesByteArray(byte[] b, int breakLen) {
            int count = 0;
            for (int i = 0; i < b.Length; i++) {
                Debug.Write($"{b[i]:X02} ");
                if (++count % breakLen == 0) {
                    Debug.WriteLine("");
                }
            }
            if (count % breakLen != 0) {
                Debug.WriteLine("");
            }
        }

        public void ShowByteCount() {
            Debug.WriteLine($"[{offset}]");
        }

        public void ShowByteCount(string comment) {
            Debug.WriteLine($"[{offset}] {comment}");
        }


        public void ShowIntValue() {
            int intval = ReadIntAtPosition(offset);
            for (int i = 0; i < 4; i++) {
                Debug.Write($"{ ReadByte():x02} ");
            }
            Debug.WriteLine($"        // {intval}");
        }


        public void ShowBytesAtPositionNoLineBreak(int offset, int len) {
            Debug.Write($"{databytes[offset]:X02}");
            for (int i = 1; i < len; i++) {
                Debug.Write($" {databytes[offset + i]:X02}");
            }
        }
        public void ShowBytesNoLineBreak(int len) {
            Debug.Write($"{ReadByte():X02}");
            for (int i = 1; i < len; i++) {
                Debug.Write($" {ReadByte():X02}");
            }
        }



        //public void Print484Block2(int ind) {
        //    string name1 = readNullTermStringAtPosition(ind);
        //    Debug.WriteLine($"[{ind}] {name1}");
        //    ShowBytesAtPosition(ind, 64);
        //    string name2 = readNullTermStringAtPosition(ind + 64);
        //    if (name2.Length > 0) {
        //        Debug.WriteLine($"[{ind + 64}] {name2}");
        //    }
        //    ShowBytesAtPosition(ind + 64, 64);
        //    ShowBytesAtPosition(ind + 128, 8);
        //    string name3 = readNullTermStringAtPosition(ind + 136);
        //    if (name3.Length > 0) {
        //        Debug.WriteLine($"[{ind + 136}] {name3}");
        //    }
        //    ShowBytesAtPosition(ind + 136, 64);

        //    offset = ind + 200;
        //    int ind_offset = ind + 200;
        //    uint dynIndicator = ReadUInt();

        //    if (dynIndicator == 6) {
        //        Debug.WriteLine("06 00 00 00 // dyn-exp");
        //        ind_offset += 4;
        //        int dynLength = ReadInt();
        //        ShowBytesAtPosition(ind_offset, 4 + dynLength);
        //        ind_offset += dynLength;
        //    } else {
        //        byte[] b = BitConverter.GetBytes(dynIndicator);
        //        ShowBytesByteArray(b);
        //    }

        //    ShowBytesAtPosition(ind_offset, 28);
        //    ShowBytesAtPosition(ind_offset + 28, 64);
        //    ShowBytesAtPosition(ind_offset + 92, 128, 16);
        //    ShowBytesAtPosition(ind_offset + 220, 64);
        //}




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
            uint zerocheck = ReadUIntAtPosition(offset);
            if (zerocheck != 0) {
                throw new ShaderParserException("unexpected value!");
            }
            ShowBytesNoLineBreak(4);
            TabPrintComment("one row of zeroes indicates ps/vs header follows");

            // FIXME we need a bit more here for the features files


            Debug.WriteLine("");


        }



        public void PrintPcVsHeader() {
            ShowByteCount("ps/vs header");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID0");
            ShowBytesNoLineBreak(16);
            TabPrintComment("file ID1");
        }




        //public void PrintForSBlock(int ind) {
        //    string name1 = readNullTermStringAtPosition(ind);
        //    Debug.WriteLine($"[{ind}] {name1}");
        //    ShowBytesAtPosition(ind, 128);
        //    ShowBytesAtPosition(ind+128, 24, 4);
        //}


        public void PrintSFBlock() {
            string name1 = readNullTermStringAtPosition(offset);
            Debug.WriteLine($"[{offset}] {name1}");
            ShowBytes(128);
            ShowBytes(24, 4);
            Debug.WriteLine("");
        }

        int compatBlockCount = 0;
        public void PrintCompatibilitiesBlock() {
            Debug.WriteLine($"[{offset}] COMPAT-BLOCK[{compatBlockCount++}]");
            ShowBytes(216);
            string name1 = readNullTermStringAtPosition(offset);
            Debug.WriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            Debug.WriteLine("");
        }

        public void PrintDBlock() {
            string name1 = readNullTermStringAtPosition(offset);
            Debug.WriteLine($"[{offset}] {name1}");
            ShowBytes(128);
            ShowBytes(12,4);
            ShowBytes(12);
            Debug.WriteLine("");
        }


        int unknownBlockType1Count = 0;
        public void PrintUnknownBlockType1() {
            ShowByteCount($"TYPE1_UNKBLOCK[{unknownBlockType1Count++}]");
            ShowBytes(472);
        }






        int paramAssignmentBlockCount = 0;
        /*
         *
         *
         *
         *
         */
        public void PrintParamAssignmentBlock() {
            ShowByteCount($"PARAM-BLOCK[{paramAssignmentBlockCount++}]");
            string name1 = readNullTermStringAtPosition(offset);
            Debug.WriteLine($"// {name1}");
            ShowBytesAtPosition(offset, 64);
            offset += 64;
            string name2 = readNullTermStringAtPosition(offset);
            if (name2.Length > 0) {
            Debug.WriteLine($"// {name2}");
            }
            ShowBytesAtPosition(offset, 64);
            offset += 64;

            ShowBytesAtPosition(offset, 8);
            offset += 8;

            string name3 = readNullTermStringAtPosition(offset);
            if (name3.Length > 0) {
            Debug.WriteLine($"// {name3}");
            }
            ShowBytesAtPosition(offset, 64);
            offset += 64;

            uint dynIndicator = ReadUIntAtPosition(offset);

            if (dynIndicator == 6 || dynIndicator == 7) {
                // Debug.WriteLine("06 00 00 00 // dyn-exp");
                ShowBytes(4);
                int dynLength = ReadIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment("dyn-exp len", 0);
                ShowBytesNoLineBreak(dynLength);
                TabPrintComment("dynamic expression", 0);

            } else {
                ShowBytes(4);
            }

            ShowBytesAtPosition(offset, 24);
            offset += 24;
            ShowBytesAtPosition(offset, 64);
            offset += 64;
            ShowBytesAtPosition(offset, 128, 16);
            offset += 128;
            ShowBytesAtPosition(offset, 64);
            offset += 64;

            Debug.WriteLine("");

        }




        public void PrintMipmapBlock() {
            Debug.WriteLine($"[{offset}]");
            ShowBytes(24, 4);
            string name1 = readNullTermStringAtPosition(offset);
            Debug.WriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            Debug.WriteLine("");
        }




        public void TabPrintComment(string message) {
            TabPrintComment(message, 4);
        }

        public void TabPrintComment(string message, int tabLength) {
            string space = "";
            Debug.WriteLine($"{space.PadLeft(tabLength)} // {message}");
        }


        // int zFrameCount = 0;

        public void PrintCompressedZFrame(int zframeId) {
            Debug.WriteLine($"[{offset}] zframe[{zframeId}]");
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
            Debug.WriteLine("// ...");
            offset += compressed_length;
            Debug.WriteLine("");
        }





        public void parseZFramesSection() {
            ShowByteCount();
            uint zframe_count = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{zframe_count} nr of zframes");
            Debug.WriteLine("");

            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame ID's");
            for (int i = 0; i < zframe_count; i++) {
                uint zframe_index = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(8);
                TabPrintComment($"zframe[{zframe_index}]");
                zFrameIndexes.Add(zframe_index);
            }
            Debug.WriteLine("");

            ShowByteCount("zFrame file offsets");
            foreach(int zframeIndex in zFrameIndexes) {
                uint zframe_offset = ReadUIntAtPosition(offset);
                ShowBytesNoLineBreak(4);
                TabPrintComment($"{zframe_offset,-10} index of zframe[{zframeIndex}]");
            }

            uint total_size = ReadUIntAtPosition(offset);
            ShowBytesNoLineBreak(4);
            TabPrintComment($"{total_size} - end of file");
            Debug.WriteLine("");

            foreach(int zframeIndex in zFrameIndexes) {
                PrintCompressedZFrame(zframeIndex);
            }
        }



        public void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
            ShowByteCount();
            Debug.WriteLine("EOF");
        }



    }
}









