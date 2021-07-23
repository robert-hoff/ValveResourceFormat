using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MyGUI.Types.Audio;
using MyGUI.Types.Exporter;
using MyGUI.Types.Renderer;
using MyGUI.Utils;
using SkiaSharp.Views.Desktop;
using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.ResourceTypes;
using System.Diagnostics;

namespace MyGUI.Types.Viewers {
    public class Resource : IViewer {
        public static bool IsAccepted(uint magic) {
            return magic == MyValveResourceFormat.Resource.KnownHeaderVersion;
        }

        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input) {
            var tab = new TabPage();
            var resource = new MyValveResourceFormat.Resource {
                FileName = vrfGuiContext.FileName,
            };

            // byte[] here is 5400 long which is the length of the file
            //
            //      armor_of_reckless_vigor_weapon.vmdl_c
            //
            // threrefore resource.Read(new MemoryStream(input)) is called here
            if (input != null) {
                resource.Read(new MemoryStream(input));
            } else {
                resource.Read(vrfGuiContext.FileName);
            }

            var resTabs = new TabControl {
                Dock = DockStyle.Fill,
            };

            switch (resource.ResourceType) {
                case ResourceType.Texture:
                    var tab2 = new TabPage("TEXTURE") {
                        AutoScroll = true,
                    };

                    try {
                        var tex = (Texture)resource.DataBlock;

                        var control = new Forms.Texture {
                            BackColor = Color.Black,
                        };
                        control.SetImage(tex.GenerateBitmap().ToBitmap(), Path.GetFileNameWithoutExtension(vrfGuiContext.FileName),
                            tex.ActualWidth, tex.ActualHeight);

                        tab2.Controls.Add(control);
                        Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs,
                            $"Export {Path.GetFileName(vrfGuiContext.FileName)} as an image", vrfGuiContext.FileName,
                            new ExportData { Resource = resource });
                    } catch (Exception e) {
                        var control = new TextBox {
                            Dock = DockStyle.Fill,
                            Font = new Font(FontFamily.GenericMonospace, 8),
                            Multiline = true,
                            ReadOnly = true,
                            Text = e.ToString(),
                        };

                        tab2.Controls.Add(control);
                    }

                    resTabs.TabPages.Add(tab2);
                    break;

                case ResourceType.Panorama:
                    if (((Panorama)resource.DataBlock).Names.Count > 0) {
                        var nameTab = new TabPage("PANORAMA NAMES");
                        var nameControl = new DataGridView {
                            Dock = DockStyle.Fill,
                            AutoSize = true,
                            ReadOnly = true,
                            AllowUserToAddRows = false,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            DataSource =
                                new BindingSource(
                                    new BindingList<Panorama.NameEntry>(((Panorama)resource.DataBlock).Names), null),
                        };
                        nameTab.Controls.Add(nameControl);
                        resTabs.TabPages.Add(nameTab);
                    }

                    break;

                case ResourceType.PanoramaLayout:
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as XML",
                        vrfGuiContext.FileName, new ExportData { Resource = resource });
                    break;

                case ResourceType.PanoramaScript:
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as JS",
                        vrfGuiContext.FileName, new ExportData { Resource = resource });
                    break;

                case ResourceType.PanoramaStyle:
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as CSS",
                        vrfGuiContext.FileName, new ExportData { Resource = resource });
                    break;

                case ResourceType.Particle:
                    var viewerControl = new GLParticleViewer(vrfGuiContext);
                    viewerControl.Load += (_, __) => {
                        var particleSystem = (ParticleSystem)resource.DataBlock;
                        var particleRenderer = new ParticleRenderer.ParticleRenderer(particleSystem, vrfGuiContext);

                        viewerControl.AddRenderer(particleRenderer);
                    };

                    var particleRendererTab = new TabPage("PARTICLE");
                    particleRendererTab.Controls.Add(viewerControl.Control);
                    resTabs.TabPages.Add(particleRendererTab);
                    break;

                case ResourceType.Sound:
                    var soundTab = new TabPage("SOUND");
                    var ap = new AudioPlayer(resource, soundTab);
                    resTabs.TabPages.Add(soundTab);

                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs,
                        $"Export {Path.GetFileName(vrfGuiContext.FileName)} as {((Sound)resource.DataBlock).SoundType}", vrfGuiContext.FileName,
                        new ExportData { Resource = resource });

                    break;

                case ResourceType.World:
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as glTF",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext });
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as GLB",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext, FileType = ExportFileType.GLB });

                    var worldmeshTab = new TabPage("MAP");
                    worldmeshTab.Controls.Add(
                        new GLWorldViewer(vrfGuiContext, (World)resource.DataBlock).ViewerControl);
                    resTabs.TabPages.Add(worldmeshTab);
                    break;

                case ResourceType.WorldNode:
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as glTF",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext });
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as GLB",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext, FileType = ExportFileType.GLB });

                    var nodemeshTab = new TabPage("WORLD NODE");
                    nodemeshTab.Controls.Add(new GLWorldViewer(vrfGuiContext, (WorldNode)resource.DataBlock)
                        .ViewerControl);
                    resTabs.TabPages.Add(nodemeshTab);
                    break;

                case ResourceType.Model:
                    // The resource type is identified as model, ExportDel is a signature or *delegate*
                    // there are two functions here
                    //
                    //          ExportDel                   meaning 'export delegate' - possibly this is just the tooltip
                    //          ExportData
                    //
                    // new ExportDel(AddToExport)
                    //
                    // must be the function that is called


                    /*
                     *
                     * The Invoke methods takes two arguments
                     *
                     *          Delegate mathod, object[] args
                     *
                     * The arguments object[] args will be *passed* to the delegate
                     *
                     *
                     * The point of execution of the line line Program.MainForm.Invoke(..) happens
                     * then I open a file in the vpk browser
                     *
                     *
                     *
                     *
                     *
                     */
                    Program.MainForm.Invoke(
                                new ExportDel(AddToExport),
                                resTabs,
                                $"Export {Path.GetFileName(vrfGuiContext.FileName)} as glTF",
                                vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext }
                            );



                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as GLB",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext, FileType = ExportFileType.GLB });

                    var modelRendererTab = new TabPage("MODEL");
                    modelRendererTab.Controls.Add(new GLModelViewer(vrfGuiContext, (Model)resource.DataBlock)
                        .ViewerControl);
                    resTabs.TabPages.Add(modelRendererTab);
                    break;

                case ResourceType.Mesh:

                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as glTF",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext });
                    Program.MainForm.Invoke(new ExportDel(AddToExport), resTabs, $"Export {Path.GetFileName(vrfGuiContext.FileName)} as GLB",
                        vrfGuiContext.FileName, new ExportData { Resource = resource, VrfGuiContext = vrfGuiContext, FileType = ExportFileType.GLB });

                    var meshRendererTab = new TabPage("MESH");
                    meshRendererTab.Controls.Add(new GLModelViewer(vrfGuiContext, new Mesh(resource)).ViewerControl);
                    resTabs.TabPages.Add(meshRendererTab);
                    break;

                case ResourceType.Material:



                    // R: this stuff seems to be about rendering the material in the viewport
                    // but where is the place where the data-blocks get populated?

                    var materialViewerControl = new GLMaterialViewer();


                    materialViewerControl.Load += (_, __) => {
                        var material = vrfGuiContext.MaterialLoader.LoadMaterial(resource);
                        var materialRenderer = new MaterialRenderer(material);

                        materialViewerControl.AddRenderer(materialRenderer);
                    };



                    var materialRendererTab = new TabPage("MATERIAL");


                    materialRendererTab.Controls.Add(materialViewerControl.Control);


                    // R: so even with this disabled, the tabs for the dataentries seem to work
                    resTabs.TabPages.Add(materialRendererTab);



                    break;



                case ResourceType.PhysicsCollisionMesh:
                    var physRendererTab = new TabPage("PHYSICS");
                    physRendererTab.Controls.Add(new GLModelViewer(vrfGuiContext, (PhysAggregateData)resource.DataBlock).ViewerControl);
                    resTabs.TabPages.Add(physRendererTab);
                    break;
            }


            foreach (var block in resource.Blocks) {
                if (block.Type == BlockType.RERL) {
                    var externalRefsTab = new TabPage("External Refs");

                    var externalRefs = new DataGridView {
                        Dock = DockStyle.Fill,
                        AutoGenerateColumns = true,
                        AutoSize = true,
                        ReadOnly = true,
                        AllowUserToAddRows = false,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        DataSource =
                            new BindingSource(
                                new BindingList<ResourceExtRefList.ResourceReferenceInfo>(resource.ExternalReferences
                                    .ResourceRefInfoList), null),
                    };

                    externalRefsTab.Controls.Add(externalRefs);

                    resTabs.TabPages.Add(externalRefsTab);

                    continue;
                }

                if (block.Type == BlockType.NTRO) {
                    if (((ResourceIntrospectionManifest)block).ReferencedStructs.Count > 0) {
                        var externalRefsTab = new TabPage("Introspection Manifest: Structs");

                        var externalRefs = new DataGridView {
                            Dock = DockStyle.Fill,
                            AutoGenerateColumns = true,
                            AutoSize = true,
                            ReadOnly = true,
                            AllowUserToAddRows = false,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            DataSource =
                                new BindingSource(
                                    new BindingList<ResourceIntrospectionManifest.ResourceDiskStruct>(
                                        ((ResourceIntrospectionManifest)block).ReferencedStructs), null),
                        };

                        externalRefsTab.Controls.Add(externalRefs);
                        resTabs.TabPages.Add(externalRefsTab);
                    }

                    if (((ResourceIntrospectionManifest)block).ReferencedEnums.Count > 0) {
                        var externalRefsTab = new TabPage("Introspection Manifest: Enums");
                        var externalRefs2 = new DataGridView {
                            Dock = DockStyle.Fill,
                            AutoGenerateColumns = true,
                            AutoSize = true,
                            ReadOnly = true,
                            AllowUserToAddRows = false,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            DataSource =
                                new BindingSource(
                                    new BindingList<ResourceIntrospectionManifest.ResourceDiskEnum>(
                                        ((ResourceIntrospectionManifest)block).ReferencedEnums), null),
                        };

                        externalRefsTab.Controls.Add(externalRefs2);
                        resTabs.TabPages.Add(externalRefsTab);
                    }

                    //continue;
                }

                var tab2 = new TabPage(block.Type.ToString());
                try {
                    var control = new TextBox();
                    control.Font = new Font(FontFamily.GenericMonospace, control.Font.Size);

                    if (block.Type == BlockType.DATA) {
                        switch (resource.ResourceType) {
                            case ResourceType.Sound:
                                control.Text = Utils.Utils.NormalizeLineEndings(((Sound)block).ToString());
                                break;
                            case ResourceType.Particle:
                            case ResourceType.Mesh:
                                if (block is BinaryKV3 blockKeyvalues) {
                                    //Wrap it around a KV3File object to get the header.
                                    control.Text =
                                        Utils.Utils.NormalizeLineEndings(blockKeyvalues.GetKV3File().ToString());
                                } else {
                                    control.Text = Utils.Utils.NormalizeLineEndings(block.ToString());
                                }

                                break;
                            default:

                                control.Text = Utils.Utils.NormalizeLineEndings(block.ToString());

                                string dataresult = control.Text;
                                Debug.WriteLine(dataresult);

                                break;
                        }
                    } else {
                        control.Text = Utils.Utils.NormalizeLineEndings(block.ToString());
                    }

                    control.Dock = DockStyle.Fill;
                    control.Multiline = true;
                    control.ReadOnly = true;
                    control.ScrollBars = ScrollBars.Both;
                    tab2.Controls.Add(control);
                } catch (Exception e) {
                    Console.WriteLine(e);

                    var bv = new System.ComponentModel.Design.ByteViewer();
                    bv.Dock = DockStyle.Fill;
                    tab2.Controls.Add(bv);

                    Program.MainForm.Invoke((MethodInvoker)(() => {
                        resource.Reader.BaseStream.Position = block.Offset;
                        bv.SetBytes(resource.Reader.ReadBytes((int)block.Size));
                    }));
                }

                resTabs.TabPages.Add(tab2);
            }

            if (resource.ResourceType == ResourceType.PanoramaLayout
                || resource.ResourceType == ResourceType.PanoramaScript
                || resource.ResourceType == ResourceType.PanoramaStyle
                || resource.ResourceType == ResourceType.SoundEventScript
                || resource.ResourceType == ResourceType.SoundStackScript
                || resource.ResourceType == ResourceType.EntityLump) {
                foreach (TabPage tab2 in resTabs.TabPages) {
                    if (tab2.Text == "DATA") {
                        resTabs.SelectTab(tab2);
                        break;
                    }
                }
            }

            tab.Controls.Add(resTabs);

            return tab;
        }

        private delegate void ExportDel(Control control, string name, string filename, ExportData data);

        private void AddToExport(Control control, string name, string filename, ExportData data) {
            /*
             * The point of execution here ALSO happens when I open a file in the vpk browser
             * The same method, i.e. this method, is called regardless of resource type
             *
             * The method will be called TWICE. once for each of the options glTF and GLB
             *
             *      data.FileType = Auto              => glTF
             *      data.FileType = GLB               => GLB
             *
             *
             * name         is the name of the control          "Export armore_of_... as glTF"
             * filename     is the orginal name of the file     armor_of_reckless_vigor_weapon.vmdl_c
             *
             *
             * Also, ExportData data contain the resource type, i.e. the field
             *
             *      data.Resource
             *
             * The data.Resource at this point has already extracted the headers for the data
             * (which is used to populate the tabs in the GUI)
             *
             *
             *
             *
             */
            Program.MainForm.ExportToolStripButton.Enabled = true;

            var ts = new ToolStripMenuItem {
                Size = new Size(150, 20),
                // name = "Export armore of ... " is assigned to the Text part
                Text = name,

                // R: it looks like the tooptip is actually disabled, but if assigned as the filename makes it convenient
                // to retrieve when calling the ExportFile function
                ToolTipText = filename,

                // R: this hasn't got a declared type, or it's 'var' like the ToolStripMenuItem object ts
                // from the docs:
                // Tag is a standard ToolStripItem property used to store various stash
                // it will be used later to get the resource object when exporting
                Tag = data,
            };

            //This is required for the dialog to know the default name and path.
            //This makes it trivial to dump without exploring our nested TabPages.

            // R: this must be the event to execute on click?
            ts.Click += ExportToolStripMenuItem_Click;

            Program.MainForm.ExportToolStripButton.DropDownItems.Add(ts);

            void ControlExposed(object sender, EventArgs e) {
                control.Disposed -= ControlExposed;
                ts.Click -= ExportToolStripMenuItem_Click;
                Program.MainForm.ExportToolStripButton.DropDownItems.Remove(ts);

                if (Program.MainForm.ExportToolStripButton.DropDownItems.Count == 0) {
                    Program.MainForm.ExportToolStripButton.Enabled = false;
                }
            }

            control.Disposed += ControlExposed;
        }


        private void ExportToolStripMenuItem_Click(object sender, EventArgs e) {
            //ToolTipText is the full filename
            var menuItem = (ToolStripMenuItem)sender;
            var fileName = menuItem.ToolTipText;

            // R: exports the file, the var Tag object is cast to the expected type
            ExportFile.Export(fileName, menuItem.Tag as ExportData);
        }
    }
}
