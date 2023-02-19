using MyShaderAnalysis.util;
using System;
using System.Collections.Generic;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.filearchive.ReadShaderFile;
using static MyShaderAnalysis.filearchive.FileArchive;

namespace MyShaderAnalysis.snippetcode
{
    public class CrcTrialsSha1
    {
        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";

        public static void RunTrials()
        {
            Trial1();
            // Trial2();
        }

        // search for small glsl sources
        static void SearchForSmallGlsl()
        {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, VcsProgramType.Undetermined, -1);
            foreach (var filenamepath in vcsFiles)
            {
                ShaderFile shaderfile = InstantiateShaderFile(filenamepath);

                // Console.WriteLine($"{filenamepath}");
                foreach (var item in shaderfile.ZframesLookup)
                {
                    ZFrameDataDescription zframeData = item.Value;
                    if (zframeData.UncompressedLength < 100000)
                    {
                        ZFrameFile zframeFile = new(zframeData.GetDecompressedZFrame(), filenamepath, zframeData.ZframeId,
                            shaderfile.VcsProgramType, shaderfile.VcsPlatformType, shaderfile.VcsShaderModelType, shaderfile.VcsVersion);
                        foreach (var source in zframeFile.GpuSources)
                        {
                            // if ((source.offset1-1)%64==0 && source.offset1<500) {
                            if (source.Sourcebytes.Length < 5000)
                            {
                                Console.WriteLine($"{RemoveBaseDir(filenamepath)}   {zframeData.ZframeId:x08}  " +
                                    $"{source.Sourcebytes.Length}  {source.GetEditorRefIdAsString()}");
                            }
                        }
                    }
                }
            }
        }

        static void SpacePrintBits()
        {
            // glsl-48adaecab90362da8e4e54028c2c2162.txt
            byte[] bytes01 = ParseString("45 73 EB F2 66 12 25 9B FC 68 13 B2 93 75 16 D8 C9 42 C3 7E");
            byte[] bytes02 = ParseString("48 AD AE CA B9 03 62 DA 8E 4E 54 02 8C 2C 21 62");
            // glsl-25abff568a269b7a34c464ca09e7de0c.TXT
            byte[] bytes11 = ParseString("4C D7 AA 99 45 75 7C F7 6F 8B 91 82 4B 76 00 80 3E C8 24 61");
            byte[] bytes12 = ParseString("25 AB FF 56 8A 26 9B 7A 34 C4 64 CA 09 E7 DE 0C");
            // 25d9ec3840bb0d4c5c6240491715b161
            byte[] bytes21 = ParseString("A4 74 A6 DB B0 38 6C 0B E3 89 C2 28 00 B5 DB 6C 4A 88 8B A0");
            byte[] bytes22 = ParseString("25 D9 EC 38 40 BB 0D 4C 5C 62 40 49 17 15 B1 61");
            // glsl-48ada2e41203fd4ae9ae05970a6b610c.txt
            byte[] bytes31 = ParseString("D1 F9 99 39 43 9F 6D 21 0B 95 56 19 4B C9 0E F1 FE 81 1D 3E");
            byte[] bytes32 = ParseString("48 AD A2 E4 12 03 FD 4A E9 AE 05 97 0A 6B 61 0C");

            SpacePrintBits(bytes01);
            SpacePrintBits(bytes02);
            Console.WriteLine($"");
            SpacePrintBits(bytes11);
            SpacePrintBits(bytes12);
            Console.WriteLine($"");
            SpacePrintBits(bytes21);
            SpacePrintBits(bytes22);
            Console.WriteLine($"");
            SpacePrintBits(bytes31);
            SpacePrintBits(bytes32);
        }

        static void SpacePrintBits(byte[] bytes0)
        {
            int SPACELEN = 8;
            // int SPACELEN = 10;
            for (int i = 0; i < bytes0.Length * 8; i++)
            {
                if (bytes0.Length == 20)
                {
                    SPACELEN = 10;
                }
                if (i > 0 && i % SPACELEN == 0)
                {
                    Console.Write($" ");
                    if (SPACELEN == 8)
                    {
                        Console.Write($"  ");
                    }
                }
                Console.Write($"{BitAt(i, bytes0)}");
            }
            Console.WriteLine($"");
        }

        static void Trial1()
        {
            // -- showing sha1 on selected files (160 bit) and their guid

            // glsl-25abff568a269b7a34c464ca09e7de0c.TXT
            // 4C D7 AA 99 45 75 7C F7 6F 8B 91 82 4B 76 00 80 3E C8 24 61
            // 25 AB FF 56 8A 26 9B 7A 34 C4 64 CA 09 E7 DE 0C
            byte[] bytes01 = ParseString("4C D7 AA 99 45 75 7C F7 6F 8B 91 82 4B 76 00 80 3E C8 24 61");
            byte[] bytes02 = ParseString("25 AB FF 56 8A 26 9B 7A 34 C4 64 CA 09 E7 DE 0C");

            // glsl-48ada2e41203fd4ae9ae05970a6b610c.txt
            // D1 F9 99 39 43 9F 6D 21 0B 95 56 19 4B C9 0E F1 FE 81 1D 3E
            // 48 AD A2 E4 12 03 FD 4A E9 AE 05 97 0A 6B 61 0C
            byte[] bytes11 = ParseString("D1 F9 99 39 43 9F 6D 21 0B 95 56 19 4B C9 0E F1 FE 81 1D 3E");
            byte[] bytes12 = ParseString("48 AD A2 E4 12 03 FD 4A E9 AE 05 97 0A 6B 61 0C");

            // glsl-48adaecab90362da8e4e54028c2c2162.txt
            // 45 73 EB F2 66 12 25 9B FC 68 13 B2 93 75 16 D8 C9 42 C3 7E
            // 48 AD AE CA B9 03 62 DA 8E 4E 54 02 8C 2C 21 62
            byte[] bytes21 = ParseString("45 73 EB F2 66 12 25 9B FC 68 13 B2 93 75 16 D8 C9 42 C3 7E");
            byte[] bytes22 = ParseString("48 AD AE CA B9 03 62 DA 8E 4E 54 02 8C 2C 21 62");

            // 25d9ec3840bb0d4c5c6240491715b161
            // A4 74 A6 DB B0 38 6C 0B E3 89 C2 28 00 B5 DB 6C 4A 88 8B A0
            // 25 D9 EC 38 40 BB 0D 4C 5C 62 40 49 17 15 B1 61
            byte[] bytes31 = ParseString("A4 74 A6 DB B0 38 6C 0B E3 89 C2 28 00 B5 DB 6C 4A 88 8B A0");
            byte[] bytes32 = ParseString("25 D9 EC 38 40 BB 0D 4C 5C 62 40 49 17 15 B1 61");

            for (int i = 0; i < 8; i++)
            {
                for (int j = 1; j < 16; j++)
                {
                    bool[] result1 = Generator2(i, j, bytes01, bytes02);
                    if (result1.Length > 0)
                    {
                        // Console.WriteLine($"{i,3},{j,3} ok {BoolArrayToString(result)}");
                        bool[] result2 = Generator2(i, j, bytes11, bytes12);
                        // Console.WriteLine($"{CompareBoolArrays(result1, result2)}");
                        // int match = CompareBoolArrays(result1, result2);
                        int match = TotalMatchCountTrue(result1, result2);
                        if (match > 115)
                        {
                            Console.WriteLine($"{i},{j} match {match}   {TotalMatchCountTrue(result1, result2)}");
                            Console.WriteLine($"{BoolArrayToString(result1)}");
                            Console.WriteLine($"{BoolArrayToString(result2)}");
                            Console.WriteLine($"");
                        }
                    }
                }
            }
        }

        static int UPTO = 120;
        static int SKIP = 8;

        static bool[] Generator2(int start, int inc, byte[] bytes1, byte[] bytes2)
        {
            bool[] check = new bool[160];
            int ind2 = start;
            bool[] matches = new bool[UPTO];
            for (int i = 0; i < UPTO; i++)
            {
                if (check[ind2])
                {
                    return Array.Empty<bool>();
                }
                check[ind2] = true;
                ind2 += inc;
                if (ind2 >= 160)
                {
                    ind2 -= 160;
                }
                if (i % SKIP > 0) continue;
                int b1 = BitAt(i, bytes2);
                int b2 = BitAt(ind2, bytes1);
                matches[i] = b1 == b2;
            }
            return matches;
        }

        static int TotalMatchCountTrue(bool[] bool0, bool[] bool1)
        {
            int matchLen = 0;
            for (int i = 0; i < bool0.Length; i++)
            {
                if (bool0[i] == bool1[i])
                {
                    matchLen++;
                }
            }
            return matchLen;
        }

        static int CompareBoolArrays(bool[] bool0, bool[] bool1)
        {
            int matchLen = 1;
            bool matchingOn = bool0[0] == bool1[0];
            for (int i = 1; i < bool0.Length; i++)
            {
                if (bool0[i] == bool1[i] == matchingOn)
                {
                    matchLen++;
                } else
                {
                    return matchLen;
                }
            }
            return matchLen;
        }

        static int BitAtTotallyWrong(int ind, byte[] bytes0)
        {
            int i = ind / 8;
            int b = 1 << ind % 8;
            return (bytes0[i] & b) > 0 ? 1 : 0;
        }

        static int BitAt(int ind, byte[] bytes0)
        {
            int i = ind / 8;
            int b = 1 << 7 - ind % 8;
            return (bytes0[i] & b) > 0 ? 1 : 0;
        }

        static byte[] ParseString(string bytestring)
        {
            var tokens = bytestring.Split(" ");
            var databytes = new byte[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }

        static byte[] ParseStringReverse(string bytestring)
        {
            var tokens = bytestring.Split(" ");
            var databytes = new byte[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                databytes[tokens.Length - i - 1] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }

        static string BoolArrayToString(bool[] bool0)
        {
            string result = "";
            for (int i = 0; i < UPTO; i++)
            {
                if (i % SKIP > 0)
                {
                    result += "_";
                } else
                {
                    result += bool0[i] ? "T" : "F";
                }
            }
            return result;
        }

        // find closely matching sourcefile names
        static void Trial2()
        {
            string[] files = Directory.GetFiles("X:/glsl-txt");
            for (int i = 1; i < files.Length; i += 2)
            {
                int matchLen = CompareStrings(files[i - 1], files[i]);
                if (matchLen > 21)
                {
                    Console.WriteLine($"{files[i - 1]}");
                    Console.WriteLine($"{files[i]}");
                }
                if (i == 10)
                {
                    break;
                }
            }
        }

        static int CompareStrings(string s1, string s2)
        {
            int matchLen = 0;
            int maxLen = Math.Min(s1.Length, s2.Length);
            for (int i = 0; i < maxLen; i++)
            {
                if (s1[i] == s2[i])
                {
                    matchLen++;
                } else
                {
                    return matchLen;
                }
            }
            return matchLen;
        }

        private static string RemoveBaseDir(string filenamepath)
        {
            string dirname = Path.GetDirectoryName(filenamepath).Replace("\\", "/");
            string filename = Path.GetFileName(filenamepath).Replace("\\", "/");
            if (dirname.EndsWith(@"/shaders/vfx"))
            {
                return @"/shaders/vfx/" + filename;
            } else if (dirname.EndsWith(@"/shaders-core/vfx"))
            {
                return @"/shaders-core/vfx/" + filename;
            } else
            {
                return filenamepath;
            }
        }
    }
}

