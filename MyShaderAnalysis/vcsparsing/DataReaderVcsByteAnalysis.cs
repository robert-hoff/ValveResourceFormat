using System;
using System.Collections.Generic;
using System.IO;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;



namespace MyShaderAnalysis.vcsparsing {

    public class DataReaderVcsByteAnalysis : DataReader {

        const string SERVER_BASEDIR = @"Z:\dev\www\vcs.codecreation.dev";

        private VcsFileType filetype;
        private string vcsFilename = null;
        public DataReaderVcsByteAnalysis(string filenamepath) : base(File.ReadAllBytes(filenamepath)) {
            this.filetype = GetVcsFileType(filenamepath);
            this.vcsFilename = filenamepath;
        }

        private bool writeHtmlLinks = false;
        public void SetWriteHtmlLinks(bool writeHtmlLinks) {
            this.writeHtmlLinks = writeHtmlLinks;
        }

        private bool shortenOutput = true;
        public void SetShortenOutput(bool shortenOutput) {
            this.shortenOutput = shortenOutput;
        }


        private uint zFrameCount = 0;
        const int LIMIT_ZFRAMES = 4;


        public void PrintByteAnalysis() {
            if (filetype == VcsFileType.Features) {
                PrintVcsFeaturesHeader();
            } else if (filetype == VcsFileType.VertexShader || filetype == VcsFileType.PixelShader
                  || filetype == VcsFileType.GeometryShader || filetype == VcsFileType.PotentialShadowReciever) {
                PrintVsPsHeader();
            } else {
                throw new ShaderParserException($"can't parse this filetype: {filetype}");
            }
            uint blockDelim = ReadUIntAtPosition();
            if (blockDelim != 17) {
                throw new ShaderParserException($"unexpected block delim value! {blockDelim}");
            }
            ShowByteCount();
            ShowBytes(4, $"block DELIM always 17");
            BreakLine();
            PrintAllSfBlocks();
            PrintAllCompatibilityBlocks();
            PrintAllDBlocks();
            PrintAllUknownBlocks();
            PrintAllParamBlocks();
            PrintAllMipmapBlocks();
            PrintAllBufferBlocks();
            // for some reason only features and vs files observe symbol blocks
            if (filetype == VcsFileType.Features || filetype == VcsFileType.VertexShader) {
                PrintAllSymbolNameBlocks();
            }
            PrintZframes();
            if (shortenOutput && zFrameCount > LIMIT_ZFRAMES) {
                return;
            }
            EndOfFile();
        }

        private void PrintVcsFeaturesHeader() {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            ShowBytes(4, "version 64");
            BreakLine();
            ShowByteCount("features header");
            int has_psrs_file = ReadIntAtPosition();
            ShowBytes(4, "has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));
            int unknown_val = ReadIntAtPosition();
            ShowBytes(4, $"unknown_val = {unknown_val} (usually 0)");
            int len_name_description = ReadIntAtPosition();
            ShowBytes(4, $"{len_name_description} len of name");
            BreakLine();
            string name_desc = ReadNullTermStringAtPosition();
            ShowByteCount(name_desc);
            ShowBytes(len_name_description + 1);
            BreakLine();
            ShowByteCount();
            uint arg1 = ReadUIntAtPosition(0);
            uint arg2 = ReadUIntAtPosition(4);
            uint arg3 = ReadUIntAtPosition(8);
            uint arg4 = ReadUIntAtPosition(12);
            ShowBytes(16, 4, breakLine: false);
            TabComment($"({arg1},{arg2},{arg3},{arg4})");
            uint arg5 = ReadUIntAtPosition(0);
            uint arg6 = ReadUIntAtPosition(4);
            uint arg7 = ReadUIntAtPosition(8);
            uint arg8 = ReadUIntAtPosition(12);
            ShowBytes(16, 4, breakLine: false);
            TabComment($"({arg5},{arg6},{arg7},{arg8})");
            BreakLine();
            ShowByteCount();
            int nr_of_arguments = ReadIntAtPosition();
            ShowBytes(4, $"nr of arguments {nr_of_arguments}");
            if (has_psrs_file == 1) {
                // NOTE nr_of_arguments is overwritten
                nr_of_arguments = ReadIntAtPosition();
                ShowBytes(4, $"nr of arguments overriden ({nr_of_arguments})");
            }
            BreakLine();
            ShowByteCount();
            for (int i = 0; i < nr_of_arguments; i++) {
                string default_name = ReadNullTermStringAtPosition();
                Comment($"{default_name}");
                ShowBytes(128);
                uint has_s_argument = ReadUIntAtPosition();
                ShowBytes(4);
                if (has_s_argument > 0) {
                    uint sSymbolArgValue = ReadUIntAtPosition(64);
                    string sSymbolName = ReadNullTermStringAtPosition();
                    Comment($"{sSymbolName}");
                    ShowBytes(68);
                }
            }
            BreakLine();
            ShowByteCount("File IDs");
            ShowBytes(16, "file ID0");
            if (writeHtmlLinks) {
                OutputWrite($"{GetVsHtmlLink(vcsFilename, ReadBytesAsString(16))}");
            } else {
                ShowBytes(16, breakLine: false);
            }
            TabComment("file ID1 - ref to vs file");
            if (writeHtmlLinks) {
                OutputWrite($"{GetPsHtmlLink(vcsFilename, ReadBytesAsString(16))}");
            } else {
                ShowBytes(16, breakLine: false);
            }
            TabComment("file ID2 - ref to ps file");
            ShowBytes(16, "file ID3");
            ShowBytes(16, "file ID4");
            ShowBytes(16, "file ID5");
            ShowBytes(16, "file ID6");
            if (has_psrs_file == 0) {
                ShowBytes(16, "file ID7 - shared by all Dota2 vcs files");
            }
            if (has_psrs_file == 1) {
                ShowBytes(16, "file ID7 - reference to psrs file");
                ShowBytes(16, "file ID8 - shared by all Dota2 vcs files");
            }
            BreakLine();
        }

        private void PrintVsPsHeader() {
            ShowByteCount("vcs file");
            ShowBytes(4, "\"vcs2\"");
            ShowBytes(4, "version 64");
            BreakLine();
            ShowByteCount("ps/vs header");
            int has_psrs_file = ReadIntAtPosition();
            ShowBytes(4, $"has_psrs_file = {(has_psrs_file > 0 ? "True" : "False")}");
            ShowBytes(16, "file ID0");
            ShowBytes(16, "file ID1 - shared by all Valve v64 vcs files");
            BreakLine();
        }

        private void PrintAllSfBlocks() {
            ShowByteCount();
            uint sfBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{sfBlockCount} SF blocks (usually 152 bytes each)");
            BreakLine();
            for (int i = 0; i < sfBlockCount; i++) {
                PrintSfBlock();
            }
        }

        private void PrintSfBlock() {
            ShowByteCount();
            for (int i = 0; i < 2; i++) {
                string name1 = ReadNullTermStringAtPosition();
                if (name1.Length > 0) {
                    Comment($"{name1}");
                }
                ShowBytes(64);
            }
            int arg0 = ReadIntAtPosition(0);
            int arg1 = ReadIntAtPosition(4);
            int arg2 = ReadIntAtPosition(8);
            int arg3 = ReadIntAtPosition(12);
            int arg4 = ReadIntAtPosition(16);
            int arg5 = ReadIntAtPosition(20);
            ShowBytes(16, 4, breakLine: false);
            TabComment($"({arg0},{arg1},{arg2},{arg3})");
            ShowBytes(4, $"({arg4}) known values [-1,28]");
            ShowBytes(4, $"{arg5} additional string params");
            int string_offset = offset;
            List<string> names = new();
            for (int i = 0; i < arg5; i++) {
                string paramname = ReadNullTermStringAtPosition(string_offset, rel: false);
                names.Add(paramname);
                string_offset += paramname.Length + 1;
            }
            if (names.Count > 0) {
                PrintStringList(names);
                ShowBytes(string_offset - offset);
            }
            BreakLine();
        }

        private void PrintStringList(List<string> names) {
            if (names.Count == 0) {
                return;
            }
            OutputWrite($"// {names[0]}");
            for (int i = 1; i < names.Count; i++) {
                OutputWrite($", {names[i]}");
            }
            BreakLine();
        }

        private void PrintAllCompatibilityBlocks() {
            ShowByteCount();
            uint combatibilityBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{combatibilityBlockCount} compatibility blocks (472 bytes each)");
            BreakLine();
            for (int i = 0; i < combatibilityBlockCount; i++) {
                PrintCompatibilityBlock(i);
            }
        }

        private void PrintCompatibilityBlock(int compatBlockId) {
            ShowByteCount($"COMPAT-BLOCK[{compatBlockId}]");
            ShowBytes(216);
            string name1 = ReadNullTermStringAtPosition();
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllDBlocks() {
            ShowByteCount();
            uint dBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{dBlockCount} D-blocks (152 bytes each)");
            BreakLine();
            for (int i = 0; i < dBlockCount; i++) {
                PrintDBlock(i);
            }
        }

        private void PrintDBlock(int dBlockId) {
            string dBlockName = ReadNullTermStringAtPosition();
            ShowByteCount($"DBLOCK[{dBlockId}]");
            Comment(dBlockName);
            ShowBytes(128);
            ShowBytes(12, 4);
            ShowBytes(12);
            BreakLine();
        }

        private void PrintAllUknownBlocks() {
            ShowByteCount();
            uint unknownBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{unknownBlockCount} unknown blocks (472 bytes each)");
            BreakLine();
            for (int i = 0; i < unknownBlockCount; i++) {
                PrintUnknownBlock(i);
            }
        }

        private void PrintUnknownBlock(int unknownBlockId) {
            ShowByteCount($"UNKBLOCK[{unknownBlockId}]");
            ShowBytes(472);
            BreakLine();
        }

        private void PrintAllParamBlocks() {
            ShowByteCount();
            uint paramBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            BreakLine();
            for (int i = 0; i < paramBlockCount; i++) {
                PrintParameterBlock(i);
            }
        }

        private void PrintParameterBlock(int paramBlockId) {
            ShowByteCount($"PARAM-BLOCK[{paramBlockId}]");
            string name1 = ReadNullTermStringAtPosition();
            OutputWriteLine($"// {name1}");
            ShowBytes(64);
            string name2 = ReadNullTermStringAtPosition();
            if (name2.Length > 0) {
                OutputWriteLine($"// {name2}");
            }
            ShowBytes(64);
            ShowBytes(8);
            string name3 = ReadNullTermStringAtPosition();
            if (name3.Length > 0) {
                OutputWriteLine($"// {name3}");
            }
            ShowBytes(64);
            uint paramType = ReadUIntAtPosition();
            OutputWriteLine($"// param-type, 6 or 7 lead dynamic-exp. Known values: 0,1,5,6,7,8,10,11,13");
            ShowBytes(4);
            if (paramType == 6 || paramType == 7) {
                int dynLength = ReadIntAtPosition();
                ShowBytes(4, breakLine: false);
                TabComment("dyn-exp len", 1);

                ShowBytes(dynLength, breakLine: false);
                TabComment("dynamic expression", 1);
            }
            // 6 int parameters follow the dynamic expression
            ShowBytes(24, 4);
            // a rarely seen file reference
            string name4 = ReadNullTermStringAtPosition();
            if (name4.Length > 0) {
                OutputWriteLine($"// {name4}");
            }
            ShowBytes(64);
            // float or int arguments
            int a0 = ReadIntAtPosition(0);
            int a1 = ReadIntAtPosition(4);
            int a2 = ReadIntAtPosition(8);
            int a3 = ReadIntAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadIntAtPosition(0);
            a1 = ReadIntAtPosition(4);
            a2 = ReadIntAtPosition(8);
            a3 = ReadIntAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadIntAtPosition(0);
            a1 = ReadIntAtPosition(4);
            a2 = ReadIntAtPosition(8);
            a3 = ReadIntAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            float f0 = ReadFloatAtPosition(0);
            float f1 = ReadFloatAtPosition(4);
            float f2 = ReadFloatAtPosition(8);
            float f3 = ReadFloatAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            f0 = ReadFloatAtPosition(0);
            f1 = ReadFloatAtPosition(4);
            f2 = ReadFloatAtPosition(8);
            f3 = ReadFloatAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            f0 = ReadFloatAtPosition(0);
            f1 = ReadFloatAtPosition(4);
            f2 = ReadFloatAtPosition(8);
            f3 = ReadFloatAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);
            a0 = ReadIntAtPosition(0);
            a1 = ReadIntAtPosition(4);
            a2 = ReadIntAtPosition(8);
            a3 = ReadIntAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            a0 = ReadIntAtPosition(0);
            a1 = ReadIntAtPosition(4);
            a2 = ReadIntAtPosition(8);
            a3 = ReadIntAtPosition(12);
            ShowBytes(16, breakLine: false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);
            // a command word, or pair of these
            string name5 = ReadNullTermStringAtPosition();
            if (name5.Length > 0) {
                OutputWriteLine($"// {name5}");
            }
            ShowBytes(32);
            string name6 = ReadNullTermStringAtPosition();
            if (name6.Length > 0) {
                OutputWriteLine($"// {name6}");
            }
            ShowBytes(32);
            BreakLine();
        }

        private string Format(float val) {
            if (val == -1e9) return "-inf";
            if (val == 1e9) return "inf";
            return $"{val}";
        }

        private string Format(int val) {
            if (val == -999999999) return "-inf";
            if (val == 999999999) return "inf";
            return "" + val; ;
        }

        private void PrintAllMipmapBlocks() {
            ShowByteCount();
            uint mipmapBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            BreakLine();
            for (int i = 0; i < mipmapBlockCount; i++) {
                PrintMipmapBlock(i);
            }
        }

        private void PrintMipmapBlock(int mipmapBlockId) {
            ShowByteCount($"MIPMAP-BLOCK[{mipmapBlockId}]");
            ShowBytes(24, 4);
            string name1 = ReadNullTermStringAtPosition();
            Comment($"{name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllBufferBlocks() {
            ShowByteCount();
            uint bufferBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{bufferBlockCount} Buffer blocks (variable length)");
            BreakLine();
            for (int i = 0; i < bufferBlockCount; i++) {
                PrintBufferBlock(i);
            }
        }

        private void PrintBufferBlock(int bufferBlockId) {
            string blockname = ReadNullTermStringAtPosition();
            ShowByteCount($"BUFFER-BLOCK[{bufferBlockId}] {blockname}");
            ShowBytes(64);
            uint bufferSize = ReadUIntAtPosition();
            ShowBytes(4, $"{bufferSize} buffer-size");
            ShowBytes(4);
            uint paramCount = ReadUIntAtPosition();
            ShowBytes(4, $"{paramCount} param-count");
            for (int i = 0; i < paramCount; i++) {
                string paramname = ReadNullTermStringAtPosition();
                OutputWriteLine($"// {paramname}");
                ShowBytes(64);
                uint paramIndex = ReadUIntAtPosition();
                ShowBytes(4, breakLine: false);
                TabComment($"{paramIndex} buffer-offset", 28);
                uint vertexSize = ReadUIntAtPosition();
                uint attributeCount = ReadUIntAtPosition(4);
                uint size = ReadUIntAtPosition(8);
                ShowBytes(12, $"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, length)");
            }
            BreakLine();
            ShowBytes(4, "bufferID (some kind of crc/check)");
            BreakLine();
            BreakLine();
        }

        private void PrintAllSymbolNameBlocks() {
            ShowByteCount();
            uint symbolBlockCount = ReadUIntAtPosition();
            ShowBytes(4, $"{symbolBlockCount} symbol/names blocks");
            for (int i = 0; i < symbolBlockCount; i++) {
                BreakLine();
                PrintSymbolNameBlock(i);
            }
            BreakLine();
        }

        private void PrintSymbolNameBlock(int symbolsBlockId) {
            ShowByteCount($"SYMBOL-NAMES-BLOCK[{symbolsBlockId}]");
            uint symbolGroupCount = ReadUIntAtPosition();
            ShowBytes(4, $"{symbolGroupCount} string groups in this block");
            for (int i = 0; i < symbolGroupCount; i++) {
                for (int j = 0; j < 3; j++) {
                    string symbolname = ReadNullTermStringAtPosition();
                    OutputWriteLine($"// {symbolname}");
                    ShowBytes(symbolname.Length + 1);
                }
                ShowBytes(4);
                BreakLine();
            }
            if (symbolGroupCount == 0) BreakLine();
        }

        private void PrintZframes() {
            ShowByteCount();
            zFrameCount = ReadUIntAtPosition();
            ShowBytes(4, $"{zFrameCount} zframes");
            BreakLine();
            if (zFrameCount == 0) {
                return;
            }
            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame IDs");
            for (int i = 0; i < zFrameCount; i++) {
                uint zframeId = ReadUIntAtPosition();
                ShowBytes(8, breakLine: false);
                TabComment($"{getZFrameIdString(zframeId)}    {Convert.ToString(zframeId, 2).PadLeft(20, '0')}");
                zFrameIndexes.Add(zframeId);
            }
            BreakLine();
            if (shortenOutput && zFrameCount > LIMIT_ZFRAMES) {
                Comment("rest of data contains compressed zframes");
                BreakLine();
                return;
            }
            ShowByteCount("zFrame file offsets");
            foreach (uint zframeId in zFrameIndexes) {
                uint zframe_offset = ReadUIntAtPosition();
                ShowBytes(4, $"{zframe_offset} offset of {getZFrameIdString(zframeId)}");
            }
            uint total_size = ReadUIntAtPosition();
            ShowBytes(4, $"{total_size} - end of file");
            OutputWriteLine("");
            foreach (uint zframeId in zFrameIndexes) {
                PrintCompressedZFrame(zframeId);
            }
        }


        int LIMIT_ZFRAME_DATA_SHOWN = 96;
        // int LIMIT_ZFRAME_DATA_SHOWN = 4000;


        public void PrintCompressedZFrame(uint zframeId) {
            OutputWriteLine($"[{offset}] {getZFrameIdString(zframeId)}");
            bool isLzma = false;
            uint zstdDelimOrChunkSize = ReadUIntAtPosition();
            if (zstdDelimOrChunkSize == ShaderFile.ZSTD_DELIM) {
                ShowBytes(4, $"Zstd delim (0x{ShaderFile.ZSTD_DELIM:x08})");
            } else {
                ShowBytes(4, $"Lzma chunk size {zstdDelimOrChunkSize}");
                uint lzmaDelim = ReadUIntAtPosition();
                if (lzmaDelim != ShaderFile.LZMA_DELIM) {
                    throw new ShaderParserException("Unknown compression, neither ZStd nor Lzma found");
                }
                isLzma = true;
                ShowBytes(4, $"Lzma delim (0x{ShaderFile.LZMA_DELIM:x08})");
            }
            int uncompressed_length = ReadIntAtPosition();
            ShowBytes(4, $"{uncompressed_length,-8} uncompressed length");
            int compressed_length = ReadIntAtPosition();
            ShowBytes(4, $"{compressed_length,-8} compressed length");

            if (isLzma) {
                ShowBytes(5, "Decoder properties");
            }
            ShowBytesAtPosition(0, compressed_length > LIMIT_ZFRAME_DATA_SHOWN ? LIMIT_ZFRAME_DATA_SHOWN : compressed_length);
            if (compressed_length > LIMIT_ZFRAME_DATA_SHOWN) {
                Comment($"... ({compressed_length - LIMIT_ZFRAME_DATA_SHOWN} bytes not shown)");
            }
            offset += compressed_length;
            BreakLine();
        }

        private string getZFrameIdString(uint zframeId) {
            if (writeHtmlLinks) {
                // return GetZframeHtmlLink(zframeId, vcsFilename);
                string serverdir = SERVER_BASEDIR;
                string basedir = $"/vcs-all/{GetCoreOrDotaString(vcsFilename)}/zsource/";

                return GetZframeHtmlLinkCheckExists(zframeId, vcsFilename, serverdir, basedir);
            } else {
                return $"zframe[0x{zframeId:x08}]";
            }
        }



    }
}




