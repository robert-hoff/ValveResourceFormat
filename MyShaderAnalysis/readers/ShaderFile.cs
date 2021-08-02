using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis.readers {


    public enum FILETYPE {
        unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
    };


    public class ShaderFile {

        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
        byte[] databytes;

        DataBlockFeaturesHeader featuresHeader = null;





        public ShaderFile(string filenamepath) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);
            databytes = File.ReadAllBytes(filenamepath);

            if (vcsFiletype == FILETYPE.features_file) {
                featuresHeader = new DataBlockFeaturesHeader(databytes);
            }



            Debug.WriteLine($"parse file");
        }








        private static FILETYPE GetVcsFileType(string filenamepath) {
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





    public class ShaderParserException : Exception {
        public ShaderParserException() { }
        public ShaderParserException(string message) : base(message) {}
        public ShaderParserException(string message, Exception innerException) : base(message, innerException) {}
    }



}


