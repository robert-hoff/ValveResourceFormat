using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;


namespace ValveTrials
{
    static class ShaderTab
    {

        public static (TabPage, TextBox) Create()
        {
            var tab = new TabPage("snazzy title");
            var control = new TextBox();
            control.Font = new Font(FontFamily.GenericMonospace, control.Font.Size);
            control.Text = "helloText";
            control.Dock = DockStyle.Fill;
            control.Multiline = true;
            control.ReadOnly = true;
            control.ScrollBars = ScrollBars.Both;
            tab.Controls.Add(control);
            return (tab, control);
        }

    }
}
