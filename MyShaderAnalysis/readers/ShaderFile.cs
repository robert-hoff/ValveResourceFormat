using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis.readers {

    public class ShaderFile {

        public enum FILETYPE {
            unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
        };


        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
        byte[] databytes;


        // public bool DisableOutput = false;


        public ShaderFile(string filenamepath) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);
            databytes = File.ReadAllBytes(filenamepath);

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

            DataReaderVcsByteAnalysis vcsByteAnalysis = new DataReaderVcsByteAnalysis(databytes, vcsFiletype);
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



