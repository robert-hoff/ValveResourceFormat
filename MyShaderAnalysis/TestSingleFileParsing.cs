using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.compat;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.codestash.FileArchives;
using static MyShaderAnalysis.codestash.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.codestash
{
    public class TestSingleFileParsing
    {

        public static void RunTrials()
        {
            // RunTestSingleFilePrintout();
            // RunTestZFrameFilePrintout();
            // RunTestZFrameBytePrintout1();
            // RunTestZFrameBytePrintout2();
            // ShowZFrameCount();

            // RunZframeBytesSetExample2();
            // RunZframeBytesSetExample1();

            RunTestZFrameBytePrintoutV62();
        }



        static void RunZframeBytesSetExample2()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_core_vulkan, "test_vulkan_50_hs.vcs"); int zframeIndex = 1;
            // TestZFrameFilePrintout(filenamepath, zframeIndex);
            // TestZFrameBytePrintout1(filenamepath, zframeIndex);
            TestZFrameBytePrintout2(filenamepath, zframeIndex);
        }


        static void RunZframeBytesSetExample1()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
            Console.WriteLine($"");
            Console.WriteLine($"");
            TestZFrameBytePrintout1(filenamepath, zframeIndex);
            Console.WriteLine($"");
            Console.WriteLine($"");
            TestZFrameBytePrintout2(filenamepath, zframeIndex);
        }



        static void RunTestSingleFilePrintout()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "cs_compress_dxt5_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_psrs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pc, "hero_pc_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pc, "multiblend_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pc, "multiblend_pc_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "generic_light_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "generic_light_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pc, "generic_light_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pc, "generic_light_pc_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "generic_light_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_vs.vcs");
            // strange file that doesn't contain any data
            string filenamepath = GetFilenamepath(ARCHIVE.dotacore_pcgl, "msaa_resolve_cs_pcgl_50_features.vcs");
            TestSingleFilePrintout(filenamepath);
        }

        static void RunTestZFrameFilePrintout()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "cs_surface_interactions_vulkan_50_cs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 1;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
        }

        static void RunTestZFrameBytePrintout2()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
        }




        static void TestSingleFilePrintout(string filenamepath)
        {
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            new PrintVcsFileSummary(shaderFile);
        }


        static void TestZFrameBytePrintout2(string filenamepath, int zframeIndex)
        {
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
            zframeFile.PrintByteDetail();
        }


        static void RunTestZFrameBytePrintout1()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pc, "multiblend_pc_30_vs.vcs"); int zframeIndex = 0;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
        }

        static void TestZFrameBytePrintout1(string filenamepath, int zframeIndex)
        {
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;

            byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zframeIndex);
            DataReaderZFrameBytes dataReaderZframe = new DataReaderZFrameBytes(zframeBytes,
                shaderFile.vcsProgramType, shaderFile.vcsPlatformType, shaderFile.vcsShaderModelType);
            long zframeId = shaderFile.GetZFrameIdByIndex(zframeIndex);
            Console.Write($"parsing zframe[0x{zframeId:x08}]");
            dataReaderZframe.PrintByteDetail();
        }



        static void RunTestZFrameBytePrintoutV62()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl_v62, "bloom_dota_pcgl_30_features.vcs"); int zframeIndex = 0;
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;
            byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zframeIndex);

            ParseV62ZFrame zframeParser = new ParseV62ZFrame(zframeBytes, shaderFile);
            zframeParser.PrintByteDetail();

        }



        static void TestZFrameFilePrintout(string filenamepath, int zframeIndex)
        {
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;
            // ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex, omitParsing: true);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
            // zframeFile.PrintByteDetail();
            // zframeFile.PrintGlslSource(0, Console.Write);
            new PrintZFrameSummary(shaderFile, zframeFile, showRichTextBoxLinks: true);
            // Console.WriteLine($"{zframeFile.gpuSourceCount}");
        }



        static void ShowZFrameCount()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs");
            Console.WriteLine($"{filenamepath}");
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            Console.WriteLine($"{shaderFile.GetZFrameCount()} zframes");
        }



    }
}




