using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            List<int> zframeIndexes = datareader.SearchForByteSequence(new byte[] { 0x28, 0xb5, 0x2f, 0xfd });

            byte[] headerbytes = null;


            if (zframeIndexes.Count > 0) {
                headerbytes = datareader.ReadBytes(zframeIndexes[0] - 12);
            } else {
                headerbytes = datareader.databytes;
            }


            //if (zframeIndexes.Count > 0) {
            //    headerData = new DataPart(datareader.databytes, 0, zframeIndexes[0] - 12);
            //} else {
            //    headerData = new DataPart(datareader.databytes);
            //}


            // datareader.ShowAllBytes();
            datareader.ShowBytes(1962, 484);

        }



    }



}




