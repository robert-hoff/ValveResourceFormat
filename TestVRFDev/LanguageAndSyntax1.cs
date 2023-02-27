using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestVRFDev
{
    public static class LanguageAndSyntax1
    {
        public static void Mainz()
        {
            ArrowOperator();
        }

        public static void TestGenericSort()
        {
            // SortThis testClass = new SortThis(12);
            // Debug.WriteLine(testClass.val);

            List<SortThis> testlist = new()
            {
                new SortThis(2),
                new SortThis(-10),
                new SortThis(10)
            };
            testlist.Sort();
            Debug.WriteLine(testlist[0].val);
        }

        private class SortThis : IComparable<SortThis>
        {
            public int val;
            public SortThis(int val)
            {
                this.val = val;
            }

            public int CompareTo(SortThis other)
            {
                return val - other.val;
            }
        }

        public static void PrintFloatNumber()
        {
            Debug.WriteLine("{0:0.00}", 9.2);
        }

        public static void PrintHexNumber()
        {
            Debug.WriteLine("0x{0:x8}", 123123123);
        }

        // instead of copy-pasting from here type 'for' and tab,tab
        public static void ForLoop()
        {
            for (int i = 0; i < 100; i++)
            {
                Debug.WriteLine(i);
            }
        }

        // found the syntax in GUI > Types > Exporter > ExportFile.cs
        // where it's part of the statement
        // extractDialog.OnProcess += (_,__) => {..}
        public static void InterestingStruct()
        {
            //object hihih += (_, __) => {
            //	Debug.WriteLine("hello!");
            //};
        }

        public static void ArrowOperator()
        {
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
        public static void VariablesWithUnderscore()
        {
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
        public static void TupleTypes()
        {
            (double, int) my_tuple = (4.5, 3);
            Debug.WriteLine(my_tuple.Item1);
            Debug.WriteLine(my_tuple.Item2);
        }

        public static void SliceExample()
        {
            byte[] myBytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Span<byte> dataspan = new Span<byte>(myBytes);
            Span<byte> blockStorage = dataspan.Slice(2, 8); // returns 8-length block indexes [2,9]

            for (int i = 0; i < 8; i++)
            {
                Debug.WriteLine("{0}", blockStorage[i]);
            }
        }

        public static void RunCrc32()
        {
            byte[] data = { 1, 2, 3 };
            uint val = Crc32.Compute(data);
            Debug.WriteLine("0x{0:x8}", val);
        }
    }
}

