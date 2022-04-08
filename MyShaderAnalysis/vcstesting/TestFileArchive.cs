using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.parsetrials;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.vcstesting
{
    public class TestFileArchive
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


        public static void TestVulkanGpuSources()
        {
            int LIMIT_ZFRAMES = 20;
            int LIMIT_GPU_SOURCES = 20;

            // FileArchive vcsArchive = new FileArchive(ARCHIVE.alyx_hlvr_vulkan_v64, LIMIT_NR: 20000);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.alyx_core_vulkan_v64, LIMIT_NR: 20000);
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                for (int zi = 0; zi < Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAMES); zi++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zi);
                    for (int i = 0; i < Math.Min(zframeFile.gpuSourceCount, LIMIT_GPU_SOURCES); i++)
                    {
                        Console.WriteLine($"{shaderFile.filenamepath} zi={zi} gpu={i}");
                        VulkanSource vulkanSource = (VulkanSource)zframeFile.gpuSources[i];
                        try
                        {
                            new ParseVulkanSource(vulkanSource, outputWriter: (x) => { }).PrintByteDetailSpirvReflection();
                        } catch (Exception e)
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
            ARCHIVE[] archives = { ARCHIVE.the_lab_pc_v62};
            foreach (ARCHIVE archive in archives)
            {
                // FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Features, VcsShaderModelType._30);
                FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Features, VcsShaderModelType.Undetermined);
                TestShaderFiles(vcsArchive);
            }
        }


        public static void RunTestShaderFilesAllArchives()
        {
            var archives = Enum.GetValues(typeof(ARCHIVE));
            foreach (ARCHIVE archive in archives)
            {
                FileArchive vcsArchive = new FileArchive(archive, VcsProgramType.Undetermined, VcsShaderModelType.Undetermined);
                TestShaderFiles(vcsArchive);
            }
        }


        public static void TestShaderFiles(FileArchive vcsArchive)
        {
            foreach (ShaderFile shaderFile in vcsArchive.GetShaderFiles())
            {
                Console.WriteLine($"{shaderFile.filenamepath} zframe count = {shaderFile.GetZFrameCount()}");
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
                Console.WriteLine($"{Path.GetFileName(shaderFile.filenamepath)} zframe count = {shaderFile.GetZFrameCount()}");
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
                Console.WriteLine($"{shaderFile.filenamepath}");
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
                } catch (Exception e)
                {
                    Console.WriteLine($"Error couldn't parse {vcsTokens.filename} {e.Message}");
                }
            }
        }


        public static void TestShaderFilesBytesShowOutput()
        {
            FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_testset_pcgl_v64, VcsProgramType.VertexShader, VcsShaderModelType._30);
            foreach (FileVcsTokens vcsTokens in vcsArchive.GetFileVcsTokens())
            {
                new DataReaderVcsBytes(vcsTokens.filenamepath).PrintByteDetail();
            }
        }


        public static void ShowVcsFiles()
        {
            // FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64, VcsProgramType.Features, VcsShaderModelType._30);
            FileArchive vcsArchive = new FileArchive(ARCHIVE.the_lab_pc_v62);
            foreach (FileVcsTokens vcsTokens in vcsArchive.GetFileVcsTokens())
            {
                Console.WriteLine($"{vcsTokens}");
            }
        }



    }
}



