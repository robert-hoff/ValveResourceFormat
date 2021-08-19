
namespace MyShaderAnalysis.vcsparsing {

    public abstract class ShaderDataBlock {

        protected DataReader datareader;
        protected int start;

        protected ShaderDataBlock(DataReader datareader, int offset) {
            this.start = offset;
            this.datareader = datareader;
        }
        public abstract void PrintByteSummary();

        public int ReadIntegerAtPosition(int relOffset) {
            return datareader.ReadIntAtPosition(start + relOffset, rel: false);
        }

    }


}










