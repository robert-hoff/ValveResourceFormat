using System.Collections.Generic;

/*
 * most of these need implementation, the complete parser is implemented in DataReaderVcsByteAnalysis.cs
 * but DataReaderVcsByteAnalysis only prints the data and doesn't make any attempt to capture it
 *
 */
namespace MyShaderAnalysis.vcsparsing {

    public class DataBlockFeaturesHeader : DataReader {

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


        public void PrintSummary() {
            OutputWriteLine("features-header summary");
        }


    }




    // needs implemenation
    public class DataBlockVsPsHeader : DataReader {
        public DataBlockVsPsHeader(byte[] data, int start) : base(data, start) {
        }
    }

    public class DataBlockSfBlock : DataReader {
        string name0;
        string name1;
        int arg0;
        int arg1;
        int arg2;
        int arg3;
        int arg4;
        List<string> additionalParams = new();

        public DataBlockSfBlock(byte[] data, int start) : base(data, start) {
            name0 = ReadNullTermStringAtPosition();
            offset += 64;
            name1 = ReadNullTermStringAtPosition();
            offset += 64;
            arg0 = ReadInt();
            arg1 = ReadInt();
            arg2 = ReadInt();
            arg3 = ReadInt();
            arg4 = ReadInt();
            int additionalStringsCount = ReadInt();
            for (int i = 0; i < additionalStringsCount; i++) {
                additionalParams.Add(ReadNullTermString());
            }
        }
    }

    /*
     * needs implemenation
     * All CompatibilityBlocks are 472, the current parser works by instantiating
     * the blocks and incrementing the offste by 472. (i.e. no data is retrieved)
     *
     */
    public class CompatibilityBlock : DataReader {
        public CompatibilityBlock(byte[] data, int start) : base(data, start) {

        }
    }

    // needs implemenation (parser works by moving the offset 152 bytes for each d-block)
    public class DBlock : DataReader {
        public DBlock(byte[] data, int start) : base(data, start) {

        }
    }

    // needs implemenation (parser works by moving the offset 472 bytes for each unknown-block)
    public class UnknownBlock : DataReader {
        public UnknownBlock(byte[] data, int start) : base(data, start) {

        }
    }

    public class ParamBlock : DataReader {
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

        public ParamBlock(byte[] data, int start) : base(data, start) {
            name0 = ReadNullTermStringAtPosition();
            offset += 64;
            name1 = ReadNullTermStringAtPosition();
            offset += 64;
            lead0 = ReadInt();
            lead1 = ReadFloat();
            name2 = ReadNullTermStringAtPosition();
            offset += 64;
            paramType = ReadInt();
            if (paramType == 6 || paramType == 7) {
                int dynExpLen = ReadInt();
                dynExp = ReadBytes(dynExpLen);
            }
            arg0 = ReadInt();
            arg1 = ReadInt();
            arg2 = ReadInt();
            arg3 = ReadInt();
            arg4 = ReadInt();
            arg5 = ReadInt();

            fileref = ReadNullTermStringAtPosition();
            offset += 64;
            for (int i = 0; i < 4; i++) {
                ranges0[i] = ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges1[i] = ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges2[i] = ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges3[i] = ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges4[i] = ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges5[i] = ReadFloat();
            }
            for (int i = 0; i < 4; i++) {
                ranges6[i] = ReadInt();
            }
            for (int i = 0; i < 4; i++) {
                ranges7[i] = ReadInt();
            }
            command0 = ReadNullTermStringAtPosition();
            offset += 32;
            command1 = ReadNullTermStringAtPosition();
            offset += 32;
        }
    }

    // needs implemenation (parser works by moving the offset 280 bytes for each mipmap-block)
    public class MipmapBlock : DataReader {
        public MipmapBlock(byte[] data, int start) : base(data, start) {

        }
    }

    public class BufferBlock : DataReader {
        string name;
        int bufferSize;
        List<(string, int, int, int, int)> bufferParams = new();
        uint blockId;

        public BufferBlock(byte[] data, int start) : base(data, start) {
            name = ReadNullTermStringAtPosition();
            offset += 64;
            bufferSize = ReadInt();
            offset += 4; // next 4 bytes are always 0
            int paramCount = ReadInt();
            for (int i = 0; i < paramCount; i++) {
                string paramName = ReadNullTermStringAtPosition();
                offset += 64;
                int bufferIndex = ReadInt();
                int arg0 = ReadInt();
                int arg1 = ReadInt();
                int arg2 = ReadInt();
                bufferParams.Add((paramName, bufferIndex, arg0, arg1, arg2));
            }
            blockId = ReadUInt();
        }
    }

    public class SymbolsBlock : DataReader {
        List<(string, string, string, int)> symbolParams = new();

        public SymbolsBlock(byte[] data, int start) : base(data, start) {
            int namesCount = ReadInt();
            for (int i = 0; i < namesCount; i++) {
                string name0 = ReadNullTermString();
                string name1 = ReadNullTermString();
                string name2 = ReadNullTermString();
                int int0 = ReadInt();
                symbolParams.Add((name0, name1, name2, int0));
            }
        }
    }




}









