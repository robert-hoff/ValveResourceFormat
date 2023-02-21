using System;
using System.Collections.Generic;

namespace MyShaderAnalysis.compat
{
    public class CompatRules4
    {
        public static void RunTrials()
        {
            // Trial1MultiblendPcgl30PsAttempt7();
            Trial1MultiblendPcgl30PsAttempt6();
        }

        /*
         *
         * Another interpretation of a 3 way rule such as (1,2,3) is
         *
         * 1 excludes both of 2,3, but doesn't say anything about 2,3 together
         * I feel this interpretation makes more sense, but it doesn't match the data
         *
         *
         *
         */
        static void Trial1MultiblendPcgl30PsAttempt7()
        {
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

            AddExclusion(1, 2, 3);             // compat[0]
            AddExclusion(2, 3, 4);             // compat[1]
            AddExclusion(2, 3, 5);             // compat[2]
            AddExclusion(2, 3, 7);             // compat[4]
            // AddExlusion(2, 3, 0);             // compat[6] - index 0 is removed
            AddExclusion(2, 3, 10);             // compat[8]
            AddExclusion(2, 3, 11);            // compat[9]
            AddExclusion(13, 6, 2);            // compat[11]
            AddExclusion(14, 6, 2);            // compat[14]
            AddExclusion(15, 6, 2);            // compat[17]
            AddExclusion(16, 6, 2);            // compat[20]
            AddExclusion(2, 6, 17);            // compat[23]
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
            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // for (int i = 0; i < 262144; i++) {
            // for (int i = 2048; i < 4096; i++) {
            for (int i = 0; i < 100; i++)
            {
                bool exclude = false;
                foreach ((int, int) exclRule in exclusions)
                {
                    int b0 = exclRule.Item1;
                    int b1 = exclRule.Item2;
                    if ((i & b0) > 0 && (i & b1) > 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int) incRule in inclusions)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    if ((i & b0) > 0 && (i & b1) == 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int, int) incRule in inclusionsTriple)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) == 0 && (i & b2) == 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int, int) incRule in exclusionsTriple)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) > 0 && (i & b2) > 0)
                    {
                        exclude = true;
                    }
                }

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }
            }
        }

        /*
         *
         * This interpretation says the three excluded parameters cannot occur together
         * (but doesn't seem accurate)
         *
         * But maybe on the provision that the rules from the vs file are carried forward?
         *
         *
         *
         */
        static void Trial1MultiblendPcgl30PsAttempt6()
        {
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

            AddExclusionTriple(1, 2, 3);             // compat[0]
            AddExclusionTriple(2, 3, 4);             // compat[1]
            AddExclusionTriple(2, 3, 5);             // compat[2]
            AddExclusionTriple(2, 3, 7);             // compat[4]
            // AddExclusionTriple(2, 3, 0);             // compat[6] - index 0 is removed
            AddExclusionTriple(2, 3, 10);             // compat[8]
            AddExclusionTriple(2, 3, 11);            // compat[9]
            AddExclusionTriple(13, 6, 2);            // compat[11]
            AddExclusionTriple(14, 6, 2);            // compat[14]
            AddExclusionTriple(15, 6, 2);            // compat[17]
            AddExclusionTriple(16, 6, 2);            // compat[20]
            AddExclusionTriple(2, 6, 17);            // compat[23]
            AddExlusion(9, 17);            // compat[24]
            AddExlusion(6, 17);            // compat[26]

            // inclusions
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            AddInclusionNoRemap(12, 3);    // compat[10] - with a double entry this 12->12   4->3
            AddInclusion(13, 5);           // compat[12]
            AddInclusion(13, 4);           // compat[13]
            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // including these from the vs file still leaves in (2,3) which the system doesn't want
            // [ 0]   3     S_MODE_FORWARD, S_MODE_DEPTH                         (1,2)
            // [ 1]   3     S_MODE_DEPTH, S_SHADER_QUALITY                       (2,4)
            // [ 2]   3     S_MODE_DEPTH, S_NORMAL_MAP                           (2,5)
            // [ 3]   2     S_NORMAL_MAP, S_SHADER_QUALITY                       this is the (5,4) rule
            // [ 4]   3     S_MODE_DEPTH, S_TOOLS_ENABLED                        assuming S_TOOLS_ENABLED is disabled
            // [ 5]   3     S_MODE_DEPTH, S_WORLDSPACE_UVS, S_SCROLL_WAVES       S_SCROLL_WAVES doesn't exist in ps
            AddExlusion(1, 2);
            AddExlusion(2, 4);
            AddExlusion(2, 5);

            // this isn't even close, so .. the only working interpretation of the exclusion rules
            // is that all three exclude each other

            // for (int i = 0; i < 262144; i++) {
            // for (int i = 2048; i < 4096; i++) {
            for (int i = 0; i < 100; i++)
            {
                bool exclude = false;
                foreach ((int, int) exclRule in exclusions)
                {
                    int b0 = exclRule.Item1;
                    int b1 = exclRule.Item2;
                    if ((i & b0) > 0 && (i & b1) > 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int) incRule in inclusions)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    if ((i & b0) > 0 && (i & b1) == 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int, int) incRule in inclusionsTriple)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) == 0 && (i & b2) == 0)
                    {
                        exclude = true;
                    }
                }
                foreach ((int, int, int) incRule in exclusionsTriple)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) > 0 && (i & b2) > 0)
                    {
                        exclude = true;
                    }
                }

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }
            }
        }

        static List<(int, int)> exclusions = new();
        static List<(int, int)> inclusions = new();
        static List<(int, int, int)> inclusionsTriple = new();
        static List<(int, int, int)> exclusionsTriple = new();
        static Dictionary<int, int> remappingTable = new();

        static void AddExlusionNoRemap(int b0, int b1)
        {
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            exclusions.Add((num0, num1));
        }

        static void AddExlusion(int b0, int b1)
        {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            if (b0 == -1 || b1 == -1)
            {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            exclusions.Add((num0, num1));
        }

        static void AddExclusion(int b0, int b1, int b2)
        {
            AddExlusion(b0, b1);
            AddExlusion(b0, b2);
        }

        static void AddInclusion(int b0, int b1)
        {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            if (b0 == -1 || b1 == -1)
            {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            inclusions.Add((num0, num1));
        }

        static void AddInclusionNoRemap(int b0, int b1)
        {
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            inclusions.Add((num0, num1));
        }

        static void AddInclusionTriple(int b0, int b1, int b2)
        {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            b2 = remappingTable[b2];
            if (b0 == -1 || b1 == -1 || b2 == -1)
            {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            int num2 = 1 << (b2);
            inclusionsTriple.Add((num0, num1, num2));
        }

        static void AddExclusionTriple(int b0, int b1, int b2)
        {
            b0 = remappingTable[b0];
            b1 = remappingTable[b1];
            b2 = remappingTable[b2];
            if (b0 == -1 || b1 == -1 || b2 == -1)
            {
                return;
            }
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            int num2 = 1 << (b2);
            exclusionsTriple.Add((num0, num1, num2));
        }
    }
}

