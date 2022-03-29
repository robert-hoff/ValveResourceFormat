using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using MyShaderAnalysis.utilhelpers;
// using ZstdSharp;
using static MyShaderAnalysis.utilhelpers.FileSystemOld;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis
{
    class DemoCode
    {

        public static void RunTrials()
        {
            ParseV62File();
            // ManualZFrameDecompression();
            // ShowVcsFilesForGivenArchive();
        }


        public const string THE_LAB_SOURCE = "X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx";

        static void Trial999()
        {
            // string filenamepath = $"{V65_EXAMPLES_SOURCE}/cables_pc_40_features.vcs";
            // string filenamepath = $"{V65_EXAMPLES_SOURCE}/cables_pc_40_ps.vcs";
            // string filenamepath = $"{V65_EXAMPLES_SOURCE}/cables_pc_40_vs.vcs";
            // string filenamepath = $"{V65_EXAMPLES_SOURCE}/hero_pc_40_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/aerial_perspective_pc_30_features.vcs";
            // ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
        }

        // two ways to print the byte analysis
        static void ParseV62File()
        {
            string filenamepath = @"X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx/bilateral_blur_pc_30_ps.vcs";
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);

            // shaderFile.PrintByteAnalysis();
            // new ParseV62Files(filenamepath);
            new DataReaderVcsBytes(filenamepath).PrintByteAnalysis();
        }


        static void ManualZFrameDecompression()
        {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE_V65}/crystal_pc_40_vs.vcs";
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);

            byte[] result = shaderFile.GetCompressedZFrameData(0);

            // Console.WriteLine($"{shaderFile.GetZFrameCount()}");
            // Console.WriteLine($"{result.Length}");
            // Console.WriteLine(BytesToString(result));

            byte[] uncompressedData = DecompressZstdFrame(result);
            Console.WriteLine(BytesToString(uncompressedData));

            // Console.WriteLine(uncompressedData.Length);
        }


        static byte[] DecompressZstdFrame(byte[] compressedZstdFrame)
        {
            byte[] zstdDict = ZstdDictionary.GetDictionary();
            var zstdDecoder = new ZstdSharp.Decompressor();
            zstdDecoder.LoadDictionary(zstdDict);
            byte[] decompressedData = zstdDecoder.Unwrap(compressedZstdFrame).ToArray();
            return decompressedData;
        }


        static void ShowVcsFilesForGivenArchive()
        {
            // gets all vcs files from both directories given
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE);

            // gets all files ending with 40_vs.vcs from the given directory
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, null, VcsProgramType.VertexShader, 40);
            foreach (var filenamepath in vcsFiles)
            {
                Console.WriteLine(filenamepath);
            }
        }



    }
}
