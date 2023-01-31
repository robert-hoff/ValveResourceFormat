using System;
using System.Collections.Generic;

using ValveResourceFormat.CompiledShader;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.parsetrials;
using static MyShaderAnalysis.utilhelpers.FileArchives;


namespace MyShaderAnalysis.batchtesting1
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

            // RunBatchTestZFramesSuppressOutput();
            // RunTestZFrameBytePrintoutV62BatchSuppressOutput();
            // RunTestZFrameBytePrintoutV62Batch();
            // RunTestZFrameBytePrintoutV62();


            ShowGpuSource();
        }



        static void RunZframeBytesSetExample2()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_core_vulkan, "test_vulkan_50_hs.vcs"); int zframeIndex = 1;
            // TestZFrameFilePrintout(filenamepath, zframeIndex);
            // TestZFrameBytePrintout1(filenamepath, zframeIndex);
            TestZFrameBytePrintout2(filenamepath, zframeIndex);
        }


        static void RunZframeBytesSetExample1()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
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
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "cs_compress_dxt5_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "hero_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "hero_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "hero_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "hero_pcgl_30_psrs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pc_v64, "hero_pc_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pc_v64, "multiblend_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pc_v64, "multiblend_pc_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "generic_light_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "generic_light_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pc_v64, "generic_light_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pc_v64, "generic_light_pc_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "generic_light_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "spritecard_pcgl_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "spritecard_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "spritecard_pcgl_30_vs.vcs");
            // strange file that doesn't contain any data
            string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "msaa_resolve_cs_pcgl_50_features.vcs");

            // v44 file (will throw)
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "debugoverlay_wireframe_pc_40_gs.vcs");

            TestSingleFilePrintout(filenamepath);
        }

        static void RunTestZFrameFilePrintout()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "cs_surface_interactions_vulkan_50_cs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 1;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
        }

        static void RunTestZFrameBytePrintout2()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            TestZFrameFilePrintout(filenamepath, zframeIndex);
        }




        static void TestSingleFilePrintout(string filenamepath)
        {
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
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
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pc_v64, "multiblend_pc_30_vs.vcs"); int zframeIndex = 0;
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



        static void RunTestZFrameBytePrintoutV62BatchSuppressOutput()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "multiblend_pcgl_30_features.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v62, "depth_only_pcgl_30_features.vcs");
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;
            int MAX_ZFRAMES = 100;
            for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), MAX_ZFRAMES); i++)
            {
                byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(i);
                ParseV62ZFrame zframeParser = new ParseV62ZFrame(zframeBytes, shaderFile, outputWriter: (x) => { });
                zframeParser.PrintByteDetail();
                // Console.WriteLine(new String('_', 40));
                // Console.WriteLine($"");
                // Console.WriteLine($"");
            }
        }


        static void RunBatchTestZFramesSuppressOutput()
        {

            // ARCHIVE archive = ARCHIVE.dotagame_pcgl_v62;
            ARCHIVE archive = ARCHIVE.dota_core_pcgl_v62;
            // ARCHIVE archive = ARCHIVE.the_lab_v62;
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(GetArchiveDir(archive), VcsProgramType.Features);
            foreach (var filenamepath in vcsFiles)
            {
                Console.WriteLine($"{filenamepath}");

                ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
                BatchTestZFramesSuppressOutput(shaderFile);

                //try
                //{
                //    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
                //    BatchTestZFramesSuppressOutput(shaderFile);
                //} catch (Exception) {}
            }
        }


        static void BatchTestZFramesSuppressOutput(ShaderFile shaderFile)
        {
            int MAX_ZFRAMES = 100;
            for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), MAX_ZFRAMES); i++)
            {
                Console.WriteLine($"parsing {shaderFile.filenamepath} zframeIndex = {i}");
                byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(i);
                Console.WriteLine($"zframe size {zframeBytes.Length}");
                ParseV62ZFrame zframeParser = new ParseV62ZFrame(zframeBytes, shaderFile, outputWriter: (x) => { });
                zframeParser.PrintByteDetail();
                // Console.WriteLine(new String('_', 40));
                // Console.WriteLine($"");
                // Console.WriteLine($"");
            }
        }


        static void RunTestZFrameBytePrintoutV62Batch()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "multiblend_pcgl_30_features.vcs");
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath); ;
            // ParseV62ZFrame zframeParser = new ParseV62ZFrame(zframeBytes, shaderFile);
            // zframeParser.PrintByteDetail();

            int MAX_ZFRAMES = 10;
            for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), MAX_ZFRAMES); i++)
            {
                byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(i);
                ParseV62ZFrame zframeParser = new ParseV62ZFrame(zframeBytes, shaderFile);
                zframeParser.PrintByteDetail();
                Console.WriteLine(new string('_', 40));
                Console.WriteLine($"");
                Console.WriteLine($"");
            }
        }


        static void RunTestZFrameBytePrintoutV62()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "bloom_dota_pcgl_30_features.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "multiblend_pcgl_30_features.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "hero_pcgl_30_features.vcs"); int zframeIndex = 15;
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v62, "hero_pcgl_30_features.vcs"); int zframeIndex = 100;
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v62, "visualize_physics_pcgl_40_features.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v62, "depth_only_pcgl_30_features.vcs"); int zframeIndex = 6;
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v62, "depth_only_pcgl_30_features.vcs"); int zframeIndex = 7;


            Console.WriteLine($"{filenamepath} zframeIndex = {zframeIndex}\n");
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
            string filenamepath = GetFilenamepath(ARCHIVE.dota_core_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            Console.WriteLine($"{filenamepath}");
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            Console.WriteLine($"{shaderFile.GetZFrameCount()} zframes");
        }






        static void ShowGpuSource()
        {
            // GetFilenamepath(ARCHIVE.alyx_core_vulkan_v64, "debug_wireframe_2d_vulkan_50_ps.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "debug_wireframe_2d_vulkan_50_vs.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "debug_wireframe_2d_vulkan_50_ps.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "msaa_resolve_cs_vulkan_50_cs.vcs");
            // FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_ps.vcs");
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.alyx_core_vulkan_v64, "blend_vulkan_41_ps.vcs");

            ZFrameFile zframeFile = fileTokens.GetZframeFileByIndex(0);


            // zframeFile.PrintByteDetail();


            VulkanSource vulkanSource = (VulkanSource)zframeFile.gpuSources[2];
            ParseVulkanSource vulkanParser = new ParseVulkanSource(vulkanSource);
            // vulkanParser.PrintByteDetailSpirvReflection();
            vulkanParser.PrintByteDetail();
        }


    }
}




