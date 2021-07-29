using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyShaderAnalysis {


    class ProgramEntry {

        static void Main() {




            ShaderAnalysis2.RunTrials();
            // ShaderAnalysis1.RunTrials();
            // ShaderTesting.RunTrials();

            // Murmur32.RunTrials();






        }


        static void TestGenericSort() {
            // SortThis testClass = new SortThis(12);
            // Debug.WriteLine(testClass.val);

            List<SortThis> testlist = new();
            testlist.Add(new SortThis(2));
            testlist.Add(new SortThis(-10));
            testlist.Add(new SortThis(10));

            testlist.Sort();

            Debug.WriteLine(testlist[0].val);
        }




        class SortThis : IComparable<SortThis> {

            public int val;
            public SortThis(int val) {
                this.val = val;
            }

            public int CompareTo(SortThis other) {
                return val-other.val;
            }
        }






    }




}
