using System;
using System.Collections.Generic;
using ValveResourceFormat.Utils;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace ValveResourceFormat.CompiledShader
{
    public class FeaturesHeaderBlock : ShaderDataBlock
    {
        public int VcsFileVersion { get; }
        public bool HasPsrsFile { get; }
        public int Version { get; }
        public string FileDescription { get; }
        public int DevShader { get; }
        public int Arg1 { get; }
        public int Arg2 { get; }
        public int Arg3 { get; }
        public int Arg4 { get; }
        public int Arg5 { get; }
        public int Arg6 { get; }
        public int Arg7 { get; } = -1;
        public List<(string, string)> MainParams { get; } = new();
        public List<(string, string)> EditorIDs { get; } = new();
        public FeaturesHeaderBlock(ShaderDataReader datareader) : base(datareader)
        {
            int vcsMagicId = datareader.ReadInt32();
            if (vcsMagicId != ShaderFile.MAGIC)
            {
                throw new UnexpectedMagicException($"Wrong magic ID, VCS expects 0x{ShaderFile.MAGIC:x}",
                    vcsMagicId, nameof(vcsMagicId));
            }
            VcsFileVersion = datareader.ReadInt32();
            if (VcsFileVersion != 65 && VcsFileVersion != 64 && VcsFileVersion != 62)
            {
                throw new UnexpectedMagicException($"Unsupported version {VcsFileVersion}, versions 65, 64 and 62 are supported",
                    VcsFileVersion, nameof(VcsFileVersion));
            }

            int psrs_arg = 0;
            if (VcsFileVersion >= 64)
            {
                psrs_arg = datareader.ReadInt32();
            }

            if (psrs_arg != 0 && psrs_arg != 1)
            {
                throw new ShaderParserException($"unexpected value psrs_arg = {psrs_arg}");
            }
            HasPsrsFile = psrs_arg > 0;
            Version = datareader.ReadInt32();
            datareader.ReadInt32(); // length of name, but not needed because it's always null-term
            FileDescription = datareader.ReadNullTermString();
            DevShader = datareader.ReadInt32();
            Arg1 = datareader.ReadInt32();
            Arg2 = datareader.ReadInt32();
            Arg3 = datareader.ReadInt32();
            Arg4 = datareader.ReadInt32();
            Arg5 = datareader.ReadInt32();
            Arg6 = datareader.ReadInt32();

            if (VcsFileVersion >= 64)
            {
                Arg7 = datareader.ReadInt32();
            }

            int nr_of_arguments = datareader.ReadInt32();
            if (HasPsrsFile)
            {
                // nr_of_arguments is overwritten
                nr_of_arguments = datareader.ReadInt32();
            }
            for (int i = 0; i < nr_of_arguments; i++)
            {
                string string_arg0 = datareader.ReadNullTermStringAtPosition();
                string string_arg1 = "";
                datareader.BaseStream.Position += 128;
                if (datareader.ReadInt32() > 0)
                {
                    string_arg1 = datareader.ReadNullTermStringAtPosition();
                    datareader.BaseStream.Position += 68;
                }
                MainParams.Add((string_arg0, string_arg1));
            }
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", "// Editor ref. ID0 (produces this file)"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", $"// Editor ref. ID1 - usually a ref to the vs file ({VcsProgramType.VertexShader})"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", $"// Editor ref. ID2 - usually a ref to the ps file ({VcsProgramType.PixelShader})"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", "// Editor ref. ID3"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", "// Editor ref. ID4"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", "// Editor ref. ID5"));
            EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", "// Editor ref. ID6"));

            if (VcsFileVersion >= 64)
            {
                if (HasPsrsFile)
                {
                    EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}", $"// Editor ref. ID7 - ref to psrs file ({VcsProgramType.PixelShaderRenderState})"));
                    EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}",
                        $"// Editor ref. ID8 - common editor reference shared by multiple files "));
                }
                else
                {
                    EditorIDs.Add(($"{datareader.ReadBytesAsString(16)}",
                        "// Editor ref. ID7- common editor reference shared by multiple files"));
                }
            }
        }

        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount("vcs file");
            Datareader.ShowBytes(4, "\"vcs2\"");
            int vcs_version = Datareader.ReadInt32AtPosition();
            Datareader.ShowBytes(4, $"version {vcs_version}");
            Datareader.BreakLine();
            Datareader.ShowByteCount("features header");
            int has_psrs_file = 0;
            if (vcs_version >= 64)
            {
                has_psrs_file = Datareader.ReadInt32AtPosition();
                Datareader.ShowBytes(4, "has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));
            }
            int unknown_val = Datareader.ReadInt32AtPosition();
            Datareader.ShowBytes(4, $"unknown_val = {unknown_val} (usually 0)");
            int len_name_description = Datareader.ReadInt32AtPosition();
            Datareader.ShowBytes(4, $"{len_name_description} len of name");
            Datareader.BreakLine();
            string name_desc = Datareader.ReadNullTermStringAtPosition();
            Datareader.ShowByteCount(name_desc);
            Datareader.ShowBytes(len_name_description + 1);
            Datareader.BreakLine();
            Datareader.ShowByteCount();
            uint arg0 = Datareader.ReadUInt32AtPosition(0);
            uint arg1 = Datareader.ReadUInt32AtPosition(4);
            uint arg2 = Datareader.ReadUInt32AtPosition(8);
            uint arg3 = Datareader.ReadUInt32AtPosition(12);
            Datareader.ShowBytes(16, 4, breakLine: false);
            Datareader.TabComment($"({arg0},{arg1},{arg2},{arg3})");
            uint arg4 = Datareader.ReadUInt32AtPosition(0);
            uint arg5 = Datareader.ReadUInt32AtPosition(4);
            uint arg6 = Datareader.ReadUInt32AtPosition(8);
            if (vcs_version >= 64)
            {
                uint arg7 = Datareader.ReadUInt32AtPosition(12);
                Datareader.ShowBytes(16, 4, breakLine: false);
                Datareader.TabComment($"({arg4},{arg5},{arg6},{arg7})");
            }
            else
            {
                Datareader.ShowBytes(12, 4, breakLine: false);
                Datareader.TabComment($"({arg4},{arg5},{arg6})");
            }

            Datareader.BreakLine();
            Datareader.ShowByteCount();
            int argument_count = Datareader.ReadInt32AtPosition();
            Datareader.ShowBytes(4, $"argument_count = {argument_count}");
            if (has_psrs_file == 1)
            {
                // nr_of_arguments becomes overwritten
                argument_count = Datareader.ReadInt32AtPosition();
                Datareader.ShowBytes(4, $"argument_count = {argument_count} (overridden)");
            }
            Datareader.BreakLine();
            Datareader.ShowByteCount();
            for (int i = 0; i < argument_count; i++)
            {
                string default_name = Datareader.ReadNullTermStringAtPosition();
                Datareader.Comment($"{default_name}");
                Datareader.ShowBytes(128);
                uint has_s_argument = Datareader.ReadUInt32AtPosition();
                Datareader.ShowBytes(4);
                if (has_s_argument > 0)
                {
                    uint sSymbolArgValue = Datareader.ReadUInt32AtPosition(64);
                    string sSymbolName = Datareader.ReadNullTermStringAtPosition();
                    Datareader.Comment($"{sSymbolName}");
                    Datareader.ShowBytes(68);
                }
            }
            Datareader.BreakLine();
            Datareader.ShowByteCount("Editor/Shader stack for generating the file");
            Datareader.ShowBytes(16, "Editor ref. ID0 (produces this file)");
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"Editor ref. ID1 - usually a ref to the vs file ({VcsProgramType.VertexShader})");
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"Editor ref. ID2 - usually a ref to the ps file ({VcsProgramType.PixelShader})");
            Datareader.ShowBytes(16, "Editor ref. ID3");
            Datareader.ShowBytes(16, "Editor ref. ID4");
            Datareader.ShowBytes(16, "Editor ref. ID5");
            Datareader.ShowBytes(16, "Editor ref. ID6");
            if (vcs_version >= 64 && has_psrs_file == 0)
            {
                Datareader.ShowBytes(16, "Editor ref. ID7 - common editor reference shared by multiple files");
            }
            if (vcs_version >= 64 && has_psrs_file == 1)
            {
                Datareader.ShowBytes(16, $"Editor ref. ID7 - reference to psrs file ({VcsProgramType.PixelShaderRenderState})");
                Datareader.ShowBytes(16, "Editor ref. ID8 - common editor reference shared by multiple files");
            }
            Datareader.BreakLine();
        }
    }

    public class VsPsHeaderBlock : ShaderDataBlock
    {
        public int VcsFileVersion { get; }
        public bool HasPsrsFile { get; }
        public string FileID0 { get; }
        public string FileID1 { get; }
        public VsPsHeaderBlock(ShaderDataReader datareader) : base(datareader)
        {
            int vcsMagicId = datareader.ReadInt32();
            if (vcsMagicId != ShaderFile.MAGIC)
            {
                throw new UnexpectedMagicException($"Wrong magic ID, VCS expects 0x{ShaderFile.MAGIC:x}",
                    vcsMagicId, nameof(vcsMagicId));
            }
            VcsFileVersion = datareader.ReadInt32();
            if (VcsFileVersion != 65 && VcsFileVersion != 64 && VcsFileVersion != 62)
            {
                throw new UnexpectedMagicException($"Unsupported version {VcsFileVersion}, versions 65, 64 and 62 are supported",
                    VcsFileVersion, nameof(VcsFileVersion));
            }
            int psrs_arg = 0;
            if (VcsFileVersion >= 64)
            {
                psrs_arg = datareader.ReadInt32();
                if (psrs_arg != 0 && psrs_arg != 1)
                {
                    throw new ShaderParserException($"Unexpected value psrs_arg = {psrs_arg}");
                }
            }
            HasPsrsFile = psrs_arg > 0;
            FileID0 = datareader.ReadBytesAsString(16);
            FileID1 = datareader.ReadBytesAsString(16);
        }

        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount("vcs file");
            Datareader.ShowBytes(4, "\"vcs2\"");
            int vcs_version = Datareader.ReadInt32AtPosition();
            Datareader.ShowBytes(4, $"version {vcs_version}");
            Datareader.BreakLine();
            Datareader.ShowByteCount("ps/vs header");
            if (vcs_version >= 64)
            {
                int has_psrs_file = Datareader.ReadInt32AtPosition();
                Datareader.ShowBytes(4, $"has_psrs_file = {(has_psrs_file > 0 ? "True" : "False")}");
            }
            Datareader.BreakLine();
            Datareader.ShowByteCount("Editor/Shader stack for generating the file");
            Datareader.ShowBytes(16, "Editor ref. ID0 (produces this file)");
            Datareader.ShowBytes(16, "Editor ref. ID1 - common editor reference shared by multiple files");
        }
    }

    // SfBlocks are usually 152 bytes long, occasionally they have extra string parameters
    public class SfBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public string Name0 { get; }
        public string Name1 { get; }
        public int Arg0 { get; }
        public int Arg1 { get; }
        public int Arg2 { get; }
        public int Arg3 { get; }
        public int Arg4 { get; }
        public int Arg5 { get; }
        public List<string> AdditionalParams { get; } = new();
        public SfBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            Name0 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Name1 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Arg0 = datareader.ReadInt32();
            Arg1 = datareader.ReadInt32();
            Arg2 = datareader.ReadInt32();
            Arg3 = datareader.ReadInt32();
            Arg4 = datareader.ReadInt32();
            Arg5 = datareader.ReadInt32AtPosition();
            int additionalStringsCount = datareader.ReadInt32();
            for (int i = 0; i < additionalStringsCount; i++)
            {
                AdditionalParams.Add(datareader.ReadNullTermString());
            }
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount();
            for (int i = 0; i < 2; i++)
            {
                string name1 = Datareader.ReadNullTermStringAtPosition();
                if (name1.Length > 0)
                {
                    Datareader.Comment($"{name1}");
                }
                Datareader.ShowBytes(64);
            }
            int arg0 = Datareader.ReadInt32AtPosition(0);
            int arg1 = Datareader.ReadInt32AtPosition(4);
            int arg2 = Datareader.ReadInt32AtPosition(8);
            int arg3 = Datareader.ReadInt32AtPosition(12);
            int arg4 = Datareader.ReadInt32AtPosition(16);
            int arg5 = Datareader.ReadInt32AtPosition(20);
            Datareader.ShowBytes(16, 4, breakLine: false);
            Datareader.TabComment($"({arg0},{arg1},{arg2},{arg3})");
            Datareader.ShowBytes(4, $"({arg4}) known values [-1,28]");
            Datareader.ShowBytes(4, $"{arg5} additional string params");
            int string_offset = (int)Datareader.BaseStream.Position;
            List<string> names = new();
            for (int i = 0; i < arg5; i++)
            {
                string paramname = Datareader.ReadNullTermStringAtPosition(string_offset, rel: false);
                names.Add(paramname);
                string_offset += paramname.Length + 1;
            }
            if (names.Count > 0)
            {
                PrintStringList(names);
                Datareader.ShowBytes(string_offset - (int)Datareader.BaseStream.Position);
            }
            Datareader.BreakLine();
        }
        private void PrintStringList(List<string> names)
        {
            if (names.Count == 0)
            {
                return;
            }
            Datareader.OutputWrite($"// {names[0]}");
            for (int i = 1; i < names.Count; i++)
            {
                Datareader.OutputWrite($", {names[i]}");
            }
            Datareader.BreakLine();
        }
    }

    // SfConstraintsBlocks are always 472 bytes long
    public class SfConstraintsBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public int RelRule { get; }  // 1 = dependency-rule (feature file), 2 = dependency-rule (other files), 3 = exclusion
        public int Arg0 { get; } // this is just 1 for features files and 2 for all other files
        public int[] Flags { get; }
        public int[] Range0 { get; }
        public int[] Range1 { get; }
        public int[] Range2 { get; }
        public string Description { get; }
        public SfConstraintsBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            RelRule = datareader.ReadInt32();
            Arg0 = datareader.ReadInt32();
            // flags are at (8)
            Flags = ReadByteFlags();
            // range 0 at (24)
            Range0 = ReadIntRange();
            datareader.BaseStream.Position += 68 - Range0.Length * 4;
            // range 1 at (92)
            Range1 = ReadIntRange();

            datareader.BaseStream.Position += 60 - Range1.Length * 4;
            // range 2 at (152)
            Range2 = ReadIntRange();
            datareader.BaseStream.Position += 64 - Range2.Length * 4;
            Description = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 256;
        }
        private int[] ReadIntRange()
        {
            List<int> ints0 = new();
            while (Datareader.ReadInt32AtPosition() >= 0)
            {
                ints0.Add(Datareader.ReadInt32());
            }
            return ints0.ToArray();
        }
        private int[] ReadByteFlags()
        {
            int count = 0;
            long savedPosition = Datareader.BaseStream.Position;
            while (Datareader.ReadByte() > 0 && count < 16)
            {
                count++;
            }
            int[] byteFlags = new int[count];
            Datareader.BaseStream.Position = savedPosition;
            for (int i = 0; i < count; i++)
            {
                byteFlags[i] = Datareader.ReadByte();
            }
            Datareader.BaseStream.Position = savedPosition + 16;
            return byteFlags;
        }
        public string RelRuleDescribe()
        {
            return RelRule == 3 ? "EXC(3)" : $"INC({RelRule})";
        }
        public string GetByteFlagsAsString()
        {
            return CombineIntArray(Flags);
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount($"SF-CONTRAINTS-BLOCK[{BlockIndex}]");
            Datareader.ShowBytes(216);
            string name1 = Datareader.ReadNullTermStringAtPosition();
            Datareader.OutputWriteLine($"[{Datareader.BaseStream.Position}] {name1}");
            Datareader.ShowBytes(256);
            Datareader.BreakLine();
        }
    }

    // DBlocks are always 152 bytes long
    public class DBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public string Name0 { get; }
        public string Name1 { get; } // it looks like d-blocks might have the provision for 2 strings (but not seen in use)
        public int Arg0 { get; }
        public int Arg1 { get; }
        public int Arg2 { get; }
        public int Arg3 { get; }
        public int Arg4 { get; }
        public int Arg5 { get; }
        public DBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            Name0 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Name1 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Arg0 = datareader.ReadInt32();
            Arg1 = datareader.ReadInt32();
            Arg2 = datareader.ReadInt32();
            Arg3 = datareader.ReadInt32();
            Arg4 = datareader.ReadInt32();
            Arg5 = datareader.ReadInt32();
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            string dBlockName = Datareader.ReadNullTermStringAtPosition();
            Datareader.ShowByteCount($"D-BLOCK[{BlockIndex}]");
            Datareader.Comment(dBlockName);
            Datareader.ShowBytes(128);
            Datareader.ShowBytes(12, 4);
            Datareader.ShowBytes(12);
            Datareader.BreakLine();
        }
    }

    // DConstraintsBlock are always 472 bytes long
    public class DConstraintsBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public int RelRule { get; }  // 2 = dependency-rule (other files), 3 = exclusion (1 not present, as in the compat-blocks)
        public int Arg0 { get; } // ALWAYS 3 (for sf-constraint-blocks this value is 1 for features files and 2 for all other files)
        public int Arg1 { get; } // arg1 at (88) sometimes has a value > -1 (in compat-blocks this value is always seen to be -1)
        public int[] Flags { get; }
        public int[] Range0 { get; }
        public int[] Range1 { get; }
        public int[] Range2 { get; }
        public string Description { get; }

        public DConstraintsBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            RelRule = datareader.ReadInt32();
            Arg0 = datareader.ReadInt32();
            if (Arg0 != 3)
            {
                throw new ShaderParserException("unexpected value!");
            }
            // flags at (8)
            Flags = ReadByteFlags();
            // range0 at (24)
            Range0 = ReadIntRange();
            datareader.BaseStream.Position += 64 - Range0.Length * 4;
            // integer at (88)
            Arg1 = datareader.ReadInt32();
            // range1 at (92)
            Range1 = ReadIntRange();
            datareader.BaseStream.Position += 60 - Range1.Length * 4;
            // range1 at (152)
            Range2 = ReadIntRange();
            datareader.BaseStream.Position += 64 - Range2.Length * 4;
            // there is a provision here for a description, but for the dota2 archive this is always null
            Description = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 256;
        }
        private int[] ReadIntRange()
        {
            List<int> ints0 = new();
            while (Datareader.ReadInt32AtPosition() >= 0)
            {
                ints0.Add(Datareader.ReadInt32());
            }
            return ints0.ToArray();
        }
        private int[] ReadByteFlags()
        {
            int count = 0;
            long savedPosition = Datareader.BaseStream.Position;
            while (Datareader.ReadByte() > 0 && count < 16)
            {
                count++;
            }
            int[] byteFlags = new int[count];
            Datareader.BaseStream.Position = savedPosition;
            for (int i = 0; i < count; i++)
            {
                byteFlags[i] = Datareader.ReadByte();
            }
            Datareader.BaseStream.Position = savedPosition;
            Datareader.BaseStream.Position += 16;
            return byteFlags;
        }
        public string ReadByteFlagsAsString()
        {
            return CombineIntArray(Flags);
        }
        public bool AllFlagsAre3()
        {
            bool flagsAre3 = true;
            foreach (int flag in Flags)
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
            string relRuleKeyDesciption = $"{RelRuleDescribe().PadRight(p[0])}{CombineIntArray(Range1).PadRight(p[1])}" +
                $"{CombineIntArray(Flags, includeParenth: true).PadRight(p[2])}{CombineIntArray(Range2).PadRight(p[3])}";
            return relRuleKeyDesciption;
        }
        public string GetResolvedNames(List<SfBlock> sfBlocks, List<DBlock> dBlocks)
        {
            List<string> names = new();
            for (int i = 0; i < Flags.Length; i++)
            {
                if (Flags[i] == 2)
                {
                    names.Add(sfBlocks[Range0[i]].Name0);
                    continue;
                }
                if (Flags[i] == 3)
                {
                    names.Add(dBlocks[Range0[i]].Name0);
                    continue;
                }
                throw new ShaderParserException("this cannot happen!");
            }
            return CombineStringArray(names.ToArray());
        }
        public string RelRuleDescribe()
        {
            return RelRule == 3 ? "EXC(3)" : $"INC({RelRule})";
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount($"D-CONSTRAINTS-BLOCK[{BlockIndex}]");
            Datareader.ShowBytes(472);
            Datareader.BreakLine();
        }
    }

    public class ParamBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public string Name0 { get; }
        public string Name1 { get; }
        public string Name2 { get; }
        public int Type { get; }
        public float Res0 { get; }
        public int Lead0 { get; }
        public byte[] DynExp { get; } = Array.Empty<byte>();
        public int Arg0 { get; }
        public int Arg1 { get; }
        public int Arg2 { get; }
        public int Arg3 { get; }
        public int Arg4 { get; }
        public int Arg5 { get; } = -1;
        public string Fileref { get; }
        public int[] Ranges0 { get; } = new int[4];
        public int[] Ranges1 { get; } = new int[4];
        public int[] Ranges2 { get; } = new int[4];
        public float[] Ranges3 { get; } = new float[4];
        public float[] Ranges4 { get; } = new float[4];
        public float[] Ranges5 { get; } = new float[4];
        public int[] Ranges6 { get; } = new int[4];
        public int[] Ranges7 { get; } = new int[4];
        public string Command0 { get; }
        public string Command1 { get; }
        public byte[] V65Data { get; } = Array.Empty<byte>();
        public ParamBlock(ShaderDataReader datareader, int blockIndex, int vcsVersion) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            Name0 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Name1 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Type = datareader.ReadInt32();
            Res0 = datareader.ReadSingle();
            Name2 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            Lead0 = datareader.ReadInt32();
            if (Lead0 == 6 || Lead0 == 7)
            {
                int dynExpLen = datareader.ReadInt32();
                DynExp = datareader.ReadBytes(dynExpLen);
            }

            // check to see if this reads 'SBMS' (unknown what this is, instance found in v65 hero_pc_40_features.vcs file)
            byte[] checkSBMS = datareader.ReadBytesAtPosition(0, 4);
            if (checkSBMS[0] == 0x53 && checkSBMS[1] == 0x42 && checkSBMS[2] == 0x4D && checkSBMS[3] == 0x53)
            {
                // note - bytes are ignored
                datareader.ReadBytes(4);
                int dynExpLength = datareader.ReadInt32();
                datareader.ReadBytes(dynExpLength);
            }

            Arg0 = datareader.ReadInt32();
            Arg1 = datareader.ReadInt32();
            Arg2 = datareader.ReadInt32();
            Arg3 = datareader.ReadInt32();
            Arg4 = datareader.ReadInt32();
            if (vcsVersion > 62)
            {
                Arg5 = datareader.ReadInt32();
            }

            Fileref = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            for (int i = 0; i < 4; i++)
            {
                Ranges0[i] = datareader.ReadInt32();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges1[i] = datareader.ReadInt32();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges2[i] = datareader.ReadInt32();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges3[i] = datareader.ReadSingle();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges4[i] = datareader.ReadSingle();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges5[i] = datareader.ReadSingle();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges6[i] = datareader.ReadInt32();
            }
            for (int i = 0; i < 4; i++)
            {
                Ranges7[i] = datareader.ReadInt32();
            }
            Command0 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 32;
            Command1 = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 32;

            if (vcsVersion == 65)
            {
                V65Data = datareader.ReadBytes(6);
            }
        }

        public void PrintByteDetail(int vcsVersion)
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount($"PARAM-BLOCK[{BlockIndex}]");
            string name1 = Datareader.ReadNullTermStringAtPosition();
            Datareader.OutputWriteLine($"// {name1}");
            Datareader.ShowBytes(64);
            string name2 = Datareader.ReadNullTermStringAtPosition();
            if (name2.Length > 0)
            {
                Datareader.OutputWriteLine($"// {name2}");
            }
            Datareader.ShowBytes(64);
            Datareader.ShowBytes(8);
            string name3 = Datareader.ReadNullTermStringAtPosition();
            if (name3.Length > 0)
            {
                Datareader.OutputWriteLine($"// {name3}");
            }
            Datareader.ShowBytes(64);
            uint paramType = Datareader.ReadUInt32AtPosition();
            Datareader.OutputWriteLine($"// param-type, 6 or 7 lead dynamic-exp. Known values: 0,1,5,6,7,8,10,11,13");
            Datareader.ShowBytes(4);
            if (paramType == 6 || paramType == 7)
            {
                int dynLength = Datareader.ReadInt32AtPosition();
                Datareader.ShowBytes(4, breakLine: false);
                Datareader.TabComment("dyn-exp len", 1);
                Datareader.TabComment("dynamic expression");
                Datareader.ShowBytes(dynLength);
            }

            // check to see if this reads 'SBMS' (unknown what this is, instance found in v65 hero_pc_40_features.vcs file)
            byte[] checkSBMS = Datareader.ReadBytesAtPosition(0, 4);
            if (checkSBMS[0] == 0x53 && checkSBMS[1] == 0x42 && checkSBMS[2] == 0x4D && checkSBMS[3] == 0x53)
            {
                Datareader.ShowBytes(4, "SBMS");
                int dynLength = Datareader.ReadInt32AtPosition();
                Datareader.ShowBytes(4, "dyn-exp len");
                Datareader.ShowBytes(dynLength, "dynamic expression", 1);
            }

            // 5 or 6 int arguments follow depending on version
            Datareader.ShowBytes(20, 4);
            // v64,65 has an additional argument
            if (vcsVersion >= 64)
            {
                Datareader.ShowBytes(4);
            }

            // a rarely seen file reference
            string name4 = Datareader.ReadNullTermStringAtPosition();
            if (name4.Length > 0)
            {
                Datareader.OutputWriteLine($"// {name4}");
            }
            Datareader.ShowBytes(64);
            // float or int arguments
            int a0 = Datareader.ReadInt32AtPosition(0);
            int a1 = Datareader.ReadInt32AtPosition(4);
            int a2 = Datareader.ReadInt32AtPosition(8);
            int a3 = Datareader.ReadInt32AtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = Datareader.ReadInt32AtPosition(0);
            a1 = Datareader.ReadInt32AtPosition(4);
            a2 = Datareader.ReadInt32AtPosition(8);
            a3 = Datareader.ReadInt32AtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = Datareader.ReadInt32AtPosition(0);
            a1 = Datareader.ReadInt32AtPosition(4);
            a2 = Datareader.ReadInt32AtPosition(8);
            a3 = Datareader.ReadInt32AtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            float f0 = Datareader.ReadSingleAtPosition(0);
            float f1 = Datareader.ReadSingleAtPosition(4);
            float f2 = Datareader.ReadSingleAtPosition(8);
            float f3 = Datareader.ReadSingleAtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            f0 = Datareader.ReadSingleAtPosition(0);
            f1 = Datareader.ReadSingleAtPosition(4);
            f2 = Datareader.ReadSingleAtPosition(8);
            f3 = Datareader.ReadSingleAtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            f0 = Datareader.ReadSingleAtPosition(0);
            f1 = Datareader.ReadSingleAtPosition(4);
            f2 = Datareader.ReadSingleAtPosition(8);
            f3 = Datareader.ReadSingleAtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            a0 = Datareader.ReadInt32AtPosition(0);
            a1 = Datareader.ReadInt32AtPosition(4);
            a2 = Datareader.ReadInt32AtPosition(8);
            a3 = Datareader.ReadInt32AtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = Datareader.ReadInt32AtPosition(0);
            a1 = Datareader.ReadInt32AtPosition(4);
            a2 = Datareader.ReadInt32AtPosition(8);
            a3 = Datareader.ReadInt32AtPosition(12);
            Datareader.ShowBytes(16, breakLine: false);
            Datareader.TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            // a command word, or pair of these
            string name5 = Datareader.ReadNullTermStringAtPosition();
            if (name5.Length > 0)
            {
                Datareader.OutputWriteLine($"// {name5}");
            }
            Datareader.ShowBytes(32);
            string name6 = Datareader.ReadNullTermStringAtPosition();
            if (name6.Length > 0)
            {
                Datareader.OutputWriteLine($"// {name6}");
            }
            Datareader.ShowBytes(32);

            if (vcsVersion == 65)
            {
                Datareader.ShowBytes(6, "unknown bytes specific to vcs version 65");
            }

            Datareader.BreakLine();
        }
        private static string Fmt(float val)
        {
            if (val == -1e9) return "-inf";
            if (val == 1e9) return "inf";
            return $"{val}";
        }
        private static string Fmt(int val)
        {
            if (val == -999999999) return "-inf";
            if (val == 999999999) return "inf";
            return "" + val; ;
        }
    }

    // MipmapBlocks are always 280 bytes long
    public class MipmapBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public string Name { get; }
        public byte[] Arg0 { get; }
        public int Arg1 { get; }
        public int Arg2 { get; }
        public int Arg3 { get; }
        public int Arg4 { get; }
        public int Arg5 { get; }

        public MipmapBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            Arg0 = datareader.ReadBytes(4);
            Arg1 = datareader.ReadInt32();
            Arg2 = datareader.ReadInt32();
            Arg3 = datareader.ReadInt32();
            Arg4 = datareader.ReadInt32();
            Arg5 = datareader.ReadInt32();
            Name = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 256;
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount($"MIPMAP-BLOCK[{BlockIndex}]");
            Datareader.ShowBytes(24, 4);
            string name1 = Datareader.ReadNullTermStringAtPosition();
            Datareader.Comment($"{name1}");
            Datareader.ShowBytes(256);
            Datareader.BreakLine();
        }
    }

    public class BufferBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public string Name { get; }
        public int BufferSize { get; }
        public int Arg0 { get; }
        public int ParamCount { get; }
        public List<(string, int, int, int, int)> BufferParams { get; } = new();
        public uint BlockCrc { get; }
        public BufferBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            Name = datareader.ReadNullTermStringAtPosition();
            datareader.BaseStream.Position += 64;
            BufferSize = datareader.ReadInt32();
            // datareader.MoveOffset(4); // these 4 bytes are always 0
            Arg0 = datareader.ReadInt32();
            ParamCount = datareader.ReadInt32();
            for (int i = 0; i < ParamCount; i++)
            {
                string paramName = datareader.ReadNullTermStringAtPosition();
                datareader.BaseStream.Position += 64;
                int bufferIndex = datareader.ReadInt32();
                int arg0 = datareader.ReadInt32();
                int arg1 = datareader.ReadInt32();
                int arg2 = datareader.ReadInt32();
                BufferParams.Add((paramName, bufferIndex, arg0, arg1, arg2));
            }
            BlockCrc = datareader.ReadUInt32();
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            string blockname = Datareader.ReadNullTermStringAtPosition();
            Datareader.ShowByteCount($"BUFFER-BLOCK[{BlockIndex}] {blockname}");
            Datareader.ShowBytes(64);
            uint bufferSize = Datareader.ReadUInt32AtPosition();
            Datareader.ShowBytes(4, $"{bufferSize} buffer-size");
            Datareader.ShowBytes(4);
            uint paramCount = Datareader.ReadUInt32AtPosition();
            Datareader.ShowBytes(4, $"{paramCount} param-count");
            for (int i = 0; i < paramCount; i++)
            {
                string paramname = Datareader.ReadNullTermStringAtPosition();
                Datareader.OutputWriteLine($"// {paramname}");
                Datareader.ShowBytes(64);
                uint paramIndex = Datareader.ReadUInt32AtPosition();
                Datareader.ShowBytes(4, breakLine: false);
                Datareader.TabComment($"{paramIndex} buffer-offset", 28);
                uint vertexSize = Datareader.ReadUInt32AtPosition();
                uint attributeCount = Datareader.ReadUInt32AtPosition(4);
                uint size = Datareader.ReadUInt32AtPosition(8);
                Datareader.ShowBytes(12, $"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, length)");
            }
            Datareader.BreakLine();
            Datareader.ShowBytes(4, "bufferID (some kind of crc/check)");
            Datareader.BreakLine();
            Datareader.BreakLine();
        }
    }

    public class VertexSymbolsBlock : ShaderDataBlock
    {
        public int BlockIndex { get; }
        public int SymbolsCount { get; }
        public List<(string, string, string, int)> SymbolsDefinition { get; } = new();

        public VertexSymbolsBlock(ShaderDataReader datareader, int blockIndex) : base(datareader)
        {
            this.BlockIndex = blockIndex;
            SymbolsCount = datareader.ReadInt32();
            for (int i = 0; i < SymbolsCount; i++)
            {
                string name = datareader.ReadNullTermString();
                string type = datareader.ReadNullTermString();
                string option = datareader.ReadNullTermString();
                int semanticIndex = datareader.ReadInt32();
                SymbolsDefinition.Add((name, type, option, semanticIndex));
            }
        }
        public void PrintByteDetail()
        {
            Datareader.BaseStream.Position = start;
            Datareader.ShowByteCount($"SYMBOL-NAMES-BLOCK[{BlockIndex}]");
            uint symbolGroupCount = Datareader.ReadUInt32AtPosition();
            Datareader.ShowBytes(4, $"{symbolGroupCount} string groups in this block");
            for (int i = 0; i < symbolGroupCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string symbolname = Datareader.ReadNullTermStringAtPosition();
                    Datareader.OutputWriteLine($"// {symbolname}");
                    Datareader.ShowBytes(symbolname.Length + 1);
                }
                Datareader.ShowBytes(4);
                Datareader.BreakLine();
            }
            if (symbolGroupCount == 0) Datareader.BreakLine();
        }

    }
}
