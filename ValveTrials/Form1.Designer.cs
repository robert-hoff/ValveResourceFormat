
namespace ValveTrials {
	partial class Form1 {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.mainTabs = new System.Windows.Forms.TabControl();
			this.myContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItemsToRight = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItemsToLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItems = new System.Windows.Forms.ToolStripMenuItem();

			this.goToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.myContextMenuStrip.SuspendLayout();
			this.SuspendLayout();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			// 
			// mainTabs
			// 
			this.mainTabs.Location = new System.Drawing.Point(1, -5);
			this.mainTabs.Name = "tabControl1";
			this.mainTabs.SelectedIndex = 0;
			this.mainTabs.Size = new System.Drawing.Size(803, 454);
			this.mainTabs.TabIndex = 0;

            this.mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mainTabs.Location = new System.Drawing.Point(0, 24);
            this.mainTabs.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.Padding = new System.Drawing.Point(0, 0);
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(1101, 508);
            this.mainTabs.TabIndex = 1;
            this.mainTabs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnTabClick);
			// 
			// contextMenuStrip1
			// 
			this.myContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.goToolStripMenuItem});
			this.myContextMenuStrip.Name = "contextMenuStrip1";
			this.myContextMenuStrip.Size = new System.Drawing.Size(181, 48);
			// 
			// goToolStripMenuItem
			// 
			this.goToolStripMenuItem.Name = "goToolStripMenuItem";
			this.goToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.goToolStripMenuItem.Text = "go!";
			this.goToolStripMenuItem.Click += new System.EventHandler(this.goToolStripMenuItem_Click);

            // R: copied all the 'close to the right-left' nonsense in from VRF
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.closeToolStripMenuItemsToRight,
            this.closeToolStripMenuItemsToLeft,
            this.closeToolStripMenuItems});
            this.contextMenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(234, 124);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(233, 30);
            this.closeToolStripMenuItem.Text = "Close this &tab";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItemsToRight
            // 
            this.closeToolStripMenuItemsToRight.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItemsToRight.Image")));
            this.closeToolStripMenuItemsToRight.Name = "closeToolStripMenuItemsToRight";
            this.closeToolStripMenuItemsToRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.closeToolStripMenuItemsToRight.Size = new System.Drawing.Size(233, 30);
            this.closeToolStripMenuItemsToRight.Text = "Close all tabs to &right";
            this.closeToolStripMenuItemsToRight.Click += new System.EventHandler(this.CloseToolStripMenuItemsToRight_Click);
            // 
            // closeToolStripMenuItemsToLeft
            // 
            this.closeToolStripMenuItemsToLeft.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItemsToLeft.Image")));
            this.closeToolStripMenuItemsToLeft.Name = "closeToolStripMenuItemsToLeft";
            this.closeToolStripMenuItemsToLeft.Size = new System.Drawing.Size(233, 30);
            this.closeToolStripMenuItemsToLeft.Text = "Close all tabs to &left";
            this.closeToolStripMenuItemsToLeft.Click += new System.EventHandler(this.CloseToolStripMenuItemsToLeft_Click);
            // 
            // closeToolStripMenuItems
            // 
            this.closeToolStripMenuItems.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItems.Image")));
            this.closeToolStripMenuItems.Name = "closeToolStripMenuItems";
            this.closeToolStripMenuItems.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.closeToolStripMenuItems.Size = new System.Drawing.Size(233, 30);
            this.closeToolStripMenuItems.Text = "Close &all tabs";
            this.closeToolStripMenuItems.Click += new System.EventHandler(this.CloseToolStripMenuItems_Click);            


            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 532);
            // this.Controls.Add(this.mainTabs);
			this.Controls.Add(this.mainTabs); // R: mine
            // this.Controls.Add(this.menuStrip1);
            // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            // this.MainMenuStrip = this.menuStrip1;
            this.myContextMenuStrip.ResumeLayout(false); // R: mine
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(347, 340);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            // this.Load += new System.EventHandler(this.MainForm_Load);
            // this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            // this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // this.menuStrip1.ResumeLayout(false);
            // this.menuStrip1.PerformLayout();
            this.myContextMenuStrip.ResumeLayout(false);
            // this.vpkContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
		}


		public System.Windows.Forms.ContextMenuStrip getContextMenu() {
			return myContextMenuStrip;
		}

		#endregion
		private System.Windows.Forms.TabControl mainTabs;
		private System.Windows.Forms.ContextMenuStrip myContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem goToolStripMenuItem;

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItemsToLeft;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItemsToRight;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItems;
	}
}
