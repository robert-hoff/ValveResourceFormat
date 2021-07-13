using System;

namespace MyVrfReader {
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ExtensionAttribute : Attribute {
		public string Extension { get; }

		public ExtensionAttribute(string extension) {
			Extension = extension;
		}
	}
}
