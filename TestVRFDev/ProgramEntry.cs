using System;
using System.Diagnostics;
using System.IO;
using TestVRFDev.RunMyMathsLibDll;

#pragma warning disable CA2000 // Dispose objects before losing scope
namespace TestVRFDev
{
    public static class ProgramEntry
    {
        private class ToDebugWriter : StringWriter
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

        public static void Main()
        {
            Console.SetOut(new ToDebugWriter());
            RunMyMathsLib.Run();
        }
    }
}

