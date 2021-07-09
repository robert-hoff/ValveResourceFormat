using System;
using System.Diagnostics;

[assembly: CLSCompliant(false)]
namespace TestVRF
{



  class Program
  {

    static void Main(string[] args)
    {



                        

      int j = 100;

      String hexstring = String.Format("0x{0:x8}", 1410770216);
      Trace.WriteLine(hexstring);

      int i = 10;
      Trace.WriteLine("hi");

      Program my_prog = new Program(90);

      Trace.WriteLine(my_prog.my_fieldvar);

      DogDog growly_dog = new DogDog();
      growly_dog.bark("high");

    }


    public Program(int my_fieldvar)
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



