using System;
using MyShaderAnalysis.parsing;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.filearchive.ReadShaderFile;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.codestash
{
    public class LzmaAnalysis
    {
        public static void RunTrials()
        {
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial6();
            // Trial8();
            // Trial10();
            Trial11();
        }

        static void Trial11()
        {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zId);
        }

        static void Trial10()
        {
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs"; long zId = 0x1a1;
            // string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs"; long zId = 0;
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/bloom_dota_pc_40_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/visualize_physics_pc_30_ps.vcs"; long zId = 0x10;
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(zId);
            (VcsProgramType, VcsPlatformType, VcsShaderModelType) vcsFileProperties = ComputeVCSFileName(filenamepath);
            DataReaderZFrameBytes datareader = new(zframeDatabytes,
                vcsFileProperties.Item1, vcsFileProperties.Item2, vcsFileProperties.Item3, shaderFile.VcsVersion);
            datareader.PrintByteDetail();
        }

        static void Trial8()
        {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            DataReaderVcsBytes shaderByteAnalysis = new(filenamepath);
            shaderByteAnalysis.SetShortenOutput(false);
            shaderByteAnalysis.PrintByteDetail();
        }

        static void Trial6()
        {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x1a1);
            DataReaderZFrameBytes datareader = new(zframeDatabytes, shaderFile.VcsProgramType,
                shaderFile.VcsPlatformType, shaderFile.VcsShaderModelType, shaderFile.VcsVersion);
            datareader.PrintByteDetail();
        }

        /*
         *
         * These numbers kind of looked like floats to me, but don't seem to be converted into
         * anything recognizable
         *
         * 00 05 80 36      3.8152793E-06
         * 00 03 80 37      1.5260186E-05
         *
         *
         *
         */
        static void Trial3()
        {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            DataReaderVcsBytes reader = new(filenamepath);
            reader.BaseStream.Position = 9525;
            reader.BaseStream.Position = 10340;
            float num = reader.ReadSingleAtPosition();
            // int num = reader.ReadIntAtPosition();
            Console.WriteLine($"{num}");
            reader.ShowBytes(4);
        }

        /*
         * FD FF FF FF read as an int evaluates as -3
         *
         *
         */
        static void Trial2()
        {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            DataReaderVcsBytes reader = new(filenamepath);
            reader.BaseStream.Position = 2032;
            int num = reader.ReadInt32AtPosition();
            Console.WriteLine($"{num}");
            reader.ShowBytes(4);
        }

        /*
         *
         * file ending with lzma compression
         *
         */
        static void Trial1()
        {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";

            // ConsoleConsole.WriteLine($"{filenamepath}");
            // ShaderFile shaderfile = new ShaderFile(filenamepath);

            DataReaderVcsBytes reader = new DataReaderVcsBytes(filenamepath);
            reader.PrintByteDetail();

            // reader.ShowBytes(300);

            reader.ShowByteCount();
            int offset0 = reader.ReadInt32AtPosition();
            reader.ShowBytes(4, $"zframe[0] offset {offset0}");
            int offset1 = reader.ReadInt32AtPosition();
            reader.ShowBytes(4, $"zframe[1] offset {offset1}");
            int offset2 = reader.ReadInt32AtPosition();
            reader.ShowBytes(4, $"EOF {offset2}");
            reader.BreakLine();

            reader.ShowByteCount();
            int chunkOffset = (int) reader.BaseStream.Position;
            int chunkSize = reader.ReadInt32AtPosition();
            reader.ShowBytes(4, $"{chunkSize}");
            reader.ShowBytes(4);
            reader.ShowBytesWithIntValue();
            reader.ShowBytesWithIntValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.BaseStream.Position = chunkOffset + chunkSize + 4;
            reader.ShowByteCount();
            int chunkOffset2 = (int) reader.BaseStream.Position;
            int chunkSize2 = reader.ReadInt32AtPosition();
            reader.ShowBytes(4, $"{chunkSize2}");
            reader.ShowBytes(4);
            reader.ShowBytesWithIntValue();
            reader.ShowBytesWithIntValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.BaseStream.Position = chunkOffset2 + chunkSize2 + 4;

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                throw new ShaderParserException("End of file not reached!");
            }
            reader.ShowByteCount();
            reader.OutputWriteLine("EOF");
            reader.BreakLine();
        }
    }
}


