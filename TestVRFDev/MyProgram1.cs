using System;
using System.Diagnostics;

[assembly: CLSCompliant(false)]
namespace TestVRFDev
{
    class MyProgram1
    {
        static void Mainz(string[] args)
        {
            int j = 100;

            String hexstring = String.Format("0x{0:x8}", 1410770216);
            Trace.WriteLine(hexstring);

            int i = 10;
            Trace.WriteLine("hi");

            MyProgram1 my_prog = new MyProgram1(111190);

            Trace.WriteLine(my_prog.my_fieldvar);

            DogDog growly_dog = new DogDog();
            growly_dog.bark("high");
        }

        public MyProgram1(int my_fieldvar)
        {
            this.my_fieldvar = my_fieldvar;
        }

        int my_fieldvar;
    }

    class MathsCat
    {
        public int mulmul(int a, int b)
        {
            return a * b;
        }
    }

    class DogDog
    {
        public int bark(string level)
        {
            Trace.WriteLine("BARK!");
            return 100000;
        }
    }
}

