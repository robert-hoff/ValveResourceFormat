
namespace ValveResourceFormat.ShaderParser
{
    public abstract class ShaderDataBlock
    {
        protected ShaderDataReader datareader { get; }
        protected int start { get; }
        protected ShaderDataBlock(ShaderDataReader datareader, int offsetAtStartOfBlock)
        {
            this.start = offsetAtStartOfBlock;
            this.datareader = datareader;
        }
        // todo -remove
        public int ReadIntegerAtPosition(int relOffset)
        {
            return datareader.ReadIntAtPosition(start + relOffset, rel: false);
        }
    }
}

