using System;
using System.Collections.Generic;
using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.serverhtml;
using MyShaderAnalysis.util;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.filearchive.FileArchive;

//  17      16      15    14     13   12     11   10   9     8   7   6   5  4  3  2  1  0
// 98304  49152  24576  12288  6144  2048  1024  512  256  128  64  32  16  8  4  2  1  1

namespace MyShaderAnalysis.compat.background
{
    public class CompatRulesMultiframe
    {
        public static void RunTrials()
        {
            Trial1();
            // TestShadernameShorten();
        }

        static void TestShadernameShorten()
        {
            SortedDictionary<string, int> map = new();

            string dir_game = FileArchive.GetArchiveDir(ARCHIVE.dota_game_pcgl_v64);
            string dir_core = FileArchive.GetArchiveDir(ARCHIVE.dota_core_pcgl_v64);

            List<string> allVcsFiles = GetVcsFiles(dir_game, dir_core, VcsProgramType.Undetermined, -1);
            foreach (string filenamepath in allVcsFiles)
            {
                ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
                foreach (SfBlock item in shaderFile.SfBlocks)
                {
                    map[item.Name] = 1;
                }
            }
            foreach (KeyValuePair<string, int> item in map)
            {
                Console.WriteLine($"{item.Key,-60} {ShaderUtilHelpers.ShortenShaderParam(item.Key)}");
            }
        }

        // static int[] offset = { 1, 1, 2, 4, 8, 16, 32, 64, 128, 384, 768, 1536 };
        // static int[] layers = { 0, 1, 1, 1, 1,  1,  1,  1,   2,  1,    1,    1 };

        static int[] offset;
        static int[] layers;

        public static void Trial1()
        {
            string dir_game = FileArchive.GetArchiveDir(ARCHIVE.dota_game_pcgl_v64);
            string vcsFilenamepath = @$"{dir_game}/multiblend_pcgl_30_ps.vcs";
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(vcsFilenamepath);

            offset = new int[shaderFile.SfBlocks.Count];
            layers = new int[shaderFile.SfBlocks.Count];

            offset[0] = 1;
            layers[0] = shaderFile.SfBlocks[0].RangeMax;

            for (int i = 1; i < shaderFile.SfBlocks.Count; i++)
            {
                int curLayer = shaderFile.SfBlocks[i].RangeMax;
                layers[i] = curLayer;
                offset[i] = offset[i - 1] * (layers[i - 1] + 1);
            }

            // exclusions
            AddExclusion(1, 2, 3);         // rule[0]
            AddExclusion(2, 3, 4);         // rule[1]
            AddExclusion(2, 3, 5);         // rule[2]
            AddExclusion(2, 3, 7);         // rule[4]
            AddExclusion(2, 3, 0);         // rule[6]
            AddExclusion(2, 3, 10);        // rule[8]
            AddExclusion(2, 3, 11);        // rule[9]
            AddExclusion(13, 6, 2);        // rule[11]
            AddExclusion(14, 6, 2);        // rule[14]
            AddExclusion(15, 6, 2);        // rule[17]
            AddExclusion(16, 6, 2);        // rule[20]
            AddExclusion(2, 6, 17);        // rule[23]
            AddExclusion(17, 9);           // rule[24]
            AddExclusion(6, 17);           // rule[26]
            // inclusions
            AddInclusion(5, 4);            // compat[3]
            AddInclusion(7, 4);            // compat[5]
            AddInclusion(9, 8);            // compat[7]
            AddInclusion(12, 4);           // compat[10]
            AddInclusion(13, 5);           // compat[12]
            AddInclusion(13, 4);           // compat[13]
            AddInclusion(14, 5);           // compat[15]
            AddInclusion(14, 4);           // compat[16]
            AddInclusion(15, 5);           // compat[18]
            AddInclusion(15, 4);           // compat[19]
            AddInclusion(16, 5);           // compat[21]
            AddInclusion(16, 4);           // compat[22]
            AddInclusion(11, 10);          // compat[25]

            FileWriter fw = GetFileWriterHtml("compat test", "compat test multiblend_pcgl_30");

            string htmlBasedir = "/multiblend_pcgl_30/";
            for (int zframeId = 0; zframeId < offset[17] * 2; zframeId++)
            {
                // for (int i = 0; i <100; i++) {
                if (CheckZFrame(zframeId))
                {
                    int[] thisState = GetBitPattern(zframeId);
                    string sfNameList = GetSfNameList(shaderFile, thisState);
                    // string stateList = GetStateString(thisState);
                    // StaticAnalysis.OutputWriteLine($"zframe[{i:x08}]   {Convert.ToString(i, 2).PadLeft(20, '0')}    {sfNameList}");
                    // StaticAnalysis.OutputWriteLine($"zframe[{i:x08}]  {sfNameList}");
                    fw.WriteLine($"Zframe[0x{zframeId:x08}]  {sfNameList}");
                }
            }
            fw.CloseStreamWriter();
        }

        private static FileWriter GetFileWriterHtml(string htmltitle, string htmlHeader)
        {
            string outputFilenamepath = ServerNames.GetServerTestFile();
            FileWriter fw = new FileWriter(outputFilenamepath);
            fw.WriteHtmlHeader(htmltitle, htmlHeader);
            return fw;
        }

        static string GetSfNameList(ShaderFile shaderFile, int[] thisState)
        {
            string nameList = "";
            for (int i = 0; i < thisState.Length; i++)
            {
                int val = thisState[i];
                if (val == 0)
                {
                    // nameList += "".PadRight(7);
                    // Console.WriteLine($"{i}");
                    continue;
                }
                string paramLayer = "";
                if (layers[i] > 1)
                {
                    paramLayer = $"({val})";
                }

                // Console.WriteLine($"{i}");
                string parmDisplayName = $"{ShaderUtilHelpers.ShortenShaderParam(shaderFile.SfBlocks[i].Name)}{paramLayer}";
                // nameList += parmDisplayName.PadRight(7);
                nameList += parmDisplayName.ToLower() + ", ";
            }
            return nameList;
        }

        // WARN - get these array lengths sorted out

        static bool CheckZFrame(int zframe)
        {
            int[] state = GetBitPattern(zframe);
            for (int j = 2; j < offset.Length; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    int s1 = state[i];
                    int s2 = state[j];
                    if (s1 == 0 || s2 == 0)
                    {
                        continue;
                    }
                    if (exclusions[i, j] == true)
                    {
                        return false;
                    }
                    if (inclusions[i, j] == true)
                    {
                        return false;
                    }
                }
            }

            for (int i = 1; i < offset.Length; i++)
            {
                int s1 = state[i];
                if (s1 == 0) { continue; }
                for (int j = 1; j < offset.Length; j++)
                {
                    if (inclusions[i, j] && state[j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static int[] GetBitPattern(int testNum)
        {
            int[] state = new int[layers.Length];
            for (int i = 1; i < layers.Length; i++)
            {
                int res = (testNum / offset[i]) % (layers[i] + 1);
                state[i] = res;
            }
            return state;
        }

        static bool[,] exclusions = new bool[100, 100];
        static bool[,] inclusions = new bool[100, 100];

        static void AddExclusion(int s1, int s2, int s3)
        {
            AddExclusion(s1, s2);
            AddExclusion(s1, s3);
            AddExclusion(s2, s3);
        }

        static void AddExclusion(int s1, int s2)
        {
            exclusions[s1, s2] = true;
            exclusions[s2, s1] = true;
        }
        static void AddInclusion(int s1, int s2)
        {
            inclusions[s1, s2] = true;
        }

        static void ShowStateForZframe(int num)
        {
            int[] state = GetBitPattern(num);
            ShowIntArray(state);
        }

        static string GetStateString(int[] state)
        {
            string stateStr = "";
            for (int i = 1; i < state.Length; i++)
            {
                stateStr = $"{state[i]}{stateStr}";
            }
            // return $"{stateStr[0..^1]}";
            return stateStr;
        }

        static void ShowIntArray(int[] state, string space = "")
        {
            string stateStr = "";
            for (int i = 0; i < state.Length; i++)
            {
                stateStr = $"{state[i]}{space}{stateStr}";
            }
            Console.WriteLine($"{stateStr[0..^1]}");
        }
    }
}


