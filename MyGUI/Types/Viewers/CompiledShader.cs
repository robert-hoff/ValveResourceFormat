using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MyGUI.Utils;
using ValveResourceFormat.CompiledShader;

namespace MyGUI.Types.Viewers {
    public static class CompiledShader {
        public static bool IsAccepted(uint magic) {
            return magic == ShaderFile.MAGIC;
        }

        public static void Hello() {

        }



        /*
         *
         * The Create method is called both when selecting a vcs file directly from the file-dialog
         * or when selecting a vcs file from inside a valve-pak (vpk archive)
         *
         * It's clear looking at this that a TextBox is used when opening a new shader file
         *
         *
         *
         */
        public static TabPage Createz(VrfGuiContext vrfGuiContext, byte[] input, TabPage parentTab) {
            var tab = new TabPage();
            var control = new MyRichTextBox(vrfGuiContext, input, parentTab);
            tab.Controls.Add(control);
            return tab;
        }
    }

    public class MyRichTextBox : RichTextBox
    {
        private readonly TabPage parentTab;
        private ShaderFile shaderFile;

        public MyRichTextBox(VrfGuiContext vrfGuiContext, byte[] input, TabPage parentTab) : base()
        {
            this.parentTab = parentTab;
            shaderFile = new ShaderFile();
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            if (input != null) {
                shaderFile.Read(vrfGuiContext.FileName, new MemoryStream(input));
            } else {
                shaderFile.Read(vrfGuiContext.FileName);
            }
            shaderFile.PrintSummary(buffer.Write);
            Font = new Font(FontFamily.GenericMonospace, Font.Size);
            DetectUrls = true;
            Dock = DockStyle.Fill;
            Multiline = true;
            ReadOnly = true;
            WordWrap = false;
            Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
            ScrollBars = RichTextBoxScrollBars.Both;
            LinkClicked += new LinkClickedEventHandler(Link_Clicked);
        }

        private void Link_Clicked(object sender, LinkClickedEventArgs e)
        {
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            long zframeId = Convert.ToInt64(e.LinkText.Substring(2));
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zframeId, OutputWriter: buffer.Write);
            zframeFile.PrintByteAnalysis();
            parentTab.Text = $"Z[{zframeId:x08}]";
            Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
            Console.WriteLine($"console print: {zframeId}");
        }
    }
}
