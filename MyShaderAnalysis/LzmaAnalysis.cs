using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.vcsparsing;
using System.Collections.Generic;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using MyValveResourceFormat.ThirdParty;
using System;

namespace MyShaderAnalysis {


    public class LzmaAnalysis {



        public static void RunTrials() {
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial6();
            // Trial8();
            // Trial10();
            Trial11();
        }


        static void Trial11() {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            ShaderFile shaderFile = new(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zId);

        }



        static void Trial10() {
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs"; long zId = 0x1a1;
            // string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs"; long zId = 0;
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/bloom_dota_pc_40_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/visualize_physics_pc_30_ps.vcs"; long zId = 0x10;
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(zId);
            DataReaderZFrameByteAnalysis datareader = new(zframeDatabytes, GetVcsFileType(filenamepath), GetVcsSourceType(filenamepath));
            datareader.PrintByteAnalysis();

        }



        static void Trial8() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            shaderByteAnalysis.SetShortenOutput(false);
            shaderByteAnalysis.PrintByteAnalysis();
        }


        static void Trial6() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x1a1);
            DataReaderZFrameByteAnalysis datareader = new(zframeDatabytes, shaderFile.vcsFileType, shaderFile.vcsSourceType);
            datareader.PrintByteAnalysis();
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
        static void Trial3() {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            DataReaderVcsByteAnalysis reader = new(filenamepath);

            // reader.offset = 9525;
            reader.offset = 10340;

            float num = reader.ReadFloatAtPosition();
            // int num = reader.ReadIntAtPosition();
            Debug.WriteLine($"{num}");
            reader.ShowBytes(4);
        }

        /*
         * FD FF FF FF read as an int evaluates as -3
         *
         *
         */
        static void Trial2() {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            DataReaderVcsByteAnalysis reader = new(filenamepath);
            reader.offset = 2032;
            int num = reader.ReadIntAtPosition();
            Debug.WriteLine($"{num}");
            reader.ShowBytes(4);

        }

        /*
         *
         * file ending with lzma compression
         *
         */
        static void Trial1() {


            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";

            // Debug.WriteLine($"{filenamepath}");
            // ShaderFile shaderfile = new ShaderFile(filenamepath);

            DataReaderVcsByteAnalysis reader = new DataReaderVcsByteAnalysis(filenamepath);
            reader.PrintByteAnalysis();

            // reader.ShowBytes(300);

            reader.ShowByteCount();
            int offset0 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"zframe[0] offset {offset0}");
            int offset1 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"zframe[1] offset {offset1}");
            int offset2 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"EOF {offset2}");
            reader.BreakLine();


            reader.ShowByteCount();
            int chunkOffset = reader.offset;
            int chunkSize = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"{chunkSize}");
            reader.ShowBytes(4);
            reader.PrintIntWithValue();
            reader.PrintIntWithValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.offset = chunkOffset + chunkSize + 4;
            reader.ShowByteCount();
            int chunkOffset2 = reader.offset;
            int chunkSize2 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"{chunkSize2}");
            reader.ShowBytes(4);
            reader.PrintIntWithValue();
            reader.PrintIntWithValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.offset = chunkOffset2 + chunkSize2 + 4;

            reader.EndOfFile();
            // reader.ShowByteCount();
            // reader.ShowBytes(4);

        }


    }


}





