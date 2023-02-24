using System;
using System.Collections.Generic;

namespace MyShaderAnalysis.compat.background
{
    public class CompatRules3
    {
        public static void RunTrials()
        {
            // Trial1MultiblendPcgl30VsAttempt2();

            // THIS IS MY BEST INTERPRATION YET - DON'T CHANGE IT
            Trial1MultiblendPcgl30PsAttempt5();

            // Trial1MultiblendPcgl30PsAttempt4();

            // Console.WriteLine($"{1<<100}");
        }

        /*
         * It looks like S_LAYER_BORDER_TINT might have two entries
         *
         * This seemed promising and is correct up until 17f8
         *
         *                          765432210987654321
         * 6136    17f8           00000001011111111000
         * 6152    1808           00000001100000001000
         *
         * The problematic entry is
         *
         * 6152    1808           00000001100000001000
         *
         * The next entry should be
         *
         *         1818           00000001100000011000
         *
         * thinking more about this layer stuff, the system DOES want the generation of
         *
         *         1008           00000001000000001000
         *
         * which the code is currently agreeing with. Here I'm interpreting the top bit as somehow
         * related to S_LAYER_BORDER_TINT on layer 2 (or something like that)
         *
         *
         *
         * Now, if this double layer thing appears together, as in 1808. The system seems to want both
         * 4,5 to be present.
         *
         * The system is not liking this either
         *
         *                           65432210987654321
         * 6200    1838           00000001100000111000
         *
         * Which is the presence of a 6. Now, 6 has no mention of layers like the 4 and 5 rules.
         *
         *
         *
         *
         *
         *
         *
         *
         *
         *
         * Bute the SYSTEM WANTS this one. Why? 6 should be excluded by all the parallax layers
         * with exclusion rules of this type (16,6,2)
         *
         *          65432210987654321
         *       00011000000000000000
         *
         * If S_PARALLAX_MAP_ON_2 and S_PARALLAX_MAP_ON_3 operate on different layers this doesn't
         * seem to be expressed in the data. Maybe it's known externally, but if so, why is
         * S_LAYER_BORDER_TINT expressed with layer information?
         *
         *
         *
         *
         *
         *
         *
         *
         *
         */
        static void Trial1MultiblendPcgl30PsAttempt5()
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
            remappingTable.Add(14, 14);  // S_PARALLAX_MAP_ON_1       mapped to 13
            remappingTable.Add(15, 15);  // S_PARALLAX_MAP_ON_2       mapped to 14
            remappingTable.Add(16, 16);  // S_PARALLAX_MAP_ON_3       mapped to 15
            remappingTable.Add(17, 17);  // S_GLOBAL_TINT             mapped to 16

            // remappingTable.Add(18, 11);  // S_LAYER_BORDER_TINT       mapped to 11

            AddExclusion(1, 2);             // compat[0]
            AddExclusion(1, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[1]
            AddExclusion(2, 4);             // compat[1]
            AddExclusion(3, 4);             // compat[1]
            // AddExlusion(2, 3);          // compat[2] - already listed
            AddExclusion(2, 5);             // compat[2]
            AddExclusion(3, 5);             // compat[2]
            // AddExlusion(2, 3);          // compat[4] - already listed
            AddExclusion(2, 7);             // compat[4]
            AddExclusion(3, 7);             // compat[4]
            // AddExlusion(2, 0);          // compat[6] - index 0 is removed
            // AddExlusion(3, 0);          // compat[6] - index 0 is removed
            // AddExlusion(2, 3);          // compat[6] - already listed
            // AddExlusion(2, 3);          // compat[8] - already listed
            AddExclusion(2, 10);            // compat[8]
            AddExclusion(3, 10);            // compat[8]
            // AddExlusion(2, 3);          // compat[9] - already listed
            AddExclusion(2, 11);            // compat[9]
            AddExclusion(3, 11);            // compat[9]
            AddExclusion(2, 6);             // compat[11]
            AddExclusion(2, 13);            // compat[11]
            AddExclusion(6, 13);            // compat[11]
            // AddExlusion(2, 6);          // compat[14] - already listed
            AddExclusion(2, 14);            // compat[14]
            AddExclusion(6, 14);            // compat[14]

            // AddExlusion(2, 6);             // compat[17] - already listed
            AddExclusion(2, 15);            // compat[17]
            AddExclusion(6, 15);            // compat[17]

            // AddExlusion(2, 6);             // compat[20] - already listed
            AddExclusion(2, 16);            // compat[20]
            AddExclusion(6, 16);            // compat[20]

            // AddExlusion(2, 17);            // compat[23] - already listed
            AddExclusion(2, 17);            // compat[23]
            AddExclusion(6, 17);            // compat[23]

            AddExclusion(9, 17);            // compat[24]
            AddExclusion(6, 17);            // compat[24]

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

            // ------------- LATEST EFFORTS
            /*
             *
             * I'm still not supposed to generate this
             *
             *
             *      432210987654321
             * 00000001100000001000
             *
             * but I think I see something
             *
             * the parameter that is doubling up is called
             *
             *          S_LAYER_BORDER_TINT
             *
             * and there are 3 include rules that have something to do with layers
             *
             *      (5,4)           1           <-- can this 1 mean 'this rule only applies to layer 1' ????
             *      (7,4)           1
             *     (12,4)           1
             *
             * In the descriptions this argument written as 0,1,2 or 0,1 reader 'requires less than 3, or less than 2 layers'
             *
             *
             *
             *
             *
             *
             *
             *
             *
             * it's something like, parameter 4 can only be used in one layer
             * the above thing shouldn't be generated because S_LAYER_BORDER_TINT in this double sense
             *
             * hm, I can't see anything fall out of that directly though ...........
             *
             *
             * There are other problems too, later it wasnt me to generate
             *
             * with the remapping and double 12 entry, these would be parameters 16 and 15
             *
             *
             *    65432210987654321
             * 00011000000000000000
             *
             * I assume these would be S_PARALLAX_MAP_ON_3 and S_PARALLAX_MAP_ON_2 which sounds like a layer thing
             *
             * There are additional parameter or rules in the ps file though .. these might have something to do with it also
             *
             * UNKBLOCK[2]
             * INC(2)   2,13,14,15,16
             *
             *
             *
             *
             *
             *
             *
             *
             *
             */

            // ------------- BEFORE DOUBLE ENTRY

            // I'm not generating this index, which I'm supposed to. This is based on the rule
            // EXC(3)    13,6,2          S_PARALLAX_MAP_ON_0, S_RENDER_BACKFACES, S_MODE_DEPTH

            // 76543210987654321
            // 00001000000101000

            // And this number is weird, it seems the rule imply that 13 requires both 5 and 4. Not either.
            //       43210987654321
            // 00000001000000001000

            // The latest problem is that I'm generating this. Which the system doesn't want
            // the exclusion of this rule makes sense to me though because of EXC(13,5)

            //       43210987654321
            // 00000001100000001000

            // my theory earlier was that '12 consumes 4' so now 13 would need 5, but the (5,4) (7,4) inclusion rules
            // contradict this idea because the system includes

            //       43210987654321
            // 00000001000001001000

            // IS IT POSSIBLE THAT 12 IS REPEATED in two places? the pattern seems to suggest this
            // I'm not getting this, why not
            //      432210987654321
            // 00000001000000001000

            for (int i = 0; i < 262144; i++)
            {
                // for (int i = 2048; i < 4096; i++) {
                // for (int i = 0; i < 5000; i++) {
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

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }
            }
        }

        static void Trial1MultiblendPcgl30PsAttempt4()
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
            remappingTable.Add(13, 12);  // S_PARALLAX_MAP_ON_0       mapped to 12
            remappingTable.Add(14, 13);  // S_PARALLAX_MAP_ON_2       mapped to 13
            remappingTable.Add(15, 14);  // S_PARALLAX_MAP_ON_3       mapped to 14
            remappingTable.Add(16, 15);  // S_PARALLAX_MAP_ON_4       mapped to 15
            remappingTable.Add(17, 16);  // S_GLOBAL_TINT             mapped to 16

            AddExclusion(1, 2);             // compat[0]
            AddExclusion(1, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[1]
            AddExclusion(2, 4);             // compat[1]
            AddExclusion(3, 4);             // compat[1]
            // AddExlusion(2, 3);             // compat[2] - already listed
            AddExclusion(2, 5);             // compat[2]
            AddExclusion(3, 5);             // compat[2]
            // AddExlusion(2, 3);             // compat[4] - already listed
            AddExclusion(2, 7);             // compat[4]
            AddExclusion(3, 7);             // compat[4]
            // AddExlusion(2, 0);             // compat[6] - index 0 is removed
            // AddExlusion(3, 0);             // compat[6] - index 0 is removed
            // AddExlusion(2, 3);             // compat[6] - already listed
            // AddExlusion(2, 3);            // compat[8] - already listed
            AddExclusion(2, 10);            // compat[8]
            AddExclusion(3, 10);            // compat[8]
            // AddExlusion(2, 3);            // compat[9] - already listed
            AddExclusion(2, 11);            // compat[9]
            AddExclusion(3, 11);            // compat[9]
            // AddExlusion(2, 6);             // compat[11]
            // AddExlusion(2, 13);            // compat[11]
            // AddExlusion(6, 13);            // compat[11]
            // AddExlusion(2, 6);             // compat[14] - already listed
            // AddExlusion(2, 14);             // compat[14]
            // AddExlusion(6, 14);             // compat[14]

            // haven't written in [17],[20] and [24] because they are written as decreasing
            // trying some variations
            AddExclusion(2, 6);

            AddExclusion(2, 17);            // compat[23]
            AddExclusion(6, 17);            // compat[23]

            // inclusions
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            // AddInclusion(13, 5);           // compat[12]
            // AddInclusion(13, 4);           // compat[13]

            AddInclusionTriple(13, 4, 5);     // rule [12] and [13] interpreted as 13 can be fulfilled with either 4 or 5

            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // I'm not generating this index, which I'm supposed to. This is based on the rule
            // EXC(3)    13,6,2          S_PARALLAX_MAP_ON_0, S_RENDER_BACKFACES, S_MODE_DEPTH

            // 76543210987654321
            // 00001000000101000

            // And this number is weird, it seems the rule imply that 13 requires both 5 and 4. Not either.
            //       43210987654321
            // 00000001000000001000

            // The latest problem is that I'm generating this. Which the system doesn't want
            // the exclusion of this rule makes sense to me though because of EXC(13,5)

            //       43210987654321
            // 00000001100000001000

            // my theory earlier was that '12 consumes 4' so now 13 would need 5, but the (5,4) (7,4) inclusion rules
            // contradict this idea because the system includes

            //       43210987654321
            // 00000001000001001000

            // IS IT POSSIBLE THAT 12 IS REPEATED in two places? the pattern seems to suggest this

            // for (int i = 0; i < 262144; i++) {
            for (int i = 0; i < 20000; i++)
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

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }
            }
        }

        static bool CheckExclusion(int num)
        {
            Console.WriteLine($"{num}");
            Console.WriteLine($"{Convert.ToString(num, 2).PadLeft(20, '0')}");
            return true;
        }

        /*
         *
         *                              (S_TOOLS_ENABLED) index removed
         *   0    S_MODE_FORWARD
         *   1    S_MODE_DEPTH
         *   2    S_MODE_TOOLS_WIREFRAME
         *   3    S_SHADER_QUALITY
         *   4    S_NORMAL_MAP
         *   5    S_RENDER_BACKFACES
         *   6    S_SPECULAR
         *   7    S_WORLDSPACE_UVS
         *   8    S_SHOW_WORLDSPACE_BLEND
         *   9    S_TINT_MASK
         *  10    S_TINT_MASK_2
         *  11    GHOST_PARAM
         *  11    S_LAYER_BORDER_TINT
         *  12    S_PARALLAX_MAP_ON_0
         *  13    S_PARALLAX_MAP_ON_1
         *  14    S_PARALLAX_MAP_ON_2
         *  15    S_PARALLAX_MAP_ON_3
         *  16    S_GLOBAL_TINT
         *
         *
         *
         *
         */
        static void Trial1MultiblendPcgl30PsAttempt3()
        {
            //  0    S_TOOLS_ENABLED
            //  1    S_MODE_FORWARD
            //  2    S_MODE_DEPTH
            //  3    S_MODE_TOOLS_WIREFRAME
            //  4    S_SHADER_QUALITY
            //  5    S_NORMAL_MAP
            //  6    S_RENDER_BACKFACES
            //  7    S_SPECULAR
            //  8    S_WORLDSPACE_UVS
            //  9    S_SHOW_WORLDSPACE_BLEND
            // 10    S_TINT_MASK
            // 11    S_TINT_MASK_2
            // 12    S_LAYER_BORDER_TINT
            // 13    S_PARALLAX_MAP_ON_0
            // 14    S_PARALLAX_MAP_ON_1
            // 15    S_PARALLAX_MAP_ON_2
            // 16    S_PARALLAX_MAP_ON_3
            // 17    S_GLOBAL_TINT

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
            remappingTable.Add(13, 12);  // S_PARALLAX_MAP_ON_0       mapped to 12
            remappingTable.Add(14, 13);  // S_PARALLAX_MAP_ON_2       mapped to 13
            remappingTable.Add(15, 14);  // S_PARALLAX_MAP_ON_3       mapped to 14
            remappingTable.Add(16, 15);  // S_PARALLAX_MAP_ON_4       mapped to 15
            remappingTable.Add(17, 16);  // S_GLOBAL_TINT             mapped to 16

            // I'm not generating this zframe, which the system wants
            // so what if I add a ghost parameter at position 12 or 13 (this doesn't seem to do anything beneficial)
            //      43 210987654321

            // 00000001000000001000

            //       43210987654321
            // 00000001000000001000
            // This isn't generated because 13 is specified as depending on 5
            //
            // I'd say the logical interpretation is that (13,4),(13,5) taken together must mean
            // 13 can be fulfilled with either 4 or 5. Which kind of makes sense

            // YES - this does get me further (can now generate the first 337 frames correctly)

            // next I'm not generating, but this is because 13 and 6 are excluded ... ?
            //       43210987654321
            // 00000001000000101000

            // if we remap where the parallax starts I get this thing being included
            // which makes perfect sense ...
            //
            // NOTE a remapping system should presumably always display such a number
            //
            // 76543_210987654321
            // 000001000000000000

            AddExclusion(1, 2);             // compat[0]
            AddExclusion(1, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[0]
            AddExclusion(2, 3);             // compat[1]
            AddExclusion(2, 4);             // compat[1]
            AddExclusion(3, 4);             // compat[1]
            // AddExlusion(2, 3);             // compat[2] - already listed
            AddExclusion(2, 5);             // compat[2]
            AddExclusion(3, 5);             // compat[2]
            // AddExlusion(2, 3);             // compat[4] - already listed
            AddExclusion(2, 7);             // compat[4]
            AddExclusion(3, 7);             // compat[4]
            // AddExlusion(2, 0);             // compat[6] - index 0 is removed
            // AddExlusion(3, 0);             // compat[6] - index 0 is removed
            // AddExlusion(2, 3);             // compat[6] - already listed
            // AddExlusion(2, 3);            // compat[8] - already listed
            AddExclusion(2, 10);            // compat[8]
            AddExclusion(3, 10);            // compat[8]
            // AddExlusion(2, 3);            // compat[9] - already listed
            AddExclusion(2, 11);            // compat[9]
            AddExclusion(3, 11);            // compat[9]
            AddExclusion(2, 6);             // compat[11]
            AddExclusion(2, 13);            // compat[11]
            AddExclusion(6, 13);            // compat[11]
            // AddExlusion(2, 6);             // compat[14] - already listed
            AddExclusion(2, 14);             // compat[14]
            AddExclusion(6, 14);             // compat[14]

            AddExclusion(2, 17);            // compat[23]
            AddExclusion(6, 17);            // compat[23]

            // inclusions
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            // AddInclusion(13, 5);           // compat[12]
            // AddInclusion(13, 4);           // compat[13]

            AddInclusionTriple(13, 4, 5);     // rule [12] and [13] interpreted as 13 can be fulfilled with either 4 or 5

            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            // for (int i = 0; i < 262144; i++) {
            for (int i = 0; i < 20000; i++)
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

                if (!exclude)
                {
                    Console.WriteLine($"{i,3}    {i:x04}           {Convert.ToString(i, 2).PadLeft(20, '0')}");
                }
            }
        }

        static void Trial1MultiblendPcgl30VsAttempt2()
        {
            remappingTable.Add(0, 0);   // S_TRANSFORM_CONSTANT_BUFFER   mapped to 0
            remappingTable.Add(1, -1);   // S_TOOLS_ENABLED removed
            remappingTable.Add(2, 1);   // S_MODE_FORWARD                mapped to 1
            remappingTable.Add(3, 2);   // S_MODE_DEPTH                  mapped to 2
            remappingTable.Add(4, 3);   // S_SHADER_QUALITY              mapped to 3
            remappingTable.Add(5, 4);   // S_NORMAL_MAP                  mapped to 4
            remappingTable.Add(6, 5);   // S_SPECULAR                    mapped to 5
            remappingTable.Add(7, 6);   // S_SCROLL_WAVES                mapped to 6
            remappingTable.Add(8, 7);   // S_WORLDSPACE_UVS              mapped to 7
            remappingTable.Add(9, 8);   // S_NO_VERTEX_COLOR             mapped to 8

            AddExclusion(2, 3);
            AddExclusion(3, 4);
            AddExclusion(3, 5);
            AddExclusion(3, 1);
            AddExclusion(3, 7);
            AddExclusion(3, 8);
            AddExclusion(7, 8);
            AddInclusion(5, 4);

            for (int i = 0; i < 512; i++)
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
                if (!exclude)
                {
                    Console.WriteLine($"{i:x04}               {Convert.ToString(i, 2).PadLeft(9, '0')}");
                }
            }
        }

        // static int index_removed = 999;
        // static int index_added = 999;

        static List<(int, int)> exclusions = new();
        static List<(int, int)> inclusions = new();
        static List<(int, int, int)> inclusionsTriple = new();
        static Dictionary<int, int> remappingTable = new();

        static void AddExlusionNoRemap(int b0, int b1)
        {
            int num0 = 1 << (b0);
            int num1 = 1 << (b1);
            exclusions.Add((num0, num1));
        }

        static void AddExclusion(int b0, int b1)
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
    }
}


