using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;


namespace ValveTrials
{
    static class ShaderTextDisplayTab
    {



        /*
         *
         * RichTextBox class
         * https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox?view=net-5.0
         *
         * Articles
         * https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/richtextbox-control-windows-forms?view=netframeworkdesktop-4.8
         *
         *
         *
         *
         */
        public static (TabPage, MyRichTextBox) Create()
        {
            var tab = new TabPage("snazzy title");
            // var control = new RichTextBox();
            var control = new MyRichTextBox();
            // control.Font = new Font(FontFamily.GenericMonospace, control.Font.Size);
            // control.DetectUrls = false;
            // control.Dock = DockStyle.Fill;
            // control.Multiline = true;
            // control.ReadOnly = true;
            // control.WordWrap = false;
            // control.Text = "link link <a href=\"https://stackoverflow.com/questions/435607\">gogo</a>";
            // control.ScrollBars = ScrollBars.Both;
            // control.ScrollBars = RichTextBoxScrollBars.Both;
            // control.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            // control.ScrollBars = RichTextBoxScrollBars.Both;
            tab.Controls.Add(control);
            return (tab, control);
        }






    }
}
