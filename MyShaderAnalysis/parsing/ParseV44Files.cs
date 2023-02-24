using System;
using System.Collections.Generic;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

/*
 * Almost identical to DataReaderVcsBytes - all relevant changes were written into DataReaderVcsBytes since
 *
 *
 */
#pragma warning disable IDE0033 // Use explicitly provided tuple name
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1823 // Avoid unused private fields
namespace MyShaderAnalysis.parsing
{
    class ParseV44Files : ShaderDataReader
    {
        private VcsProgramType vcsProgramType;
        private VcsPlatformType vcsSourceType;
        private VcsShaderModelType vcsModelType;

        public ParseV44Files(string filenamepath) : base(new MemoryStream(File.ReadAllBytes(filenamepath)))
        {
            vcsProgramType = ComputeVCSFileName(filenamepath).Item1;
            vcsFilename = filenamepath;

            if (vcsProgramType == VcsProgramType.Features)
            {
                PrintVcsFeaturesHeader();
            }
            else if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.PixelShader
                   || vcsProgramType == VcsProgramType.GeometryShader || vcsProgramType == VcsProgramType.PixelShaderRenderState
                   || vcsProgramType == VcsProgramType.ComputeShader || vcsProgramType == VcsProgramType.HullShader
                   || vcsProgramType == VcsProgramType.DomainShader || vcsProgramType == VcsProgramType.RaytracingShader)
            {
                version = PrintVsPsHeader();
            }
            else
            {
                throw new ShaderParserException($"can't parse this filetype: {vcsProgramType}");
            }

            ShowByteCount();
            ShowBytes(4, $"block DELIM always 17, or minor version? (but stayed the same for v65 and v64)");
            BreakLine();

            PrintAllSfBlocks();
            PrintAllCompatibilityBlocks();
            PrintAllDBlocks();
            PrintAllUknownBlocks();
            PrintAllParamBlocks();

            // return;

            PrintAllMipmapBlocks();
            PrintAllBufferBlocks();

            // for some reason only features and vs files observe symbol blocks
            if (vcsProgramType == VcsProgramType.Features || vcsProgramType == VcsProgramType.VertexShader)
            {
                PrintAllSymbolNameBlocks();
            }
            PrintZframes();
            if (shortenOutput && zFrameCount > LIMIT_ZFRAMES)
            {
                return;
            }
            ShowEndOfFile();
        }

        // private bool shortenOutput = true;
        private bool shortenOutput = false;

        public void SetShortenOutput(bool shortenOutput)
        {
            this.shortenOutput = shortenOutput;
        }

        private bool writeHtmlLinks = false;
        private string vcsFilename = null;

        private uint zFrameCount = 0;
        const int LIMIT_ZFRAMES = 4;
        private int version = -1;

        private void PrintVcsFeaturesHeader()
        {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            version = ReadInt32AtPosition();
            ShowBytes(4, $"version {version}");
            BreakLine();
            ShowByteCount("features header");
            int has_psrs_file = 0;
            if (version == 64 || version == 65)
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
            if (version == 64 || version == 65)
            {
                uint arg7 = ReadUInt32AtPosition(12);
                ShowBytes(16, 4, breakLine: false);
                TabComment($"({arg4},{arg5},{arg6},{arg7})");
            }
            else
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
                // NOTE nr_of_arguments is overwritten
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
            ShowByteCount("File IDs");
            ShowBytes(16, "file ID0");
            if (writeHtmlLinks)
            {
                OutputWrite($"{GetVsHtmlLink(vcsFilename, ReadBytesAsString(16))}");
            }
            else
            {
                ShowBytes(16, breakLine: false);
            }
            TabComment("file ID1 - ref to vs file");
            if (writeHtmlLinks)
            {
                OutputWrite($"{GetPsHtmlLink(vcsFilename, ReadBytesAsString(16))}");
            }
            else
            {
                ShowBytes(16, breakLine: false);
            }
            TabComment("file ID2 - ref to ps file");
            ShowBytes(16, "file ID3");
            ShowBytes(16, "file ID4");
            ShowBytes(16, "file ID5");
            ShowBytes(16, "file ID6");
            if ((version == 64 || version == 65) && has_psrs_file == 0)
            {
                ShowBytes(16, "file ID7 - shared by all Dota2 vcs files");
            }
            if ((version == 64 || version == 65) && has_psrs_file == 1)
            {
                ShowBytes(16, "file ID7 - reference to psrs file");
                ShowBytes(16, "file ID8 - shared by all Dota2 vcs files");
            }
            BreakLine();
        }

        private int PrintVsPsHeader()
        {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            int version = ReadInt32AtPosition();
            ShowBytes(4, $"version {version}");
            BreakLine();
            ShowByteCount("ps/vs header");
            if (version == 64 || version == 65)
            {
                int has_psrs_file = ReadInt32AtPosition();
                ShowBytes(4, $"has_psrs_file = {(has_psrs_file > 0 ? "True" : "False")}");
            }
            ShowBytes(16, "file ID0");
            ShowBytes(16, "file ID1 - shared by all Valve v64 vcs files");
            BreakLine();
            return version;
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
            int string_offset = (int) BaseStream.Position;
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
                ShowBytes(string_offset - (int) BaseStream.Position);
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

        private void PrintAllParamBlocks()
        {
            ShowByteCount();
            uint paramBlockCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            BreakLine();
            for (int i = 0; i < paramBlockCount; i++)
            {
                PrintParameterBlock(i);

                //if (i == 81)
                //{
                //    break;
                //}
            }
        }

        private void PrintParameterBlock(int paramBlockId)
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
                ShowBytes(4, "dyn-exp len");
                ShowBytes(dynLength, "dynamic expression", 1);
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
            if (version >= 64)
            {
                ShowBytes(4);
            }
            if (version >= 44)
            {
                ShowBytes(16, 4);
            }

            // a rarely seen file reference
            // took this out for v44, the parameter section seems to break at the right point
            // but there is some unusual data following

            //string name4 = ReadNullTermStringAtPosition();
            //if (name4.Length > 0)
            //{
            //    OutputWriteLine($"// {name4}");
            //}
            //ShowBytes(64);

            // float or int arguments
            int a0 = ReadInt32AtPosition(0);
            int a1 = ReadInt32AtPosition(4);
            int a2 = ReadInt32AtPosition(8);
            int a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            float f0 = ReadSingleAtPosition(0);
            float f1 = ReadSingleAtPosition(4);
            float f2 = ReadSingleAtPosition(8);
            float f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            f0 = ReadSingleAtPosition(0);
            f1 = ReadSingleAtPosition(4);
            f2 = ReadSingleAtPosition(8);
            f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            f0 = ReadSingleAtPosition(0);
            f1 = ReadSingleAtPosition(4);
            f2 = ReadSingleAtPosition(8);
            f3 = ReadSingleAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadInt32AtPosition(0);
            a1 = ReadInt32AtPosition(4);
            a2 = ReadInt32AtPosition(8);
            a3 = ReadInt32AtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
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

            if (version == 65)
            {
                ShowBytes(6);
            }

            BreakLine();
        }

        private string Format(float val)
        {
            if (val == -1e9) { return "-inf"; }
            if (val == 1e9) { return "inf"; }
            return $"{val}";
        }

        private string Format(int val)
        {
            if (val == -999999999) { return "-inf"; }
            if (val == 999999999) { return "inf"; }
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
            if (symbolGroupCount == 0) { BreakLine(); }
        }

        private void PrintZframes()
        {
            ShowByteCount();
            zFrameCount = ReadUInt32AtPosition();
            ShowBytes(4, $"{zFrameCount} zframes");
            BreakLine();
            if (zFrameCount == 0)
            {
                return;
            }
            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame IDs");
            for (int i = 0; i < zFrameCount; i++)
            {
                uint zframeId = ReadUInt32AtPosition();
                ShowBytes(8, breakLine: false);
                TabComment($"{GetZFrameIdString(zframeId)}    {Convert.ToString(zframeId, 2).PadLeft(20, '0')}");
                zFrameIndexes.Add(zframeId);
            }
            BreakLine();
            if (shortenOutput && zFrameCount > LIMIT_ZFRAMES)
            {
                Comment("rest of data contains compressed zframes");
                BreakLine();
                return;
            }
            ShowByteCount("zFrame file offsets");
            foreach (uint zframeId in zFrameIndexes)
            {
                uint zframe_offset = ReadUInt32AtPosition();
                ShowBytes(4, $"{zframe_offset} offset of {GetZFrameIdString(zframeId)}");
            }
            uint total_size = ReadUInt32AtPosition();
            ShowBytes(4, $"{total_size} - end of file");
            OutputWriteLine("");
            foreach (uint zframeId in zFrameIndexes)
            {
                PrintCompressedZFrame(zframeId);
            }
        }

        int MAX_ZFRAME_BYTES_SHOWN = 96;

        public void PrintCompressedZFrame(uint zframeId)
        {
            OutputWriteLine($"[{BaseStream.Position}] {GetZFrameIdString(zframeId)}");
            bool isLzma = false;
            uint zstdDelimOrChunkSize = ReadUInt32AtPosition();
            if (zstdDelimOrChunkSize == ShaderFile.ZSTD_DELIM)
            {
                ShowBytes(4, $"Zstd delim (0x{ShaderFile.ZSTD_DELIM:x08})");
            }
            else
            {
                ShowBytes(4, $"Lzma chunk size {zstdDelimOrChunkSize}");
                uint lzmaDelim = ReadUInt32AtPosition();
                if (lzmaDelim != ShaderFile.LZMA_DELIM)
                {
                    // throw new ShaderParserException("Unknown compression, neither ZStd nor Lzma found");
                    Comment($"neither zstd or lzma found");
                    ShowBytes((int) zstdDelimOrChunkSize);
                    return;
                }
                isLzma = true;
                ShowBytes(4, $"Lzma delim (0x{ShaderFile.LZMA_DELIM:x08})");
            }
            int uncompressed_length = ReadInt32AtPosition();
            ShowBytes(4, $"{uncompressed_length,-8} uncompressed length");
            int compressed_length = ReadInt32AtPosition();
            ShowBytes(4, $"{compressed_length,-8} compressed length");
            if (isLzma)
            {
                ShowBytes(5, "Decoder properties");
            }
            ShowBytesAtPosition(0, compressed_length > MAX_ZFRAME_BYTES_SHOWN ? MAX_ZFRAME_BYTES_SHOWN : compressed_length);
            if (compressed_length > MAX_ZFRAME_BYTES_SHOWN)
            {
                Comment($"... ({compressed_length - MAX_ZFRAME_BYTES_SHOWN} bytes not shown)");
            }
            BaseStream.Position += compressed_length;
            BreakLine();
        }

        private string GetZFrameIdString(uint zframeId)
        {
            if (writeHtmlLinks)
            {
                // return GetZframeHtmlLink(zframeId, vcsFilename);
                string serverdir = SERVER_BASEDIR;
                string basedir = $"/vcs-all/{GetCoreOrDotaString(vcsFilename)}/zsource/";

                return GetZframeHtmlLinkCheckExists(zframeId, vcsFilename, serverdir, basedir);
            }
            else
            {
                return $"zframe[0x{zframeId:x08}]";
            }
        }
    }
}

