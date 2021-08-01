using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.readers.ShaderFile;

namespace MyShaderAnalysis.readers {

    public class DataReaderVcsByteAnalysis : DataReader {


        public DataReaderVcsByteAnalysis(byte[] data, FILETYPE fileType) : base(data) {

            if (fileType == FILETYPE.features_file) {
                PrintVcsFeaturesHeader();
            }
            else if (fileType == FILETYPE.vs_file || fileType == FILETYPE.ps_file
                || fileType == FILETYPE.gs_file || fileType == FILETYPE.psrs_file) {
                PrintVsPsHeader();
            }
            else {
                throw new ShaderParserException($"can't parse this filetype: {fileType}");
            }





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
            TabComment("has_psrs_file = "+(has_psrs_file>0?"True":"False"));
            ShowBytes(4, false);
            int unknown_val = ReadIntAtPosition();
            TabComment($"unknown_val = {unknown_val} (usually 0");
            int len_name_description = ReadIntAtPosition();
            ShowBytes(4, false);
            TabComment($"{len_name_description} len of name");
            BreakLine();

            string name_desc = ReadNullTermStringAtPosition();
            // Comment(name_desc);
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
            BreakLine();

            ShowByteCount();
        }


        private void PrintVsPsHeader() {

            Debug.WriteLine("Vs/Ps file header here");
        }



    }



}







