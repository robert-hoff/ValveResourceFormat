using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.readers.ShaderFileByteAnalysis;

namespace MyShaderAnalysis.readers {

    public class DataReaderVcsByteAnalysis : DataReader02 {
        FILETYPE filetype;

        public DataReaderVcsByteAnalysis(byte[] data, FILETYPE filetype) : base(data) {
            this.filetype = filetype;

        }

        public void ParseFile() {
            BreakLine();
            if (filetype == FILETYPE.features_file) {
                PrintVcsFeaturesHeader();
            } else if (filetype == FILETYPE.vs_file || filetype == FILETYPE.ps_file
                  || filetype == FILETYPE.gs_file || filetype == FILETYPE.psrs_file) {
                PrintVsPsHeader();
            } else {
                throw new ShaderParserException($"can't parse this filetype: {filetype}");
            }
            uint blockDelim = ReadUIntAtPosition();
            if (blockDelim != 17) {
                throw new ShaderParserException($"unexpected block delim value! {blockDelim}");
            }
            ShowByteCount();
            ShowBytes(4, false);
            TabComment($"block DELIM always 17");
            BreakLine();
            PrintAllSfBlocks();
            PrintAllCompatibilityBlocks();
            PrintAllDBlocks();
            PrintAllUknownBlocks();
            PrintAllParamBlocks();
            PrintAllMipmapBlocks();
            PrintAllBufferBlocks();
            // for some reason only features and vs files observe symbol blocks
            if (filetype == FILETYPE.features_file || filetype == FILETYPE.vs_file) {
                PrintAllSymbolNameBlocks();
            }
            PrintZframes();
            EndOfFile();
        }

        private void PrintVcsFeaturesHeader() {
            ShowByteCount("vcs file");
            ShowBytes(4, false);
            TabComment("\"vcs2\"");
            ShowBytes(4, false);
            TabComment("version 64");
            BreakLine();
            ShowByteCount("features header");
            int has_psrs_file = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment("has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));
            int unknown_val = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"unknown_val = {unknown_val} (usually 0)");
            int len_name_description = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{len_name_description} len of name");
            BreakLine();

            string name_desc = ReadNullTermStringAtPosition();
            ShowByteCount(name_desc);
            ShowBytes(len_name_description + 1);
            BreakLine();

            ShowByteCount();
            uint arg1 = ReadUIntAtPosition(offset);
            uint arg2 = ReadUIntAtPosition(offset + 4);
            uint arg3 = ReadUIntAtPosition(offset + 8);
            uint arg4 = ReadUIntAtPosition(offset + 12);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4, false);
            TabComment($"({arg1},{arg2},{arg3},{arg4})");
            uint arg5 = ReadUIntAtPosition(offset);
            uint arg6 = ReadUIntAtPosition(offset + 4);
            uint arg7 = ReadUIntAtPosition(offset + 8);
            uint arg8 = ReadUIntAtPosition(offset + 12);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4, false);
            TabComment($"({arg5},{arg6},{arg7},{arg8})");
            BreakLine();

            ShowByteCount();
            int nr_of_arguments = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"nr of arguments {nr_of_arguments}");
            if (has_psrs_file == 1) {
                // NOTE nr_of_arguments is overwritten
                nr_of_arguments = ReadIntAtPosition(offset);
                ShowBytes(4, false);
                TabComment($"nr of arguments overriden ({nr_of_arguments})");
            }
            BreakLine();

            ShowByteCount();
            for (int i = 0; i < nr_of_arguments; i++) {
                string default_name = ReadNullTermStringAtPosition(offset);

                Comment($"{default_name}");
                ShowBytes(128);
                uint has_s_argument = ReadUIntAtPosition(offset);
                ShowBytes(4);

                if (has_s_argument > 0) {
                    uint sSymbolArgValue = ReadUIntAtPosition(offset + 64);
                    string sSymbolName = ReadNullTermStringAtPosition(offset);
                    Comment($"{sSymbolName}");
                    ShowBytes(68);
                }
            }

            BreakLine();
            ShowByteCount("File IDs");
            ShowBytes(16, false);
            Comment("file ID0");
            ShowBytes(16, false);
            Comment("file ID1 - ref to vs file");
            ShowBytes(16, false);
            Comment("file ID2 - ref to ps file");
            ShowBytes(16, false);
            Comment("file ID3");
            ShowBytes(16, false);
            Comment("file ID4");
            ShowBytes(16, false);
            Comment("file ID5");
            ShowBytes(16, false);
            Comment("file ID6");
            if (has_psrs_file == 0) {
                ShowBytes(16, false);
                Comment("file ID7 - shared by all Dota2 vcs files");
            }
            if (has_psrs_file == 1) {
                ShowBytes(16, false);
                Comment("file ID7 - reference to psrs file");
                ShowBytes(16, false);
                Comment("file ID8 - shared by all Dota2 vcs files");
            }
            BreakLine();
        }

        private void PrintVsPsHeader() {
            ShowByteCount("vcs file");
            ShowBytes(4, false);
            TabComment("\"vcs2\"");
            ShowBytes(4, false);
            TabComment("version 64");
            BreakLine();

            ShowByteCount("ps/vs header");
            int has_psrs_file = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment("has_psrs_file = " + (has_psrs_file > 0 ? "True" : "False"));

            ShowBytes(16, false);
            TabComment("file ID0");
            ShowBytes(16, false);
            TabComment("file ID1 - shared by all Dota2 vcs files");
            BreakLine();
        }

        private void PrintAllSfBlocks() {
            ShowByteCount();
            uint sfBlockCount = ReadUIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{sfBlockCount} SF blocks (usually 152 bytes each)");
            BreakLine();
            for (int i = 0; i < sfBlockCount; i++) {
                PrintSfBlock();
            }
        }
        private void PrintSfBlock() {
            ShowByteCount();
            for (int i = 0; i < 2; i++) {
                string name1 = ReadNullTermStringAtPosition(offset);
                if (name1.Length > 0) {
                    Comment($"{name1}");
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
            ShowBytes(4, false);
            TabComment($"({arg0},{arg1},{arg2},{arg3})");
            ShowBytes(4, false);
            TabComment($"({arg4}) known values [-1,28]");
            ShowBytes(4, false);
            TabComment($"{arg5} additional string params");
            int string_offset = offset;
            List<string> names = new();
            for (int i = 0; i < arg5; i++) {
                string paramname = ReadNullTermStringAtPosition(string_offset);
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
            ShowBytes(4, false);
            TabComment($"{combatibilityBlockCount} compatibility blocks (472 bytes each)");
            BreakLine();
            for (int i = 0; i < combatibilityBlockCount; i++) {
                PrintCompatibilityBlock(i);
            }
        }

        private void PrintCompatibilityBlock(int compatBlockId) {
            ShowByteCount($"COMPAT-BLOCK[{compatBlockId}]");
            ShowBytes(216);
            string name1 = ReadNullTermStringAtPosition(offset);
            OutputWriteLine($"[{offset}] {name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllDBlocks() {
            ShowByteCount();
            uint dBlockCount = ReadUIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{dBlockCount} D-blocks (152 bytes each)");
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
            ShowBytes(4, false);
            TabComment($"{unknownBlockCount} unknown blocks (472 bytes each)");
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
            ShowBytes(4, false);
            TabComment($"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            BreakLine();
            for (int i = 0; i < paramBlockCount; i++) {
                PrintParameterBlock(i);
            }
        }

        private void PrintParameterBlock(int paramBlockId) {
            ShowByteCount($"PARAM-BLOCK[{paramBlockId}]");
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
                ShowBytes(4, false);
                TabComment("dyn-exp len", 1);

                ShowBytes(dynLength, false);
                TabComment("dynamic expression", 1);
            }

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
            ShowBytes(16, false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);

            float f0 = ReadFloatAtPosition(offset);
            float f1 = ReadFloatAtPosition(offset + 4);
            float f2 = ReadFloatAtPosition(offset + 8);
            float f3 = ReadFloatAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);

            f0 = ReadFloatAtPosition(offset);
            f1 = ReadFloatAtPosition(offset + 4);
            f2 = ReadFloatAtPosition(offset + 8);
            f3 = ReadFloatAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);

            f0 = ReadFloatAtPosition(offset);
            f1 = ReadFloatAtPosition(offset + 4);
            f2 = ReadFloatAtPosition(offset + 8);
            f3 = ReadFloatAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"floats ({Format(f0)},{Format(f1)},{Format(f2)},{Format(f3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);

            a0 = ReadIntAtPosition(offset);
            a1 = ReadIntAtPosition(offset + 4);
            a2 = ReadIntAtPosition(offset + 8);
            a3 = ReadIntAtPosition(offset + 12);
            ShowBytes(16, false);
            TabComment($"ints   ({Format(a0)},{Format(a1)},{Format(a2)},{Format(a3)})", 10);

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
            ShowBytes(4, false);
            TabComment($"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            BreakLine();
            for (int i = 0; i < mipmapBlockCount; i++) {
                PrintMipmapBlock(i);
            }
        }

        private void PrintMipmapBlock(int mipmapBlockId) {
            ShowByteCount($"MIPMAP-BLOCK[{mipmapBlockId}]");
            ShowBytes(24, 4);
            string name1 = ReadNullTermStringAtPosition(offset);
            Comment($"{name1}");
            ShowBytes(256);
            BreakLine();
        }

        private void PrintAllBufferBlocks() {
            ShowByteCount();
            uint bufferBlockCount = ReadUIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{bufferBlockCount} Buffer blocks (variable length)");
            BreakLine();
            for (int i = 0; i < bufferBlockCount; i++) {
                PrintBufferBlock(i);
            }
        }

        private void PrintBufferBlock(int bufferBlockId) {
            string blockname = ReadNullTermStringAtPosition(offset);
            ShowByteCount($"BUFFER-BLOCK[{bufferBlockId}] {blockname}");
            ShowBytes(64);
            uint bufferSize = ReadUIntAtPosition(offset);
            ShowBytes(4, false);
            TabComment($"{bufferSize} buffer-size");
            ShowBytes(4);
            uint paramCount = ReadUIntAtPosition(offset);
            ShowBytes(4, false);
            TabComment($"{paramCount} param-count");
            for (int i = 0; i < paramCount; i++) {
                string paramname = ReadNullTermStringAtPosition(offset);
                OutputWriteLine($"// {paramname}");
                ShowBytes(64);
                uint paramIndex = ReadUIntAtPosition(offset);
                ShowBytes(4, false);
                TabComment($"{paramIndex} buffer-offset", 28);
                uint vertexSize = ReadUIntAtPosition(offset);
                uint attributeCount = ReadUIntAtPosition(offset + 4);
                uint size = ReadUIntAtPosition(offset + 8);
                ShowBytes(12, false);
                TabComment($"({vertexSize},{attributeCount},{size}) (vertex-size, attribute-count, size)");
            }
            BreakLine();
            ShowBytes(4, false);
            TabComment("blockID (some kind of crc/check)");
            BreakLine();
            BreakLine();
        }

        private void PrintAllSymbolNameBlocks() {
            ShowByteCount();
            uint symbolBlockCount = ReadUIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{symbolBlockCount} symbol/names blocks");
            for (int i = 0; i < symbolBlockCount; i++) {
                BreakLine();
                PrintSymbolNameBlock(i);
            }
        }

        private void PrintSymbolNameBlock(int symbolsBlockId) {
            ShowByteCount($"SYMBOL-NAMES-BLOCK[{symbolsBlockId}]");
            uint symbolGroupCount = ReadUIntAtPosition(offset);
            ShowBytes(4, false);
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

        private void PrintZframes() {
            ShowByteCount();
            uint zframe_count = ReadUIntAtPosition(offset);
            ShowBytes(4, false);
            TabComment($"{zframe_count} zframes");
            BreakLine();
            if (zframe_count == 0) {
                return;
            }
            List<uint> zFrameIndexes = new();
            ShowByteCount("zFrame IDs");
            for (int i = 0; i < zframe_count; i++) {
                uint zframe_index = ReadUIntAtPosition(offset);
                ShowBytes(8, false);
                TabComment($"zframe[{zframe_index}]");
                zFrameIndexes.Add(zframe_index);
            }
            OutputWriteLine("");
            ShowByteCount("zFrame file offsets");
            foreach (int zframeIndex in zFrameIndexes) {
                uint zframe_offset = ReadUIntAtPosition(offset);
                ShowBytes(4, false);
                TabComment($"{zframe_offset,-10} offset of zframe[{zframeIndex}]");
            }

            uint total_size = ReadUIntAtPosition(offset);
            ShowBytes(4, false);
            TabComment($"{total_size} - end of file");
            OutputWriteLine("");
            foreach (int zframeIndex in zFrameIndexes) {
                PrintCompressedZFrame(zframeIndex);
            }
        }

        public void PrintCompressedZFrame(int zframeId) {
            OutputWriteLine($"[{offset}] zframe[{zframeId}]");
            ShowBytes(4, false);
            TabComment("DELIM (0xfffffffd)");
            int uncompressed_length = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{uncompressed_length,-8} uncompressed length");
            // TabPrintComment(uncompressed_length.ToString().PadRight(8));
            int compressed_length = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{compressed_length,-8} compressed length");
            ShowBytesAtPosition(offset, compressed_length>96 ? 96 : compressed_length);
            if (compressed_length > 96) {
                Comment("...");
            }
            offset += compressed_length;
            BreakLine();
        }

        private void EndOfFile() {
            if (offset != databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
            ShowByteCount();
            OutputWriteLine("EOF");
        }
    }
}









