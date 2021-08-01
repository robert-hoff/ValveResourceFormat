using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.utilhelpers;

namespace MyShaderAnalysis.readers01 {

    public class ShaderFile01 {


        private bool FEATURES_FILE = false;
        private bool PC_FILE = false;
        private bool VC_FILE = false;



        // NO LONGER IN USE - CAN DELETE
        public ShaderFile01(string filepath) {

            /*
            string filename = Path.GetFileName(filepath);
            string name_wo_extension = filename[0..^4];
            if (name_wo_extension.Length >= 8 && name_wo_extension[^8..] == "features") {
                FEATURES_FILE = true;
            }
            if (name_wo_extension.Length>=2 && name_wo_extension[^2..] == "pc") {
                PC_FILE = true;
            }
            if (name_wo_extension.Length>=2 && name_wo_extension[^2..] == "vc") {
                VC_FILE = true;
            }
            if (!FEATURES_FILE && !PC_FILE && !VC_FILE) {
                throw new ShaderParserException($"unknown filetype! {filename}");
            }


            DataReader01 datareader = new(File.ReadAllBytes(filepath));
            Debug.WriteLine($"parsing {filename}\n");

            datareader.PrintVcsFileHeader();

            if (FEATURES_FILE) {
                datareader.PrintFeaturesHeader();
            } else {
                datareader.PrintPcVsHeader();
            }

            uint blockDelim = datareader.ReadUIntAtPosition(datareader.offset);
            if (blockDelim != 17) {
                throw new ShaderParserException($"unexpected block delim value! {blockDelim}");
            }
            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"block DELIM always 17");
            Debug.WriteLine("");


            datareader.ShowByteCount();
            uint sfBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{sfBlockCount} SF blocks (152 bytes each)");

            Debug.WriteLine("");
            for (int i = 0; i < sfBlockCount; i++) {
                datareader.PrintSFBlock();
            }

            datareader.ShowByteCount();
            uint combatibilityBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{combatibilityBlockCount} compatibility blocks (472 bytes each)");
            Debug.WriteLine("");

            for (int i = 0; i < combatibilityBlockCount; i++) {
                datareader.PrintCompatibilitiesBlock(i);
            }


            datareader.ShowByteCount();
            uint dBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{dBlockCount} D-blocks (152 bytes each)");
            Debug.WriteLine("");

            for (int i = 0; i < dBlockCount; i++) {
                datareader.PrintDBlock();
            }

            datareader.ShowByteCount();
            uint unknownBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{unknownBlockCount} unknown block (472 bytes each)");
            Debug.WriteLine("");

            for (int i = 0; i < unknownBlockCount; i++) {
                datareader.PrintUnknownBlockType1(i);
            }

            datareader.ShowByteCount();
            uint paramBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{paramBlockCount} Param-Blocks (may contain dynamic expressoins)");
            Debug.WriteLine("");
            for (int i = 0; i < paramBlockCount; i++) {
                datareader.PrintParameterBlock(i);
            }

            datareader.ShowByteCount();
            uint mipmapBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            Debug.WriteLine("");
            for (int i = 0; i < mipmapBlockCount; i++) {
                datareader.PrintMipmapBlock(i);
            }

            datareader.ShowByteCount();
            uint bufferBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{bufferBlockCount} buffer blocks (variable length)");
            Debug.WriteLine("");
            for (int i = 0; i < bufferBlockCount; i++) {
                datareader.PrintBufferBlock(i);
            }


            // R: no symbol blocks for the ps file ... ?
            if (!PC_FILE) {
                datareader.ShowByteCount();
                uint symbolBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
                datareader.ShowBytesNoLineBreak(4);
                datareader.TabPrintComment($"{symbolBlockCount} symbol/names blocks");
                for (int i = 0; i < symbolBlockCount; i++) {
                    Debug.WriteLine("");
                    datareader.PrintNamesBlock(i);
                }
            }

            datareader.ParseZFramesSection();
            datareader.EndOfFile();




            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            //datareader.ShowBytesAtPosition(datareader.offset, 200);
            //Debug.WriteLine("");

            */

        }



    }



}




