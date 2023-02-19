using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.parsing;
using MyShaderAnalysis.snippetcode;
using MyShaderAnalysis.util;
using System;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.filearchive.FileArchives;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

/*
 * sometime around fall 2021 beginning 2022
 *
 */
namespace MyShaderAnalysis.batchtesting
{
    public class TestBatchParsing
    {
        public static void RunTrials()
        {
            TestSingleVcsFile();
            // TestSingleFileBytes();

            // RunTestZframeBytesByArchive();
            // RunTestSingleZframeBytesSilent();
            // RunTestSingleZframeBytes();
            // RunTestSingleZframeBytesSingleId();
            // RunTestZframeParserArchive();
            // RunTestSingleZframeParser();

            // TestArchivesShaderFile();
            // TestArchivesShaderFileSummarize();
            // TestArchivesBytes();
        }

        public static void TestSingleVcsFile()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "blend_pc_41_features.vcs"); // 92500 zframes (!)
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "bilateral_blur_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "brushsplat_pc_40_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "spritecard_pc_30_features.vcs");

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            // shaderFile.PrintByteDetail(outputWriter: (x) => { }, shortenOutput: false);
            // shaderFile.PrintByteDetail(shortenOutput: false);
            shaderFile.PrintByteDetail(shortenOutput: true);
        }

        public static void TestSingleFileBytes()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_features.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "spring_meteor_pcgl_30_ps.vcs");
            // v44 file (will throw)
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "debugoverlay_wireframe_pc_40_gs.vcs");

            new DataReaderVcsBytes(filenamepath).PrintByteDetail();
            // new ParseV44Files(filenamepath);
        }

        public static void RunTestZframeBytesByArchive()
        {
            // TestZframeBytesByArchive(ARCHIVE.dota_game_pc_v65);

            TestZframeBytesByArchive(ARCHIVE.dota_game_pc_v65);
            TestZframeBytesByArchive(ARCHIVE.dota_core_pc_v65);
            TestZframeBytesByArchive(ARCHIVE.dota_core_vulkan_v65);
            TestZframeBytesByArchive(ARCHIVE.dota_game_vulkan_v65);
            TestZframeBytesByArchive(ARCHIVE.dota_game_pcgl_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_core_pcgl_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_game_pc_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_core_pc_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_core_mobile_gles_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_dac_mobile_gles_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_core_android_vulkan_v64);
            TestZframeBytesByArchive(ARCHIVE.dota_dac_android_vulkan_v64);
            TestZframeBytesByArchive(ARCHIVE.artifact_classic_core_pc_v64);
            TestZframeBytesByArchive(ARCHIVE.artifact_classic_dcg_pc_v64);
            TestZframeBytesByArchive(ARCHIVE.alyx_core_vulkan_v64);
            TestZframeBytesByArchive(ARCHIVE.alyx_core_vulkan_v64);
            TestZframeBytesByArchive(ARCHIVE.alyx_hlvr_vulkan_v64);
            TestZframeBytesByArchive(ARCHIVE.exampleset_pc_v62);
            TestZframeBytesByArchive(ARCHIVE.the_lab_pc_v62);
        }

        // public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.DomainShader)
        // public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.HullShader)
        public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.Undetermined)
        {
            string archiveName = archive.ToString();
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetArchiveDir(archive), vcsProgramType);
            string headerText = $"Parsing zframes for vcs archive {archiveName}";
            Console.WriteLine(headerText);
            Console.WriteLine(new string('-', headerText.Length));

            foreach (var f in vcsFiles)
            {
                // string reportString = $"/{archiveName}/{Path.GetFileName(f)}".PadRight(100);
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    Console.WriteLine($"/{archiveName}/{Path.GetFileName(f)} zframe-count={shaderFile.GetZFrameCount()}");
                    for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_COUNT); i++)
                    {
                        TestSingleZframeBytesSilent(shaderFile, i);
                    }
                }
                // NOTE - ignoring parsing errors at the shader-level
                catch (Exception shaderFileException)
                {
                    // Console.WriteLine($"ERROR in {i} {e.Message}");
                }
            }
        }

        static int LIMIT_ZFRAME_COUNT = 100;

        public static void RunTestSingleZframeBytesSilent()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_ps.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "hero_pcgl_30_vs.vcs");
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_COUNT); i++)
            {
                TestSingleZframeBytesSilent(shaderFile, i);
            }
            Console.WriteLine($"completed");
        }

        public static void TestSingleZframeBytesSilent(ShaderFile shaderFile, int zFrameIndex)
        {
            byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zFrameIndex);
            DataReaderZFrameBytes dataReaderZframeBytes = new DataReaderZFrameBytes(zframeBytes, shaderFile.VcsProgramType,
                shaderFile.VcsPlatformType, shaderFile.VcsShaderModelType, showStatusMessage: false, outputWriter: (x) => { });
            try
            {
                dataReaderZframeBytes.PrintByteDetail();
            } catch (Exception e)
            {
                long zframeId = shaderFile.GetZFrameIdByIndex(zFrameIndex);
                Console.WriteLine($"ERROR in Z[0x{zframeId:x08}] zframeIndex={zFrameIndex} {e.Message}");
            }
        }

        public static void RunTestSingleZframeBytes()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "hero_pcgl_30_vs.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "black_vulkan_40_vs.vcs");

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
            {
                TestSingleZframeBytes(shaderFile, i);
            }
        }

        public static void RunTestSingleZframeBytesSingleId()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "hero_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_vulkan_v65, "black_vulkan_40_vs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.artifact_classiccore_pc, "generic_pc_30_ps.vcs"); int zframeIndex = 74;
            // string filenamepath = GetFilenamepath(ARCHIVE.artifact_classiccore_pc, "generic_pc_30_ps.vcs"); int zframeIndex = 74;

            // X:/hl2alyx-export/alyx-vulkan-hlvr/vr_projected_decals_vulkan_50_ps.vcs
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_projected_decals_vulkan_50_ps.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_core_vulkan, "test_vulkan_50_hs.vcs"); int zframeIndex = 1;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 1;

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            TestSingleZframeBytes(shaderFile, zframeIndex);
        }

        public static void TestSingleZframeBytes(ShaderFile shaderFile, int zframeIndex)
        {
            byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zframeIndex);
            // Console.WriteLine($"{zframeBytes.Length}");

            DataReaderZFrameBytes dataReaderZframe = new DataReaderZFrameBytes(zframeBytes,
                shaderFile.VcsProgramType, shaderFile.VcsPlatformType, shaderFile.VcsShaderModelType, showStatusMessage: false,
                // outputWriter: (x) => { });
                outputWriter: null);

            long zframeId = shaderFile.GetZFrameIdByIndex(zframeIndex);
            Console.Write($"parsing zframe[0x{zframeId:x08}]");
            dataReaderZframe.PrintByteDetail();
        }

        public static void RunTestZframeParserArchive()
        {
            //TestZframeParserArchive(ARCHIVE.dotagame_pc_v65);
            //TestZframeParserArchive(ARCHIVE.dotacore_pc_v65);
            //TestZframeParserArchive(ARCHIVE.dotacore_vulkan_v65);
            //TestZframeParserArchive(ARCHIVE.dotagame_vulkan_v65);
            //TestZframeParserArchive(ARCHIVE.dotagame_pcgl);
            //TestZframeParserArchive(ARCHIVE.dotacore_pcgl);
            //TestZframeParserArchive(ARCHIVE.dotagame_pc);
            //TestZframeParserArchive(ARCHIVE.dotacore_pc);
            //TestZframeParserArchive(ARCHIVE.dota_core_gles);
            //TestZframeParserArchive(ARCHIVE.dota_dac_gles);
            //TestZframeParserArchive(ARCHIVE.dota_core_android_vulkan);
            //TestZframeParserArchive(ARCHIVE.dota_dac_android_vulkan);
            //TestZframeParserArchive(ARCHIVE.artifact_classiccore_pc);
            //TestZframeParserArchive(ARCHIVE.artifact_classicdcg_pc);
            TestZframeParserArchive(ARCHIVE.alyx_core_vulkan_v64);
            TestZframeParserArchive(ARCHIVE.alyx_hlvr_vulkan_v64);
            TestZframeParserArchive(ARCHIVE.the_lab_pc_v62);
            TestZframeParserArchive(ARCHIVE.the_lab_pc_v62);
        }

        public static void TestZframeParserArchive(ARCHIVE archive)
        {
            string archiveName = archive.ToString();
            // List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetArchiveDir(archive), VcsProgramType.HullShader);
            string headerText = $"Parsing zframes for vcs archive {archiveName}";
            Console.WriteLine(headerText);
            Console.WriteLine(new string('-', headerText.Length));

            foreach (var f in vcsFiles)
            {
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    Console.WriteLine($"/{archiveName}/{Path.GetFileName(f)} zframe-count={shaderFile.GetZFrameCount()}");
                    for (int i = 0; i < Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_COUNT); i++)
                    {
                        TestSingleZframeParser(shaderFile, i);
                    }
                }
                // NOTE - ignoring parsing errors at the shader-level
                catch (Exception shaderFileException)
                {
                    // Console.WriteLine($"ERROR in {i} {e.Message}");
                }
            }
        }

        public static void RunTestSingleZframeParser()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_projected_decals_vulkan_50_ps.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_core_vulkan, "test_vulkan_50_hs.vcs"); int zframeIndex = 1;
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 1;

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            TestSingleZframeParser(shaderFile, zframeIndex);
        }

        public static void TestSingleZframeParser(ShaderFile shaderFile, int zFrameIndex)
        {
            try
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zFrameIndex);
            } catch (Exception e)
            {
                long zframeId = shaderFile.GetZFrameIdByIndex(zFrameIndex);
                Console.WriteLine($"ERROR in Z[0x{zframeId:x08}] zframeIndex={zFrameIndex} {e.GetType()} {e.Message}");
            }
        }

        public static void TestArchivesShaderFile()
        {
            TestArchiveShader(ARCHIVE.dota_game_pc_v65);
            TestArchiveShader(ARCHIVE.dota_core_pc_v65);
            TestArchiveShader(ARCHIVE.dota_core_vulkan_v65);
            TestArchiveShader(ARCHIVE.dota_game_vulkan_v65);
            TestArchiveShader(ARCHIVE.dota_game_pcgl_v64);
            TestArchiveShader(ARCHIVE.dota_core_pcgl_v64);
            TestArchiveShader(ARCHIVE.dota_game_pc_v64);
            TestArchiveShader(ARCHIVE.dota_core_pc_v64);
            TestArchiveShader(ARCHIVE.dota_core_mobile_gles_v64);
            TestArchiveShader(ARCHIVE.dota_dac_mobile_gles_v64);
            TestArchiveShader(ARCHIVE.dota_core_android_vulkan_v64);
            TestArchiveShader(ARCHIVE.dota_dac_android_vulkan_v64);
            TestArchiveShader(ARCHIVE.artifact_classic_core_pc_v64);
            TestArchiveShader(ARCHIVE.artifact_classic_dcg_pc_v64);
            TestArchiveShader(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveShader(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveShader(ARCHIVE.alyx_hlvr_vulkan_v64);
            TestArchiveShader(ARCHIVE.exampleset_pc_v62);
            TestArchiveShader(ARCHIVE.the_lab_pc_v62);
        }

        public static void TestArchiveShader(ARCHIVE archive)
        {
            string archiveName = archive.ToString();
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetArchiveDir(archive), VcsProgramType.Undetermined);

            string headerText = $"Parsing vcs archive {archiveName}";
            Console.WriteLine(headerText);
            Console.WriteLine(new string('-', headerText.Length));

            foreach (var f in vcsFiles)
            {
                string reportString = $"     /{archiveName}/{Path.GetFileName(f)}".PadRight(100);
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    Console.WriteLine($"{reportString} OK");
                } catch (Exception e)
                {
                    Console.WriteLine($"{reportString} ERROR\n {e.Message}");
                }
            }
            Console.WriteLine();
        }

        public static void TestArchivesShaderFileSummarize()
        {
            TestArchiveShaderFileSummarise(ARCHIVE.dota_game_pc_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_pc_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_vulkan_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_game_vulkan_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_game_pcgl_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_pcgl_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_game_pc_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_pc_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_mobile_gles_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_dac_mobile_gles_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_android_vulkan_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_dac_android_vulkan_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.artifact_classic_core_pc_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.artifact_classic_dcg_pc_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.alyx_hlvr_vulkan_v64);
            TestArchiveShaderFileSummarise(ARCHIVE.exampleset_pc_v62);
            TestArchiveShaderFileSummarise(ARCHIVE.the_lab_pc_v62);
        }

        public static void TestArchiveShaderFileSummarise(ARCHIVE archive, bool showSuccesses = false)
        {
            string archiveName = archive.ToString();
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetArchiveDir(archive), VcsProgramType.Undetermined);
            // List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.GeometryShader);

            string headerText = $"Parsing vcs archive {archiveName}";
            Console.WriteLine(headerText);
            Console.WriteLine(new string('-', headerText.Length));

            StatCounter statCounter = new();
            foreach (var f in vcsFiles)
            {
                VcsProgramType vcsProgType = ComputeVcsProgramType(f);
                string reportString = $"     /{archiveName}/{Path.GetFileName(f)}".PadRight(100);
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    if (showSuccesses)
                    {
                        Console.WriteLine($"{reportString} OK");
                    }
                    statCounter.recPassed(vcsProgType);
                } catch (Exception e)
                {
                    Console.WriteLine($"Error in /{archiveName}/{Path.GetFileName(f)} - {e.Message}");
                    statCounter.recFail(vcsProgType);
                }
            }
            Console.WriteLine($"{statCounter.getReport()}\n\n");
        }

        public static void TestArchivesBytes()
        {
            TestArchiveBytes(ARCHIVE.dota_game_pc_v65);
            TestArchiveBytes(ARCHIVE.dota_core_pc_v65);
            TestArchiveBytes(ARCHIVE.dota_core_vulkan_v65);
            TestArchiveBytes(ARCHIVE.dota_game_vulkan_v65);
            TestArchiveBytes(ARCHIVE.dota_game_pcgl_v64);
            TestArchiveBytes(ARCHIVE.dota_core_pcgl_v64);
            TestArchiveBytes(ARCHIVE.dota_game_pc_v64);
            TestArchiveBytes(ARCHIVE.dota_core_pc_v64);
            TestArchiveBytes(ARCHIVE.dota_core_mobile_gles_v64);
            TestArchiveBytes(ARCHIVE.dota_dac_mobile_gles_v64);
            TestArchiveBytes(ARCHIVE.dota_core_android_vulkan_v64);
            TestArchiveBytes(ARCHIVE.dota_dac_android_vulkan_v64);
            TestArchiveBytes(ARCHIVE.artifact_classic_core_pc_v64);
            TestArchiveBytes(ARCHIVE.artifact_classic_dcg_pc_v64);
            TestArchiveBytes(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveBytes(ARCHIVE.alyx_core_vulkan_v64);
            TestArchiveBytes(ARCHIVE.alyx_hlvr_vulkan_v64);
            TestArchiveBytes(ARCHIVE.exampleset_pc_v62);
            TestArchiveBytes(ARCHIVE.the_lab_pc_v62);
        }

        /*
         * This test depends on the shaderFile parsing correctly first,
         * then it will attempt to use the inbuilt PrintByteDetail() method, which parses the bytes again
         *
         *
         */
        public static void TestArchiveBytes(ARCHIVE archive)
        {
            string archiveName = archive.ToString();
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetArchiveDir(archive), VcsProgramType.Undetermined);
            foreach (var f in vcsFiles)
            {
                // new DataReaderVcsBytes(f, outputWriter: (x) => { }, showStatusMessage: true).PrintByteDetail(archiveName);

                Console.WriteLine($"{f}");
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    shaderFile.PrintByteDetail(outputWriter: (x) => { });
                } catch (Exception e)
                {
                    Console.WriteLine($"ERROR {e.Message}");
                }
            }
        }
    }
}

