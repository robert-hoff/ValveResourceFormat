using System;
using System.Collections.Generic;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0049 // Simplify Names
#pragma warning disable IDE0051 // Remove unused private members
namespace MyShaderAnalysis.compat.background
{
    public class CompatRules
    {
        public static void RunTrials()
        {
            Trial1MultiblendPcgl30Vs();
            // Trial1MultiblendPcgl30Ps();
        }

        static void Trial1MultiblendPcgl30Vs()
        {
            AddExlusion(2, 3);
            AddExlusion(3, 4);
            AddExlusion(3, 5);
            AddExlusion(3, 7);
            AddExlusion(3, 8);
            AddExlusion(7, 8);
            AddInclusion(5, 4);

            for (int i = 0; i < 512; i++)
            {
                Boolean exclude = false;
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
                if (!exclude)
                {
                    Console.WriteLine($"{i:x04}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }

        static List<(int, int)> exclusions = new();
        static List<(int, int)> inclusions = new();
        static List<(int, int, int)> inclusionsTriples = new();

        static void AddExlusion(int b0, int b1)
        {
            int num0 = 1 << (b0 - 1);
            int num1 = 1 << (b1 - 1);
            exclusions.Add((num0, num1));
        }

        static void AddInclusion(int b0, int b1)
        {
            int num0 = 1 << (b0 - 1);
            int num1 = 1 << (b1 - 1);
            inclusions.Add((num0, num1));
        }

        static void AddInclusionTriples(int b0, int b1, int b2)
        {
            int num0 = 1 << (b0 - 1);
            int num1 = 1 << (b1 - 1);
            int num2 = 1 << (b2 - 1);
            inclusionsTriples.Add((num0, num1, num2));
        }

        /*
         *
         * I'm supposed to exclude the following
         * 6152             1100000001000           13,12,4
         *
         * There are no examples of 13,12 together including just 4
         * There is the rule (12,4)
         *
         * So maybe if 4 *provides* for 12 then 13 will need a 5. So i.e. (12,4) is some kind of pair
         * working together, or think of 4 as acting for 12 and not available for another pairing
         *
         *
         *
         */
        static void Trial1MultiblendPcgl30Ps()
        {
            AddExlusion(1, 2);             // compat[0]
            AddExlusion(1, 3);             // compat[0]
                                           // AddExlusion(2, 3);             // uncertain if compat[0] implies this
            AddExlusion(2, 3);             // compat[1]
            AddExlusion(2, 4);             // compat[1]
            AddExlusion(3, 4);             // compat[1] (again uncertain)
            AddExlusion(2, 5);             // compat[2]
            AddExlusion(3, 5);             // compat[2]
            AddExlusion(2, 7);             // compat[4]
            AddExlusion(3, 7);             // compat[4]
            AddExlusion(2, 10);            // compat[8]
            AddExlusion(3, 10);            // compat[8]
            AddExlusion(2, 11);            // compat[9]
            AddExlusion(3, 11);            // compat[9]
            AddExlusion(2, 6);            // compat[23]
            AddExlusion(2, 17);            // compat[23]
            AddExlusion(6, 17);            // compat[23]
                                           // AddExlusion(6, 17);            // compat[26]         // already described in compat[23]

            // AddInclusionTriples(5,4,1);    // compat[3]
            AddInclusion(5, 4);               // compat[3]
                                              // AddInclusionTriples(7,4,1);    // compat[5]
            AddInclusion(7, 4);               // compat[5]
            AddInclusion(9, 8);               // compat[7]
                                              // AddInclusionTriples(12,4,1);   // compat[10]         (12,4,1)
            AddInclusion(12, 4);              // compat[10]
            AddInclusion(14, 5);           // compat[16]

            // interpret these two as 13 must include *either* 5 or 4
            // AddInclusion(13, 5);           // compat[12]
            // AddInclusion(13, 4);           // compat[13]
            AddInclusionTriples(13, 4, 5);   // compat[10]

            AddInclusion(15, 2);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // not sure what to do with these
            // 2,3,0                        // compat[6]
            // 13,6,2                       // compat[11]
            // 14,6,2                       // compat[14]
            // 15,6,2                       // compat[17]
            // 16,6,2                       // compat[20]
            // 17,9                         // compat[23]

            for (int i = 0; i < 262144; i++)
            {
                Boolean exclude = false;
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
                foreach ((int, int, int) incRule in inclusionsTriples)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    int b2 = incRule.Item3;
                    if ((i & b0) > 0 && (i & b1) == 0 && (i & b2) == 0)
                    {
                        exclude = true;
                    }
                }

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }
    }
}

