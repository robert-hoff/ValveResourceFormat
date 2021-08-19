using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.vcsparsing
{
    public class CompiledShader
    {
        public const int MAGIC_VCS2 = 0x32736376; // "vcs2"
        public const uint ZSTD_DELIM = 0xFFFFFFFD;
        public const uint LZMA_DELIM = 0x414D5A4C;
        public const int ZSTD_COMPRESSION = 1;
        public const int LZMA_COMPRESSION = 2;
    }
}
