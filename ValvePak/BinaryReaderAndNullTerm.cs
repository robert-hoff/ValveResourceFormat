using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDatabase.ValvePak {
    class BinaryReaderAndNullTerm : BinaryReader {

        public BinaryReaderAndNullTerm(Stream input) : base(input) { }


        public string ReadNullTermString(Encoding enc) {
            StringBuilder sb = new StringBuilder("", 500);
            char c = (char)ReadByte();
            while (c > 0) {
                sb.Append(c);
                c = (char)ReadByte();
                if (sb.Length > 1000) {
                    throw new Exception("String too long in ReadNullTermString method");
                }
            }
            return sb.ToString();
        }

    }



}



