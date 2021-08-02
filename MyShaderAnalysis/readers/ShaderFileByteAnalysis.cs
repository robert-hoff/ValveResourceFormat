using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.readers01;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis.readers {

    public class ShaderFileByteAnalysis {


        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
        byte[] databytes;


        // public bool DisableOutput = false;


        public ShaderFileByteAnalysis(string filenamepath) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);
            databytes = File.ReadAllBytes(filenamepath);
        }


        private ShaderReader01  myShaderReader = null;

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
            int zcount = getZframeCount();
            if (max == -1) {
                max = zcount;
            }
            int numberToParse = zcount > max ? max : zcount;

            if (min >= zcount) {
                Debug.WriteLine($"our of range [{min},{max}) for {RemoveBaseDir(filenamepath)}, nothing to parse. zmax = {zcount - 1}");
                return;
            } else {
                Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)} frames [{min},{numberToParse})");
            }
            for (int i = min; i < numberToParse; i++) {
                // Trial1ZVsFrame01(shaderReader, i, filetype, runningTest);
                DataReaderZFrameByteAnalysis zframeByteAnalysis = getZFrameByteAnalysisReader(i);
                zframeByteAnalysis.DisableOutput = disableOutput;
                if (!disableStatus) {
                    Debug.WriteLine($"{RemoveBaseDir(filenamepath)}-ZFRAME{i:d03}");
                }
                zframeByteAnalysis.ParseFile();
            }
        }



        private int getZframeCount() {
            if (myShaderReader == null) {
                myShaderReader = new ShaderReader01(filenamepath);
            }
            return myShaderReader.zFrames.Count;
        }


        private DataReaderZFrameByteAnalysis getZFrameByteAnalysisReader(int zframeId) {
            if (myShaderReader == null) {
                myShaderReader = new ShaderReader01(filenamepath);
            }
            int zcount = myShaderReader.zFrames.Count;
            if (zframeId > zcount-1) {
                Debug.WriteLine($"zframe index out of range ({zframeId}). Max index = {zcount-1}");
                return null;
            }
            byte[] zframeDatabytes = myShaderReader.getZframeDataBytes(zframeId);
            DataReaderZFrameByteAnalysis zframeByteAnalysis = new(zframeDatabytes, vcsFiletype);
            return zframeByteAnalysis;
        }




        public void PrintByteAnalysis() {
            DataReaderVcsByteAnalysis vcsByteAnalysis = new(databytes, vcsFiletype);
            Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}");
            Debug.WriteLine($"");
            vcsByteAnalysis.ParseFile();
        }


        public void WriteByteAnalysisToFile(string outputDir) {
            string outputFilename = filename[0..^4] + "-decompiled.txt";
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


        static FILETYPE GetVcsFileType(string filenamepath) {
            if (filenamepath.EndsWith("features.vcs")) {
                return FILETYPE.features_file;
            }
            if (filenamepath.EndsWith("vs.vcs")) {
                return FILETYPE.vs_file;
            }
            if (filenamepath.EndsWith("ps.vcs")) {
                return FILETYPE.ps_file;
            }
            if (filenamepath.EndsWith("psrs.vcs")) {
                return FILETYPE.psrs_file;
            }
            if (filenamepath.EndsWith("gs.vcs")) {
                return FILETYPE.gs_file;
            }

            throw new ShaderParserException($"don't know what this file is {filenamepath}");
        }




    }
}



