using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MyGUI.Utils;

namespace MyGUI.Types.Viewers {
    public class CompiledShader : IViewer {
        public static bool IsAccepted(uint magic) {
            return magic == MyValveResourceFormat.CompiledShader.MAGIC;
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
        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input) {
            var tab = new TabPage();
            var shader = new MyValveResourceFormat.CompiledShader();

            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            var oldOut = Console.Out;
            Console.SetOut(buffer);

            if (input != null) {
                shader.Read(vrfGuiContext.FileName, new MemoryStream(input));
            } else {
                shader.Read(vrfGuiContext.FileName);
            }

            Console.SetOut(oldOut);

            var control = new TextBox();
            control.Font = new Font(FontFamily.GenericMonospace, control.Font.Size);
            control.Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
            control.Dock = DockStyle.Fill;
            control.Multiline = true;
            control.ReadOnly = true;
            control.ScrollBars = ScrollBars.Both;
            // control.TabStop = false;
            // control.TabIndex = 1;
            tab.Controls.Add(control);

            return tab;
        }
    }
}
