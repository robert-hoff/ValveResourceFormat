using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MyGUI.Utils;
using ValveResourceFormat.CompiledShader;
using System.Runtime.InteropServices;
using SteamDatabase.ValvePak;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyGUI.Types.Viewers {
    public class CompiledShader : IViewer {

        public static bool IsAccepted(uint magic) {
            return magic == ShaderFile.MAGIC;
        }
        public class ShaderTabcontrol : TabControl {
            public ShaderTabcontrol() : base() { }
        }


        ShaderTabcontrol tabControl;
        // TabControl tabControl;

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

            GetRelatedFiles(vrfGuiContext, input);





            // tab here is only used as a container for the new controls, assigning a title won't do anything
            var tab = new TabPage();
            tabControl = new ShaderTabcontrol {
                Dock = DockStyle.Fill,
            };

            tabControl.MouseClick += new MouseEventHandler(OnTabClick);
            // tabControl.Selected += new TabControlEventHandler(TabControl1_Selected);
            // tabControl.Selecting += new TabControlCancelEventHandler(TabControl1_Selecting);


            var mainFileTab = new TabPage(Path.GetFileName(vrfGuiContext.FileName));
            // var control = new MyRichTextBox(shaderFile, vrfGuiContext, input);
            var control = new ShaderRichTextBox(shaderFile, tabControl);
            mainFileTab.Controls.Add(control);
            tabControl.Controls.Add(mainFileTab);
            tab.Controls.Add(tabControl);
            // tabControl.Validated += new EventHandler(textBox1_Validated);

            control.MouseEnter += new EventHandler(MouseEnterHandler);
            // mainFileTab.Focus();
            // control.Focus();
            return tab;
        }




        private static SortedDictionary<(VcsProgramType, string), ShaderFile> GetRelatedFiles(VrfGuiContext vrfGuiContext, byte[] input) {

            SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection = new();

            if (input != null) {
                // search packages
                ShaderFile targetFile = new();
                targetFile.Read(vrfGuiContext.FileName, new MemoryStream(input));
                VcsProgramType vcsProgramType = ComputeVCSFileName($"{vrfGuiContext.FileName}").Item1;
                shaderCollection.Add((vcsProgramType, vrfGuiContext.FileName), targetFile);
                string vcsCollectionName = vrfGuiContext.FileName.Substring(0, vrfGuiContext.FileName.LastIndexOf('_')); // in the form water_dota_pcgl_40

                List<PackageEntry> vcsEntries = vrfGuiContext.CurrentPackage.Entries["vcs"];
                foreach (var vcsEntry in vcsEntries) {
                    // vcsEntry.FileName is in the form bloom_dota_pcgl_30_ps (without vcs extension)
                    if (vcsEntry.FileName.StartsWith(vcsCollectionName)) {
                        VcsProgramType programType = ComputeVCSFileName($"{vcsEntry.FileName}.vcs").Item1;
                        if (!shaderCollection.ContainsKey((programType, $"{vcsEntry.FileName}.vcs"))) {
                            ShaderFile relatedShaderFile = new();
                            vrfGuiContext.CurrentPackage.ReadEntry(vcsEntry, out var shaderDatabytes);
                            relatedShaderFile.Read($"{vcsEntry.FileName}.vcs", new MemoryStream(shaderDatabytes));
                            shaderCollection.Add((programType, $"{vcsEntry.FileName}.vcs"), relatedShaderFile);
                        }
                    }
                }
            } else {
                // search file-system

            }



            return shaderCollection;
        }





        private void MouseEnterHandler(object sender, EventArgs e) {
            ShaderRichTextBox shaderRTB = sender as ShaderRichTextBox;
            shaderRTB.Focus();
        }
        private void textBox1_Validated(object sender, EventArgs e) {
            Console.WriteLine("validated event");
        }
        private void TabControl1_Selecting(Object sender, TabControlCancelEventArgs e) {
            Console.WriteLine("tb-selecting");
        }
        private void TabControl1_Selected(object sender, TabControlEventArgs e) {
            Console.WriteLine("tb-selected");
            ((ShaderTabcontrol)sender).BeginInvoke(new Action(() => e.TabPage.Controls.OfType<RichTextBox>().ToList().ForEach(rtb => rtb.Focus())));
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
                shaderFile.PrintSummary(buffer.Write, showRichTextBoxLinks: true);
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
                //if (SelectionStart == 0) {
                //    SelectionStart = Text.IndexOf(e.LinkText);
                //    SelectionLength = 0;
                //}

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
