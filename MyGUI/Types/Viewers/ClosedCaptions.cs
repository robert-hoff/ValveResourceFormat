using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MyGUI.Utils;
using MyValveResourceFormat.ClosedCaptions;

namespace MyGUI.Types.Viewers
{
    public class ClosedCaptions : IViewer
    {
        public static bool IsAccepted(uint magic)
        {
            return magic == MyValveResourceFormat.ClosedCaptions.ClosedCaptions.MAGIC;


        }

        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input)
        {
            var tab = new TabPage();
            var captions = new MyValveResourceFormat.ClosedCaptions.ClosedCaptions();

            if (input != null)
            {
                captions.Read(vrfGuiContext.FileName, new MemoryStream(input));
            }
            else
            {
                captions.Read(vrfGuiContext.FileName);
            }

            var control = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = new BindingSource(new BindingList<ClosedCaption>(captions.Captions), null),
                ScrollBars = ScrollBars.Both,
            };
            tab.Controls.Add(control);

            return tab;
        }
    }
}
