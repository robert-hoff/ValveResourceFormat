using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.vcsparsing {


    public class DataBlockFeaturesHeader : ShaderDataBlock {

        public bool has_psrs_file;
        public int unknown_val;
        public string file_description;
        // not sure what these are
        public int arg0;
        public int arg1;
        public int arg2;
        public int arg3;
        public int arg4;
        public int arg5;
        public int arg6;
        public int arg7;
        public List<(string, string)> mainParams = new();
        public List<string> fileIDs = new();


        public DataBlockFeaturesHeader(ShaderDataReader datareader, int start) : base(datareader, start) {
            int psrs_arg = datareader.ReadInt();
            if (psrs_arg != 0 && psrs_arg != 1) {
                throw new ShaderParserException($"unexpected value psrs_arg = {psrs_arg}");
            }
            has_psrs_file = psrs_arg > 0;
            unknown_val = datareader.ReadInt();
            datareader.ReadInt(); // length of name, but not needed because it's always null-term
            file_description = datareader.ReadNullTermString();
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadInt();
            arg6 = datareader.ReadInt();
            arg7 = datareader.ReadInt();

            int nr_of_arguments = datareader.ReadInt();
            // NOTE nr_of_arguments is overwritten
            if (has_psrs_file) {
                nr_of_arguments = datareader.ReadInt();
            }

            for (int i = 0; i < nr_of_arguments; i++) {
                string string_arg0 = datareader.ReadNullTermStringAtPosition();
                string string_arg1 = "";
                datareader.offset += 128;
                if (datareader.ReadInt() > 0) {
                    string_arg1 = datareader.ReadNullTermStringAtPosition();
                    datareader.offset += 68;
                }
                mainParams.Add((string_arg0, string_arg1));
            }

            for (int i = 0; i < 8; i++) {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
            if (has_psrs_file) {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
        }


        public override void PrintByteSummary() {
            datareader.offset = start;
            datareader.ShowByteCount("features header");
            int has_psrs_file = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, "has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));
            int unknown_val = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"unknown_val = {unknown_val} (usually 0)");
            int len_name_description = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"{len_name_description} len of name");
            datareader.BreakLine();
            string name_desc = datareader.ReadNullTermStringAtPosition();
            datareader.ShowByteCount(name_desc);
            datareader.ShowBytes(len_name_description + 1);
            datareader.BreakLine();
            datareader.ShowByteCount();
            uint arg1 = datareader.ReadUIntAtPosition(0);
            uint arg2 = datareader.ReadUIntAtPosition(4);
            uint arg3 = datareader.ReadUIntAtPosition(8);
            uint arg4 = datareader.ReadUIntAtPosition(12);
            datareader.ShowBytes(16, 4, breakLine: false);
            datareader.TabComment($"({arg1},{arg2},{arg3},{arg4})");
            uint arg5 = datareader.ReadUIntAtPosition(0);
            uint arg6 = datareader.ReadUIntAtPosition(4);
            uint arg7 = datareader.ReadUIntAtPosition(8);
            uint arg8 = datareader.ReadUIntAtPosition(12);
            datareader.ShowBytes(16, 4, breakLine: false);
            datareader.TabComment($"({arg5},{arg6},{arg7},{arg8})");
            datareader.BreakLine();
            datareader.ShowByteCount();
            int nr_of_arguments = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"nr of arguments {nr_of_arguments}");
            if (has_psrs_file == 1) {
                // NOTE nr_of_arguments is overwritten
                nr_of_arguments = datareader.ReadIntAtPosition();
                datareader.ShowBytes(4, $"nr of arguments overriden ({nr_of_arguments})");
            }
            datareader.BreakLine();
            datareader.ShowByteCount();
            for (int i = 0; i < nr_of_arguments; i++) {
                string default_name = datareader.ReadNullTermStringAtPosition();
                datareader.Comment($"{default_name}");
                datareader.ShowBytes(128);
                uint has_s_argument = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(4);
                if (has_s_argument > 0) {
                    uint sSymbolArgValue = datareader.ReadUIntAtPosition(64);
                    string sSymbolName = datareader.ReadNullTermStringAtPosition();
                    datareader.Comment($"{sSymbolName}");
                    datareader.ShowBytes(68);
                }
            }
            datareader.BreakLine();
            datareader.ShowByteCount("File IDs");
            datareader.ShowBytes(16, "file ID0");
            datareader.ShowBytes(16, breakLine: false);
            datareader.TabComment("file ID1 - ref to vs file");
            datareader.ShowBytes(16, breakLine: false);
            datareader.TabComment("file ID2 - ref to ps file");
            datareader.ShowBytes(16, "file ID3");
            datareader.ShowBytes(16, "file ID4");
            datareader.ShowBytes(16, "file ID5");
            datareader.ShowBytes(16, "file ID6");
            if (has_psrs_file == 0) {
                datareader.ShowBytes(16, "file ID7 - shared by all Dota2 vcs files");
            }
            if (has_psrs_file == 1) {
                datareader.ShowBytes(16, "file ID7 - reference to psrs file");
                datareader.ShowBytes(16, "file ID8 - shared by all Dota2 vcs files");
            }
            datareader.BreakLine();
        }


        public void ShowMainParams() {

            foreach (var parampair in mainParams) {
                Debug.WriteLine($"       {parampair.Item1.PadRight(50)} {parampair.Item2}");
            }

        }

    }







    // needs implemenation
    public class DataBlockVsPsHeader : ShaderDataBlock {
        public DataBlockVsPsHeader(ShaderDataReader datareader, int start) : base(datareader, start) {
            datareader.offset += 36;
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class DataBlockSfBlock : ShaderDataBlock {
        public int blockId;
        public string name0;
        public string name1;
        public int arg0;
        public int arg1;
        public int arg2; // layers
        public int arg3;
        public int arg4;
        public int arg5;
        public List<string> additionalParams = new();

        public DataBlockSfBlock(ShaderDataReader datareader, int start, int blockId) : base(datareader, start) {
            this.blockId = blockId;
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadIntAtPosition();
            int additionalStringsCount = datareader.ReadInt();
            for (int i = 0; i < additionalStringsCount; i++) {
                additionalParams.Add(datareader.ReadNullTermString());
            }
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class CompatibilityBlock : ShaderDataBlock {

        public int blockIndex;
        public int relRule;  // 1 = dependency (feature file), 2 = dependency (other files), 3 = exclusion
        public int arg0; // this is just 1 for features files and 2 for all other files
        public int[] flags;
        public int[] range0;
        public int[] range1;
        public int[] range2;
        public string description;

        public CompatibilityBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start) {
            this.blockIndex = blockIndex;
            relRule = datareader.ReadInt();
            arg0 = datareader.ReadInt();
            // flags are at (8)
            flags = ReadByteFlagsUpdated();
            // range 0 at (24)
            range0 = ReadIntRange();
            datareader.offset += 68 - range0.Length * 4;
            // range 1 at (92)
            range1 = ReadIntRange();
            datareader.offset += 60 - range1.Length * 4;
            // range 2 at (152)
            range2 = ReadIntRange();
            datareader.offset += 64 - range2.Length * 4;


            // datareader.offset += 472;

            description = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 256;


        }
        private int[] ReadIntRange() {
            List<int> ints0 = new();
            while (datareader.ReadIntAtPosition() >= 0) {
                ints0.Add(datareader.ReadInt());
            }
            return ints0.ToArray();
        }

        private int[] ReadByteFlagsUpdated() {
            int count = 0;
            int ind = datareader.offset;
            while (datareader.databytes[ind] > 0 && count < 16) {
                count++;
                ind++;
            }
            int[] byteFlags = new int[count];
            for (int i = 0; i < count; i++) {
                byteFlags[i] = datareader.databytes[datareader.offset + i];
            }
            datareader.offset += 16;
            return byteFlags;
        }


        public string RelRuleDescribe() {
            return relRule == 3 ? "EXC(3)" : $"INC({relRule})";
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }



        // 1 to 5 byte flags occur at position 8 (it looks like there is provision for a maximum of 16 byte-flags)
        public string ReadByteFlags() {
            // byte[] bflag = new byte[16];
            string bflags = "";
            int ind = 8;
            while (datareader.databytes[start + ind] > 0 || ind >= 24) {
                bflags += $"{datareader.databytes[start + ind++]}, ";
            }
            return $"({bflags[0..^2]})";
        }
    }




    // needs implemenation (parser works by moving the offset 152 bytes for each d-block)
    public class DBlock : ShaderDataBlock {

        public int blockIndex;
        public string name0;
        public string name1; // it looks like d-blocks might have the provision for 2 strings (unsure)
        public int arg0;
        public int arg1;
        public int arg2;
        public int arg3;
        public int arg4;
        public int arg5;


        public DBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start) {
            this.blockIndex = blockIndex;
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadInt();
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }



    public class UnknownBlock : ShaderDataBlock {

        public int blockIndex;
        public int relRule;  // 2 = dependency (other files), 3 = exclusion (1 not present, as in the compat-blocks)
        public int arg0; // ALWAYS 3 (for compat-blocks, this value is 1 for features files and 2 for all other files)
        public int arg1; // arg1 at (88) sometimes has a value > -1 (in compat-blocks this value is always seen to be -1)
        public int[] flags;
        public int[] range0;
        public int[] range1;
        public int[] range2;
        public string description;

        public UnknownBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start) {
            this.blockIndex = blockIndex;
            relRule = datareader.ReadInt();
            arg0 = datareader.ReadInt();
            if (arg0 != 3) {
                throw new ShaderParserException("unexpected value!");
            }
            // flags at (8)
            flags = ReadByteFlagsUpdated();
            // range0 at (24)
            range0 = ReadIntRange();
            datareader.offset += 64 - range0.Length * 4;
            // integer at (88)
            arg1 = datareader.ReadInt();
            // range1 at (92)
            range1 = ReadIntRange();
            datareader.offset += 60 - range1.Length * 4;
            // range1 at (152)
            range2 = ReadIntRange();
            datareader.offset += 64 - range2.Length * 4;
            // there seems to be a provision here for a description, for the dota2 archive it is always null
            description = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 256;
        }

        private int[] ReadIntRange() {
            List<int> ints0 = new();
            while (datareader.ReadIntAtPosition() >= 0) {
                ints0.Add(datareader.ReadInt());
            }
            return ints0.ToArray();
        }

        private int[] ReadByteFlagsUpdated() {
            int count = 0;
            int ind = datareader.offset;
            while (datareader.databytes[ind] > 0 && count < 16) {
                count++;
                ind++;
            }
            int[] byteFlags = new int[count];
            for (int i = 0; i < count; i++) {
                byteFlags[i] = datareader.databytes[datareader.offset + i];
            }
            datareader.offset += 16;
            return byteFlags;
        }


        // FIXME - I shouldn't need this method
        // 1 to 5 byte flags occur at position 8 (it looks like there is provision for a maximum of 16 byte-flags)
        public string ReadByteFlags() {
            // byte[] bflag = new byte[16];
            string bflags = "";
            int ind = 8;
            while (datareader.databytes[start + ind] > 0 || ind >= 24) {
                bflags += $"{datareader.databytes[start + ind++]},";
            }
            return $"({bflags[0..^1]})";
        }

        public bool AllFlagsAre3() {
            bool flagsAre3 = true;
            foreach (int flag in flags) {
                if (flag != 3) {
                    flagsAre3 = false;
                }
            }
            return flagsAre3;
        }


        public string GetConciseDescription(int[] usePadding = null) {
            int[] p = { 10, 8, 15, 5 };
            if (usePadding != null) {
                p = usePadding;
            }
            string relRuleKeyDesciption = $"{RelRuleDescribe().PadRight(p[0])}{CombineIntArray(range1).PadRight(p[1])}" +
                $"{CombineIntArray(flags, includeParenth: true).PadRight(p[2])}{CombineIntArray(range2).PadRight(p[3])}";
            return relRuleKeyDesciption;
        }

        public string GetResolvedNames(List<DataBlockSfBlock> sfBlocks, List<DBlock> dBlocks) {
            List<string> names = new();
            for (int i = 0; i < flags.Length; i++) {
                if (flags[i] == 2) {
                    names.Add(sfBlocks[range0[i]].name0);
                    continue;
                }
                if (flags[i] == 3) {
                    names.Add(dBlocks[range0[i]].name0);
                    continue;
                }
                throw new ShaderParserException("this cannot happen!");
            }
            return CombineStringArray(names.ToArray());
        }


        public string RelRuleDescribe() {
            return relRule == 3 ? "EXC(3)" : $"INC({relRule})";
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class ParamBlock : ShaderDataBlock {
        public string name0;
        public string name1;
        public string name2;
        public int pt0;
        public float res0;
        public int main0;
        public byte[] dynExp = Array.Empty<byte>();
        public int arg0;
        public int arg1;
        public int arg2;
        public int arg3;
        public int arg4;
        public int arg5;
        public string fileref;
        public int[] ranges0 = new int[4];
        public int[] ranges1 = new int[4];
        public int[] ranges2 = new int[4];
        public float[] ranges3 = new float[4];
        public float[] ranges4 = new float[4];
        public float[] ranges5 = new float[4];
        public int[] ranges6 = new int[4];
        public int[] ranges7 = new int[4];
        public string command0;
        public string command1;

        public ParamBlock(ShaderDataReader datareader, int start) : base(datareader, start) {
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            pt0 = datareader.ReadInt();
            res0 = datareader.ReadFloat();
            name2 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            main0 = datareader.ReadInt();
            if (main0 == 6 || main0 == 7) {
                int dynExpLen = datareader.ReadInt();
                dynExp = datareader.ReadBytes(dynExpLen);
            }
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadInt();
            fileref = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            for (int i = 0; i < 4; i++) {
                ranges0[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges1[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges2[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges3[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges4[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges5[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges6[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges7[i] = datareader.ReadInt();
            }
            command0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 32;
            command1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 32;



            int myint = 10;
        }


        public void ShowBlock() {

            Debug.WriteLine($"name0 {s(20)} {name0}");
            Debug.WriteLine($"name1 {s(20)} {name1}");
            Debug.WriteLine($"lead0,lead1 {s(14)} ({pt0},{res0})");
            Debug.WriteLine($"name2 {s(20)} {name2}");
            Debug.WriteLine($"paramType {s(16)} {main0}");
            Debug.WriteLine($"dynExp {s(19)} {ShaderDataReader.BytesToString(dynExp)}");
            Debug.WriteLine($"arg0 {s(21)} {arg0,9}");
            Debug.WriteLine($"arg1 {s(21)} {arg1,9}");
            Debug.WriteLine($"arg2 {s(21)} {arg2,9}");
            Debug.WriteLine($"arg3 {s(21)} {arg3,9}");
            Debug.WriteLine($"arg4 {s(21)} {arg4,9}");
            Debug.WriteLine($"arg5 {s(21)} {arg5,9}");
            Debug.WriteLine($"fileref {s(17)} {fileref}");
            Debug.WriteLine($"ranges0 {s(17)} {CombineIntArray(ranges0)}");
            Debug.WriteLine($"ranges1 {s(17)} {CombineIntArray(ranges1)}");
            Debug.WriteLine($"ranges2 {s(17)} {CombineIntArray(ranges2)}");
            Debug.WriteLine($"ranges3 {s(17)} {ranges3[0]},{ranges3[1]},{ranges3[2]},{ranges3[3]}");
            Debug.WriteLine($"ranges4 {s(17)} {ranges4[0]},{ranges4[1]},{ranges4[2]},{ranges4[3]}");
            Debug.WriteLine($"ranges5 {s(17)} {ranges5[0]},{ranges5[1]},{ranges5[2]},{ranges5[3]}");
            Debug.WriteLine($"ranges6 {s(17)} {CombineIntArray(ranges6)}");
            Debug.WriteLine($"ranges7 {s(17)} {CombineIntArray(ranges7)}");
            Debug.WriteLine($"command0 {s(16)} {command0}");
            Debug.WriteLine($"command1 {s(16)} {command1}");
        }



        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    // needs implemenation (parser works by moving the offset 280 bytes for each mipmap-block)
    public class MipmapBlock : ShaderDataBlock {
        public MipmapBlock(ShaderDataReader datareader, int start) : base(datareader, start) {

        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class BufferBlock : ShaderDataBlock {
        string name;
        int bufferSize;
        List<(string, int, int, int, int)> bufferParams = new();
        uint blockId;

        public BufferBlock(ShaderDataReader datareader, int start) : base(datareader, start) {
            name = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            bufferSize = datareader.ReadInt();
            datareader.offset += 4; // next 4 bytes are always 0
            int paramCount = datareader.ReadInt();
            for (int i = 0; i < paramCount; i++) {
                string paramName = datareader.ReadNullTermStringAtPosition();
                datareader.offset += 64;
                int bufferIndex = datareader.ReadInt();
                int arg0 = datareader.ReadInt();
                int arg1 = datareader.ReadInt();
                int arg2 = datareader.ReadInt();
                bufferParams.Add((paramName, bufferIndex, arg0, arg1, arg2));
            }
            blockId = datareader.ReadUInt();
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class SymbolsBlock : ShaderDataBlock {
        List<(string, string, string, int)> symbolParams = new();

        public SymbolsBlock(ShaderDataReader datareader, int start) : base(datareader, start) {
            int namesCount = datareader.ReadInt();
            for (int i = 0; i < namesCount; i++) {
                string name0 = datareader.ReadNullTermString();
                string name1 = datareader.ReadNullTermString();
                string name2 = datareader.ReadNullTermString();
                int int0 = datareader.ReadInt();
                symbolParams.Add((name0, name1, name2, int0));
            }
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


}








