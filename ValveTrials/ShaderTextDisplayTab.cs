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
            var control = new MyRichTextBox();
            tab.Controls.Add(control);
            return (tab, control);
        }






    }
}
