using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCSharpLib;

namespace TestVRFDev {

    // this can only be used together with the 'sizeof' method if calling method is declared as 'unsafe'
    // (which must also be specifed as a build option)
    struct myGoodStruct
    {
        int i;
        float f;
    }

    class ProgramEntry {
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



        unsafe static void Main() {

            Console.SetOut(new ToDebugWriter());


            // Console.WriteLine($"hello");
            Console.WriteLine($"{Calculate.Add(2, 2)}");

            var i = sizeof(myGoodStruct);
            Console.WriteLine($"{i}");


        }

    }
}
