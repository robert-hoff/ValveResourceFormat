using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers {

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

}





