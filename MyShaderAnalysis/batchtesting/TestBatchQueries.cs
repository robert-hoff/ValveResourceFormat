using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.batchtesting
{
    internal class TestBatchQueries
    {
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;


        public static void Run()
        {
            ShowSingleZFrame();
            // ShowZFrame();
            // ShowFileListing();
        }



        /*
         *
         * [ 0] cables_vulkan_50_ps.vcs zFrameCount = 34
         * [ 1] cables_vulkan_50_vs.vcs zFrameCount = 6
         * [ 2] debug_wireframe_2d_vulkan_50_ps.vcs zFrameCount = 1
         * [ 3] debug_wireframe_2d_vulkan_50_vs.vcs zFrameCount = 2
         * [ 4] error_vulkan_50_ps.vcs zFrameCount = 1
         * [ 5] error_vulkan_50_vs.vcs zFrameCount = 2
         * [ 6] gaussian_bloom_blur_vulkan_50_ps.vcs zFrameCount = 1
         * [ 7] gaussian_bloom_blur_vulkan_50_vs.vcs zFrameCount = 1
         * [ 8] generate_decal_normals_vulkan_40_ps.vcs zFrameCount = 1
         * [ 9] generate_decal_normals_vulkan_40_vs.vcs zFrameCount = 1
         * [10] generic_vulkan_50_ps.vcs zFrameCount = 348
         *
         */
        public static void ShowSingleZFrame()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            ZFrameFile zFrame = fileArchive.GetZFrameFile(10, 100);
            // Debug.WriteLine($"{zFrame.PrintByteDetail}");
            zFrame.PrintByteDetail();
        }


        public static void ShowZFrame()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            for (int i = 0; i < fileArchive.GetFileCount() + 1; i++)
            {
                Debug.WriteLine($"{fileArchive.GetFile(i)} zFrameCount = {fileArchive.GetZFrameCount(i)}");
            }
        }

        public static void ShowFileListing()
        {
            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, VS, PS, useModularLookup: true);
            foreach (var fileTokens in fileArchive.GetFileVcsTokens())
            {
                Debug.WriteLine($"{fileTokens}");
            }
        }


    }
}
