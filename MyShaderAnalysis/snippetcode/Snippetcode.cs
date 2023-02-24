using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyShaderFile.CompiledShader;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0033 // Use explicitly provided tuple name
#pragma warning disable IDE0039 // Use local function
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0071 // Simplify interpolation
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable CS0219 // Variable is assigned but its value is never used
namespace MyShaderAnalysis.snippetcode
{
    class Snippetcode
    {
        public static void RunTrials()
        {
            IteratorExample();
            // BytesToString();
            // ByteCheck();
            // DumbTest6();
            // GettingGsDataOut5();
            // GettingGsDataOut4();
            // GettingGsDataOut3();
            // GettingGsDataOut2();
            // GettingGsDataOut1();
            // AddTwoNumbers();
            // ConvertIntToByteString();
            // DumbTest();
            // DumbTest2();
            // TestDictionaryStats();
            // ConvertHexMapToString();
            // PrintByteCounter();
            // TestDictionaryLookups();
            // ZframeLabel();
        }

        public static void IteratorExample()
        {
            foreach (int number in MyIterator())
            {
                Console.WriteLine($"{number}");
            }
        }

        public static IEnumerable MyIterator()
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine($"returning double {i}");
                yield return i * 2;
            }
        }

        static void BytesToString()
        {
            // int BREAKLEN = 32;
            int BREAKLEN = 12;
            byte[] databytes = { 1, 2, 3, 40, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < databytes.Length; i++)
            {
                if (i > 0 && i % BREAKLEN == 0)
                {
                    sb.Append('\n');
                }
                sb.Append($"{databytes[i]:X02} ");
            }
            Console.WriteLine($"{sb.ToString()}");
        }

        public static string BytesToString(byte[] databytes, int breakLen = 32)
        {
            if (databytes == null || databytes.Length == 0)
            {
                return "";
            }
            if (breakLen == -1)
            {
                breakLen = int.MaxValue;
            }
            int count = 0;
            string bytestring = "";
            for (int i = 0; i < databytes.Length; i++)
            {
                bytestring += $"{databytes[i]:X02} ";
                if (++count % breakLen == 0)
                {
                    bytestring += "\n";
                }
            }
            return bytestring.Trim();
        }

        static void ByteCheck()
        {
            Console.WriteLine($"byte check");

            // byte[] input = Array.Empty<byte>();
            // byte[] input = { 1, 2, 3, 4 };
            byte[] input = { 1, 2, 3, 0 };

            // if the byte array contains any instane of 0 this will not trigger
            if (!input.Contains<byte>(0x00))
            {
                Console.WriteLine($"hello");
            }
        }

        static void DumbTest6()
        {
            static int add(int x, int y) => x + y;
            Console.WriteLine($"{add(10, 20)}");
        }

        static void GettingGsDataOut5()
        {
            string filenamepath = "debugoverlay_wireframe_pc_40_gs.vcs";
            var fetch = (string a, out VcsProgramType p1, out VcsPlatformType p2, out VcsShaderModelType p3) =>
            {
                p1 = ComputeVCSFileName(a).Item1; p2 = ComputeVCSFileName(a).Item2; p3 = ComputeVCSFileName(a).Item3;
            };
            fetch(filenamepath, out VcsProgramType p1, out VcsPlatformType p2, out VcsShaderModelType p3);
            Console.WriteLine($"{p1}");
            Console.WriteLine($"{p2}");
            Console.WriteLine($"{p3}");
        }

        static void GettingGsDataOut4()
        {
            string filenamepath = "debugoverlay_wireframe_pc_40_gs.vcs";
            var fetch = (string a, out VcsProgramType p1, out VcsPlatformType p2, out VcsShaderModelType p3) =>
            {
                (VcsProgramType ProgramType, VcsPlatformType PlatformType, VcsShaderModelType ShaderModelType) items = ComputeVCSFileName(a); p1 = items.Item1; p2 = items.Item2; p3 = items.Item3;
            };
            fetch(filenamepath, out VcsProgramType p1, out VcsPlatformType p2, out VcsShaderModelType p3);
            Console.WriteLine($"{p1}");
            Console.WriteLine($"{p2}");
            Console.WriteLine($"{p3}");
        }

        static void GettingGsDataOut3()
        {
            string filenamepath = "debugoverlay_wireframe_pc_40_gs.vcs";
            var vcsProg = (string a, out (VcsProgramType, VcsPlatformType, VcsShaderModelType) vpg) => { vpg = ComputeVCSFileName(a); };
            vcsProg(filenamepath, out (VcsProgramType p1, VcsPlatformType p2, VcsShaderModelType p3) pp);
            Console.WriteLine($"{pp.p1}");
            Console.WriteLine($"{pp.p2}");
            Console.WriteLine($"{pp.p3}");
        }

        static void GettingGsDataOut2()
        {
            string filenamepath = "debugoverlay_wireframe_pc_40_gs.vcs";
            static (VcsProgramType ProgramType, VcsPlatformType PlatformType, VcsShaderModelType ShaderModelType)
                vcsProg(string a) => ComputeVCSFileName(a);
            (VcsProgramType ProgramType, VcsPlatformType PlatformType, VcsShaderModelType ShaderModelType) res = vcsProg(filenamepath);
            Console.WriteLine($"{res.Item1}");
        }

        static void GettingGsDataOut1()
        {
            string filenamepath = "debugoverlay_wireframe_pc_40_gs.vcs";
            ComputeVCSFileNameFetch1(filenamepath, out (VcsProgramType p1, VcsPlatformType p2, VcsShaderModelType p3) vcsprog);
            Console.WriteLine($"{vcsprog.p1}");
            Console.WriteLine($"{vcsprog.p2}");
            Console.WriteLine($"{vcsprog.p3}");
        }

        private static void ComputeVCSFileNameFetch1(string filenamepath,
            out (VcsProgramType, VcsPlatformType, VcsShaderModelType) vcsprog)
        {
            vcsprog = ComputeVCSFileName(filenamepath);
        }

        static void AddTwoNumbers()
        {
            Func<int, int, int> add = (int x, int y) => x + y;
            Console.WriteLine($"{add(10, 20)}");
        }

        static void ConvertIntToByteString()
        {
            Func<byte[], string> toByteString = (byte[] b) => $"{b[0]:x02} {b[1]:x02} {b[2]:x02} {b[3]:x02}";
            // -- alternative
            // static string toByteString(byte[] b) => $"{b[0]:x02} {b[1]:x02} {b[2]:x02} {b[3]:x02}";
            string byteString = toByteString(BitConverter.GetBytes(1010));
            Console.WriteLine($"{byteString}");
        }

        static void DumbTest2()
        {
            dict = new Dictionary<int, (bool, DateTime)>
            {
                { 0, (true, DateTime.Now) }
            };
            SomeMethod(1);
            SomeMethod(0);
        }

        private static Dictionary<int, (bool, DateTime)> dict;

        public static void SomeMethod(int number)
        {
            if (dict.TryGetValue(number, out (bool isTrue, DateTime timestamp) booltime))
            {
                Console.WriteLine($"Found it: {booltime.isTrue}, {booltime.timestamp}");
            }
            else
            {
                Console.WriteLine($"{number} Not Found");
            }
        }

        static void TestDictionaryStats()
        {
            StatCounter statCounter = new StatCounter();
            statCounter.recPassed(VcsProgramType.Features);
            statCounter.recPassed(VcsProgramType.VertexShader);
            statCounter.recFail(VcsProgramType.Features);
            Console.WriteLine($"{statCounter.getReport()}");
        }

        static void ConvertHexMapToString()
        {
            string input1 =
            "38 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 02 00 11 00 0D 00 08 00 01 00 52 00 80 00 9B 00 " +
            "A3 00 C0 00 A2 00 C3 00 BB 00 C1 00 BA 00 9C 00 BC 00 A6 00 A5 00 A4 00 9F 00 BF 00 1D 00 1E 00 " +
            "13 00 15 00 1A 00 24 00 1F 00 26 00 81 00 78 00 82 00 51 00 53 00 7F 00 84 00 54 00 83 00 56 00 " +
            "55 00 57 00 42 00 47 00 43 00 BE 00 BD 00 20 00 21 00 1B 00 1C 00 22 00 23 00 25 00 07 00 56 65 " +
            "72 74 65 78 50 61 69 6E 74 53 74 72 65 61 6D 43 6F 75 6E 74 00 B1 20 E7 E0 05 FF FF 00 00 00 00 " +
            "01 00 00 00 56 65 72 74 65 78 50 61 69 6E 74 55 49 32 4C 61 79 65 72 00 31 61 EB E8 09 FF FF 00 " +
            "00 00 00 01 4C 69 67 68 74 53 69 6D 5F 44 69 66 66 75 73 65 41 6C 62 65 64 6F 54 65 78 74 75 72 " +
            "65 00 16 C5 FF 0E 0E 9C 00 52 65 70 72 65 73 65 6E 74 61 74 69 76 65 54 65 78 74 75 72 65 00 11 " +
            "04 70 5D 0E 9C 00 53 75 70 70 6F 72 74 73 4D 61 70 70 69 6E 67 44 69 6D 65 6E 73 69 6F 6E 73 00 " +
            "6D 0E 46 89 09 FF FF 00 00 00 00 01 4C 61 79 65 72 43 6F 6C 6F 72 30 00 CA 70 E8 6C 0E 9C 00 4C " +
            "61 79 65 72 43 6F 6C 6F 72 31 00 62 BF B9 90 0E A6 00 01 00 00 00";

            string input2 =
            "43 54 41 42 1C 00 00 00 F6 03 00 00 00 03 FF FF 13 00 00 00 1C 00 00 00 00 01 00 00 EF 03 00 00 " +
            "98 01 00 00 02 00 81 00 01 00 06 02 A8 01 00 00 00 00 00 00 B8 01 00 00 02 00 82 00 01 00 0A 02 " +
            "A8 01 00 00 00 00 00 00 CF 01 00 00 02 00 74 00 01 00 D2 01 A8 01 00 00 00 00 00 00 E0 01 00 00 " +
            "02 00 08 00 03 00 22 00 F4 01 00 00 00 00 00 00 04 02 00 00 02 00 80 00 01 00 02 02 A8 01 00 00 " +
            "00 00 00 00 14 02 00 00 02 00 7F 00 01 00 FE 01 A8 01 00 00 00 00 00 00 22 02 00 00 02 00 7E 00 " +
            "01 00 FA 01 A8 01 00 00 00 00 00 00 30 02 00 00 03 00 00 00 01 00 00 00 3C 02 00 00 00 00 00 00 " +
            "4C 02 00 00 03 00 04 00 01 00 00 00 58 02 00 00 00 00 00 00 68 02 00 00 03 00 03 00 01 00 00 00 " +
            "88 02 00 00 00 00 00 00 98 02 00 00 03 00 01 00 01 00 00 00 A4 02 00 00 00 00 00 00 B4 02 00 00 " +
            "03 00 02 00 01 00 00 00 C4 02 00 00 00 00 00 00 D4 02 00 00 02 00 76 00 01 00 DA 01 E4 02 00 00 " +
            "00 00 00 00 F4 02 00 00 02 00 7C 00 01 00 F2 01 10 03 00 00 00 00 00 00 20 03 00 00 02 00 78 00 " +
            "01 00 E2 01 10 03 00 00 00 00 00 00 3B 03 00 00 02 00 7A 00 01 00 EA 01 10 03 00 00 00 00 00 00 " +
            "54 03 00 00 02 00 1E 00 01 00 7A 00 9C 03 00 00 00 00 00 00 AC 03 00 00 02 00 14 00 01 00 52 00 " +
            "9C 03 00 00 00 00 00 00 D8 03 00 00 02 00 7D 00 01 00 F6 01 9C 03 00 00 00 00 00 00 67 5F 62 56 " +
            "69 73 75 61 6C 69 7A 65 4C 4F 44 00 00 00 03 00 01 00 01 00 01 00 00 00 00 00 00 00 67 5F 66 6C " +
            "41 6C 70 68 61 54 65 73 74 52 65 66 65 72 65 6E 63 65 00 67 5F 66 6C 42 75 6D 70 53 74 72 65 6E " +
            "67 74 68 00 67 5F 6D 61 74 57 6F 72 6C 64 54 6F 56 69 65 77 00 AB AB AB 03 00 03 00 04 00 04 00 " +
            "01 00 00 00 00 00 00 00 67 5F 6E 4C 4F 44 54 68 72 65 73 68 6F 6C 64 00 67 5F 6E 4D 61 78 53 61 " +
            "6D 70 6C 65 73 00 67 5F 6E 4D 69 6E 53 61 6D 70 6C 65 73 00 67 5F 74 43 6F 6C 6F 72 00 AB AB AB " +
            "04 00 0C 00 01 00 01 00 01 00 00 00 00 00 00 00 67 5F 74 48 65 69 67 68 74 00 AB AB 04 00 0C 00 " +
            "01 00 01 00 01 00 00 00 00 00 00 00 67 5F 74 4D 65 74 61 6C 6E 65 73 73 52 65 66 6C 65 63 74 61 " +
            "6E 63 65 46 72 65 73 6E 65 6C 00 AB 04 00 0C 00 01 00 01 00 01 00 00 00 00 00 00 00 67 5F 74 4E " +
            "6F 72 6D 61 6C 00 AB AB 04 00 0C 00 01 00 01 00 01 00 00 00 00 00 00 00 67 5F 74 52 6F 75 67 68 " +
            "6E 65 73 73 00 AB AB AB 04 00 0C 00 01 00 01 00 01 00 00 00 00 00 00 00 67 5F 76 43 6F 6C 6F 72 " +
            "54 69 6E 74 00 AB AB AB 01 00 03 00 01 00 03 00 01 00 00 00 00 00 00 00 67 5F 76 4D 65 74 61 6C " +
            "6E 65 73 73 53 63 61 6C 65 41 6E 64 42 69 61 73 00 AB AB AB 01 00 03 00 01 00 02 00 01 00 00 00 " +
            "00 00 00 00 67 5F 76 52 65 66 6C 65 63 74 61 6E 63 65 53 63 61 6C 65 41 6E 64 42 69 61 73 00 67 " +
            "5F 76 52 6F 75 67 68 6E 65 73 73 53 63 61 6C 65 41 6E 64 42 69 61 73 00 67 5F 76 53 68 61 72 65 " +
            "64 5F 5F 67 5F 62 52 6F 75 67 68 6E 65 73 73 50 61 72 61 6D 73 5F 5F 67 5F 62 53 74 65 72 65 6F " +
            "45 6E 61 62 6C 65 64 5F 5F 67 5F 66 6C 53 74 65 72 65 6F 43 61 6D 65 72 61 49 6E 64 65 78 00 AB " +
            "01 00 03 00 01 00 04 00 01 00 00 00 00 00 00 00 67 5F 76 53 68 61 72 65 64 5F 5F 67 5F 76 43 61 " +
            "6D 65 72 61 44 69 72 57 73 5F 5F 67 5F 66 6C 56 69 65 77 70 6F 72 74 4D 61 78 5A 00 67 5F 76 54 " +
            "65 78 74 75 72 65 44 69 6D 5F 67 5F 74 43 6F 6C 6F 72 00 70 73 5F 33 5F 30 00 4D 69 63 72 6F 73 " +
            "6F 66 74 20 28 52 29 20 48 4C 53 4C 20 53 68 61 64 65 72 20 43 6F 6D 70 69 6C 65 72 20 39 2E 32 " +
            "39 2E 39 35 32 2E 33 31 31 31 00 AB";

            string input3 =
            "2E 00 00 00 00 00 00 00 00 00 00 00 09 00 12 00 15 00 03 00 05 00 11 00 13 00 07 00 02 00 08 00 " +
            "10 00 6D 00 23 00 1B 00 1C 00 1D 00 22 00 27 00 29 00 2A 00 31 00 32 00 33 00 34 00 35 00 36 00 " +
            "1F 00 24 00 30 00 4D 00 25 00 26 00 2C 00 2F 00 2E 00 62 00 67 00 50 00 51 00 7A 00 7B 00 17 00 " +
            "19 00 1E 00 21 00 2D 00 08 00 44 65 70 74 68 50 61 73 73 42 61 74 63 68 49 44 00 05 51 5B 2C 05 " +
            "FF FF 00 00 00 00 01 00 00 00 42 61 73 65 43 6F 6C 6F 72 54 65 78 74 75 72 65 00 92 A9 2A 88 0E " +
            "6D 00 52 65 70 72 65 73 65 6E 74 61 74 69 76 65 54 65 78 74 75 72 65 00 11 04 70 5D 0E 6D 00 53 " +
            "75 70 70 6F 72 74 73 4D 61 70 70 69 6E 67 44 69 6D 65 6E 73 69 6F 6E 73 00 6D 0E 46 89 09 FF FF " +
            "00 00 00 00 01 74 72 61 6E 73 6C 75 63 65 6E 74 00 06 68 DB 4A 09 FF FF 12 00 00 00 1A 01 04 07 " +
            "00 0F 00 07 00 00 80 3F 02 11 00 1A 03 00 61 6C 70 68 61 74 65 73 74 00 33 4E 5A F2 09 FF FF 03 " +
            "00 00 00 1A 02 00 46 6F 72 77 61 72 64 4C 61 79 65 72 4F 6E 6C 79 00 C4 29 CF 2B 09 FF FF 00 00 " +
            "00 00 01 4E 6F 72 6D 61 6C 4D 61 70 54 65 78 74 75 72 65 00 2E 56 C6 1C 0E 7A 00 01 00 00 00";

            string assembleString = "";
            int count = 0;
            foreach (string hexcode in input3.Split(' '))
            {
                int hexChar = Convert.ToInt32(hexcode, 16);
                if (hexChar >= 32 && hexChar <= 126)
                {
                    assembleString += (char) hexChar;
                }
                else
                {
                    assembleString += ".";
                }
                if (++count % 32 == 0)
                {
                    assembleString += "\n";
                }
            }
            Console.WriteLine($"{assembleString}");
        }

        static void ZframeLabel()
        {
            // zframes ids range from 0 to large numbers, the largest known id is
            // 0x018cbfb1 or 26001329
            // a standardised way to print them then is using a 8-length hex representation
            // Console.WriteLine(0x018cbfb1);
            Console.WriteLine($"{60:x08}");
        }

        /*
         *
         * To keep track of zframes use a sorted dictionary.
         * Can either look up zframes based on the index they appear in the file, or based on their ID
         *
         * I'm not sure I need the order they appear in the file though (possibly not - but it doesn't matter)
         *
         */
        static void TestDictionaryLookups()
        {
            SortedDictionary<int, int> myDict = new();
            myDict.Add(0, 0);
            myDict.Add(1, 1);
            myDict.Add(2, 2);
            myDict.Add(4, 4);
            myDict.Add(8, 8);
            Console.WriteLine(myDict.ElementAt(3));
            Console.WriteLine(myDict[4]);
        }

        static void PrintByteCounter()
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine($"({i * 32 + 216}) 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            }
        }
    }
}

