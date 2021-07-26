using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace MyShaderAnalysis.readers {

    class ShaderFile {



        /*
         *
         *
         * FOR FILE
         *                  glow_output_pcgl_30_ps.vcs
         *
         *
         *
         * right before the first zframe, we hace
         * 94 08 00 00        2196        this is the offset of the first compressed zframe
         *                                the zframe follows 12 header bytes
         *                                This is EQUIVALENT to the size of the header
         * 32 0B 00 00        2866        this is the offset of the second compressed zframe
         *                                which also follows 12 header bytes
         * E3 10 00 00        4323        this is the size of the file
         *
         *
         *
         *
         *
         *
         *
         *
         *
         *
         */
        public ShaderFile(string filepath) {

            DataReader datareader = new(File.ReadAllBytes(filepath));
            //List<int> zframeIndexes = datareader.SearchForByteSequence(new byte[] { 0x28, 0xb5, 0x2f, 0xfd });
            //byte[] headerbytes = null;
            //if (zframeIndexes.Count > 0) {
            //    headerbytes = datareader.ReadBytes(zframeIndexes[0] - 12);
            //} else {
            //    headerbytes = datareader.databytes;
            //}


            //if (zframeIndexes.Count > 0) {
            //    headerData = new DataPart(datareader.databytes, 0, zframeIndexes[0] - 12);
            //} else {
            //    headerData = new DataPart(datareader.databytes);
            //}


            // datareader.ShowAllBytes();

            // datareader.PrintForSBlock(52);

            //datareader.offset = 5412;
            //datareader.PrintCompressedZFrame();
            //datareader.PrintCompressedZFrame();
            //datareader.PrintCompressedZFrame();
            //datareader.PrintCompressedZFrame();
            //datareader.EndOfFile();

            //for (int i = 0; i < 9; i++) {
            //    datareader.Print484Block();
            //}


            //datareader.offset = 5352;
            //datareader.ShowBytes(40, 4);
            //datareader.ShowOffset();
            //datareader.ShowIntValue();
            //datareader.ShowIntValue();
            //datareader.ShowIntValue();
            //datareader.ShowIntValue();
            //datareader.ShowIntValue();

            // datareader.Print484Block();
            //datareader.PrintMipmapBlock();
            //datareader.PrintMipmapBlock();
            //datareader.ShowOffset();
            //datareader.ShowBytes(100, 4);




            // datareader.ShowBytesAtPosition(4788, 5000);



            // for (int i = 0; i < 9; i++) {
            // datareader.ShowBytesAtPosition(7165 + i * 280, 280);
            // datareader.Print484Block(372+i*484);
            // }





            //datareader.PrintVcsFileHeader();
            //datareader.PrintPcVsHeader();



            //datareader.offset = 52;
            //for (int i = 0; i < 10; i++) {
            //    datareader.PrintForSBlock();
            //}

            //datareader.offset = 1576;
            //for (int i = 0; i < 6; i++) {
            //    datareader.PrintCompatibilitiesBlock();
            //}


            //datareader.offset = 4412;
            //for (int i = 0; i < 7; i++) {
            //    datareader.PrintDBlock();
            //}


            //datareader.offset = 5480;
            //for (int i = 0; i < 7; i++) {
            //    datareader.PrintUnknownBlockType1();
            //}

            //datareader.offset = 8788;
            //for (int i = 0; i < 40; i++) {
            //    datareader.PrintParamAssignmentBlock();
            //}



            //datareader.offset = 28220;
            //datareader.ShowByteCount($"{datareader.readNullTermStringAtPosition(datareader.offset)}");
            //datareader.ShowBytes(64);
            //for (int i = 0; i < 3; i++) {
            //    uint val = datareader.ReadUIntAtPosition(datareader.offset);
            //    datareader.ShowBytesNoLineBreak(4);
            //    datareader.TabPrintComment($"{val}");
            //}
            //Debug.WriteLine("");
            //for (int i = 0; i < 81; i++) {
            //    string name1 = datareader.readNullTermStringAtPosition(datareader.offset);
            //    datareader.ShowByteCount($"{name1}");
            //    datareader.ShowBytes(64);
            //    for (int j = 0; j < 4; j++) {
            //        uint val = datareader.ReadUIntAtPosition(datareader.offset);
            //        datareader.ShowBytesNoLineBreak(4);
            //        datareader.TabPrintComment($"{val}");
            //    }
            //}


            //datareader.offset = 34780;
            //for (int i = 0; i < 10; i++) {
            //    datareader.ShowByteCount();
            //    datareader.ShowBytes(500);
            //}



            datareader.offset = 37244;
            datareader.parseZFramesSection();






            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            // datareader.ShowBytes(200);
            datareader.ShowBytesAtPosition(datareader.offset, 200);
            Debug.WriteLine("");

        }



    }



}




