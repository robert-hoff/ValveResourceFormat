using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MyShaderAnalysis.readers {

    public class ShaderReader {

        const string ANALYSIS_DIR = @"X:\checkouts\ValveResourceFormat\files_under_analysis";
        byte[] zstdDictionary;
        DataReader datareader;

        DataPart headerData;
        List<DataPart> zFrames = new();


        public ShaderReader(string filepath) {
            datareader = new DataReader(File.ReadAllBytes(filepath));
            zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_2bc2fa87.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE3.dat");

            List<int> zframeIndexes = datareader.SearchForByteSequence(new byte[] {0x28, 0xb5, 0x2f, 0xfd});

            if (zframeIndexes.Count > 0) {
                headerData = new DataPart(datareader.databytes, 0, zframeIndexes[0] - 12);
            } else {
                headerData = new DataPart(datareader.databytes);
            }


            int zFrameSizes = 0;

            foreach (int ind in zframeIndexes) {
                int zFrameLength = datareader.ReadIntAtPosition(ind-4) + 12;
                ZFrameCompressed zFrameData = new(datareader.databytes, ind-12, zFrameLength);

                Debug.WriteLine(zFrameLength);

                byte[] zframedatabytes = zFrameData.DecompressFrame(zstdDictionary);
                zFrames.Add(new DataPart(zframedatabytes, 0));
                zFrameSizes += zFrameData.length;

            }

            int totalSize = headerData.length + zFrameSizes;
            if (totalSize != datareader.databytes.Length) {
                throw new ShaderParserException("sizes don't add up!");
            }
            // Debug.WriteLine($"nr of zframes = {zFrames.Count}");

            uint vcsMagic = datareader.ReadUInt();




            //Debug.WriteLine("HEADER");
            //headerData.PrintAllBytes();
            //Debug.WriteLine("ZFRAME");
            //zFrames[0].PrintAllBytes();




        }






    }
}




