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
using VrfPackage = SteamDatabase.ValvePak.Package;


namespace MyGUI.Types.Viewers {
    public class CompiledShader : IViewer {

        public static bool IsAccepted(uint magic) {
            return magic == ShaderFile.MAGIC;
        }
        public class ShaderTabcontrol : TabControl {
            public ShaderTabcontrol() : base() { }
        }

        ShaderTabcontrol tabControl;
        ShaderFile shaderFile;
        SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection;


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

            shaderCollection = GetShaderCollection(vrfGuiContext.FileName, vrfGuiContext.CurrentPackage);
            string filename = Path.GetFileName(vrfGuiContext.FileName);
            shaderFile = shaderCollection[(ComputeVcsProgramType(filename), filename)];

            // tab here is only used as a container for the new controls, assigning a title won't do anything
            var tab = new TabPage();
            tabControl = new ShaderTabcontrol {
                Dock = DockStyle.Fill,
            };

            tabControl.MouseClick += new MouseEventHandler(OnTabClick);
            var mainFileTab = new TabPage(Path.GetFileName(vrfGuiContext.FileName));
            var control = new ShaderRichTextBox(shaderFile, tabControl, shaderCollection);
            mainFileTab.Controls.Add(control);
            tabControl.Controls.Add(mainFileTab);
            tab.Controls.Add(tabControl);
            control.MouseEnter += new EventHandler(MouseEnterHandler);
            return tab;
        }



        private static SortedDictionary<(VcsProgramType, string), ShaderFile> GetShaderCollection(string targetFilename, VrfPackage vrfPackage) {
            SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection = new();
            if (vrfPackage != null) {
                // search the package
                string vcsCollectionName = targetFilename.Substring(0, targetFilename.LastIndexOf('_')); // in the form water_dota_pcgl_40
                List<PackageEntry> vcsEntries = vrfPackage.Entries["vcs"];
                // vcsEntry.FileName is in the form bloom_dota_pcgl_30_ps (without vcs extension)
                foreach (var vcsEntry in vcsEntries) {
                    if (vcsEntry.FileName.StartsWith(vcsCollectionName)) {
                        VcsProgramType programType = ComputeVcsProgramType($"{vcsEntry.FileName}.vcs");
                        vrfPackage.ReadEntry(vcsEntry, out var shaderDatabytes);
                        ShaderFile relatedShaderFile = new();
                        relatedShaderFile.Read($"{vcsEntry.FileName}.vcs", new MemoryStream(shaderDatabytes));
                        shaderCollection.Add((programType, $"{vcsEntry.FileName}.vcs"), relatedShaderFile);
                    }
                }
            } else {
                // search file-system
                string filename = Path.GetFileName(targetFilename);
                string vcsCollectionName = filename.Substring(0, filename.LastIndexOf('_'));
                foreach (var vcsFile in Directory.GetFiles(Path.GetDirectoryName(targetFilename))) {
                    if (Path.GetFileName(vcsFile).StartsWith(vcsCollectionName)) {
                        VcsProgramType programType = ComputeVcsProgramType(vcsFile);
                        ShaderFile relatedShaderFile = new();
                        relatedShaderFile.Read(vcsFile);
                        shaderCollection.Add((programType, Path.GetFileName(vcsFile)), relatedShaderFile);
                    }
                }
            }
            return shaderCollection;
        }

        private static void MouseEnterHandler(object sender, EventArgs e) {
            ShaderRichTextBox shaderRTB = sender as ShaderRichTextBox;
            shaderRTB.Focus();
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
                // Console.WriteLine($"Closing {thisTab.Text}");
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
            SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection;
            private TabControl tabControl;
            private List<string> relatedFiles = new();


            public ShaderRichTextBox(ShaderFile shaderFile, TabControl tabControl,
                SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection = null, bool byteVersion = false) : base() {
                this.shaderFile = shaderFile;
                this.tabControl = tabControl;
                if (shaderCollection != null) {
                    this.shaderCollection = shaderCollection;
                    foreach (var vcsFilenames in shaderCollection.Keys) {
                        relatedFiles.Add(vcsFilenames.Item2);
                    }
                }

                var buffer = new StringWriter(CultureInfo.InvariantCulture);
                if (!byteVersion) {
                    shaderFile.PrintSummary(buffer.Write, showRichTextBoxLinks: true, relatedfiles: relatedFiles);
                } else {
                    shaderFile.PrintByteAnalysis(OutputWriter: buffer.Write);
                }
                Font = new Font(FontFamily.GenericMonospace, Font.Size);
                DetectUrls = true;
                Dock = DockStyle.Fill;
                Multiline = true;
                ReadOnly = true;
                WordWrap = false;
                Text = Utils.Utils.NormalizeLineEndings(buffer.ToString());
                ScrollBars = RichTextBoxScrollBars.Both;
                LinkClicked += new LinkClickedEventHandler(ShaderRichTextBoxLinkClicked);
            }





            private void ShaderRichTextBoxLinkClicked(object sender, LinkClickedEventArgs evt) {

                string linkText = evt.LinkText[2..]; // remove two starting backslahses
                string[] linkTokens = linkText.Split("\\");


                VcsProgramType programType = ComputeVcsProgramType(linkTokens[0]);
                if (programType != VcsProgramType.Undetermined) {
                    ShaderFile shaderFile = shaderCollection[(programType, linkTokens[0])];
                    if (linkTokens.Length > 1 && linkTokens[1].Equals("bytes")) {
                        var newShaderTab = new TabPage($"{programType} bytes");
                        var shaderRichTextBox = new ShaderRichTextBox(shaderFile, tabControl, byteVersion: true);
                        shaderRichTextBox.MouseEnter += new EventHandler(MouseEnterHandler);
                        newShaderTab.Controls.Add(shaderRichTextBox);
                        tabControl.Controls.Add(newShaderTab);
                    } else {
                        var newShaderTab = new TabPage($"{programType}");
                        var shaderRichTextBox = new ShaderRichTextBox(shaderFile, tabControl, shaderCollection);
                        shaderRichTextBox.MouseEnter += new EventHandler(MouseEnterHandler);
                        newShaderTab.Controls.Add(shaderRichTextBox);
                        tabControl.Controls.Add(newShaderTab);
                    }
                    return;
                }



                long zframeId = Convert.ToInt64(linkText, 16);
                var zframeTab = new TabPage($"Z[{zframeId:X08}]");
                var zframeRichTextBox = new ZFrameRichTextBox(shaderFile, zframeId);
                zframeTab.Controls.Add(zframeRichTextBox);
                tabControl.Controls.Add(zframeTab);

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
