using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyGUI.Utils;

namespace MyGUI.Types.Viewers
{
    public class ByteViewer : IViewer
    {
        public static bool IsAccepted() => true;

        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input)
        {
            var tab = new TabPage();
            var resTabs = new TabControl
            {
                Dock = DockStyle.Fill,
            };
            tab.Controls.Add(resTabs);

            var bvTab = new TabPage("Hex");
            var bv = new System.ComponentModel.Design.ByteViewer
            {
                Dock = DockStyle.Fill,
            };
            bvTab.Controls.Add(bv);
            resTabs.TabPages.Add(bvTab);

            input ??= File.ReadAllBytes(vrfGuiContext.FileName);

            string textContent = !input.Contains<byte>(0x00) ? System.Text.Encoding.UTF8.GetString(input) : BytesAsHexString(input);
            var textTab = new TabPage(!input.Contains<byte>(0x00) ? "Text" : "Bytes");
            var textBox = new System.Windows.Forms.RichTextBox
            {
                Dock = DockStyle.Fill,
                ScrollBars = RichTextBoxScrollBars.Both,
                Multiline = true,
                ReadOnly = true,
                WordWrap = false,
                Text = textContent,
            };
            textBox.Font = new Font(FontFamily.GenericMonospace, textBox.Font.Size);
            textTab.Controls.Add(textBox);
            resTabs.TabPages.Add(textTab);
            resTabs.SelectedTab = textTab;

            Program.MainForm.Invoke((MethodInvoker)(
                () => bv.SetBytes(input)
            ));

            return tab;
        }

        private static string BytesAsHexString(byte[] databytes)
        {
            int BREAKLEN = 32;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < databytes.Length; i++)
            {
                if (i > 0 && i % BREAKLEN == 0)
                {
                    sb.Append($"\n");
                }
                sb.Append($"{databytes[i]:X02} ");
            }
            return sb.ToString();
        }
    }
}
