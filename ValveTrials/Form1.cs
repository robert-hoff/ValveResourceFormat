using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValveTrials {

	public partial class Form1 : Form {

		public Form1() {
			InitializeComponent();
			tabControl1.TabPages.Clear();
			TabPage consoleTab = ConsoleTab.CreateTab(getContextMenu());
			tabControl1.TabPages.Add(consoleTab);
			Console.WriteLine($"VRF-Testing v{Application.ProductVersion}");
		}

		private void goToolStripMenuItem_Click(object sender, EventArgs e) {
			TestVRF1.doIt();
		}
	}
}









