using System;
using System.Diagnostics;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0060 // Remove unused parameter
[assembly: CLSCompliant(false)]
namespace TestVRFDev
{
    public class MyProgram1
    {
        public static void Mainz(string[] args)
        {
            int j = 100;
            string hexstring = string.Format("0x{0:x8}", 1410770216);
            Trace.WriteLine(hexstring);
            int i = 10;
            Trace.WriteLine("hi");
            MyProgram1 my_prog = new MyProgram1(111190);

            Trace.WriteLine(my_prog.my_fieldvar);

            DogDog growly_dog = new DogDog();
            growly_dog.Bark("high");
        }

        public MyProgram1(int my_fieldvar)
        {
            this.my_fieldvar = my_fieldvar;
        }

        int my_fieldvar;
    }

    class MathsCat
    {
        public static int Mulmul(int a, int b)
        {
            return a * b;
        }
    }

    class DogDog
    {
        private string BarkSound = "BARK!";
        public int Bark(string level)
        {
            Trace.WriteLine(BarkSound);
            return 100000;
        }
    }
}

