using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.readers {


    public class DataPart {

        protected int length;
        protected DataReader datareader;

        public DataPart(byte[] databytes, int offset, int length) {
            this.length = length;
            this.datareader = new DataReader(databytes, offset);
        }






    }


}







