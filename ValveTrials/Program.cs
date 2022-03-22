using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValveTrials
{
    class Program
    {
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 window = new();
            Application.Run(window);


            // ApplicationConfiguration.Initialize();
            // Application.Run(new ByteViewerForm());

        }
    }
}
