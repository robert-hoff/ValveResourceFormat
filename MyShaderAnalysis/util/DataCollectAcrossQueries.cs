using System;
using System.Collections.Generic;

namespace MyShaderAnalysis.util
{
    public class DataCollectAcrossQueries
    {
        // --- collect int and string mechanisms

        public static SortedDictionary<int, int> collectValuesInt = new();
        public static SortedDictionary<string, int> collectValuesString = new();

        public static void CollectIntValue(int val)
        {
            int currIterator = collectValuesInt.GetValueOrDefault(val, 0);
            collectValuesInt[val] = currIterator + 1;
        }

        public static void CollectStringValue(string val)
        {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;
        }

        public static void PrintReport(bool showCount = true)
        {
            List<int> intvalues = new();
            foreach (int i in collectValuesInt.Keys)
            {
                intvalues.Add(i);
            }
            intvalues.Sort();
            foreach (int i in intvalues)
            {
                if (showCount)
                {
                    collectValuesInt.TryGetValue(i, out int instanceCount);
                    Console.WriteLine($"{i,5}        {instanceCount,3}");
                }
                else
                {
                    Console.WriteLine($"{i}");
                }
            }

            List<string> stringvalues = new();
            foreach (string s in collectValuesString.Keys)
            {
                stringvalues.Add(s);
            }
            stringvalues.Sort();
            foreach (string s in stringvalues)
            {
                if (showCount)
                {
                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Console.WriteLine($"{s.PadRight(120)}        {instanceCount,3}");
                }
                else
                {
                    Console.WriteLine($"{s}");
                }
            }
            collectValuesInt = new();
            collectValuesString = new();
        }
    }
}


