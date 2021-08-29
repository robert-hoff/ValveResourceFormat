using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MyGUI.Utils;
using ValveResourceFormat.CompiledShader;

namespace MyGUI.Types.Viewers {
    public class CompiledShader : IViewer {
        public static bool IsAccepted(uint magic) {
            return magic == ShaderFile.MAGIC;
        }
        public class ShaderTabcontrol : TabControl { }

        ShaderTabcontrol tabControl;

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
            ShaderFile shaderFile = new ShaderFile();
            if (input != null) {
                shaderFile.Read(vrfGuiContext.FileName, new MemoryStream(input));
            } else {
                shaderFile.Read(vrfGuiContext.FileName);
            }

            // tab here is only used as a container for the new controls, assigning a title won't do anything
            var tab = new TabPage();
            tabControl = new ShaderTabcontrol {
                Dock = DockStyle.Fill,
            };

            tabControl.MouseClick += new MouseEventHandler(OnTabClick);

            var mainFileTab = new TabPage(Path.GetFileName(vrfGuiContext.FileName));
            // var control = new MyRichTextBox(shaderFile, vrfGuiContext, input);
            var control = new ShaderRichTextBox(shaderFile, tabControl);
            mainFileTab.Controls.Add(control);
            tabControl.Controls.Add(mainFileTab);
            tab.Controls.Add(tabControl);
            return tab;
        }

        private void OnTabClick(object sender, MouseEventArgs e) {
            //Work out what tab we're interacting with
            var tabControl = sender as TabControl;
            var tabs = tabControl.TabPages;
            TabPage thisTab = tabs.Cast<TabPage>().Where((t, i) => tabControl.GetTabRect(i).Contains(e.Location)).First();
            if (e.Button == MouseButtons.Right) {
                var tabIndex = GetTabIndex(thisTab);
                // don't close the main tab
                if (tabIndex == 0) {
                    return;
                }
                Console.WriteLine($"Closing {thisTab.Text}");
                if (tabIndex == tabControl.SelectedIndex && tabIndex > 0) {
                    tabControl.SelectedIndex = tabIndex - 1;
                }
                tabControl.TabPages.Remove(thisTab);
                thisTab.Dispose();
            }
        }

        private int GetTabIndex(TabPage tab) {
            for (int i = 0; i < tabControl.TabPages.Count; i++) {
                if (tabControl.TabPages[i] == tab) {
                    return i;
                }
            }
            return -1;
        }





        private class ShaderRichTextBox : RichTextBox {
            private ShaderFile shaderFile;
            private TabControl tabControl;

            // public MyRichTextBox(ShaderFile shaderFile, VrfGuiContext vrfGuiContext, byte[] input) : base() {
            public ShaderRichTextBox(ShaderFile shaderFile, TabControl tabPage) : base() {
                this.shaderFile = shaderFile;
                this.tabControl = tabPage;
                var buffer = new StringWriter(CultureInfo.InvariantCulture);
                shaderFile.PrintSummary(buffer.Write);
                Font = new Font(FontFamily.GenericMonospace, Font.Size);
                DetectUrls = true;
                Dock = DockStyle.Fill;
                Multiline = true;
                ReadOnly = true;
                WordWrap = false;
                Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
                ScrollBars = RichTextBoxScrollBars.Both;
                LinkClicked += new LinkClickedEventHandler(ShaderRichTextBoxLinkClicked);
                // MouseClick += new MouseEventHandler(ShaderRichTextBoxMouseClick);
                // GotFocus += new EventHandler(MyGotFocus);
            }

            //public override void OnShown() {
            //}


            private void MyGotFocus(object sender, EventArgs e) {
            }

            private void ShaderRichTextBoxMouseClick(object sender, MouseEventArgs e) {
            }

            private void ShaderRichTextBoxLinkClicked(object sender, LinkClickedEventArgs e) {
                long zframeId = Convert.ToInt64(e.LinkText.Substring(2), 16);
                var zframeTab = new TabPage($"Z[{zframeId:X08}]");
                var zframeRichTextBox = new ZFrameRichTextBox(shaderFile, zframeId);
                zframeTab.Controls.Add(zframeRichTextBox);
                tabControl.Controls.Add(zframeTab);
                // to prevent undesirable scrolling, set the caret position to the beginning of the link
                if (SelectionStart == 0) {
                    SelectionStart = Text.IndexOf(e.LinkText);
                    SelectionLength = 0;
                }

            }
        }


        private class ZFrameRichTextBox : RichTextBox {
            public ZFrameRichTextBox(ShaderFile shaderFile, long zframeId) : base() {
                var buffer = new StringWriter(CultureInfo.InvariantCulture);
                ZFrameFile zframeFile = shaderFile.GetZFrameFile(zframeId, OutputWriter: buffer.Write);
                zframeFile.PrintByteAnalysis();
                Font = new Font(FontFamily.GenericMonospace, Font.Size);
                DetectUrls = true;
                Dock = DockStyle.Fill;
                Multiline = true;
                ReadOnly = true;
                WordWrap = false;
                Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
                ScrollBars = RichTextBoxScrollBars.Both;
            }
        }
    }
}
