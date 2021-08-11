using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis {
    public class CompatRulesWater {




        public static void RunTrials() {
            Trial1Water_30_Ps();
        }




        /*
         *
         * focusing on water_dota_pcgl_30_ps.vcs
         *
         * system doesn't want
         *
         *          09887654321
         * 00000000000100000000
         *
         *
         * again the 8 arguments takes two positions. And this is specified in the SF block description
         *
         * but the system doesn't want this either, wait .. is it possible that we need 3 positions for 8?
         * then it would be correct again
         *
         *         098887654321
         * 00000000001000000000
         *
         *
         *
         *
         */
        static void Trial1Water_30_Ps() {



            remappingTable.Add(0, -1);    // S_TOOLS_ENABLED
            remappingTable.Add(1, 0);   // S_MODE_FORWARD
            remappingTable.Add(2, 1);    // S_MODE_DEPTH
            remappingTable.Add(3, 2);    // S_MODE_TOOLS_WIREFRAME
            remappingTable.Add(4, 3);    // S_RENDER_BACKFACES
            remappingTable.Add(5, 4);    // S_FRESNEL
            remappingTable.Add(6, 5);    // S_FLOW_NORMALS
            remappingTable.Add(7, 6);    // S_FLOW_COLOR
            remappingTable.Add(8, 7);    // S_FLOW_DEBUG
            remappingTable.Add(9, 10);    // S_USE_FOG_COLOR
            remappingTable.Add(10, 11);   // S_FORCE_CHEAP_WATER
            remappingTable.Add(11, 12);   // S_FORCE_NULL_WATER



            AddExlusion(1, 3);             // compat[]
            AddExlusion(1, 2);             // compat[]
            AddExlusion(2, 3);             // compat[]
            AddExlusion(2, 6);             // compat[]
            AddExlusion(3, 6);             // compat[]
            AddExlusion(2, 7);             // compat[]
            AddExlusion(3, 7);             // compat[]
            AddExlusion(2, 8);             // compat[]
            AddExlusion(3, 8);             // compat[]
            AddExlusion(2, 9);             // compat[]
            AddExlusion(3, 9);             // compat[]
            AddExlusion(10,11);            // compat[]


            //AddInclusionNoRemap(8, 5);
            //int num0 = 1 << (9);
            //int num1 = 1 << (5);
            //inclusions.Add((num0, num1));


            AddInclusion(8, 6);            // compat[]
            AddInclusion(7, 6);            // compat[]



            for (int i = 0; i < 4096; i++) {
            // for (int i = 0; i < 20; i++) {
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





