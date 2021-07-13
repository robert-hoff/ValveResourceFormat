
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyVrfReader {

	public class BinaryReaderAndNullTerm : BinaryReader {

		public BinaryReaderAndNullTerm(Stream input) : base(input) { }


		public string ReadNullTermString(Encoding enc) {
			StringBuilder sb = new StringBuilder("", 500);
			char c = (char)ReadByte();
			while (c > 0) {
				sb.Append(c);
				c = (char)ReadByte();
				if (sb.Length > 5000000) {
					throw new Exception("String too long in ReadNullTermString method");
				}
			}
			return sb.ToString();
		}

	}



}



