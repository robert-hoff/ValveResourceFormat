
namespace ValveResourceFormat.CompiledShader
{
    public abstract class ShaderDataBlock
    {
        public ShaderDataReader datareader { get; }
        protected long start { get; }
        protected ShaderDataBlock(ShaderDataReader datareader, long offsetAtStartOfBlock)
        {
            this.start = offsetAtStartOfBlock;
            this.datareader = datareader;
        }
    }
}

