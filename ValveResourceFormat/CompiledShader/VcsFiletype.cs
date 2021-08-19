namespace ValveResourceFormat.ShaderParser
{
    public enum VcsFileType
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
