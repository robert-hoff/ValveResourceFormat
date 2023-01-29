using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.batchtesting
{
    internal class TestSpirvReflection
    {
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;

        public static void Run()
        {
            // TestSpecificFile2();
            TestSpecificFile();
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
            // string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes); // will throw an exception
            Debug.WriteLine($"{BytesToString(spirvBytes)}");


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
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                FileVcsTokens fileVcsTokens = fileArchive.GetFileVcsTokens(i);
                int zFrameCount = fileArchive.GetZFrameCount(i);
                for (int j = 0; j < zFrameCount; j++)
                {
                    ZFrameFile zFrameFile = fileArchive.GetZFrameFile(i, j);
                    new DecompileSpirvTester(zFrameFile);
                }
                if (i==2)
                {
                    break;
                }
            }
        }


        public static void ProblemFile()
        {
            // FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_gs.vcs");
            FileVcsTokens vcsTokens = new FileVcsTokens(ARCHIVE.alyx_hlvr_vulkan_v64, "visualize_cloth_vulkan_50_ps.vcs");
            int zframeCount = vcsTokens.GetShaderFile().GetZFrameCount();
            for (int i = 0; i < zframeCount; i++)
            {
                ZFrameFile zFrameFile = vcsTokens.GetZframeFileByIndex(i);
                int sourceCount = zFrameFile.gpuSources.Count;
                for (int j = 0; j < sourceCount; j++)
                {
                    // Debug.WriteLine($"file {vcsTokens} got zframeByIndex[{i}] source[{j}]");

                    Debug.WriteLine($"file {vcsTokens}, parsing zframeByIndex[{i}] source[{j}]");
                    VulkanSource vulkanSource = (VulkanSource)zFrameFile.gpuSources[0];
                    byte[] spirvBytes = vulkanSource.GetSpirvBytes();
                    string decompiledSource = DecompileSpirvDll.DecompileVulkan(spirvBytes);
                }
            }
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
            VulkanSource vulkanSource = (VulkanSource) firstZFrameFile.gpuSources[0];
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
                VulkanSource vulkanSource = (VulkanSource) zFrameFile.gpuSources[0];
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
            ARCHIVE archive = ARCHIVE.alyx_hlvr_vulkan_v64;
            // ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
            FileArchive fileArchive = new(archive, useModularLookup: true);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                CreateHtmlServerFiles.SaveAllServerFiles(archive, fileArchive.GetFileVcsTokens(i).filename, 10, 10);
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


