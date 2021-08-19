using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.vcsparsing {

    public enum VcsFileType {
        Undetermined,
        Any,
        Features,
        VertexShader,
        PixelShader,
        GeometryShader,
        ComputeShader,             // TODO - needs implementation
        PotentialShadowReciever,
    }


}
