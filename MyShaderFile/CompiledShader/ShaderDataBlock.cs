
namespace ValveResourceFormat.CompiledShader
{
    public abstract class ShaderDataBlock
    {
        public ShaderDataReader DataReader { get; }
        protected long start { get; }
        protected ShaderDataBlock(ShaderDataReader datareader)
        {
            this.start = datareader.BaseStream.Position;
            this.DataReader = datareader;
        }
    }
}

