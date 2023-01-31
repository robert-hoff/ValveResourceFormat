using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.batchtesting2
{
    internal class TestSpirvReflection
    {
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            ReadFile();
            // TargetSingleFile();
            // CreateSpirvSamples();
            // WriteSpirvBytesToFile();
            // TestSpecificFile2();
            // TestSpecificFile();
            // BatchTestSpirvReflection();
            // ProblemFile();
            // NotAProblemFile();
            // IsolateSpirvReflection3();
            // IsolateSpirvReflection2();
            // IsolateSpirvReflection1();
            // BatchTestFirstSourceEachFile();
            // ShowZFramesAndSourceCount();
            // PrintServerFiles();
            // TestSpirVReflection();
        }



        public static void ReadFile()
        {
            // byte[] bytes = File.ReadAllBytes("X:\\checkouts\\SPIRV-Cross\\vcs_vulkan_samples\\textoverlay.frag.spv");
            byte[] bytes = File.ReadAllBytes("X:\\checkouts\\SPIRV-Cross\\vcs_vulkan_samples\\phongpass.frag.spv");
            Debug.WriteLine($"{BytesToString(bytes)}");


            // ByteSequenceToString.ConvertByteSequenceToString("6D 61 69 6E");
            // ByteSequenceToString.ConvertByteSequenceToString("74 79 70 65 2E 32 64 2E 69 6D 61 67 65");
            ByteSequenceToString.ConvertByteSequenceToString("47 4C 53 4C 2E 73 74 64 2E 34 35 30");
            // Debug.WriteLine($"{str}");


        }


        public static void TargetSingleFile()
        {
            // Shaderfile https://vcs.codecreation.dev/dota_game_vulkan_v65/multiblend_vulkan_40/multiblend_vulkan_40_vs-summary2.html
            // ZFrame[0] https://vcs.codecreation.dev/dota_game_vulkan_v65/multiblend_vulkan_40/zframes/multiblend_vulkan_40_vs-ZFRAME00000000-summary.html
            // ZFrame[0],Source[5] https://vcs.codecreation.dev/dota_game_vulkan_v65/multiblend_vulkan_40/vulkan/vulkan-ff91ed8e94e9bab6c01265d479fb4d86.html
            int zframeId = 0;
            int sourceIndex = 5;

            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.dota_game_vulkan_v65, "multiblend_vulkan_40_vs.vcs");
            ZFrameFile zFrameFile = vcsTokens.GetZframeFile(0);
            VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[5];
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();

            Debug.WriteLine($"file ref. {vulkanSource.GetEditorRefIdAsString()}");
            string spirvReflected = DecompileSpirvDll.DecompileVulkan(spirvBytes);
            // Debug.WriteLine($"{spirvReflected}");

            string filename = "sample4.spv";
            string directory = @"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples";
            SaveSpirVBytesToFile(spirvBytes, filename, directory);
        }


        public static void CreateSpirvSamples()
        {
            // byte[] spirvBytes = File.ReadAllBytes(@"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples\source2.spv");
            byte[] spirvBytes = File.ReadAllBytes(@"X:\checkouts\SPIRV-Cross\vcs_vulkan_samples\sample3.spv");
            // Debug.WriteLine($"{BytesToString(spirvBytes)}");

            string filename = "sample2.spv";
            string directory = @"Z:\dev\www\vcs.codecreation.dev\spirv-samples\";
            //Debug.WriteLine($"write spirv bytes to {directory}{filename}");
            File.WriteAllBytes($"{directory}{filename}", spirvBytes);

        }


        public static void WriteSpirvBytesToFile()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_ps.vcs");
            ZFrameFile zFrameFile = vcsTokens.GetZframeFile(0x44);
            VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            Debug.WriteLine($"{BytesToString(spirvBytes)}");

            string filename = "sample3.spv";
            string directory = @"X:\checkouts\VRF-Main\files_samples\";

            Debug.WriteLine($"write spirv bytes to {directory}{filename}");
            File.WriteAllBytes($"{directory}{filename}", spirvBytes);



        }


        /*
         * this file WORKS
         * but decompiling the spirv bytes with the meta data bytes does not work
         *
         *
         */
        public static void TestSpecificFile2()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_ps.vcs");
            ZFrameFile zFrameFile = vcsTokens.GetZframeFile(0);
            VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
            // byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
            // Debug.WriteLine($"{decompiledSource}");

            // -- decompiling the source bytes (with the meta data)
            byte[] sourceBytes = vulkanSource.sourcebytes;
            string decompiledSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            // Debug.WriteLine($"{BytesToString(sourceBytes)}");
        }


        /*
         * This file demonstrates it is not the size of the bytes that is the problem
         *
         * cables_vulkan_50_ps.vcs zframeId[0x44] source[0] source-size=1104
         *
         */
        public static void TestSpecificFile()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_ps.vcs");
            ZFrameFile zFrameFile = vcsTokens.GetZframeFile(0x44);
            VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes); // fixed!! I fixed it
            Debug.WriteLine($"{decompiledSource}");

            // Debug.WriteLine($"{BytesToString(spirvBytes)}");
            // -- decompiling the source bytes (with the meta data) also throws an exception
            // byte[] sourceBytes = vulkanSource.sourcebytes;
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            // Debug.WriteLine($"{BytesToString(sourceBytes)}");
        }


        /*
         * This identifies a few more problematic files. For example
         *
         * cables_vulkan_50_ps.vcs zframeId[0x44] source[0] source-size=1104
         *
         *
         *
         */
        public static void BatchTestSpirvReflection()
        {
            // FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS);
            FileArchive fileArchive = new(ARCHIVE.dota_game_vulkan_v65, VS, PS);

            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                FileVcsTokens fileVcsTokens = fileArchive.GetFileVcsTokens(i);
                Debug.WriteLine($"testing file {fileVcsTokens}");
                int zFrameCount = fileArchive.GetZFrameCount(i);
                int MAX_ZFRAMES = 100;
                for (int j = 0; j < Math.Min(zFrameCount, MAX_ZFRAMES); j++)
                {
                    ZFrameFile zFrameFile = fileArchive.GetZFrameFile(i, j);
                    new DecompileSpirvTester(zFrameFile);
                }
                //if (i==20)
                //{
                //    break;
                //}
            }
        }


        public static void ProblemFile()
        {


            // FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_gs.vcs");
            // FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_ps.vcs");
            // FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_ps.vcs");

            // string filename = "visualize_cloth_vulkan_50_gs.vcs"; int zFrameId = 0; int sourceId = 0; // this file works
            // string filename = "visualize_cloth_vulkan_50_ps.vcs"; int zFrameId = 0; int sourceId = 0;
            string filename = "cables_vulkan_50_ps.vcs"; int zFrameId = 0; int sourceId = 0;

            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, filename);
            ZFrameFile zFrameFile = vcsTokens.GetZframeFile(zFrameId);
            VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[sourceId];
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();

            // -- test parser
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);

            // -- show source
            Debug.WriteLine($"{DecompileSpirvDll.DecompileVulkan(spirvBytes)}");

            // -- save spirv bytes to file
            // SaveSpirVBytesToFile(spirvBytes, "sample3.spv");

        }

        const string DEFAULT_OUTPUT_DIR = @"X:\checkouts\VRF-Main\files_samples\";

        private static void SaveSpirVBytesToFile(byte[] spirvBytes, string filename, string directory = "")
        {
            if (directory.Length == 0)
            {
                directory = DEFAULT_OUTPUT_DIR;
            }
            Debug.WriteLine($"Saving spirv bytes to {directory}\\{filename}");
            File.WriteAllBytes($"{directory}\\{filename}", spirvBytes);
        }

        public static void NotAProblemFile()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_gs.vcs");
            //

            int zframeCount = vcsTokens.GetShaderFile().GetZFrameCount();
            for (int i = 0; i < zframeCount; i++)
            {
                int sourceCount = vcsTokens.GetZframeFile(i).gpuSources.Count;
                for (int j = 0; j < sourceCount; j++)
                {
                    // vcsTokens.GetZframeFile(i).PrintGpuSource(j);
                    ZFrameFile zFrameFile = vcsTokens.GetZframeFile(i);
                    VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
                    byte[] spirvBytes = vulkanSource.GetSpirvBytes();
                    string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
                    Debug.WriteLine($"FILE {vcsTokens} zframe[{i}] source[{j}]\n");
                    Debug.WriteLine($"{decompiledSource}\n\n");
                }
            }
        }


        public static void IsolateSpirvReflection3()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_ice_surface_vulkan_50_ps.vcs");
            ZFrameFile firstZFrameFile = vcsTokens.GetZframeFile(0);
            VulkanSource vulkanSource = (VulkanSource)firstZFrameFile.gpuSources[0];
            byte[] sourceBytes = vulkanSource.sourcebytes;
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            Debug.WriteLine($"{decompiledSource}");
        }


        public static void IsolateSpirvReflection2()
        {
            // FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_ps.vcs");
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_gs.vcs");
            ZFrameFile firstZFrameFile = vcsTokens.GetZframeFile(0);
            VulkanSource vulkanSource = (VulkanSource)firstZFrameFile.gpuSources[0];
            byte[] sourceBytes = vulkanSource.sourcebytes;
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            Debug.WriteLine($"{decompiledSource}");
        }


        /*
         * The source bytes (3146) containing some meta data, being slightly
         * longer than the spirv bytes (3128)
         *
         */
        public static void IsolateSpirvReflection1()
        {
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_gs.vcs");
            ZFrameFile firstZFrameFile = vcsTokens.GetZframeFile(0);
            VulkanSource vulkanSource = (VulkanSource)firstZFrameFile.gpuSources[0];
            byte[] sourceBytes = vulkanSource.sourcebytes;
            byte[] spirvBytes = vulkanSource.GetSpirvBytes();
            Debug.WriteLine($"{sourceBytes.Length}");
            Debug.WriteLine($"{spirvBytes.Length}");
        }


        /*
         *
         * outputWriter normally defined as
         *
         *      HandleOutputWrite outputWriter = (x) => { Console.Write(x); };
         *
         * pass the following to disable the output
         *
         *      HandleOutputWrite outputWriter = (x) => { };
         *
         * NOTE - I changed the parsing to target DecompileSpirvDll.DecompileVulkan
         * rather than writing it into the ZFrameFile
         *
         *
         */

        const int MAX_SOURCE_SIZE = 24627;
        const int PROBLEMATIC_SIZE = 24628;
        public static void BatchTestFirstSourceEachFile()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                int zFrameIndex = 0;
                int sourceIndex = 0;
                FileVcsTokens fileVcsTokens = fileArchive.GetFileVcsTokens(i);
                ZFrameFile zFrameFile = fileArchive.GetZFrameFile(i, zFrameIndex);
                // zFrameFile.PrintGpuSource(sourceIndex, outputWriter: (x) => { });
                VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
                if (vulkanSource.arg0 == -1)
                {
                    Debug.WriteLine($"{fileVcsTokens} zframe={zFrameIndex} sourceIndex={sourceIndex}");
                    Debug.WriteLine($"source size 0, skipping");
                    continue;
                }

                byte[] spirvBytes = vulkanSource.GetSpirvBytes();

                //if (spirvBytes.Length > MAX_SOURCE_SIZE)
                //{
                //    Debug.WriteLine($"{fileVcsTokens} zframe={zFrameIndex} sourceIndex={sourceIndex} sourceSize={spirvBytes.Length}");
                //    Debug.WriteLine($"file too big, skipping");
                //    continue;
                //}

                if (spirvBytes.Length == PROBLEMATIC_SIZE)
                {
                    Debug.WriteLine($"{fileVcsTokens} zframe={zFrameIndex} sourceIndex={sourceIndex} sourceSize={spirvBytes.Length}");
                    Debug.WriteLine($"FILE IS THE WRONG SIZE, skipping");
                    continue;
                }

                Debug.WriteLine($"parsing file {fileVcsTokens} zframe={zFrameIndex} sourceIndex={sourceIndex} sourceSize={spirvBytes.Length}");
                string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
            }
        }

        /*
         * shows zframes and source count for the _first_ zframe
         *
         *
         */
        public static void ShowZFramesAndSourceCount()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS);
            for (int i = 0; i < fileArchive.GetFileCount(); i++)
            {
                FileVcsTokens fileVcsTokens = fileArchive.GetFileVcsTokens(i);
                int zFrameCount = fileArchive.GetZFrameCount(i);
                int sourceCount = fileArchive.GetSourceCount(i, 0);
                Debug.WriteLine($"{fileVcsTokens,-50} {i,8} {zFrameCount,8} {sourceCount,8}");
            }
        }

        public static void PrintServerFiles()
        {
            // ARCHIVE archive = ARCHIVE.alyx_hlvr_vulkan_v64;
            ARCHIVE archive = ARCHIVE.dota_game_vulkan_v65;
            // ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
            FileArchive fileArchive = new(archive, useModularLookup: true);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                CreateHtmlServerFiles.SaveAllServerFiles(archive, fileArchive.GetFileVcsTokens(i).filename, 6, 10);
            }
        }


        public static void TestSpirVReflection()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            ZFrameFile zFrame = fileArchive.GetZFrameFile(10, 100);

            VulkanSource vulkanSource = (VulkanSource)zFrame.gpuSources[0];
            Debug.WriteLine($"ARG0");
            Debug.WriteLine($"{vulkanSource.arg0}");
            Debug.WriteLine($"---\n\n");
            // byte[] sourceBytes = vulkanSource.sourcebytes;
            byte[] sourceBytes = vulkanSource.GetSpirvBytes();
            string spirvSource = DecompileSpirvDll.DecompileVulkan(sourceBytes);
            Debug.WriteLine($"{spirvSource}");

        }



    }
}


