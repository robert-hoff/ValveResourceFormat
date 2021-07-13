using System.IO;
using System.Text;
using MyValveResourceFormat.Blocks;

namespace MyValveResourceFormat.ResourceTypes {
    public class Plaintext : ResourceData {
        public string Data { get; private set; }

        public override void Read(BinaryReader reader, Resource resource) {
            reader.BaseStream.Position = Offset;

            Data = Encoding.UTF8.GetString(reader.ReadBytes((int)Size));
        }

        public override string ToString() {
            return Data;
        }
    }
}
