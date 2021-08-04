
namespace TestVRF.vcsparsing {

    public abstract class DataBlock {

        protected DataReader datareader;
        protected int start;

        protected DataBlock(DataReader datareader, int offset) {
            this.start = offset;
            this.datareader = datareader;
        }
        public abstract void PrintByteSummary();
    }


}










