using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Forms;
using GUI.Utils;
using SteamDatabase.ValvePak;

namespace GUI.Controls
{
    /// <summary>
    /// Represents a user control in which a TreeView and ListView are used to view a directory/file listing. In addition to a normal TreeView,
    /// this control allows for searching to occur within the TreeView and have the results displayed in a ListView with details about the resulting
    /// items.
    /// </summary>
    public partial class TreeViewWithSearchResults : UserControl
    {
        private readonly ImageList imageList;
        public bool DeletedFilesRecovered { get; private set; }

        public event TreeNodeMouseClickEventHandler TreeNodeMouseDoubleClick; // when a TreeNode is double clicked
        public event TreeNodeMouseClickEventHandler TreeNodeRightClick; // when a TreeNode is single clicked
        public event EventHandler<ListViewItemClickEventArgs> ListViewItemDoubleClick; // when a ListViewItem is double clicked
        public event EventHandler<ListViewItemClickEventArgs> ListViewItemRightClick; // when a ListViewItem is single clicked

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewWithSearchResults"/> class.
        /// Constructor to require an image list for display on listed TreeView nodes and ListView items.
        /// </summary>
        /// <param name="imageList">Image list.</param>
        public TreeViewWithSearchResults(ImageList imageList)
            : this()
        {
            this.imageList = imageList;
            Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewWithSearchResults"/> class.
        /// Require a default constructor for the designer.
        /// </summary>
        private TreeViewWithSearchResults()
        {
            InitializeComponent();

            mainListView.MouseDoubleClick += MainListView_MouseDoubleClick;
            mainListView.MouseDown += MainListView_MouseDown;
            mainListView.Resize += MainListView_Resize;
            mainListView.Disposed += MainListView_Disposed;
            mainListView.FullRowSelect = true;

            mainTreeView.HideSelection = false;
            mainTreeView.NodeMouseDoubleClick += MainTreeView_NodeMouseDoubleClick;
            mainTreeView.NodeMouseClick += MainTreeView_NodeMouseClick;
            mainTreeView.AfterSelect += MainTreeView_AfterSelect;
        }

        private void MainListView_Disposed(object sender, EventArgs e)
        {
            mainListView.MouseDoubleClick -= MainListView_MouseDoubleClick;
            mainListView.MouseDown -= MainListView_MouseDown;
            mainListView.Resize -= MainListView_Resize;
            mainListView.Disposed -= MainListView_Disposed;

            mainTreeView.NodeMouseDoubleClick -= MainTreeView_NodeMouseDoubleClick;
            mainTreeView.NodeMouseClick -= MainTreeView_NodeMouseClick;
            mainTreeView.AfterSelect -= MainTreeView_AfterSelect;

            ((VrfGuiContext)mainTreeView.Tag).Dispose();
            mainTreeView.Tag = null;
            mainTreeView = null;
            mainListView = null;
        }

        private void MainTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNodeMouseDoubleClick?.Invoke(sender, e);
        }

        private void MainTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // if user selected a folder, show the contents of that folder in the list view
            if (e.Action != TreeViewAction.Unknown && ((VrfTreeViewData)e.Node.Tag).IsFolder)
            {
                mainListView.BeginUpdate();
                mainListView.Items.Clear();

                foreach (TreeNode node in e.Node.Nodes)
                {
                    AddNodeToListView(node);
                }

                mainListView.EndUpdate();
            }
        }

        private void MainTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mainTreeView.SelectedNode = e.Node;

                TreeNodeRightClick?.Invoke(sender, e);
            }
        }

        private void MainListView_Resize(object sender, EventArgs e)
        {
            mainListView.BeginUpdate();
            ResizeListViewColumns();
            mainListView.EndUpdate();
        }

        private void ResizeListViewColumns()
        {
            foreach (ColumnHeader col in mainListView.Columns)
            {
                if (col.Text == "Name")
                {
                    col.Width = mainListView.ClientSize.Width - (mainListView.Columns.Count - 1) * 100;
                }
                else
                {
                    col.Width = 100;
                }
            }
        }

        /// <summary>
        /// Initializes the TreeView in the control with the contents of the passed Package. Contents are sorted and expanded by default.
        /// </summary>
        internal void InitializeTreeViewFromPackage(VrfGuiContext vrfGuiContext)
        {
            mainListView.Tag = vrfGuiContext;

            var control = mainTreeView;
            control.BeginUpdate();
            control.PathSeparator = Package.DirectorySeparatorChar.ToString();
            control.Name = "treeViewVpk";
            control.Tag = vrfGuiContext; // so we can access it later
            control.Dock = DockStyle.Fill;
            control.ImageList = imageList;
            control.ShowRootLines = false;

            control.GenerateIconList(vrfGuiContext.CurrentPackage.Entries.Keys.ToList());

            var name = Path.GetFileName(vrfGuiContext.FileName);
            var root = control.Nodes.Add("root", name, "vpk", "vpk");
            root.Tag = VrfTreeViewData.MakeFolder(vrfGuiContext.CurrentPackage.Entries.Count);
            root.Expand();

            control.TreeViewNodeSorter = new TreeViewFileSorter();

            foreach (var fileType in vrfGuiContext.CurrentPackage.Entries)
            {
                foreach (var file in fileType.Value)
                {
                    control.AddFileNode(root, file);
                }
            }

            // Expand lone folders (common in maps vpks)
            var node = root;

            while (node.Nodes.Count == 1)
            {
                node = node.Nodes[0];
                node.Expand();
            }

            control.EndUpdate();
        }

        internal void RecoverDeletedFiles()
        {
            DeletedFilesRecovered = true;

            using var progressDialog = new GenericProgressForm
            {
                Text = "Scanning for deleted files..."
            };
            progressDialog.OnProcess += (_, __) =>
            {
                progressDialog.SetProgress("Scanning for deleted files, this may take a while...");

                var vrfGuiContext = (VrfGuiContext)mainListView.Tag;
                var foundFiles = Types.Viewers.Package.RecoverDeletedFiles(vrfGuiContext.CurrentPackage);

                Invoke((MethodInvoker)(() =>
                {
                    if (foundFiles.Count == 0)
                    {
                        var rootEmpty = mainTreeView.Nodes.Add(Types.Viewers.Package.DELETED_FILES_FOLDER, "No deleted files found", "_deleted", "_deleted");
                        rootEmpty.Tag = VrfTreeViewData.MakeFolder(0);
                        return;
                    }

                    mainTreeView.BeginUpdate();
                    var root = mainTreeView.Nodes.Add(Types.Viewers.Package.DELETED_FILES_FOLDER, $"Deleted files ({foundFiles.Count} files found)", "_deleted", "_deleted");
                    root.Tag = VrfTreeViewData.MakeFolder(foundFiles.Count);

                    var vpkName = Path.GetFileName(vrfGuiContext.CurrentPackage.FileName);

                    foreach (var file in foundFiles)
                    {
                        mainTreeView.AddFileNode(root, file, skipDeletedRootFolder: true);
                    }

                    root.Expand();
                    mainTreeView.EndUpdate();
                }));
            };
            progressDialog.ShowDialog();
        }

        /// <summary>
        /// Performs a search for the entered text and search types. Before a search is performed, the contents of the ListView (previous search results) are cleared.
        /// Results of whatever search function is used are displayed in the ListView with name, file size, and file type.
        /// </summary>
        /// <param name="searchText">Value to search for in the TreeView. Matching on this value is based on the search type.</param>
        /// <param name="selectedSearchType">Determines the matching of the value. For example, full/partial text search or full path search.</param>
        internal void SearchAndFillResults(string searchText, SearchType selectedSearchType)
        {
            mainListView.BeginUpdate();
            mainListView.Items.Clear();

            var results = mainTreeView.Search(searchText, selectedSearchType);

            foreach (var node in results)
            {
                AddNodeToListView(node);
            }

            ResizeListViewColumns();

            mainListView.EndUpdate();
        }

        /// <summary>
        /// When the user clicks in the ListView, check if the user clicks outside of a ListViewItem. If so, de-select any previously selected ListViewItems. In addition,
        /// if the user right clicked an item in the ListView, let our subscribers know what was clicked and where in case a context menu is needed to be shown.
        /// </summary>
        /// <param name="sender">Object which raised event.</param>
        /// <param name="e">Event data.</param>
        private void MainListView_MouseDown(object sender, MouseEventArgs e)
        {
            var info = mainListView.HitTest(e.X, e.Y);

            // if an item was clicked in the list view
            if (info.Item != null)
            {
                // right click should just notify our subscribers
                if (e.Button == MouseButtons.Right)
                {
                    ListViewItemRightClick?.Invoke(sender, new ListViewItemClickEventArgs(info.Item, e.Location));
                }
                else if (e.Button == MouseButtons.Left)
                {
                    // left click should focus the node in its tree view
                    var node = info.Item.Tag as TreeNode;
                    if (((VrfTreeViewData)node.Tag).IsFolder)
                    {
                        node.EnsureVisible();
                        node.TreeView.SelectedNode = node;
                    }
                }
            }
            else
            {
                mainListView.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// If the user double clicks (with left mouse button) on a ListViewItem, send up an event to subscribers that such an action has occurred. Also send up
        /// whatever object is represented by the ListViewItem.
        /// </summary>
        /// <param name="sender">Object which raised event.</param>
        /// <param name="e">Event data.</param>
        private void MainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var info = mainListView.HitTest(e.X, e.Y);

                if (info.Item != null)
                {
                    // if user left double clicks a folder, open its contents and display in list view
                    var node = info.Item.Tag as TreeNode;
                    if (((VrfTreeViewData)node.Tag).IsFolder)
                    {
                        node.Expand();
                        mainListView.BeginUpdate();
                        mainListView.Items.Clear();
                        foreach (TreeNode childNode in node.Nodes)
                        {
                            AddNodeToListView(childNode);
                        }
                        mainListView.EndUpdate();
                    }

                    ListViewItemDoubleClick?.Invoke(sender, new ListViewItemClickEventArgs(info.Item.Tag));
                }
                else
                {
                    mainListView.SelectedItems.Clear();
                }
            }
        }

        /// <summary>
        /// When the form loads, create the columns that we want to see such as name, file size, and file type.
        /// </summary>
        /// <param name="sender">Object which raised event.</param>
        /// <param name="e">Event data.</param>
        private void TreeViewWithSearchResults_Load(object sender, EventArgs e)
        {
            mainListView.Columns.Add("Name");
            mainListView.Columns.Add("Size");
            mainListView.Columns.Add("Type");
            mainListView.SmallImageList = imageList;
        }

        private void AddNodeToListView(TreeNode node)
        {
            var item = new ListViewItem(node.Text)
            {
                ImageKey = node.ImageKey,
                Tag = node,
            };

            var data = (VrfTreeViewData)node.Tag;

            if (!data.IsFolder)
            {
                var file = data.PackageEntry;
                item.SubItems.Add(file.TotalLength.ToFileSizeString());
                item.SubItems.Add(file.TypeName);
            }
            else
            {
                item.SubItems.Add($"{data.ItemCount} items");
                item.SubItems.Add("folder");
            }

            mainListView.Items.Add(item);
        }
    }
}
