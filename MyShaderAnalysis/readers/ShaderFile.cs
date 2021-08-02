using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers {


    public enum FILETYPE {
        unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
    };


    public class ShaderFile {











    }














    public class ShaderParserException : Exception {
        public ShaderParserException() { }
        public ShaderParserException(string message) : base(message) {}
        public ShaderParserException(string message, Exception innerException) : base(message, innerException) {}
    }



}


