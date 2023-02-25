using System;
using System.Diagnostics;
using MyCSharpLib;

namespace TestVRFDev.RunMyMathsLibDll
{
    // this can only be used together with the 'sizeof' method if calling method is declared as 'unsafe'
    // (which must also be specifed as a build option)
    struct myGoodStruct
    {
        int i;
        float f;
    }

    /*
     * The dll that is all was created in the project Z:\dev\dll\MyCSharpLib
     */
    internal class RunMyMathsLib
    {
        unsafe public static void Run()
        {
            Debug.WriteLine($"{Calculate.Add(2, 2)}");
            var i = sizeof(myGoodStruct);
            Debug.WriteLine($"{i}");
        }
    }
}

