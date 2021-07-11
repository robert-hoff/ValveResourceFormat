using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestVRFWin {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();

           mainTabs.TabPages.Clear();
           TabPage consoleTab = ConsoleTab.CreateTab();
           mainTabs.TabPages.Add(consoleTab);
           Console.WriteLine($"VRF v{Application.ProductVersion}");


        }

        void Tabs_Selected(object sender, TabControlEventArgs e)
        {

        }



        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TestVRF1.doIt();


        }


        void Removepicture_Click()
        {

        }




    }



}
