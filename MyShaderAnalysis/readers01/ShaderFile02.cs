using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers01 {


    public class ShaderFile02 {

        string filepath;
        string filename;
        private bool FEATURES_FILE = false;
        private bool PS_FILE = false;
        private bool VS_FILE = false;
        private bool GS_FILE = false;
        private bool PSRS_FILE = false;
        public DataReader01 datareader;
        string outputFile = null;
        StreamWriter sw = null;
        private bool DisableOutput = false;


        public ShaderFile02(string filepath) {
            this.filepath = filepath;
            filename = Path.GetFileName(filepath);
            if (filepath.EndsWith("features.vcs")) {
                FEATURES_FILE = true;
            }
            if (filepath.EndsWith("ps.vcs")) {
                PS_FILE = true;
            }
            if (filepath.EndsWith("vs.vcs")) {
                VS_FILE = true;
            }
            if (filepath.EndsWith("gs.vcs")) {
                GS_FILE = true;
            }
            if (filepath.EndsWith("psrs.vcs")) {
                PSRS_FILE = true;
            }
            if (!FEATURES_FILE && !PS_FILE && !VS_FILE && !GS_FILE && !PSRS_FILE) {
                throw new ShaderParserException($"Cannot parse this filetype! {filename}");
                // Debug.WriteLine($"unknown file! {filename}");
                // return;
            }
            datareader = new(File.ReadAllBytes(filepath));
        }


        public void ConfigureWriteToFile(string outputDir) {
            // will overwrite
            string filename = Path.GetFileName(filepath);
            filename = filename[0..^4] + "-ANALYSIS.txt";
            string newFilepath = @$"{outputDir}\{filename}";
            FileStream filestream = File.Create(newFilepath);
            filestream.Flush();
            filestream.Close();
            outputFile = newFilepath;

            //R: couldn't get this to work without showing output in console
            // filestream.Flush();
            // filestream.Close();
            // Trace.Listeners.Add(new TextWriterTraceListener(newFilepath));
            // Trace.AutoFlush = true;
        }


        public void ParseShader() {
            if (DisableOutput) {
                datareader.DisableOutput = true;
            }

            if (outputFile != null) {
                sw = new StreamWriter(outputFile);
                datareader.ConfigureWriteToFile(sw);
                try {
                    StartParsing();
                    Debug.WriteLine($"wrote file {filepath}");

                } catch (Exception) {
                    Debug.WriteLine($"exception thrown for {filepath}");
                }

                sw.Flush();
                sw.Close();
            } else {
                StartParsing();
            }
        }


        private void OutputWrite(string text) {
            if (DisableOutput) {
                return;
            }

            if (sw != null) {
                sw.Write(text);
            } else {
                Debug.Write(text);
            }
        }
        private void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }

        public void SetDisableOutput(bool DisableOutput) {
            this.DisableOutput = DisableOutput;
        }


        private void StartParsing() {

            OutputWriteLine($"parsing {filename}");
            OutputWriteLine("");

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
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"block DELIM always 17");
            OutputWriteLine("");


            datareader.ShowByteCount();
            uint sfBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{sfBlockCount} SF blocks (usually 152 bytes each)");
            OutputWriteLine("");
            for (int i = 0; i < sfBlockCount; i++) {
                datareader.PrintSFBlock();
            }

            datareader.ShowByteCount();
            uint combatibilityBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{combatibilityBlockCount} compatibility blocks (472 bytes each)");
            OutputWriteLine("");

            for (int i = 0; i < combatibilityBlockCount; i++) {
                datareader.PrintCompatibilitiesBlock(i);
            }
            datareader.ShowByteCount();
            uint dBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{dBlockCount} D-blocks (152 bytes each)");
            OutputWriteLine("");

            for (int i = 0; i < dBlockCount; i++) {
                datareader.PrintDBlock();
            }
            datareader.ShowByteCount();
            uint unknownBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{unknownBlockCount} unknown block (472 bytes each)");
            OutputWriteLine("");

            for (int i = 0; i < unknownBlockCount; i++) {
                datareader.PrintUnknownBlockType1(i);
            }
            datareader.ShowByteCount();
            uint paramBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            OutputWriteLine("");
            for (int i = 0; i < paramBlockCount; i++) {
                datareader.PrintParameterBlock(i);
            }
            datareader.ShowByteCount();
            uint mipmapBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            OutputWriteLine("");
            for (int i = 0; i < mipmapBlockCount; i++) {
                datareader.PrintMipmapBlock(i);
            }
            datareader.ShowByteCount();
            uint bufferBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"{bufferBlockCount} buffer blocks (variable length)");
            OutputWriteLine("");
            for (int i = 0; i < bufferBlockCount; i++) {
                datareader.PrintBufferBlock(i);
            }

            // for some reason these do not observe symbol blocks
            if (!PS_FILE && !GS_FILE && !PSRS_FILE) {
                datareader.ShowByteCount();
                uint symbolBlockCount = datareader.ReadUIntAtPosition(datareader.offset);
                datareader.ShowBytesNoLineBreak(4);
                datareader.TabPrintComment($"{symbolBlockCount} symbol/names blocks");
                for (int i = 0; i < symbolBlockCount; i++) {
                    OutputWriteLine("");
                    datareader.PrintNamesBlock(i);
                }
            }
            datareader.ParseZFramesSection();

            // throws ShaderParserException() if not at end of file
            datareader.EndOfFile();



            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            //datareader.ShowBytesAtPosition(datareader.offset, 1000);
            //Debug.WriteLine("");






        }

    }


}






