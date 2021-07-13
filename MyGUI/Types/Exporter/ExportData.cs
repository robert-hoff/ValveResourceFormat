using MyGUI.Utils;
using MyValveResourceFormat;

namespace MyGUI.Types.Exporter {
    public class ExportData {
        public Resource Resource { get; set; }
        public VrfGuiContext VrfGuiContext { get; set; }
        public ExportFileType FileType { get; set; } = ExportFileType.Auto;
    }
}
