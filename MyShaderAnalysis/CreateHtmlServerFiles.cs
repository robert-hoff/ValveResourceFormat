using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.codestash;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using MyShaderAnalysis.vcsanalysis;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using System.Diagnostics;
using System.Globalization;
using MyShaderAnalysis.utilhelpers;

/*
 *
 * CreateHtmlServerFiles
 *
 * provide a single entry point for managing the printout of vcs files and their dependencies
 *
 * Possibly, define ShaderCollection, for example
 *
 *      3dskyboxstencil_pcgl_30 (dota-game, pcgl)
 *
 * defined by the shader collection name (3dskyboxstencil_pcgl_30) archive name (dota-game) and platform (pcgl)
 *
 * The colletion should have knowledge of all the related shaders (features.vcs, vs.vcs, ps.vcs)
 *
 *      3dskyboxstencil_pcgl_30_features.vcs
 *      3dskyboxstencil_pcgl_30_vs.vcs
 *      3dskyboxstencil_pcgl_30_ps.vcs
 *
 * and related zframes, and gpu sources
 *
 *
 * NOTE NOTE - haven't as yet got round to implement the shaders using this collection idea.
 *
 *
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis
{

    public class CreateHtmlServerFiles
    {

        public static void RunTrials()
        {

            SaveServerSets();
            // SingleFileExamples2();
            // SingleFileExamples1();
        }


        public static void SaveServerSets()
        {
            // SaveAllServerFiles(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "3dskyboxstencil_pcgl_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dotagame_pc, "3dskyboxstencil_pc_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dotagame_pc, "3dskyboxstencil_pc_40_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dotagame_pc, "multiblend_pc_40_vs.vcs", zframesToPrint: 20, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dotagame_pc, "multiblend_pc_40_ps.vcs", zframesToPrint: 20, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_dac_gles, "citycrawl_building_anim_mobile_gles_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_core_android_vulkan, "sky_model_android_vulkan_40_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_dac_android_vulkan, "global_lit_simple_android_vulkan_40_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.alyx_hlvr_vulkan_v64, "cables_vulkan_50_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);


            SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);

            // SaveAllServerFiles(ARCHIVE.dotagame_vulkan_v65, "multiblend_vulkan_40_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dotagame_vulkan_v65, "multiblend_vulkan_40_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "blend_pc_41_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
        }

        public static void SaveAllServerFiles(ARCHIVE archive, string filename, int zframesToPrint, int gpuSourcesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveAllServerFiles(zframesToPrint, gpuSourcesToPrint);
        }


        public static void SingleFileExamples2()
        {
            // ParseVcsFile vcsFile = GetVcsFile(ARCHIVE.dotagame_pc, "multiblend_pc_30_vs.vcs");
            ParseVcsFile vcsFile = GetVcsFile(ARCHIVE.the_lab_pc_v62, "bilateral_blur_pc_30_features.vcs");
            // vcsFile.SaveVcsSummaryToHtml();
            vcsFile.SaveVcsByteSummaryToHtml();
        }


        public static void SingleFileExamples1()
        {
            // SaveVcsBytesAndSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs");
            // SaveZframeSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", zframeId: 0);
            // SaveZframeSummaries(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", zframesToPrint: 20);
            // SaveGpuSource(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", zframeId: 0, gpuSourceId: 0);
            SaveGpuSources(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs", zframeId: 0, gpuSourcesToPrint: 20);
        }




        public static ParseVcsFile GetVcsFile(ARCHIVE archive, string filename)
        {
            return new ParseVcsFile(archive, filename, convertLinksToHtml: true);
        }

        public static void SaveVcsBytesAndSummary(ARCHIVE archive, string filename)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveVcsByteSummaryToHtml();
            vcsFile.SaveVcsSummaryToHtml();
        }


        public static void SaveZframeSummary(ARCHIVE archive, string filename, int zframeId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveZframeByteSummaryToHtml(zframeId);
            vcsFile.SaveZframeSummaryToHtml(zframeId);
        }


        public static void SaveZframeSummaries(ARCHIVE archive, string filename, int zframesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            // vcsFile.SaveZframeByteSummaries(requestedZframesToPrint);
            vcsFile.SaveZframeSummaries(zframesToPrint);
        }


        public static void SaveGpuSource(ARCHIVE archive, string filename, int zframeId, int gpuSourceId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveGpuSourceToHtml(zframeId, gpuSourceId);
        }


        public static void SaveGpuSources(ARCHIVE archive, string filename, int zframeId, int gpuSourcesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveGpuSources(zframeId, gpuSourcesToPrint);
        }






    }
}




