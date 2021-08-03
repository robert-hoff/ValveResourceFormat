using System.Diagnostics;
using System.IO;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis.vcsparsing {

    public class ShaderFileByteAnalysis {

        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
        byte[] databytes;
        ShaderFile shaderFile;

        public ShaderFileByteAnalysis(string filenamepath) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = ShaderFile.GetVcsFileType(filenamepath);
            databytes = File.ReadAllBytes(filenamepath);
            shaderFile = new ShaderFile(filenamepath);
        }

        public void PrintZFrameByteAnalysis(int zframeId) {
            DataReaderZFrameByteAnalysis zframeByteAnalysis = getZFrameByteAnalysisReader(zframeId);
            if (zframeByteAnalysis == null) {
                return;
            }
            Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}-ZFRAME{zframeId:d03}");
            Debug.WriteLine($"");
            zframeByteAnalysis.ParseFile();
        }

        public void ParseZFramesRange(int min, int max, bool disableOutput, bool disableStatus) {
            int zcount = shaderFile.GetZFrameCount();
            if (max == -1) {
                max = zcount;
            }
            int numberToParse = zcount > max ? max : zcount;
            if (min >= zcount) {
                Debug.WriteLine($"out of range [{min},{max}) for {RemoveBaseDir(filenamepath)}, nothing to parse. zmax = {zcount - 1}");
                return;
            } else {
                Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)} frames [{min},{numberToParse})");
            }
            for (int i = min; i < numberToParse; i++) {
                DataReaderZFrameByteAnalysis zframeByteAnalysis = getZFrameByteAnalysisReader(i);
                zframeByteAnalysis.DisableOutput = disableOutput;
                if (!disableStatus) {
                    Debug.WriteLine($"{RemoveBaseDir(filenamepath)}-ZFRAME{i:d03}");
                }
                zframeByteAnalysis.ParseFile();
            }
        }

        private DataReaderZFrameByteAnalysis getZFrameByteAnalysisReader(int zframeIndex) {
            int zcount = shaderFile.GetZFrameCount();
            if (zframeIndex > zcount-1) {
                Debug.WriteLine($"zframe index out of range ({zframeIndex}). Max index = {zcount-1}");
                return null;
            }
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrameByIndex(zframeIndex);
            DataReaderZFrameByteAnalysis zframeByteAnalysis = new(zframeDatabytes, vcsFiletype);
            return zframeByteAnalysis;
        }

        public void PrintByteAnalysis() {
            DataReaderVcsByteAnalysis vcsByteAnalysis = new(databytes, vcsFiletype);
            Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}");
            Debug.WriteLine($"");
            vcsByteAnalysis.ParseFile();
        }

        public void ParseFileDisableOutput() {
            DataReaderVcsByteAnalysis vcsByteAnalysis = new(databytes, vcsFiletype);
            vcsByteAnalysis.DisableOutput = true;
            Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}");
            vcsByteAnalysis.ParseFile();
        }

        public void WriteByteAnalysisToHtml(string outputDir) {
            string outputFilename = filename[0..^4] + "-analysis.html";
            string outputFilenamepath = @$"{outputDir}\{outputFilename}";
            StreamWriter sw = new(outputFilenamepath);
            DataReaderVcsByteAnalysis vcsByteAnalysis = new(databytes, vcsFiletype);
            vcsByteAnalysis.ConfigureWriteToFile(sw, true);
            vcsByteAnalysis.ConfigureWriteFileAsHtml(filename);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            string htmlHeader = GetHtmlHeader(filename, RemoveBaseDir(filenamepath));
            sw.WriteLine($"{htmlHeader}");
            vcsByteAnalysis.ParseFile();
            sw.WriteLine($"{GetHtmlFooter()}");
            sw.Flush();
            sw.Close();
        }

        public void WriteByteAnalysisToFile(string outputDir) {
            string outputFilename = filename[0..^4] + "-annotated.txt";
            string outputFilenamepath = @$"{outputDir}\{outputFilename}";
            StreamWriter sw = new(outputFilenamepath);
            Debug.WriteLine($"parsing {filenamepath}");
            Debug.WriteLine($"writing to {outputFilenamepath}");
            DataReaderVcsByteAnalysis vcsByteAnalysis = new(databytes, vcsFiletype);
            vcsByteAnalysis.ConfigureWriteToFile(sw, true);
            sw.WriteLine($"parsing {RemoveBaseDir(filenamepath)}");
            sw.WriteLine("");
            vcsByteAnalysis.ParseFile();
            sw.Flush();
            sw.Close();
        }



    }
}









