using System;
using System.Collections.Generic;


namespace TestVRF.vcsparsing {


    public class DataBlockFeaturesHeader : DataBlock {

        bool has_psrs_file;
        int unknown_val;
        string file_description;
        // not sure what these are
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

        public DataBlockFeaturesHeader(DataReader datareader, int start) : base(datareader, start) {
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
                string string_arg1 = null;
                datareader.offset += 128;
                if (datareader.ReadInt() > 0) {
                    string_arg1 = datareader.ReadNullTermStringAtPosition();
                    datareader.offset += 68;
                }
                mainParams.Add((string_arg0, string_arg0));
            }

            for (int i = 0; i < 8; i++) {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
            if (has_psrs_file) {
                fileIDs.Add(datareader.ReadBytesAsString(16).Replace(" ", "").ToLower());
            }
        }

        public override void PrintByteSummary() {
            datareader.OutputWriteLine("features-header summary");
            throw new NotImplementedException();
        }
    }




    // needs implemenation
    public class DataBlockVsPsHeader : DataBlock {
        public DataBlockVsPsHeader(DataReader datareader, int start) : base(datareader, start) {
            datareader.offset += 36;
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class DataBlockSfBlock : DataBlock {
        string name0;
        string name1;
        int arg0;
        int arg1;
        int arg2;
        int arg3;
        int arg4;
        List<string> additionalParams = new();

        public DataBlockSfBlock(DataReader datareader, int start) : base(datareader, start) {
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            arg0 = datareader.ReadInt();
            arg1 = datareader.ReadInt();
            arg2 = datareader.ReadInt();
            arg3 = datareader.ReadInt();
            arg4 = datareader.ReadInt();
            int additionalStringsCount = datareader.ReadInt();
            for (int i = 0; i < additionalStringsCount; i++) {
                additionalParams.Add(datareader.ReadNullTermString());
            }
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class CompatibilityBlock : DataBlock {
        public CompatibilityBlock(DataReader datareader, int start) : base(datareader, start) {

        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    // needs implemenation (parser works by moving the offset 152 bytes for each d-block)
    public class DBlock : DataBlock {
        public DBlock(DataReader datareader, int start) : base(datareader, start) {

        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    // needs implemenation (parser works by moving the offset 472 bytes for each unknown-block)
    public class UnknownBlock : DataBlock {
        public UnknownBlock(DataReader datareader, int start) : base(datareader, start) {

        }
        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class ParamBlock : DataBlock {
        string name0;
        string name1;
        string name2;
        int lead0;
        float lead1;
        int paramType;
        byte[] dynExp = null;
        int arg0;
        int arg1;
        int arg2;
        int arg3;
        int arg4;
        int arg5;
        string fileref;
        int[] ranges0 = new int[4];
        int[] ranges1 = new int[4];
        int[] ranges2 = new int[4];
        float[] ranges3 = new float[4];
        float[] ranges4 = new float[4];
        float[] ranges5 = new float[4];
        int[] ranges6 = new int[4];
        int[] ranges7 = new int[4];
        string command0;
        string command1;

        public ParamBlock(DataReader datareader, int start) : base(datareader, start) {
            name0 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            name1 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            lead0 = datareader.ReadInt();
            lead1 = datareader.ReadFloat();
            name2 = datareader.ReadNullTermStringAtPosition();
            datareader.offset += 64;
            paramType = datareader.ReadInt();
            if (paramType == 6 || paramType == 7) {
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
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    // needs implemenation (parser works by moving the offset 280 bytes for each mipmap-block)
    public class MipmapBlock : DataBlock {
        public MipmapBlock(DataReader datareader, int start) : base(datareader, start) {

        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }


    public class BufferBlock : DataBlock {
        string name;
        int bufferSize;
        List<(string, int, int, int, int)> bufferParams = new();
        uint blockId;

        public BufferBlock(DataReader datareader, int start) : base(datareader, start) {
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


    public class SymbolsBlock : DataBlock {
        List<(string, string, string, int)> symbolParams = new();

        public SymbolsBlock(DataReader datareader, int start) : base(datareader, start) {
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








