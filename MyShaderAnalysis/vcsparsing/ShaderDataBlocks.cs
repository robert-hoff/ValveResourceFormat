using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.vcsparsing
{


    public class FeaturesHeaderBlock : ShaderDataBlock
    {

        public bool has_psrs_file { get; }
        public int unknown_val { get; }
        public string file_description { get; }
        public int arg0 { get; }
        public int arg1 { get; }
        public int arg2 { get; }
        public int arg3 { get; }
        public int arg4 { get; }
        public int arg5 { get; }
        public int arg6 { get; }
        public int arg7 { get; }
        public List<(string, string)> mainParams { get; } = new();
        public List<string> fileIDs { get; } = new();


        public FeaturesHeaderBlock(ShaderDataReader datareader, int start) : base(datareader, start)
        {
            int psrs_arg = datareader.ReadInt();
            if (psrs_arg != 0 && psrs_arg != 1)
            {
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
            if (has_psrs_file)
            {
                nr_of_arguments = datareader.ReadInt();
            }

            for (int i = 0; i < nr_of_arguments; i++)
            {
                string string_arg0 = datareader.ReadNullTermStringAtPosition();
                string string_arg1 = "";
                datareader.MoveOffset(128);
                if (datareader.ReadInt() > 0)
                {
                    string_arg1 = datareader.ReadNullTermStringAtPosition();
                    datareader.MoveOffset(68);
                }
                mainParams.Add((string_arg0, string_arg1));
            }

            for (int i = 0; i < 8; i++)
            {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
            if (has_psrs_file)
            {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
        }


        public void PrintByteSummary()
        {
            datareader.SetOffset(start);
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
            if (has_psrs_file == 1)
            {
                // NOTE nr_of_arguments is overwritten
                nr_of_arguments = datareader.ReadIntAtPosition();
                datareader.ShowBytes(4, $"nr of arguments overriden ({nr_of_arguments})");
            }
            datareader.BreakLine();
            datareader.ShowByteCount();
            for (int i = 0; i < nr_of_arguments; i++)
            {
                string default_name = datareader.ReadNullTermStringAtPosition();
                datareader.Comment($"{default_name}");
                datareader.ShowBytes(128);
                uint has_s_argument = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(4);
                if (has_s_argument > 0)
                {
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
            if (has_psrs_file == 0)
            {
                datareader.ShowBytes(16, "file ID7 - shared by all Dota2 vcs files");
            }
            if (has_psrs_file == 1)
            {
                datareader.ShowBytes(16, "file ID7 - reference to psrs file");
                datareader.ShowBytes(16, "file ID8 - shared by all Dota2 vcs files");
            }
            datareader.BreakLine();
        }


        public void ShowMainParams()
        {

            foreach (var parampair in mainParams)
            {
                Debug.WriteLine($"       {parampair.Item1.PadRight(50)} {parampair.Item2}");
            }

        }

    }



    // needs implemenation
    public class VsPsHeaderBlock : ShaderDataBlock
    {
        public VsPsHeaderBlock(ShaderDataReader datareader, int start) : base(datareader, start)
        {
            datareader.MoveOffset(36);
        }
    }


    public class SfBlock : ShaderDataBlock
    {
        public int blockId { get; }
        public string name0 { get; }
        public string name1 { get; }
        public int arg0 { get; }
        public int arg1 { get; }
        public int arg2 { get; }
        public int arg3 { get; }
        public int arg4 { get; }
        public int arg5 { get; }
        public List<string> additionalParams { get; } = new();

        public SfBlock(ShaderDataReader datareader, int start, int blockId) : base(datareader, start)
        {
            this.blockId = blockId;
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadIntAtPosition();
            int additionalStringsCount = datareader.ReadInt();
            for (int i = 0; i < additionalStringsCount; i++)
            {
                additionalParams.Add(datareader.ReadNullTermString());
            }
        }

    }


    public class SfConstraintsBlock : ShaderDataBlock
    {

        public int blockIndex { get; }
        public int relRule { get; }  // 1 = dependency-rule (feature file), 2 = dependency-rule (other files), 3 = exclusion
        public int arg0 { get; } // this is just 1 for features files and 2 for all other files
        public int[] flags { get; }
        public int[] range0 { get; }
        public int[] range1 { get; }
        public int[] range2 { get; }
        public string description { get; }

        public SfConstraintsBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start)
        {
            this.blockIndex = blockIndex;
            relRule = datareader.ReadInt();
            arg0 = datareader.ReadInt();
            // flags are at (8)
            flags = ReadByteFlags();
            // range 0 at (24)
            range0 = ReadIntRange();
            datareader.MoveOffset(68 - range0.Length * 4);
            // range 1 at (92)
            range1 = ReadIntRange();
            datareader.MoveOffset(60 - range1.Length * 4);
            // range 2 at (152)
            range2 = ReadIntRange();
            datareader.MoveOffset(64 - range2.Length * 4);

            description = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(256);
        }

        private int[] ReadIntRange()
        {
            List<int> ints0 = new();
            while (datareader.ReadIntAtPosition() >= 0)
            {
                ints0.Add(datareader.ReadInt());
            }
            return ints0.ToArray();
        }

        private int[] ReadByteFlags()
        {
            int count = 0;
            datareader.SavePosition();
            while (datareader.ReadByte() > 0 && count < 16)
            {
                count++;
            }
            int[] byteFlags = new int[count];
            datareader.RestorePosition();
            for (int i = 0; i < count; i++)
            {
                byteFlags[i] = datareader.ReadByte();
            }
            datareader.RestorePosition();
            datareader.MoveOffset(16);
            return byteFlags;
        }


        public string RelRuleDescribe()
        {
            return relRule == 3 ? "EXC(3)" : $"INC({relRule})";
        }

        public string GetByteFlagsAsString()
        {
            return CombineIntArray(flags);
        }
    }



    public class DBlock : ShaderDataBlock
    {

        public int blockIndex { get; }
        public string name0 { get; }
        public string name1 { get; } // it looks like d-blocks might have the provision for 2 strings (but not seen in use)
        public int arg0 { get; }
        public int arg1 { get; }
        public int arg2 { get; }
        public int arg3 { get; }
        public int arg4 { get; }
        public int arg5 { get; }


        public DBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start)
        {
            this.blockIndex = blockIndex;
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            arg5 = datareader.ReadInt();
        }

    }



    public class DConstraintsBlock : ShaderDataBlock
    {

        public int blockIndex { get; }
        public int relRule { get; }  // 2 = dependency-rule (other files), 3 = exclusion (1 not present, as in the compat-blocks)
        public int arg0 { get; } // ALWAYS 3 (for sf-constraint-blocks, this value is 1 for features files and 2 for all other files)
        public int arg1 { get; } // arg1 at (88) sometimes has a value > -1 (in compat-blocks this value is always seen to be -1)
        public int[] flags { get; }
        public int[] range0 { get; }
        public int[] range1 { get; }
        public int[] range2 { get; }
        public string description { get; }

        public DConstraintsBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start)
        {
            this.blockIndex = blockIndex;
            relRule = datareader.ReadInt();
            arg0 = datareader.ReadInt();
            if (arg0 != 3)
            {
                throw new ShaderParserException("unexpected value!");
            }
            // flags at (8)
            flags = ReadByteFlagsUpdated();
            // range0 at (24)
            range0 = ReadIntRange();
            datareader.MoveOffset(64 - range0.Length * 4);
            // integer at (88)
            arg1 = datareader.ReadInt();
            // range1 at (92)
            range1 = ReadIntRange();
            datareader.MoveOffset(60 - range1.Length * 4);
            // range1 at (152)
            range2 = ReadIntRange();
            datareader.MoveOffset(64 - range2.Length * 4);
            // there seems to be a provision here for a description, for the dota2 archive it is always null
            description = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(256);
        }

        private int[] ReadIntRange()
        {
            List<int> ints0 = new();
            while (datareader.ReadIntAtPosition() >= 0)
            {
                ints0.Add(datareader.ReadInt());
            }
            return ints0.ToArray();
        }

        private int[] ReadByteFlagsUpdated()
        {
            int count = 0;
            datareader.SavePosition();
            while (datareader.ReadByte() > 0 && count < 16)
            {
                count++;
            }
            int[] byteFlags = new int[count];
            datareader.RestorePosition();
            for (int i = 0; i < count; i++)
            {
                byteFlags[i] = datareader.ReadByte();
            }
            datareader.RestorePosition();
            datareader.MoveOffset(16);
            return byteFlags;
        }

        // todo - check formatting
        public string ReadByteFlagsAsString()
        {
            return CombineIntArray(flags);
        }

        public bool AllFlagsAre3()
        {
            bool flagsAre3 = true;
            foreach (int flag in flags)
            {
                if (flag != 3)
                {
                    flagsAre3 = false;
                }
            }
            return flagsAre3;
        }

        public string GetConciseDescription(int[] usePadding = null)
        {
            int[] p = { 10, 8, 15, 5 };
            if (usePadding != null)
            {
                p = usePadding;
            }
            string relRuleKeyDesciption = $"{RelRuleDescribe().PadRight(p[0])}{CombineIntArray(range1).PadRight(p[1])}" +
                $"{CombineIntArray(flags, includeParenth: true).PadRight(p[2])}{CombineIntArray(range2).PadRight(p[3])}";
            return relRuleKeyDesciption;
        }

        public string GetResolvedNames(List<SfBlock> sfBlocks, List<DBlock> dBlocks)
        {
            List<string> names = new();
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i] == 2)
                {
                    names.Add(sfBlocks[range0[i]].name0);
                    continue;
                }
                if (flags[i] == 3)
                {
                    names.Add(dBlocks[range0[i]].name0);
                    continue;
                }
                throw new ShaderParserException("this cannot happen!");
            }
            return CombineStringArray(names.ToArray());
        }
        public string RelRuleDescribe()
        {
            return relRule == 3 ? "EXC(3)" : $"INC({relRule})";
        }

    }


    public class ParamBlock : ShaderDataBlock
    {
        public string name0 { get; }
        public string name1 { get; }
        public string name2 { get; }
        public int type { get; }
        public float res0 { get; }
        public int lead0 { get; }
        public byte[] dynExp { get; } = Array.Empty<byte>();
        public int arg0 { get; }
        public int arg1 { get; }
        public int arg2 { get; }
        public int arg3 { get; }
        public int arg4 { get; }
        public int arg5 { get; }
        public string fileref { get; }
        public int[] ranges0 { get; } = new int[4];
        public int[] ranges1 { get; } = new int[4];
        public int[] ranges2 { get; } = new int[4];
        public float[] ranges3 { get; } = new float[4];
        public float[] ranges4 { get; } = new float[4];
        public float[] ranges5 { get; } = new float[4];
        public int[] ranges6 { get; } = new int[4];
        public int[] ranges7 { get; } = new int[4];
        public string command0 { get; }
        public string command1 { get; }

        public ParamBlock(ShaderDataReader datareader, int start) : base(datareader, start)
        {
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            type = datareader.ReadInt();
            res0 = datareader.ReadFloat();
            name2 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            lead0 = datareader.ReadInt();
            if (lead0 == 6 || lead0 == 7)
            {
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
            datareader.MoveOffset(64);
            for (int i = 0; i < 4; i++)
            {
                ranges0[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges1[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges2[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges3[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges4[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges5[i] = datareader.ReadFloat();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges6[i] = datareader.ReadInt();
            }
            for (int i = 0; i < 4; i++)
            {
                ranges7[i] = datareader.ReadInt();
            }
            command0 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(32);
            command1 = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(32);
            int myint = 10;
        }


        public void ShowBlock()
        {

            Debug.WriteLine($"name0 {s(20)} {name0}");
            Debug.WriteLine($"name1 {s(20)} {name1}");
            Debug.WriteLine($"lead0,lead1 {s(14)} ({type},{res0})");
            Debug.WriteLine($"name2 {s(20)} {name2}");
            Debug.WriteLine($"paramType {s(16)} {lead0}");
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


    }


    public class MipmapBlock : ShaderDataBlock
    {
        public MipmapBlock(ShaderDataReader datareader, int start) : base(datareader, start)
        {
            // TODO - needs implementation
            datareader.MoveOffset(280);
        }
    }


    public class BufferBlock : ShaderDataBlock
    {
        public string name { get; }
        public int bufferSize { get; }
        public List<(string, int, int, int, int)> bufferParams { get; } = new();
        public uint blockId { get; }

        public BufferBlock(ShaderDataReader datareader, int start) : base(datareader, start)
        {
            name = datareader.ReadNullTermStringAtPosition();
            datareader.MoveOffset(64);
            bufferSize = datareader.ReadInt();
            datareader.MoveOffset(4); // next 4 bytes are always 0
            int paramCount = datareader.ReadInt();
            for (int i = 0; i < paramCount; i++)
            {
                string paramName = datareader.ReadNullTermStringAtPosition();
                datareader.MoveOffset(64);
                int bufferIndex = datareader.ReadInt();
                int arg0 = datareader.ReadInt();
                int arg1 = datareader.ReadInt();
                int arg2 = datareader.ReadInt();
                bufferParams.Add((paramName, bufferIndex, arg0, arg1, arg2));
            }
            blockId = datareader.ReadUInt();
        }
    }


    public class SymbolsBlock : ShaderDataBlock
    {
        public string name { get; }
        public string type { get; }
        public string option { get; }
        public int semanticIndex { get; }

        public SymbolsBlock(ShaderDataReader datareader, int start, int blockIndex) : base(datareader, start)
        {
            int namesCount = datareader.ReadInt();
            for (int i = 0; i < namesCount; i++)
            {
                name = datareader.ReadNullTermString();
                type = datareader.ReadNullTermString();
                option = datareader.ReadNullTermString();
                semanticIndex = datareader.ReadInt();
            }
        }




    }
}






