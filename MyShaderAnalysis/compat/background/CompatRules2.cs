using System;
using System.Collections.Generic;

namespace MyShaderAnalysis.compat.background
{
    public class CompatRules2
    {
        public static void RunTrials()
        {
            Trial1MultiblendPcgl30Ps();
            // Trial1GlobalLitSimpleVs();
            // Trial1GenericLightPcgl30Ps();
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

        static void Trial1GenericLightPcgl30Ps()
        {
            AddExlusion(1, 2);
            AddExlusion(1, 3);
            AddExlusion(2, 3); // these three described as (1,2,3) compat[0]
            AddInclusion(5, 4);

            // these is also compat[1] described as an exclusion of (0), but that doesn't seem to do anything ..

            for (int i = 0; i < 64; i++)
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
                    Console.WriteLine($"{i,3}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }

        /*
         *
         * for the file
         * \shaders\vfx\global_lit_simple_pcgl_30_vs.vcs
         *
         * For this example it's perfect
         *
         *
         *
         *
         *
         */
        static void Trial1GlobalLitSimpleVs()
        {
            AddExlusion(2, 3);
            AddExlusion(2, 4);
            AddExlusion(2, 6);
            AddExlusion(8, 9);
            AddExlusion(2, 10);

            AddInclusion(8, 7);
            AddInclusion(9, 7);

            // don't know what to do with this one
            // (2,1)        compat[3]

            for (int i = 0; i < 1024; i++)
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
                    Console.WriteLine($"{i,3}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }

        /*
         *
         * For the file multiblend_pcgl_30_ps.vcs
         *
         *
         *
         * I'm supposed to exclude the following
         * 6152             1100000001000           13,12,4
         *
         *
         * For some reason, if 12,13 appear together, then 4,5 must appear together also
         * The inclusion of 12 changes the behaviour of what 13 can appear with
         *
         * This doesn't appear to happen though with the inclusion of 7, for which the configuration
         * appears to be the same
         *
         *
         * the only other mention of 13 is the exclusion rule (13,6,2)
         *
         *
         * Q. is it possible that the exclusion rules from before apply?
         * Q. or it might be some kind of general rule. Or perhaps it is an impossible output for the vs
         * shader, and this is somehow inferred at an earlier stage?
         *
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

            AddInclusion(5, 4);               // compat[3]     (5,4) but it appears with an additionl 1 argument
            AddInclusion(7, 4);               // compat[5]     (7,4) but it appears with an additionl 1 argument
            AddInclusion(9, 8);               // compat[7]
            AddInclusion(12, 4);              // compat[10]    (12,4) but it appears with an additionl 1 argument
            AddInclusion(14, 5);              // compat[16]

            // interpret these two as 13 must include *either* 5 or 4
            // AddInclusion(13, 5);           // compat[12]
            // AddInclusion(13, 4);           // compat[13]

            AddInclusion(13, 4);     // inpterpretation of [12],[13] combined
                                     // AddInclusionTriples(13,5,4);

            AddInclusion(15, 2);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // not sure what to do with these (these are written as exclusions)
            // 2,3,0                        // compat[6]
            // 13,6,2                       // compat[11]
            // 14,6,2                       // compat[14]
            // 15,6,2                       // compat[17]
            // 16,6,2                       // compat[20]
            // 17,9                         // compat[23]

            // for (int i = 0; i < 262144; i++) {
            for (int i = 0; i < 10000; i++)
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

                // -- this is currently nonsense
                //foreach (var incRule in inclusionsTriples) {
                //    int b0 = incRule.Item1;
                //    int b1 = incRule.Item2;
                //    int b2 = incRule.Item3;
                //    if ((i&b0)>0 && (i&b1)>0 && (i&b2)==0) {
                //        exclude = true;
                //    }
                //}

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }
    }
}

