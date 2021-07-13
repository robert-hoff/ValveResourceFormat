using System;

namespace MyValveResourceFormat.Blocks.ResourceEditInfoStructs {
    public class AdditionalInputDependencies : InputDependencies {
        public override void WriteText(IndentedTextWriter writer) {
            writer.WriteLine("Struct m_AdditionalInputDependencies[{0}] =", List.Count);
            WriteList(writer);
        }
    }
}
