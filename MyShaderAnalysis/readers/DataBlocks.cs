using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers {


    public class DataBlockVsPsHeader : DataReader {
        public DataBlockVsPsHeader(byte[] data, int start) : base(data, start) {

        }
    }



    public class CompatibilityBlock : DataReader {
        public CompatibilityBlock(byte[] data, int start) : base(data, start) {

        }
    }

    public class DBlock : DataReader {
        public DBlock(byte[] data, int start) : base(data, start) {

        }
    }

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


    public class ZFrameBlock : DataReader {
        public ZFrameBlock(byte[] data, int start) : base(data, start) {

        }
    }


}
