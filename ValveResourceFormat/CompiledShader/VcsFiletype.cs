using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveResourceFormat.ShaderParser
{
    public enum VcsFiletype
    {
        Undetermined,
        Any,
        Features,                   // features.vcs
        VertexShader,               // vs.vcs
        PixelShader,                // ps.vcs
        GeometryShader,             // gs.vcs
        ComputeShader,              // cs.vcs (needs implementation)
        PotentialShadowReciever,    // psrs.vcs
    };
}
