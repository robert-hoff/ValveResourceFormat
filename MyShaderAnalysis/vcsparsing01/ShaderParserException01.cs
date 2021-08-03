using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.vcsparsing01 {
    public class ShaderParserException01 : Exception {
        public ShaderParserException01() { }
        public ShaderParserException01(string message) : base(message) { }
        public ShaderParserException01(string message, Exception innerException) : base(message, innerException) { }
    }
}
