namespace MyShaderAnalysis.vcsparsing
{
    public enum VcsFileType
    {
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
