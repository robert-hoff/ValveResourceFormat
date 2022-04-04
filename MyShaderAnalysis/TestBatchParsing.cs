using MyShaderAnalysis.codestash;
using MyShaderAnalysis.codestash.snippetcode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.codestash.FileArchives;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis
{
    public class TestBatchParsing
    {

        public static void RunTrials()
        {
            // RunTestSingleShaderFile();
            // RunTestSingleFileBytes();

            RunTestZframeBytesByArchive();
            // RunTestSingleZframeBytesSilent();
            // RunTestSingleZframeBytes();
            // RunTestSingleZframeBytesSingleId();
            // RunTestZframeParserArchive();
            // RunTestSingleZframeParser();


            // TestArchivesShaderFile();
            // TestArchivesShaderFileSummarize();
            // TestArchivesBytes();
        }


        public static void RunTestSingleShaderFile()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "blend_pc_41_features.vcs"); // 92500 zframes (!)
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "brushsplat_pc_40_features.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "spritecard_pc_30_features.vcs");
            TestSingleShaderFile(filenamepath);

        }
        public static void TestSingleShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            // shaderFile.PrintByteDetail(outputWriter: (x) => { }, shortenOutput: false);
            // shaderFile.PrintByteDetail(shortenOutput: false);
            shaderFile.PrintByteDetail(shortenOutput: true);
        }


        public static void RunTestSingleFileBytes()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_features.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "spring_meteor_pcgl_30_ps.vcs");
            TestSingleFileBytes(filenamepath);
        }


        public static void TestSingleFileBytes(string filenamepath)
        {
            new DataReaderVcsBytes(filenamepath, showStatusMessage: true).PrintByteDetail();
        }



        public static void RunTestZframeBytesByArchive()
        {
            TestZframeBytesByArchive(ARCHIVE.dotagame_pc_v65);
            TestZframeBytesByArchive(ARCHIVE.dotacore_pc_v65);
            TestZframeBytesByArchive(ARCHIVE.dotacore_vulkan_v65);
            TestZframeBytesByArchive(ARCHIVE.dotagame_vulkan_v65);
            TestZframeBytesByArchive(ARCHIVE.dotagame_pcgl);
            TestZframeBytesByArchive(ARCHIVE.dotacore_pcgl);
            TestZframeBytesByArchive(ARCHIVE.dotagame_pc);
            TestZframeBytesByArchive(ARCHIVE.dotacore_pc);
            TestZframeBytesByArchive(ARCHIVE.dota_core_gles);
            TestZframeBytesByArchive(ARCHIVE.dota_dac_gles);
            TestZframeBytesByArchive(ARCHIVE.dota_core_android_vulkan);
            TestZframeBytesByArchive(ARCHIVE.dota_dac_android_vulkan);
            TestZframeBytesByArchive(ARCHIVE.artifact_classiccore_pc);
            TestZframeBytesByArchive(ARCHIVE.artifact_classicdcg_pc);
            TestZframeBytesByArchive(ARCHIVE.alyx_core_vulkan);
            TestZframeBytesByArchive(ARCHIVE.alyx_hlvr_vulkan);
            TestZframeBytesByArchive(ARCHIVE.alyx_hlvr_vulkan);
            TestZframeBytesByArchive(ARCHIVE.v62_examples);
            TestZframeBytesByArchive(ARCHIVE.the_lab_v62);
        }


        // public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.DomainShader)
        // public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.HullShader)
        public static void TestZframeBytesByArchive(ARCHIVE archive, VcsProgramType vcsProgramType = VcsProgramType.Undetermined)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), vcsProgramType);
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
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "3dskyboxstencil_pcgl_30_ps.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_vs.vcs");
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
            DataReaderZFrameBytes dataReaderZframeBytes = new DataReaderZFrameBytes(zframeBytes, shaderFile.vcsProgramType,
                shaderFile.vcsPlatformType, shaderFile.vcsShaderModelType, showStatusMessage: false, outputWriter: (x) => { });
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
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "3dskyboxstencil_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_vs.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "black_vulkan_40_vs.vcs");

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
            {
                TestSingleZframeBytes(shaderFile, i);
            }
        }

        public static void RunTestSingleZframeBytesSingleId()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "3dskyboxstencil_pcgl_30_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_vs.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.dotacore_vulkan_v65, "black_vulkan_40_vs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.artifact_classiccore_pc, "generic_pc_30_ps.vcs"); int zframeIndex = 74;
            // string filenamepath = GetFilenamepath(ARCHIVE.artifact_classiccore_pc, "generic_pc_30_ps.vcs"); int zframeIndex = 74;

            // X:/hl2alyx-export/alyx-vulkan-hlvr/vr_projected_decals_vulkan_50_ps.vcs
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_projected_decals_vulkan_50_ps.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_ds.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan, "vr_ice_surface_vulkan_50_hs.vcs"); int zframeIndex = 0;
            // string filenamepath = GetFilenamepath(ARCHIVE.alyx_core_vulkan, "test_vulkan_50_hs.vcs"); int zframeIndex = 1;
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_ds.vcs"); int zframeIndex = 1;

            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            TestSingleZframeBytes(shaderFile, zframeIndex);
        }


        public static void TestSingleZframeBytes(ShaderFile shaderFile, int zframeIndex)
        {
            byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zframeIndex);
            // Console.WriteLine($"{zframeBytes.Length}");

            DataReaderZFrameBytes dataReaderZframe = new DataReaderZFrameBytes(zframeBytes,
                shaderFile.vcsProgramType, shaderFile.vcsPlatformType, shaderFile.vcsShaderModelType, showStatusMessage: false,
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
            TestZframeParserArchive(ARCHIVE.alyx_core_vulkan);
            TestZframeParserArchive(ARCHIVE.alyx_hlvr_vulkan);
            TestZframeParserArchive(ARCHIVE.v62_examples);
            TestZframeParserArchive(ARCHIVE.the_lab_v62);
        }



        public static void TestZframeParserArchive(ARCHIVE archive)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            // List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.HullShader);
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
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_v62, "test_pcgl_50_hs.vcs"); int zframeIndex = 0;
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




        /*
        public static void TestArchivesZFrames()
        {
            // TestArchiveZFrames(ARCHIVE.dotagame_pc_v65);
            //TestArchiveZFrames(ARCHIVE.dotacore_pc_v65);
            //TestArchiveZFrames(ARCHIVE.dotacore_vulkan_v65);
            //TestArchiveZFrames(ARCHIVE.dotagame_vulkan_v65);
            TestArchiveZFrames(ARCHIVE.dotagame_pcgl);
            //TestArchiveZFrames(ARCHIVE.dotacore_pcgl);
            //TestArchiveZFrames(ARCHIVE.dotagame_pc);
            //TestArchiveZFrames(ARCHIVE.dotacore_pc);
            //TestArchiveZFrames(ARCHIVE.dota_core_gles);
            //TestArchiveZFrames(ARCHIVE.dota_dac_gles);
            //TestArchiveZFrames(ARCHIVE.dota_core_android_vulkan);
            //TestArchiveZFrames(ARCHIVE.dota_dac_android_vulkan);
            //TestArchiveZFrames(ARCHIVE.artifact_classiccore_pc);
            //TestArchiveZFrames(ARCHIVE.artifact_classicdcg_pc);
            //TestArchiveZFrames(ARCHIVE.alyx_core_vulkan);
            //TestArchiveZFrames(ARCHIVE.alyx_hlvr_vulkan);
            //TestArchiveZFrames(ARCHIVE.v62_examples);
            //TestArchiveZFrames(ARCHIVE.the_lab_v62);
        }
        public static void TestArchiveZFrames(ARCHIVE archive)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);

            string headerText = $"Parsing vcs archive {archiveName}";
            Console.WriteLine(headerText);
            Console.WriteLine(new string('-', headerText.Length));

            foreach (var f in vcsFiles)
            {
                string reportString = $"/{archiveName}/{Path.GetFileName(f)}".PadRight(100);
                try
                {
                    ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(f);
                    Console.WriteLine($"{reportString} OK");
                    int zframeCount = 0;
                    int zframePassed = 0;
                    int zframeFailed = 0;
                    int maxZframes = shaderFile.GetZFrameCount();
                    for (int i = 0; i < Math.Min(maxZframes, 100); i++)
                    {
                        byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(zframeCount);
                        Console.WriteLine($"{zframeBytes.Length}");
                        zframeCount++;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine($"{reportString} ERROR\n {e.Message}");
                }
            }
            Console.WriteLine();
        }
        */


        public static void TestArchivesShaderFile()
        {
            TestArchiveShader(ARCHIVE.dotagame_pc_v65);
            TestArchiveShader(ARCHIVE.dotacore_pc_v65);
            TestArchiveShader(ARCHIVE.dotacore_vulkan_v65);
            TestArchiveShader(ARCHIVE.dotagame_vulkan_v65);
            TestArchiveShader(ARCHIVE.dotagame_pcgl);
            TestArchiveShader(ARCHIVE.dotacore_pcgl);
            TestArchiveShader(ARCHIVE.dotagame_pc);
            TestArchiveShader(ARCHIVE.dotacore_pc);
            TestArchiveShader(ARCHIVE.dota_core_gles);
            TestArchiveShader(ARCHIVE.dota_dac_gles);
            TestArchiveShader(ARCHIVE.dota_core_android_vulkan);
            TestArchiveShader(ARCHIVE.dota_dac_android_vulkan);
            TestArchiveShader(ARCHIVE.artifact_classiccore_pc);
            TestArchiveShader(ARCHIVE.artifact_classicdcg_pc);
            TestArchiveShader(ARCHIVE.alyx_core_vulkan);
            TestArchiveShader(ARCHIVE.alyx_hlvr_vulkan);
            TestArchiveShader(ARCHIVE.v62_examples);
            TestArchiveShader(ARCHIVE.the_lab_v62);
        }


        public static void TestArchiveShader(ARCHIVE archive)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);

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
            TestArchiveShaderFileSummarise(ARCHIVE.dotagame_pc_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dotacore_pc_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dotacore_vulkan_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dotagame_vulkan_v65);
            TestArchiveShaderFileSummarise(ARCHIVE.dotagame_pcgl);
            TestArchiveShaderFileSummarise(ARCHIVE.dotacore_pcgl);
            TestArchiveShaderFileSummarise(ARCHIVE.dotagame_pc);
            TestArchiveShaderFileSummarise(ARCHIVE.dotacore_pc);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_gles);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_dac_gles);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_android_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_dac_android_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_core_ios_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.dota_dac_ios_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.artifact_classiccore_pc);
            TestArchiveShaderFileSummarise(ARCHIVE.artifact_classicdcg_pc);
            TestArchiveShaderFileSummarise(ARCHIVE.alyx_core_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.alyx_hlvr_vulkan);
            TestArchiveShaderFileSummarise(ARCHIVE.v62_examples);
            TestArchiveShaderFileSummarise(ARCHIVE.the_lab_v62);
        }


        public static void TestArchiveShaderFileSummarise(ARCHIVE archive, bool showSuccesses = false)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);
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
            TestArchiveBytes(ARCHIVE.dotagame_pc_v65);
            TestArchiveBytes(ARCHIVE.dotacore_pc_v65);
            TestArchiveBytes(ARCHIVE.dotacore_vulkan_v65);
            TestArchiveBytes(ARCHIVE.dotagame_vulkan_v65);
            TestArchiveBytes(ARCHIVE.dotagame_pcgl);
            TestArchiveBytes(ARCHIVE.dotacore_pcgl);
            TestArchiveBytes(ARCHIVE.dotagame_pc);
            TestArchiveBytes(ARCHIVE.dotacore_pc);
            TestArchiveBytes(ARCHIVE.dota_core_gles);
            TestArchiveBytes(ARCHIVE.dota_dac_gles);
            TestArchiveBytes(ARCHIVE.dota_core_android_vulkan);
            TestArchiveBytes(ARCHIVE.dota_dac_android_vulkan);
            TestArchiveBytes(ARCHIVE.artifact_classiccore_pc);
            TestArchiveBytes(ARCHIVE.artifact_classicdcg_pc);
            TestArchiveBytes(ARCHIVE.alyx_core_vulkan);
            TestArchiveBytes(ARCHIVE.alyx_hlvr_vulkan);
            TestArchiveBytes(ARCHIVE.v62_examples);
            TestArchiveBytes(ARCHIVE.the_lab_v62);
        }


        // having problems with bilateral_blur_pc_30_features.vcs
        public static void TestArchiveBytes(ARCHIVE archive)
        {
            string archiveName = FileArchives.GetArchiveName(archive);
            List<string> vcsFiles = MyShaderUtilHelpers.GetVcsFiles(FileArchives.GetSourceDir(archive), VcsProgramType.Undetermined);
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

