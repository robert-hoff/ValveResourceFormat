using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValveTrials
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            mainTabs.TabPages.Clear();
            TabPage consoleTab = ConsoleTab.CreateTab(getContextMenu());
            mainTabs.TabPages.Add(consoleTab);
            Console.WriteLine($"VRF-Testing v{Application.ProductVersion}");
            Settings.Load();
            int left = Settings.Config.left == 0 ? 100 : Settings.Config.left;
            int top = Settings.Config.top == 0 ?  100 : Settings.Config.top;
            int width = Settings.Config.width == 0 ? 100 : Settings.Config.width;
            int height = Settings.Config.height == 0 ? 100 : Settings.Config.height;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(left, top);
            Size = new Size(width, height);
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TestVRF1.doIt2();
            // TestVRF1.doIt();
            // tabControl1.TabPages.Clear();

            (TabPage, TextBox) controlz = TestVRF1.makeANewTag();
            TabPage tab = controlz.Item1;
            mainTabs.TabPages.Add(tab);
            mainTabs.SelectTab(tab);
            // Console.WriteLine("new tab created!!!");

            controlz.Item2.DeselectAll();
            // tabControl1.Controls.
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Config.left = Left;
            Settings.Config.top = Top;
            Settings.Config.width = Width;
            Settings.Config.height = Height;
            Settings.Save();
            base.OnClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if the user presses CTRL + W, and there is a tab open, close the active tab
            if (keyData == (Keys.Control | Keys.W) && mainTabs.SelectedTab != null)
            {
                CloseTab(mainTabs.SelectedTab);
            }
            //if the user presses CTRL + Q, close all open tabs
            if (keyData == (Keys.Control | Keys.Q))
            {
                CloseAllTabs();
            }
            if (keyData == Keys.Escape)
            {
                Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnTabClick(object sender, MouseEventArgs e)
        {
            //Work out what tab we're interacting with
            var tabControl = sender as TabControl;
            var tabs = tabControl.TabPages;
            TabPage thisTab = tabs.Cast<TabPage>().Where((t, i) => tabControl.GetTabRect(i).Contains(e.Location)).First();
            if (e.Button == MouseButtons.Right)
            {
                // CloseTab(thisTab);
                var tabIndex = GetTabIndex(thisTab);
                //Can't close tabs to the left/right if there aren't any!
                closeToolStripMenuItemsToLeft.Visible = tabIndex > 1;
                closeToolStripMenuItemsToRight.Visible = tabIndex != mainTabs.TabPages.Count - 1;
                //For UX purposes, hide the option to close the console also (this is disabled later in code too)
                closeToolStripMenuItem.Visible = tabIndex != 0;
                //Show context menu at the mouse position
                contextMenuStrip1.Tag = e.Location;
                contextMenuStrip1.Show((Control)sender, e.Location);
            }
        }

        private int GetTabIndex(TabPage tab)
        {
            //Work out the index of the requested tab
            for (int i = 0; i < mainTabs.TabPages.Count; i++)
            {
                if (mainTabs.TabPages[i] == tab)
                {
                    return i;
                }
            }
            return -1;
        }

        private static TabPage FetchToolstripTabContext(object sender)
        {
            var contextMenu = ((ToolStripMenuItem)sender).Owner;
            var tabControl = ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl as TabControl;
            var tabs = tabControl.TabPages;

            return tabs.Cast<TabPage>().Where((t, i) => tabControl.GetTabRect(i).Contains((Point)contextMenu.Tag)).First();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseTab(FetchToolstripTabContext(sender));
        }

        private void CloseToolStripMenuItemsToLeft_Click(object sender, EventArgs e)
        {
            CloseTabsToLeft(FetchToolstripTabContext(sender));
        }

        private void CloseToolStripMenuItemsToRight_Click(object sender, EventArgs e)
        {
            CloseTabsToRight(FetchToolstripTabContext(sender));
        }

        private void CloseToolStripMenuItems_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
        }
        private void CloseTab(TabPage tab)
        {
            var tabIndex = GetTabIndex(tab);
            var isClosingCurrentTab = tabIndex == mainTabs.SelectedIndex;
            //The console cannot be closed!
            if (tabIndex == 0)
            {
                return;
            }
            //Close the requested tab
            Console.WriteLine($"Closing {tab.Text}");
            mainTabs.TabPages.Remove(tab);

            if (isClosingCurrentTab && tabIndex > 0)
            {
                mainTabs.SelectedIndex = tabIndex - 1;
            }
            // ShowHideSearch();
            tab.Dispose();
        }

        private void CloseAllTabs()
        {
            //Close all tabs currently open (excluding console)
            int tabCount = mainTabs.TabPages.Count;
            for (int i = 1; i < tabCount; i++)
            {
                CloseTab(mainTabs.TabPages[tabCount - i]);
            }
            // ShowHideSearch();
        }

        private void CloseTabsToLeft(TabPage basePage)
        {
            if (mainTabs.SelectedTab == null)
            {
                return;
            }
            //Close all tabs to the left of the base (excluding console)
            for (int i = GetTabIndex(basePage); i > 0; i--)
            {
                CloseTab(mainTabs.TabPages[i]);
            }
            // ShowHideSearch();
        }

        private void CloseTabsToRight(TabPage basePage)
        {
            if (mainTabs.SelectedTab == null)
            {
                return;
            }
            //Close all tabs to the right of the base one
            int tabCount = mainTabs.TabPages.Count;
            for (int i = 1; i < tabCount; i++)
            {
                if (mainTabs.TabPages[tabCount - i] == basePage)
                {
                    break;
                }
                CloseTab(mainTabs.TabPages[tabCount - i]);
            }
            // ShowHideSearch();
        }
    }
}
