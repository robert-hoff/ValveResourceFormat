using System;
using System.IO;
using System.Diagnostics;

namespace MyShaderAnalysis2 {

    class Program {

        // This class is added to direct Console.WriteLine(..) statements to the Debug output window (inside Visual Studio)
        public class ToDebugWriter : StringWriter
        {
            public override void WriteLine(string str0)
            {
                Debug.WriteLine(str0);
            }
            public override void Write(string str0)
            {
                Debug.Write(str0);
            }
        }


        static void Main(string[] args) {
            Console.SetOut(new ToDebugWriter());
            // Console.WriteLine("Hello World!");






        }



    }
}








