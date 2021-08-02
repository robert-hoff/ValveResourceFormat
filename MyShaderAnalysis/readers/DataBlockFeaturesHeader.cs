using System.Collections.Generic;
using System.Diagnostics;


namespace MyShaderAnalysis.readers {


    public class DataBlockFeaturesHeader : DataReader {

        bool has_psrs_file;
        int unknown_val;
        string file_description;


        public DataBlockFeaturesHeader(byte[] data) : base(data) {
            int magic = ReadInt();
            if (magic != 0x32736376) {
                throw new ShaderParserException($"wrong file id {magic:x}");
            }
            int version = ReadInt();
            if (version != 64) {
                throw new ShaderParserException($"wrong version {version}, expecting 64");
            }
            int psrs_arg = ReadInt();
            if (psrs_arg != 0 && psrs_arg != 1) {
                throw new ShaderParserException($"unexpected value psrs_arg = {psrs_arg}");
            }
            has_psrs_file = psrs_arg > 0;
            unknown_val = ReadInt();
            ReadInt(); // length of name, but not needed because it's always null-term
            file_description = ReadNullTermStringAtPosition();


            Debug.WriteLine($"{file_description}");

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



    }





}





