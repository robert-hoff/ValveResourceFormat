using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MyShaderAnalysis.readers01 {

    public class ShaderReader01 {

        const string ANALYSIS_DIR = @"X:\checkouts\ValveResourceFormat\files_under_analysis";
        byte[] zstdDictionary;
        DataReader01 datareader;
        public string filepath;

        DataPart01 headerData;
        // public List<DataPart> zFrames = new();
        public List<ZFrameCompressed01> zFrames = new();



        public ShaderReader01(string filepath) {
            this.filepath = filepath;
            datareader = new DataReader01(File.ReadAllBytes(filepath));
            zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_2bc2fa87.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE3.dat");

            List<int> zframeIndexes = datareader.SearchForByteSequence(new byte[] {0x28, 0xb5, 0x2f, 0xfd});

            if (zframeIndexes.Count > 0) {
                headerData = new DataPart01(datareader.databytes, 0, zframeIndexes[0] - 12);
            } else {
                headerData = new DataPart01(datareader.databytes);
            }


            int zFrameSizes = 0;

            foreach (int ind in zframeIndexes) {
                int zFrameLength = datareader.ReadIntAtPosition(ind-4) + 12;
                ZFrameCompressed01 zFrameData = new(datareader.databytes, ind-12, zFrameLength);

                zFrames.Add(zFrameData);

                // Debug.WriteLine(zFrameLength);

                // byte[] zframedatabytes = zFrameData.DecompressFrame(zstdDictionary);
                // zFrames.Add(new DataPart(zframedatabytes, 0));
                // zFrameSizes += zFrameData.length;

            }


            // TO DO - fix this crap
            //int totalSize = headerData.length + zFrameSizes;
            //if (totalSize != datareader.databytes.Length) {
            //    throw new ShaderParserException("sizes don't add up!");
            //}
            // Debug.WriteLine($"nr of zframes = {zFrames.Count}");

            uint vcsMagic = datareader.ReadUInt();




            // Debug.WriteLine("HEADER");
            // headerData.PrintAllBytes();


            return;


            for (int i = 0; i < zFrames.Count; i++) {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine($"ZFRAME[{i}]");
                zFrames[i].PrintAllBytes();
                if (i==0) {
                    break;
                }
            }



        }


        public DataReader01 getZframeDataReader(int id) {

            byte[] zframedatabytes = zFrames[id].DecompressFrame(zstdDictionary);
            return new DataReader01(zframedatabytes);
        }





    }
}




