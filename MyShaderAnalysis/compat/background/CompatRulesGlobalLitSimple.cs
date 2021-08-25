using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyShaderAnalysis
{
    public class CompatRulesGlobalLitSimple
    {




        public static void RunTrials()
        {
            Trial1GLS_30_Vs();
        }





        static void Trial1GLS_30_Vs()
        {
            // R: funny these seem to be repeated, am I missing something?

            // [ 2]   EXC(3)    2,1             S_MODE_DEPTH, S_TOOLS_ENABLED
            // [ 0]   EXC(3)    2,3             S_MODE_DEPTH, S_MODE_FORWARD
            // [ 1]   EXC(3)    2,4             S_MODE_DEPTH, S_NORMAL_MAP
            // [ 3]   EXC(3)    2,6             S_MODE_DEPTH, S_PAINT_VERTEX_COLORS
            // [ 7]   EXC(3)    2,10            S_MODE_DEPTH, S_AO_MAP
            // [ 6]   EXC(3)    8,9             S_TRAVELLING_WIND_OBJECT_X, S_TRAVELLING_WIND_OBJECT_Z
            // [10]   EXC(3)    8,9             S_TRAVELLING_WIND_OBJECT_X, S_TRAVELLING_WIND_OBJECT_Z
            // [ 5]   INC(2)    9,7             S_TRAVELLING_WIND_OBJECT_Z, S_ENABLE_WIND
            // [ 9]   INC(2)    9,7             S_TRAVELLING_WIND_OBJECT_Z, S_ENABLE_WIND
            // [ 4]   INC(2)    8,7             S_TRAVELLING_WIND_OBJECT_X, S_ENABLE_WIND
            // [ 8]   INC(2)    8,7             S_TRAVELLING_WIND_OBJECT_X, S_ENABLE_WIND


            remappingTable.Add(0, 0);   // S_TRANSFORM_CONSTANT_BUFFER
            remappingTable.Add(1, -1);    // S_TOOLS_ENABLED             removed
            remappingTable.Add(2, 1);    // S_MODE_DEPTH                 mapped to 1
            remappingTable.Add(3, 2);    // S_MODE_FORWARD               mapped to 2
            remappingTable.Add(4, 3);    // S_NORMAL_MAP                 mapped to 3
            remappingTable.Add(5, 4);    // S_SPECULAR                   mapped to 4
            remappingTable.Add(6, 5);    // S_PAINT_VERTEX_COLORS        mapped to 5
            remappingTable.Add(7, 6);    // S_ENABLE_WIND                mapped to 6
            remappingTable.Add(8, 7);    // S_TRAVELLING_WIND_OBJECT_X   mapped to 7
            remappingTable.Add(9, 8);    // S_TRAVELLING_WIND_OBJECT_Z   mapped to 8
            remappingTable.Add(10, 9);   // S_AO_MAP                     mapped to 9



            AddExlusion(2, 1);             // compat[]
            AddExlusion(2, 3);             // compat[]
            AddExlusion(2, 4);             // compat[]
            AddExlusion(2, 6);             // compat[]
            AddExlusion(2, 10);             // compat[]
            AddExlusion(8, 9);             // compat[]
            AddInclusion(8, 7);           // compat[]
            AddInclusion(9, 7);           // compat[]



            for (int i = 0; i < 1024; i++)
            {
                // for (int i = 6152; i < 50000; i++) {
                // for (int i = 2048; i < 4096; i++) {
                // for (int i = 0; i < 5000; i++) {
                bool exclude = false;
                foreach (var exclRule in exclusions)
                {
                    int b0 = exclRule.Item1;
                    int b1 = exclRule.Item2;
                    if ((i & b0) > 0 && (i & b1) > 0)
                    {
                        exclude = true;
                    }
                }
                foreach (var incRule in inclusions)
                {
                    int b0 = incRule.Item1;
                    int b1 = incRule.Item2;
                    if ((i & b0) > 0 && (i & b1) == 0)
                    {
                        exclude = true;
                    }
                }

                foreach (var incRule in inclusionsTriple)
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





