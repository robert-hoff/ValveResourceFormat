using System.Collections.Generic;
using System.Diagnostics;


namespace MyShaderAnalysis.readers {


    public class DataBlockFeaturesHeader : DataReader {

        bool has_psrs_file;
        int unknown_val;
        string file_description;

        // not sure what these are yet
        int arg0;
        int arg1;
        int arg2;
        int arg3;
        int arg4;
        int arg5;
        int arg6;
        int arg7;

        List<(string, string)> mainParams = new();
        List<string> fileIDs = new();


        public DataBlockFeaturesHeader(byte[] data, int start) : base(data, start) {
            int psrs_arg = ReadInt();
            if (psrs_arg != 0 && psrs_arg != 1) {
                throw new ShaderParserException($"unexpected value psrs_arg = {psrs_arg}");
            }
            has_psrs_file = psrs_arg > 0;
            unknown_val = ReadInt();
            ReadInt(); // length of name, but not needed because it's always null-term
            file_description = ReadNullTermString();

            arg0 = ReadInt();
            arg1 = ReadInt();
            arg2 = ReadInt();
            arg3 = ReadInt();
            arg4 = ReadInt();
            arg5 = ReadInt();
            arg6 = ReadInt();
            arg7 = ReadInt();

            int nr_of_arguments = ReadInt();
             // NOTE nr_of_arguments is overwritten
            if (has_psrs_file) {
                nr_of_arguments = ReadInt();
            }

            for (int i = 0; i < nr_of_arguments; i++) {
                string string_arg0 = ReadNullTermStringAtPosition();
                string string_arg1 = null;
                offset += 128;
                if (ReadInt() > 0) {
                    string_arg1 = ReadNullTermStringAtPosition();
                    offset += 68;
                }
                mainParams.Add((string_arg0, string_arg0));
            }

            for (int i = 0; i < 8; i++) {
                fileIDs.Add(ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
            if (has_psrs_file) {
                fileIDs.Add(ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
        }


    }





}





