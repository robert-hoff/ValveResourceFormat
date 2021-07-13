using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MyGUI.Utils;
using ValveKeyValue;
using MyValveResourceFormat.ResourceTypes;

namespace MyGUI.Types.Viewers
{
    public class BinaryKeyValues1 : IViewer
    {
        public static bool IsAccepted(uint magic)
        {
            return magic == BinaryKV1.MAGIC;
        }

        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input)
        {
            Stream stream;
            KVObject kv;

            if (input != null)
            {
                stream = new MemoryStream(input);
            }
            else
            {
                stream = File.OpenRead(vrfGuiContext.FileName);
            }

            try
            {
                kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary).Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }

            using var ms = new MemoryStream();
            using var reader = new StreamReader(ms);

            KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Serialize(ms, kv);

            ms.Seek(0, SeekOrigin.Begin);

            var text = reader.ReadToEnd();

            var control = new TextBox();
            control.Font = new Font(FontFamily.GenericMonospace, control.Font.Size);
            control.Text = Utils.Utils.NormalizeLineEndings(text);
            control.Dock = DockStyle.Fill;
            control.Multiline = true;
            control.ReadOnly = true;
            control.ScrollBars = ScrollBars.Both;

            var tab = new TabPage();
            tab.Controls.Add(control);

            return tab;
        }
    }
}
