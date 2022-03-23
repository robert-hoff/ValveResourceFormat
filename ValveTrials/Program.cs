using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValveTrials
{
    class Program
    {
        public class ToDebugWriter : StringWriter {
            public override void WriteLine(string str0)
            {
                Debug.WriteLine(str0);
            }
            public override void Write(string str0)
            {
                Debug.Write(str0);
            }
        }

        [STAThread]
        internal static void Main()
        {
            Console.SetOut(new ToDebugWriter());

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Form1 window = new();
            //Application.Run(window);

            ApplicationConfiguration.Initialize();
            Application.Run(new ByteViewerForm());

        }
    }
}
