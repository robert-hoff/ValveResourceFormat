using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using ValveResourceFormat.ShaderParser;
using ShaderAnalysis.utilhelpers;
using static ShaderAnalysis.utilhelpers.FileHelpers;
// using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;

namespace ShaderAnalysis
{
    public class TestCompiledShader
    {

        static string TEST_SHADERS_DIR = $"X:/checkouts/ValveResourceFormat/Tests/Files/Shaders";


        public static void RunTrials()
        {
            // Trial2();
            // Trial3();
            // Trial4();
            // Trial5();
            // Trial6();
            // Trial7();
            // Trial8();
        }



        static void Trial8()
        {



        }





        static void Trial7()
        {
            // byte[] dict = RetrieveZstdDict.sayHello();
            byte[] dict = ZstdDictionary.GetDictionary();
            byte[] zstdDict = File.ReadAllBytes("../../CompiledShader/zstdictionary_2bc2fa87.dat");


             //dict[19] = 8;

            //Debug.WriteLine($"{dict.Length}");
            //Debug.WriteLine($"{zstdDict.Length}");

            for (int i = 0; i < 65536; i++)
            {
                if (dict[i] != zstdDict[i])
                {
                    Debug.WriteLine($"error!");
                }
            }



        }



        static void Trial6()
        {
            byte[] zstdDict = File.ReadAllBytes("../../CompiledShader/zstdictionary_2bc2fa87.dat");

            // byte[] hello2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            // byte[] hello2 = { 155, 27, 66, 4, 5, 6, 7, 8, 9, 10 };
            // int[] val = GetPiece(hello, 3);
            // int val = GetPiece2(hello, 1);

            //GetPiece3(hello2, 0);
            //GetPiece3(hello2, 1);
            //GetPiece3(hello2, 2);
            //GetPiece3(hello2, 3);
            // Decode("0@83");
            // Decode("Va]2");



            string dict = "";
            for (int i = 0; i < 21846; i++)
            {
                dict += GetPiece3(zstdDict, i);
            }

            // Debug.WriteLine($"{dict.Length}");
            // Debug.WriteLine($"{dict[0..100]}");
            // Debug.WriteLine($"{dict[87383]}");


            for (int i = 0; i < dict.Length; i += 100)
            {
                int end = i + 100;
                if (end > dict.Length)
                {
                    end = dict.Length;
                }
                Debug.WriteLine($"\"{dict[i..end]}\"+");
            }




        }





        static byte[] Decode(string enc)
        {
            byte[] res = new byte[3];
            int val = ctv(enc[0],18)+ctv(enc[1],12)+ctv(enc[2],6)+ctv(enc[3],0);
            res[0] = (byte)(val>>16);
            res[1] = (byte)(0xFF&(val>>8));
            res[2] = (byte)(0xFF&val);
            return res;
        }

        static int ctv(char c, int shift)
        {
            return (c-0x30)<<shift;
        }





        static string GetPiece3(byte[] dict, int ind)
        {
            int val = 0;
            for (int i = ind * 3; i < ind * 3 + 3; i++)
            {
                val <<= 8;
                val += i < dict.Length ? dict[i] : 0;
            }

            string strval = $"{ValToChar(val,18)}{ValToChar(val,12)}{ValToChar(val,6)}{ValToChar(val,0)}";
            // string strval = $"{ValToChar(val>>18)}";

            // Debug.WriteLine($"{strval}");
            return strval;
        }

        static char ValToChar(int val, int shift)
        {
            return (char) ((0x3F&(val>>shift)) + 0x30);
        }
        static char ValToChar(int val)
        {
            return (char) (val + 0x30);
        }





        static int GetPiece2(byte[] dict, int ind)
        {
            int val = 0;
            for (int i = ind * 3; i < ind * 3 + 3; i++)
            {
                val <<= 8;
                val += i < dict.Length ? dict[i] : 0;
            }
            return val;

        }



        static int[] GetPiece(byte[] dict, int ind)
        {
            int[] returnArr = new int[3];
            int start = ind * 3;
            for (int i = start; i < dict.Length && i < start + 3; i++)
            {
                returnArr[i - start] = dict[i];
            }
            return returnArr;

        }









        static void Trial5()
        {
            string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_vs.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0, omitParsing: true);
            zframeFile.datareader.WriteToDebug = true;
            zframeFile.datareader.WriteToConsole = false;

            zframeFile.PrintByteAnalysis();
        }


        static void Trial4()
        {
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_ps.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_vs.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_ps.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_vs.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_ps.vcs";
            string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_vs.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            shaderFile.PrintByteAnalysis(shortenOutput: false);
        }

        static void Trial3()
        {
            string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_features.vcs";
            new DataReaderVcsTesting(filenamepath);
        }



        static void Trial2()
        {
            // string filenamepath = $"{FileSystem.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_DCG_PC_SOURCE}/bloom_dota_pc_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/generic_pc_30_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/generic_pc_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/occluder_vis_pc_40_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/refract_pc_30_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/refract_pc_30_features.vcs";
            string filenamepath = $"{FileSystem.DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_40_ps.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();

            // Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
            shaderFile.PrintByteAnalysis(shortenOutput: false);

            // Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
            shaderFile.GetZFrameFile(0).PrintByteAnalysis();

        }



        static void Trial1()
        {
            List<string> vcsFiles = GetVcsFiles(FileSystem.DOTA_CORE_PCGL_SOURCE, FileSystem.DOTA_GAME_PCGL_SOURCE, VcsFileType.Features, -1);

            foreach (var item in vcsFiles)
            {
                // ShaderFile shaderFile = new(item);
            }


        }




    }




}

