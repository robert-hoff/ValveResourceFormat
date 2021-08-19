

#pragma warning disable CA1051 // Do not declare visible instance fields
namespace ValveResourceFormat.ShaderParser
{

    public abstract class ShaderDataBlock
    {

        protected ShaderDataReader datareader;
        protected int start;

        protected ShaderDataBlock(ShaderDataReader datareader, int offsetAtStartOfBlock)
        {
            this.start = offsetAtStartOfBlock;
            this.datareader = datareader;
        }
        public int ReadIntegerAtPosition(int relOffset)
        {
            return datareader.ReadIntAtPosition(start + relOffset, rel: false);
        }
    }
}

