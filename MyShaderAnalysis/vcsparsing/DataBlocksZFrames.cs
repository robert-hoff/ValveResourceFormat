using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;



namespace MyShaderAnalysis.vcsparsing {



    public class ZDataBlock : DataBlock {

        public int blockId;
        public int h0;
        public int h1;
        public int h2;
        public byte[] dataload = null;

        public ZDataBlock(DataReader datareader, int start, int blockId) : base(datareader, start) {
            this.blockId = blockId;
            h0 = datareader.ReadInt();
            h1 = datareader.ReadInt();
            h2 = datareader.ReadInt();
            if (h0 > 0) {
                dataload = datareader.ReadBytes(h0 * 4);
            }
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }
    }



    public class GlslSource : DataBlock {

        public int sourceId;
        public int offset0;
        public int arg0 = -1;
        public int offset1 = -1;
        public byte[] sourcebytes = null;
        public byte[] fileId;

        public GlslSource(DataReader datareader, int start, int sourceId) : base(datareader, start) {
            this.sourceId = sourceId;
            offset0 = datareader.ReadInt();
            if (offset0 > 0) {
                arg0 = datareader.ReadInt();
                offset1 = datareader.ReadInt();
                sourcebytes = datareader.ReadBytes(offset1);
            }
            fileId = datareader.ReadBytes(16);
        }

        public string GetStringId() {
            string stringId = DataReader.BytesToString(fileId);
            stringId = stringId.Replace(" ", "").ToLower();

            return stringId;
        }

        public override void PrintByteSummary() {
            throw new NotImplementedException();
        }

    }




}






