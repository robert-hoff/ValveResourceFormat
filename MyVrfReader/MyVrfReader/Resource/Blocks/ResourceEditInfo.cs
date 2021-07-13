using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MyVrfReader.Blocks.ResourceEditInfoStructs;

namespace MyVrfReader.Blocks {
	/// <summary>
	/// "REDI" block. ResourceEditInfoBlock_t.
	/// </summary>
	public class ResourceEditInfo : Block {
		public override BlockType Type => BlockType.REDI;

		/// <summary>
		/// This is not a real Valve enum, it's just the order they appear in.
		/// </summary>
		public enum REDIStruct {
			InputDependencies,
			AdditionalInputDependencies,
			ArgumentDependencies,
			SpecialDependencies,
			CustomDependencies,
			AdditionalRelatedFiles,
			ChildResourceList,
			ExtraIntData,
			ExtraFloatData,
			ExtraStringData,

			End,
		}

		public Dictionary<REDIStruct, REDIBlock> Structs { get; private set; }

		public ResourceEditInfo() {
			Structs = new Dictionary<REDIStruct, REDIBlock>();
		}

		public override void Read(BinaryReader reader, Resource resource) {
			reader.BaseStream.Position = Offset;
			// Debug.WriteLine("REDI POS = {0}", reader.BaseStream.Position);


			for (var i = REDIStruct.InputDependencies; i < REDIStruct.End; i++) {
				var block = ConstructStruct(i);

				block.Offset = (uint)reader.BaseStream.Position + reader.ReadUInt32();
				block.Size = reader.ReadUInt32();

				// Debug.WriteLine(block.Size);


				Structs.Add(i, block);
			}

			foreach (var block in Structs) {
				block.Value.Read(reader, resource);
			}
		}

		public override void WriteText(IndentedTextWriter writer) {
			writer.WriteLine("ResourceEditInfoBlock_t");
			writer.WriteLine("{");
			writer.Indent++;

			foreach (var dep in Structs) {
				dep.Value.WriteText(writer);
			}

			writer.Indent--;
			writer.WriteLine("}");
		}

		private static REDIBlock ConstructStruct(REDIStruct id) {
			return id switch {

				// For each of these go to the REDIStruct and see the read method
				// for how to get the data

				REDIStruct.InputDependencies => new InputDependencies(),
				REDIStruct.AdditionalInputDependencies => new AdditionalInputDependencies(),
				REDIStruct.ArgumentDependencies => new ArgumentDependencies(),
				REDIStruct.SpecialDependencies => new SpecialDependencies(),
				REDIStruct.CustomDependencies => new CustomDependencies(),
				REDIStruct.AdditionalRelatedFiles => new AdditionalRelatedFiles(),
				REDIStruct.ChildResourceList => new ChildResourceList(),
				REDIStruct.ExtraIntData => new ExtraIntData(),
				REDIStruct.ExtraFloatData => new ExtraFloatData(),
				REDIStruct.ExtraStringData => new ExtraStringData(),
				_ => throw new InvalidDataException($"Unknown struct in REDI block: {id}"),
			};
		}
	}
}