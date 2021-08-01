using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;
// using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis.readers {

    public class ShaderFile {

        public enum FILETYPE {
            unknown, features_file, vs_file, ps_file, gs_file, psrs_file
        };


        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
        // public DataReader datareader;

        public ShaderFile(string filenamepath) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);
            // datareader = new(File.ReadAllBytes(filenamepath));

            byte[] databytes = File.ReadAllBytes(filenamepath);

            new DataReaderVcsByteAnalysis(databytes, vcsFiletype);



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



