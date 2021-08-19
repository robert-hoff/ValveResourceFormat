
namespace MyShaderAnalysis.vcsparsing {

    public abstract class ShaderDataBlock {

        protected ShaderDataReader datareader { get; }
        protected int start { get; }

        protected ShaderDataBlock(ShaderDataReader datareader, int offset) {
            this.start = offset;
            this.datareader = datareader;
        }

        public int ReadIntegerAtPosition(int relOffset) {
            return datareader.ReadIntAtPosition(start + relOffset, rel: false);
        }



    }
}









