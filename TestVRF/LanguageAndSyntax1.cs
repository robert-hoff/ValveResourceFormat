using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat;

namespace TestVRF {
    class LanguageAndSyntax1 {


        static void Mainz() {
            arrowOperator();
        }







        public static void printHexNumber() {
            Debug.WriteLine("0x{0:x8}", 123123123);
        }


        // instead of copy-pasting from here type 'for' and tab,tab
        static void forLoop() {
            for (int i = 0; i < 100; i++) {
                Debug.WriteLine(i);
            }
        }


        // found the syntax in GUI > Types > Exporter > ExportFile.cs
        // where it's part of the statement
        // extractDialog.OnProcess += (_,__) => {..}
        static void interestingStruct() {
            //object hihih += (_, __) => {
            //	Debug.WriteLine("hello!");
            //};
        }


        static void arrowOperator() {
            string[] words = { "bot", "apple", "apricot" };
            int minimalLength = words.Where(w => w.StartsWith("a")).Min(w => w.Length);
            // Debug.WriteLine(minimalLength);   // 5


            Debug.WriteLine(words.Where(w => w.StartsWith("a")));


            // to look at a result assign it to a var and insert a break point
            var t = words.Where(w => w.StartsWith("a"));


        }






        /*
         * _ and __ are valid names, but they have a special purpose in C#, see
         * https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/discards
         *
         *
         *
         */
        static void variablesWithUnderscore() {
            // int _ = 100;
            // Debug.WriteLine(_);


            // can assign the underscore a value without declaring it
            // but if assigned like this it can't be used, it is a discard variable
            // using discard variables can in some cases improve readability, for example
            // on interfaces where one is forced to handle unwanted data
            _ = 234;
        }




        /*
         * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
         */
        static void tupleTypes() {
            (double, int) my_tuple = (4.5, 3);
            Debug.WriteLine(my_tuple.Item1);
            Debug.WriteLine(my_tuple.Item2);
        }



        static void sliceExample() {
            byte[] myBytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Span<byte> dataspan = new Span<byte>(myBytes);
            Span<byte> blockStorage = dataspan.Slice(2, 8); // returns 8-length block indexes [2,9]

            for (int i = 0; i < 8; i++) {
                Debug.WriteLine("{0}", blockStorage[i]);
            }
        }


        static void runCrc32() {
            byte[] data = { 1, 2, 3 };
            uint val = Crc32.Compute(data);
            Debug.WriteLine("0x{0:x8}", val);
        }


    }



}


