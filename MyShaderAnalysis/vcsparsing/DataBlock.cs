using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.vcsparsing {


    public abstract class DataBlock {

        protected DataReader datareader;
        protected int start;

        public DataBlock(DataReader datareader, int offset) {
            this.start = offset;
            this.datareader = datareader;
        }
        public abstract void PrintByteSummary();


    }



}









