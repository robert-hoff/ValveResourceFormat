using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.CompiledShader;

using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;


namespace MyShaderAnalysis.utilhelpers
{

    public class DataReaderVcsBytes : ShaderDataReader
    {
        public const uint ZSTD_DELIM = 0xFFFFFFFD;
        public const uint LZMA_DELIM = 0x414D5A4C;

        const string SERVER_BASEDIR = @"Z:/dev/www/vcs.codecreation.dev";

        private VcsProgramType vcsProgramType;
        private string vcsFilename = null;
        public DataReaderVcsBytes(string filenamepath, HandleOutputWrite outputWriter = null, bool showStatusMessage = false) :
            base(new MemoryStream(File.ReadAllBytes(filenamepath)), outputWriter)
        {
            this.vcsProgramType = ComputeVCSFileName(filenamepath).Item1;
            this.vcsFilename = filenamepath;
            this.showStatusMessage = showStatusMessage;
        }

        // if shortenOutput = true will limit the number of zframes shown to LIMIT_ZFRAMES
        // if shortenOutput = false will print all zframes (ignoring the LIMIT_ZFRAMES value)
        private bool shortenOutput = true;
        public void SetShortenOutput(bool shortenOutput)
        {
            this.shortenOutput = shortenOutput;
        }

        // prints a status message to console "parsing file XXX"
        private bool showStatusMessage;

        private uint zFrameCount = 0;
        const int LIMIT_ZFRAMES = 4;


        public void PrintByteDetail(string archiveName = null)
        {
            BaseStream.Position = 0;
            if (showStatusMessage)
            {
                if (archiveName != null)
                {
                    string reportString = $"parsing /{ archiveName}/{ Path.GetFileName(vcsFilename)}";
                    Console.Write($"{reportString,-100}");
                } else
                {
                    Console.Write($"parsing {vcsFilename}");
                }
            }

            int vcsVersion = -1;
            if (vcsProgramType == VcsProgramType.Features)
            {
                vcsVersion = PrintVcsFeaturesHeader();
            } else if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.PixelShader
                   || vcsProgramType == VcsProgramType.GeometryShader || vcsProgramType == VcsProgramType.PixelShaderRenderState
                   || vcsProgramType == VcsProgramType.ComputeShader || vcsProgramType == VcsProgramType.HullShader
                   || vcsProgramType == VcsProgramType.DomainShader || vcsProgramType == VcsProgramType.RaytracingShader)
            {
                vcsVersion = PrintVsPsHeader();
            } else
            {
                throw new ShaderParserException($"Unknown filetype: {vcsProgramType}");
            }

            if (vcsVersion != 65 && vcsVersion != 64 && vcsVersion != 62)
            {
                if (showStatusMessage)
                {
                    Console.WriteLine($"ERROR Unsupported vcs version {vcsVersion}");
                    return;
                } else
                {
                    throw new ShaderParserException($"ERROR Unsupported vcs version {vcsVersion}");
                }
            }



            uint blockDelim = ReadUInt32AtPosition();
            ShowByteCount();
            ShowBytes(4, $"block DELIM (values seen 14,17)");
            BreakLine();
            PrintAllSfBlocks();
            PrintAllCompatibilityBlocks();
            PrintAllDBlocks();
            PrintAllUknownBlocks();
            PrintAllParamBlocks(vcsVersion);
            PrintAllMipmapBlocks();
            PrintAllBufferBlocks();
            // for some reason only features and vs files observe symbol blocks
            if (vcsProgramType == VcsProgramType.Features || vcsProgramType == VcsProgramType.VertexShader)
            {
                PrintAllSymbolNameBlocks();
            }
            PrintZframes(vcsVersion);
            ShowEndOfFile();
            if (showStatusMessage)
            {
                Console.WriteLine($"OK");
            }
        }

        private int PrintVcsFeaturesHeader()
        {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            int vcsVersion = ReadInt32AtPosition();
            ShowBytes(4, $"version {vcsVersion}");
            if (vcsVersion != 65 && vcsVersion != 64 && vcsVersion != 62)
            {
                // unsupported version
                return vcsVersion;
            }
            BreakLine();
            ShowByteCount("features header");
            int has_psrs_file = 0;
            if (vcsVersion >= 64)
            {
                has_psrs_file = ReadInt32AtPosition();
                ShowBytes(4, "has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));
            }
            int unknown_val = ReadInt32AtPosition();
            ShowBytes(4, $"unknown_val = {unknown_val} (usually 0)");
            int len_name_description = ReadInt32AtPosition();
            ShowBytes(4, $"{len_name_description} len of name");
            BreakLine();
            string name_desc = ReadNullTermStringAtPosition();
            ShowByteCount(name_desc);
            ShowBytes(len_name_description + 1);
            BreakLine();
            ShowByteCount();
            uint arg0 = ReadUInt32AtPosition(0);
            uint arg1 = ReadUInt32AtPosition(4);
            uint arg2 = ReadUInt32AtPosition(8);
            uint arg3 = ReadUInt32AtPosition(12);
            ShowBytes(16, 4, breakLine: false);
            TabComment($"({arg0},{arg1},{arg2},{arg3})");
            uint arg4 = ReadUInt32AtPosition(0);
            uint arg5 = ReadUInt32AtPosition(4);
            uint arg6 = ReadUInt32AtPosition(8);
            if (vcsVersion >= 64)
            {
                uint arg7 = ReadUInt32AtPosition(12);
                ShowBytes(16, 4, breakLine: false);
                TabComment($"({arg4},{arg5},{arg6},{arg7})");
            } else
            {
                ShowBytes(12, 4, breakLine: false);
                TabComment($"({arg4},{arg5},{arg6})");
            }

            BreakLine();
            ShowByteCount();
            int nr_of_arguments = ReadInt32AtPosition();
            ShowBytes(4, $"nr of arguments {nr_of_arguments}");
            if (has_psrs_file == 1)
            {
                // nr_of_arguments becomes overwritten
                nr_of_arguments = ReadInt32AtPosition();
                ShowBytes(4, $"nr of arguments overriden ({nr_of_arguments})");
            }
            BreakLine();
            ShowByteCount();
            for (int i = 0; i < nr_of_arguments; i++)
            {
                string default_name = ReadNullTermStringAtPosition();
                Comment($"{default_name}");
                ShowBytes(128);
                uint has_s_argument = ReadUInt32AtPosition();
                ShowBytes(4);
                if (has_s_argument > 0)
                {
                    uint sSymbolArgValue = ReadUInt32AtPosition(64);
                    string sSymbolName = ReadNullTermStringAtPosition();
                    Comment($"{sSymbolName}");
                    ShowBytes(68);
                }
            }
            BreakLine();
            ShowByteCount("Editor/Shader stack for generating the file");
            ShowBytes(16, "Editor ref. ID0 (produces this file)");
            ShowBytes(16, breakLine: false);
            TabComment($"Editor ref. ID1 - usually a ref to the vs file ({VcsProgramType.VertexShader})");
            ShowBytes(16, breakLine: false);
            TabComment($"Editor ref. ID2 - usually a ref to the ps file ({VcsProgramType.PixelShader})");
            ShowBytes(16, "Editor ref. ID3");
            ShowBytes(16, "Editor ref. ID4");
            ShowBytes(16, "Editor ref. ID5");
            ShowBytes(16, "Editor ref. ID6");
            if (vcsVersion >= 64 && has_psrs_file == 0)
            {
                ShowBytes(16, "Editor ref. ID7 - common editor reference shared by multiple files");
            }
            if (vcsVersion >= 64 && has_psrs_file == 1)
            {
                ShowBytes(16, $"Editor ref. ID7 - reference to psrs file ({VcsProgramType.PixelShaderRenderState})");
                ShowBytes(16, "Editor ref. ID7 - common editor reference shared by multiple files");
            }
            BreakLine();
            return vcsVersion;
        }

        private int PrintVsPsHeader()
        {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            int vcsVersion = ReadInt32AtPosition();
            ShowBytes(4, $"version {vcsVersion}");
            if (vcsVersion != 65 && vcsVersion != 64 && vcsVersion != 62)
            {
                // unsupported version
                return vcsVersion;
            }
            BreakLine();
            ShowByteCount("ps/vs header");
            if (vcsVersion >= 64)
            {
                int has_psrs_file = ReadInt32AtPosition();
                ShowBytes(4, $"has_psrs_file = {(has_psrs_file > 0 ? "True" : "False")}");
            }
            BreakLine();
            ShowByteCount("Editor/Shader stack for generating the file");
            ShowBytes(16, "Editor ref. ID0 (produces this file)");
            ShowBytes(16, "Editor ref. ID1 - common editor reference shared by multiple files");
            return vcsVersion;
        }


        private void PrintAllSfBlocks()
        {
            ShowByteCount();
            uint sfBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{sfBlockCount} SF blocks (usually 152 bytes each)");
            BreakLine();
            for (int i = 0; i < sfBlockCount; i++)
            {
                PrintSfBlock();
            }
        }

        private void PrintSfBlock()
        {
            ShowByteCount();
            for (int i = 0; i < 2; i++)
            {
                string name1 = ReadNullTermStringAtPosition();
                if (name1.Length > 0)
                {
                    Comment($"{name1}");
                }
                ShowBytes(64);
            }
            int arg0 = ReadInt32AtPosition(0);
            int arg1 = ReadInt32AtPosition(4);
            int arg2 = ReadInt32AtPosition(8);
            int arg3 = ReadInt32AtPosition(12);
            int arg4 = ReadInt32AtPosition(16);
            int arg5 = ReadInt32AtPosition(20);
            ShowBytes(16, 4, breakLine: false);
            TabComment($"({arg0},{arg1},{arg2},{arg3})");
            ShowBytes(4, $"({arg4}) known values [-1,28]");
            ShowBytes(4, $"{arg5} additional string params");
            int string_offset = (int)BaseStream.Position;
            List<string> names = new();
            for (int i = 0; i < arg5; i++)
            {
                string paramname = ReadNullTermStringAtPosition(string_offset, rel: false);
                names.Add(paramname);
                string_offset += paramname.Length + 1;
            }
            if (names.Count > 0)
            {
                PrintStringList(names);
                ShowBytes(string_offset - (int)BaseStream.Position);
            }
            BreakLine();
        }

        private void PrintStringList(List<string> names)
        {
            if (names.Count == 0)
            {
                return;
            }
            OutputWrite($"// {names[0]}");
            for (int i = 1; i < names.Count; i++)
            {
                OutputWrite($", {names[i]}");
            }
            BreakLine();
        }

        private void PrintAllCompatibilityBlocks()
        {
            ShowByteCount();
            uint combatibilityBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{combatibilityBlockCount} compatibility blocks (472 bytes each)");
            BreakLine();
            for (int i = 0; i < combatibilityBlockCount; i++)
            {
                PrintCompatibilityBlock(i);
            }
        }

        private void PrintCompatibilityBlock(int compatBlockId)
        {
            ShowByteCount($"COMPAT-BLOCK[{compatBlockId}]");
            ShowBytes(216);
            string name1 = ReadNullTermStringAtPosition();
            OutputWriteLine($"[{BaseStream.Position}] {name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllDBlocks()
        {
            ShowByteCount();
            uint dBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{dBlockCount} D-blocks (152 bytes each)");
            BreakLine();
            for (int i = 0; i < dBlockCount; i++)
            {
                PrintDBlock(i);
            }
        }

        private void PrintDBlock(int dBlockId)
        {
            string dBlockName = ReadNullTermStringAtPosition();
            ShowByteCount($"DBLOCK[{dBlockId}]");
            Comment(dBlockName);
            ShowBytes(128);
            ShowBytes(12, 4);
            ShowBytes(12);
            BreakLine();
        }

        private void PrintAllUknownBlocks()
        {
            ShowByteCount();
            uint unknownBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{unknownBlockCount} unknown blocks (472 bytes each)");
            BreakLine();
            for (int i = 0; i < unknownBlockCount; i++)
            {
                PrintUnknownBlock(i);
            }
        }

        private void PrintUnknownBlock(int unknownBlockId)
        {
            ShowByteCount($"UNKBLOCK[{unknownBlockId}]");
            ShowBytes(472);
            BreakLine();
        }

        private void PrintAllParamBlocks(int vcsVersion)
        {
            ShowByteCount();
            uint paramBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            BreakLine();
            for (int i = 0; i < paramBlockCount; i++)
            {
                PrintParameterBlock(i, vcsVersion);
            }
        }

        private void PrintParameterBlock(int paramBlockId, int vcsVersion)
        {
            ShowByteCount($"PARAM-BLOCK[{paramBlockId}]");
            string name1 = ReadNullTermStringAtPosition();
            OutputWriteLine($"// {name1}");
            ShowBytes(64);
            string name2 = ReadNullTermStringAtPosition();
            if (name2.Length > 0)
            {
                OutputWriteLine($"// {name2}");
            }
            ShowBytes(64);
            ShowBytes(8);
            string name3 = ReadNullTermStringAtPosition();
            if (name3.Length > 0)
            {
                OutputWriteLine($"// {name3}");
            }
            ShowBytes(64);
            uint paramType = ReadUInt32AtPosition();
            OutputWriteLine($"// param-type, 6 or 7 lead dynamic-exp. Known values: 0,1,5,6,7,8,10,11,13");
            ShowBytes(4);
            if (paramType == 6 || paramType == 7)
            {
                int dynLength = ReadInt32AtPosition();
                ShowBytes(4, breakLine: false);
                TabComment("dyn-exp len", 1);
                TabComment("dynamic expression");
                ShowBytes(dynLength);
            }

            // check to see if this reads 'SBMS' (unknown what this is, instance found in v65 hero_pc_40_features.vcs file)
            byte[] checkSBMS = ReadBytesAtPosition(0, 4);
            if (checkSBMS[0] == 0x53 && checkSBMS[1] == 0x42 && checkSBMS[2] == 0x4D && checkSBMS[3] == 0x53)
            {
                ShowBytes(4, "SBMS");
                int dynLength = ReadInt32AtPosition();
                ShowBytes(4, "dyn-exp len");
                ShowBytes(dynLength, "dynamic expression", 1);
            }

            // 5 or 6 int arguments follow depending on version
            ShowBytes(20, 4);
            // v64,65 has an additional argument
            if (vcsVersion >= 64)
            {
                ShowBytes(4);
            }

            // a rarely seen file reference
            string name4 = ReadNullTermStringAtPosition();
            if (name4.Length > 0)
            {
                OutputWriteLine($"// {name4}");
            }
            ShowBytes(64);
            // float or int arguments
            int a0 = ReadInt32AtPosition(0);
            int a1 = ReadInt32AtPosition(4);
            int a2 = ReadInt32AtPosition(8);
            int a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            float f0 = ReadSingleAtPosition(0);
            float f1 = ReadSingleAtPosition(4);
            float f2 = ReadSingleAtPosition(8);
            float f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            f0 = ReadSingleAtPosition(0);
            f1 = ReadSingleAtPosition(4);
            f2 = ReadSingleAtPosition(8);
            f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            f0 = ReadSingleAtPosition(0);
            f1 = ReadSingleAtPosition(4);
            f2 = ReadSingleAtPosition(8);
            f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Fmt(f0)},{Fmt(f1)},{Fmt(f2)},{Fmt(f3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Fmt(a0)},{Fmt(a1)},{Fmt(a2)},{Fmt(a3)})", 10);
            // a command word, or pair of these
            string name5 = ReadNullTermStringAtPosition();
            if (name5.Length > 0)
            {
                OutputWriteLine($"// {name5}");
            }
            ShowBytes(32);
            string name6 = ReadNullTermStringAtPosition();
            if (name6.Length > 0)
            {
                OutputWriteLine($"// {name6}");
            }
            ShowBytes(32);

            if (vcsVersion == 65)
            {
                ShowBytes(6, "unknown bytes specific to vcs version 65");
            }

            BreakLine();
        }

        private string Fmt(float val)
        {
            if (val == -1e9) return "-inf";
            if (val == 1e9) return "inf";
            return $"{val}";
        }

        private string Format(int val)
        {
            if (val == -999999999) return "-inf";
            if (val == 999999999) return "inf";
            return "" + val; ;
        }

        private void PrintAllMipmapBlocks()
        {
            ShowByteCount();
            uint mipmapBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            BreakLine();
            for (int i = 0; i < mipmapBlockCount; i++)
            {
                PrintMipmapBlock(i);
            }
        }

        private void PrintMipmapBlock(int mipmapBlockId)
        {
            ShowByteCount($"MIPMAP-BLOCK[{mipmapBlockId}]");
            ShowBytes(24, 4);
            string name1 = ReadNullTermStringAtPosition();
            Comment($"{name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllBufferBlocks()
        {
            ShowByteCount();
            uint bufferBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{bufferBlockCount} Buffer blocks (variable length)");
            BreakLine();
            for (int i = 0; i < bufferBlockCount; i++)
            {
                PrintBufferBlock(i);
            }
        }

        private void PrintBufferBlock(int bufferBlockId)
        {
            string blockname = ReadNullTermStringAtPosition();
            ShowByteCount($"BUFFER-BLOCK[{bufferBlockId}] {blockname}");
            ShowBytes(64);
            uint bufferSize = ReadUInt32AtPosition();
            ShowBytes(4, $"{bufferSize} buffer-size");
            ShowBytes(4);
            uint paramCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{paramCount} param-count");
            for (int i = 0; i < paramCount; i++)
            {
                string paramname = ReadNullTermStringAtPosition();
                OutputWriteLine($"// {paramname}");
                ShowBytes(64);
                uint paramIndex = ReadUInt32AtPosition();
                ShowBytes(4, breakLine: false);
                TabComment($"{paramIndex} buffer-offset", 28);
                uint vertexSize = ReadUInt32AtPosition();
                uint attributeCount = ReadUInt32AtPosition(4);
                uint size = ReadUInt32AtPosition(8);
                ShowBytes(12, $"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, length)");
            }
            BreakLine();
            ShowBytes(4, "bufferID (some kind of crc/check)");
            BreakLine();
            BreakLine();
        }

        private void PrintAllSymbolNameBlocks()
        {
            ShowByteCount();
            uint symbolBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{symbolBlockCount} symbol/names blocks");
            for (int i = 0; i < symbolBlockCount; i++)
            {
                BreakLine();
                PrintSymbolNameBlock(i);
            }
            BreakLine();
        }

        private void PrintSymbolNameBlock(int symbolsBlockId)
        {
            ShowByteCount($"SYMBOL-NAMES-BLOCK[{symbolsBlockId}]");
            uint symbolGroupCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{symbolGroupCount} string groups in this block");
            for (int i = 0; i < symbolGroupCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string symbolname = ReadNullTermStringAtPosition();
                    OutputWriteLine($"// {symbolname}");
                    ShowBytes(symbolname.Length + 1);
                }
                ShowBytes(4);
                BreakLine();
            }
            if (symbolGroupCount == 0) BreakLine();
        }


        private const int SKIP_ZFRAMES_IF_MORE_THAN = 10;
        private const int MAX_ZFRAME_BYTES_TO_SHOW = 96;

        private void PrintZframes(int vcsVersion)
        {
            // the zFrameIds and zFrameDataOffsets are listed first, before the data sections
            // (And during normal operation an application is not expected to parse beyond the zframe id and offset
            // listings - as data retrieval is expected to be performed as-needed during runtime)
            //
            // if `shortenOutput = false` is passed all zframes and data-sections will be parsed regardless of length.
            // if `shortenOutput = true` the parser returns either directly or shortly after the listings; if there are only a
            // few zframes (<= SKIP_ZFRAMES_IF_MORE_THAN) they are shown regardless.
            //
            //
            List<uint> zFrameIds = new();
            List<long> zFrameDataOffsets = new();

            ShowByteCount();
            zFrameCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{zFrameCount} zframes");
            BreakLine();
            if (zFrameCount == 0)
            {
                return;
            }
            ShowByteCount("zFrame IDs");
            for (int i = 0; i < zFrameCount; i++)
            {
                uint zframeId = ReadUInt32AtPosition();
                ShowBytes(8, breakLine: false);
                TabComment($"zframe[0x{zframeId:x08}]    {Convert.ToString(zframeId, 2).PadLeft(20, '0')} (bin.)");
                zFrameIds.Add(zframeId);
            }

            BreakLine();
            ShowByteCount("zFrame file offsets");
            foreach (uint zframeId in zFrameIds)
            {
                uint zframe_offset = ReadUInt32AtPosition();
                zFrameDataOffsets.Add(zframe_offset);
                ShowBytes(4, $"{zframe_offset} offset of zframe[0x{zframeId:x08}]");
            }
            uint endOfFilePointer = ReadUInt32AtPosition();
            ShowBytes(4, $"{endOfFilePointer} - end of file");
            BreakLine();

            if (shortenOutput && zFrameCount > SKIP_ZFRAMES_IF_MORE_THAN)
            {
                BaseStream.Position = endOfFilePointer;
                return;
            }

            for (int i = 0; i < zFrameIds.Count; i++)
            {
                BaseStream.Position = zFrameDataOffsets[i];
                PrintCompressedZFrame(zFrameIds[i]);
            }
            // in v62 the last zframe doesn't always end at end of file (to pass the end-of-file check must be explicitly set)
            if (vcsVersion == 62)
            {
                BaseStream.Position = endOfFilePointer;
            }
        }


        public void PrintCompressedZFrame(uint zframeId)
        {
            OutputWriteLine($"[{BaseStream.Position}] zframe[0x{zframeId:x08}]");
            bool isLzma = false;
            uint zstdDelimOrChunkSize = ReadUInt32AtPosition();
            if (zstdDelimOrChunkSize == ZSTD_DELIM)
            {
                ShowBytes(4, $"Zstd delim (0x{ZSTD_DELIM:x08})");
            } else
            {
                ShowBytes(4, $"Chunk size {zstdDelimOrChunkSize}");
                uint lzmaDelim = ReadUInt32AtPosition();
                if (lzmaDelim != LZMA_DELIM)
                {
                    Comment($"neither ZStd or Lzma found (frame appears to be uncompressed)");
                    ShowBytes((int)zstdDelimOrChunkSize);
                    BreakLine();
                    return;
                }
                isLzma = true;
                ShowBytes(4, $"Lzma delim (0x{LZMA_DELIM:x08})");
            }
            int uncompressed_length = ReadInt32AtPosition();
            ShowBytes(4, $"{uncompressed_length,-8} uncompressed length");
            int compressed_length = ReadInt32AtPosition();
            ShowBytes(4, $"{compressed_length,-8} compressed length");
            if (isLzma)
            {
                ShowBytes(5, "Decoder properties");
            }
            ShowBytesAtPosition(0, compressed_length > MAX_ZFRAME_BYTES_TO_SHOW ? MAX_ZFRAME_BYTES_TO_SHOW : compressed_length);
            if (compressed_length > MAX_ZFRAME_BYTES_TO_SHOW)
            {
                Comment($"... ({compressed_length - MAX_ZFRAME_BYTES_TO_SHOW} bytes not shown)");
            }
            BaseStream.Position += compressed_length;
            BreakLine();
        }




    }
}




