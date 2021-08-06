using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis {
    public class CompatRules4 {


        public static void RunTrials() {
            Trial1MultiblendPcgl30PsAttempt6();
        }






        /*
         * It looks like S_LAYER_BORDER_TINT might have two entries
         *
         *
         */
        static void Trial1MultiblendPcgl30PsAttempt6() {

            remappingTable.Add(0, -1);   // S_TOOLS_ENABLED removed
            remappingTable.Add(1, 0);    // S_MODE_FORWARD            mapped to 0
            remappingTable.Add(2, 1);    // S_MODE_DEPTH              mapped to 1
            remappingTable.Add(3, 2);    // S_MODE_TOOLS_WIREFRAME    mapped to 2
            remappingTable.Add(4, 3);    // S_SHADER_QUALITY          mapped to 3
            remappingTable.Add(5, 4);    // S_NORMAL_MAP              mapped to 4
            remappingTable.Add(6, 5);    // S_RENDER_BACKFACES        mapped to 5
            remappingTable.Add(7, 6);    // S_SPECULAR                mapped to 6
            remappingTable.Add(8, 7);    // S_WORLDSPACE_UVS          mapped to 7
            remappingTable.Add(9, 8);    // S_SHOW_WORLDSPACE_BLEND   mapped to 8
            remappingTable.Add(10, 9);   // S_TINT_MASK               mapped to 9
            remappingTable.Add(11, 10);  // S_TINT_MASK_2             mapped to 10
            remappingTable.Add(12, 11);  // S_LAYER_BORDER_TINT       mapped to 11
            remappingTable.Add(13, 13);  // S_PARALLAX_MAP_ON_0       mapped to 12
            remappingTable.Add(14, 14);  // S_PARALLAX_MAP_ON_2       mapped to 13
            remappingTable.Add(15, 15);  // S_PARALLAX_MAP_ON_3       mapped to 14
            remappingTable.Add(16, 16);  // S_PARALLAX_MAP_ON_4       mapped to 15
            remappingTable.Add(17, 17);  // S_GLOBAL_TINT             mapped to 16

            // remappingTable.Add(18, 11);  // S_LAYER_BORDER_TINT       mapped to 11


            AddExlusion(1, 2);             // compat[0]
            AddExlusion(1, 3);             // compat[0]
            AddExlusion(2, 3);             // compat[0]
            AddExlusion(2, 3);             // compat[1]
            AddExlusion(2, 4);             // compat[1]
            AddExlusion(3, 4);             // compat[1]
            // AddExlusion(2, 3);          // compat[2] - already listed
            AddExlusion(2, 5);             // compat[2]
            AddExlusion(3, 5);             // compat[2]
            // AddExlusion(2, 3);          // compat[4] - already listed
            AddExlusion(2, 7);             // compat[4]
            AddExlusion(3, 7);             // compat[4]
            // AddExlusion(2, 0);          // compat[6] - index 0 is removed
            // AddExlusion(3, 0);          // compat[6] - index 0 is removed
            // AddExlusion(2, 3);          // compat[6] - already listed
            // AddExlusion(2, 3);          // compat[8] - already listed
            AddExlusion(2, 10);            // compat[8]
            AddExlusion(3, 10);            // compat[8]
            // AddExlusion(2, 3);          // compat[9] - already listed
            AddExlusion(2, 11);            // compat[9]
            AddExlusion(3, 11);            // compat[9]
            AddExlusion(2, 6);             // compat[11]
            AddExlusion(2, 13);            // compat[11]
            AddExlusion(6, 13);            // compat[11]
            // AddExlusion(2, 6);          // compat[14] - already listed
            AddExlusion(2, 14);            // compat[14]
            AddExlusion(6, 14);            // compat[14]

            // AddExlusion(2, 6);             // compat[17] - already listed
            AddExlusion(2, 15);            // compat[17]
            AddExlusion(6, 15);            // compat[17]

            // AddExlusion(2, 6);             // compat[20] - already listed
            AddExlusion(2, 16);            // compat[20]
            AddExlusion(6, 16);            // compat[20]

            // AddExlusion(2, 17);            // compat[23] - already listed
            AddExlusion(2, 17);            // compat[23]
            AddExlusion(6, 17);            // compat[23]

            AddExlusion(9, 17);            // compat[24]
            AddExlusion(6, 17);            // compat[24]



            // inclusions
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            AddInclusionNoRemap(12, 3);    // compat[10] - with a double entry this 12->12   4->3

            AddInclusion(13, 5);           // compat[12]
            AddInclusion(13, 4);           // compat[13]

            // AddInclusionTriple(13, 4, 5);     // rule [12] and [13] interpreted as 13 can be fulfilled with either 4 or 5

            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]






            for (int i = 0; i < 262144; i++) {
            // for (int i = 2048; i < 4096; i++) {
            // for (int i = 0; i < 5000; i++) {
                bool exclude = false;
                foreach (var exclRule in exclusions) {
                    int b0 = exclRule.Item1;
                    int b1 = exclRule.Item2;
                    if ((i & b0) > 0 && (i & b1) > 0) {
                        exclude = true;
                    }
                }
                foreach (var incRule in inclusions) {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    if ((i & b0) > 0 && (i & b1) == 0) {
                        exclude = true;
                    }
                }

                foreach (var incRule in inclusionsTriple) {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) == 0 && (i & b2) == 0) {
                        exclude = true;
                    }
                }

                if (!exclude) {
                    Debug.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }

            }
        }





        static List<(int, int)> exclusions = new();
        static List<(int, int)> inclusions = new();
        static List<(int, int, int)> inclusionsTriple = new();
        static Dictionary<int, int> remappingTable = new();


        static void AddExlusionNoRemap(int b0, int b1) {
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            exclusions.Add((num0, num1));
        }



        static void AddExlusion(int b0, int b1) {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            if (b0 == -1 || b1 == -1) {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            exclusions.Add((num0, num1));
        }

        static void AddInclusion(int b0, int b1) {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            if (b0 == -1 || b1 == -1) {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            inclusions.Add((num0, num1));
        }


        static void AddInclusionNoRemap(int b0, int b1) {
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            inclusions.Add((num0, num1));
        }


        static void AddInclusionTriple(int b0, int b1, int b2) {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            b2 = remappingTable[b2];
            if (b0 == -1 || b1 == -1 || b2 == -1) {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            int num2 = 1 << (b2);
            inclusionsTriple.Add((num0, num1, num2));
        }







    }
}




