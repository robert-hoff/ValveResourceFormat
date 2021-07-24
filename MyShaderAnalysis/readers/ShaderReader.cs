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


        public ShaderReader(string filepath) {
            datareader = new DataReader(File.ReadAllBytes(filepath));
            zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_2bc2fa87.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE.dat");
            // zstdDictionary = File.ReadAllBytes(ANALYSIS_DIR + @"\zstdictionary_FALSE3.dat");


            List<int> zframeIndexes = datareader.searchForByteSequence(new byte[] { 0x28, 0xb5, 0x2f, 0xfd });

            foreach (int ind in zframeIndexes) {
                int zFrameLength = datareader.ReadIntAtPosition(ind-4) + 12;
                ZFrameCompressed zFrameData = new(datareader.databytes, ind-12, zFrameLength);
                zFrameData.decompressFrame(zstdDictionary);
            }


            uint vcsMagic = datareader.ReadUInt();





        }






    }
}
