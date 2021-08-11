using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;



namespace MyShaderAnalysis.vcsparsing {



    public class ZDataBlock : DataBlock {

        public int h0;
        public int h1;
        public int h2;
        public byte[] dataload;

        public ZDataBlock(DataReader datareader, int start) : base(datareader, start) {
            h0 = datareader.ReadInt();
            h1 = datareader.ReadInt();
            h2 = datareader.ReadInt();
            dataload = datareader.ReadBytes(h0 * 4);



        }


        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }




}






