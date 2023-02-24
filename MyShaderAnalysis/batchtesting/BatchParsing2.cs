using System;
using System.Diagnostics;
using System.IO;
using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.parsing;
using MyShaderFile.CompiledShader;

namespace MyShaderAnalysis.batchtesting
{
    public class BatchParsing2
    {
        public static void RunTrials()
        {
            TestVulkanGpuSources();
            // RunTestShaderFilesSelectedArchives();
            // RunTestShaderFilesAllArchives();
            // TestShaderFiles();
            // TestShaderFilesBytes2();
            // TestShaderFilesBytes1();
            // TestShaderFilesBytesShowOutput();
            // ShowVcsFiles();
        }

        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;
        public static void ShowSingleZFrame()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            ZFrameFile zFrame = fileArchive.GetZFrameFile(10, 100);
            // Debug.WriteLine($"{zFrame.PrintByteDetail}");
            zFrame.PrintByteDetail();
        }

        /*
         * -- outputs
         * cables_vulkan_50_ps.vcs zFrameCount = 34
         * cables_vulkan_50_vs.vcs zFrameCount = 6
         * debug_wireframe_2d_vulkan_50_ps.vcs zFrameCount = 1
         * debug_wireframe_2d_vulkan_50_vs.vcs zFrameCount = 2
         * // ...
         *
         */
        public static void ShowZFrameCountForArchive()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            for (int i = 0; i < fileArchive.GetFileCount() + 1; i++)
            {
                Debug.WriteLine($"{fileArchive.GetFileVcsTokens(i)} zFrameCount = {fileArchive.GetZFrameCount(i)}");
            }
        }

        public static void TestVulkanGpuSources()
        {
            int LIMIT_ZFRAMES = 20;
            int LIMIT_GPU_SOURCES = 20;

            // FileArchive vcsArchive = new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, maxFiles: 20000);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.alyx_core_vulkan_v64, maxFiles: 20000);
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                for (int zi = 0; zi < Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAMES); zi++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zi);
                    for (int i = 0; i < Math.Min(zframeFile.GpuSourceCount, LIMIT_GPU_SOURCES); i++)
                    {
                        Console.WriteLine($"{shaderFile.FilenamePath} zi={zi} gpu={i}");
                        VulkanSource vulkanSource = (VulkanSource) zframeFile.GpuSources[i];
                        try
                        {
                            new ParseVulkanSource(vulkanSource, outputWriter: (x) => { }).PrintByteDetailSpirvReflection();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error {e.Message}");
                        }
                    }
                }
            }
        }

        public static void RunTestShaderFilesSelectedArchives()
        {
            // ARCHIVE[] archives = { ARCHIVE.dota_core_pcgl_v64, ARCHIVE.dota_game_pcgl_v64 };
            ARCHIVE[] archives = { ARCHIVE.the_lab_pc_v62 };
            foreach (ARCHIVE archive in archives)
            {
                // FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Features, VcsShaderModelType._30);
                FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Features);
                TestShaderFiles(vcsArchive);
            }
        }

        public static void RunTestShaderFilesAllArchives()
        {
            Array archives = Enum.GetValues(typeof(ARCHIVE));
            foreach (ARCHIVE archive in archives)
            {
                FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Undetermined);
                TestShaderFiles(vcsArchive);
            }
        }

        public static void TestShaderFiles(FileArchive vcsArchive)
        {
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                Console.WriteLine($"{shaderFile.FilenamePath} zframe count = {shaderFile.GetZFrameCount()}");
            }
        }

        /*
         * ShaderFile parsing is done by FileArchive - so retrieving a ShaderFile object means that it successfully parsed.
         * Parsing errors for the shader file is reported in the FileArchive class
         *
         */
        public static void TestShaderFiles()
        {
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Undetermined, VcsShaderModelType._40);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                Console.WriteLine($"{Path.GetFileName(shaderFile.FilenamePath)} zframe count = {shaderFile.GetZFrameCount()}");
            }
        }

        public static void TestShaderFilesBytes2()
        {
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_testset_pcgl_v64, VcsProgramType.VertexShader, VcsShaderModelType._30);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Undetermined, VcsShaderModelType._40);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                Console.WriteLine($"{shaderFile.FilenamePath}");
                shaderFile.PrintByteDetail(outputWriter: (x) => { });
            }
        }

        public static void TestShaderFilesBytes1()
        {
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (FileVcsTokens vcsTokens in vcsArchive.GetFileVcsTokens())
            {
                try
                {
                    new DataReaderVcsBytes(vcsTokens.filenamepath, outputWriter: (x) => { }).PrintByteDetail();
                    Console.WriteLine($"{vcsTokens.filename} OK");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error couldn't parse {vcsTokens.filename} {e.Message}");
                }
            }
        }
    }
}


