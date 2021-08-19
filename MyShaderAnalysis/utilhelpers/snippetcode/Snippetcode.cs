using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyShaderAnalysis.utilhelpers.snippetcode {

    class Snippetcode {


        public static void RunTrials() {
            // PrintByteCounter();
            // TestDictionaryLookups();
            // ZframeLabel();
        }



        static void ZframeLabel() {
            // zframes ids range from 0 to large numbers, the largest known id is
            // 0x018cbfb1 or 26001329
            // a standardised way to print them then is using a 8-length hex representation
            // Debug.WriteLine(0x018cbfb1);
            Debug.WriteLine($"{60:x08}");
        }



        /*
         *
         * To keep track of zframes use a sorted dictionary.
         * Can either look up zframes based on the index they appear in the file, or based on their ID
         *
         * I'm not sure I need the order they appear in the file though (possibly not - but it doesn't matter)
         *
         */
        static void TestDictionaryLookups() {
            SortedDictionary<int, int> myDict = new();
            myDict.Add(0, 0);
            myDict.Add(1, 1);
            myDict.Add(2, 2);
            myDict.Add(4, 4);
            myDict.Add(8, 8);
            Debug.WriteLine(myDict.ElementAt(3));
            Debug.WriteLine(myDict[4]);
        }



        static void PrintByteCounter() {

            for (int i = 0; i < 8; i++) {
                Debug.WriteLine($"({i * 32 + 216}) 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            }

        }

    }



}









