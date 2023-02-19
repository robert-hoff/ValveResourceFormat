using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.util;
using ValveResourceFormat.CompiledShader;

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
 * The collection should have knowledge of all the related shaders (features.vcs, vs.vcs, ps.vcs)
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
 */
namespace MyShaderAnalysis.serverhtml
{
    public class CreateHtmlServerFiles
    {
        public static void RunTrials()
        {
            // PrintFileDirectoryGivenArchive();
            SaveAllServerFilesFromArchive();
            // SaveServerSets();
            // SingleFileExamples2();
            // SingleFileExamples1();
        }

        /*
         * Creates html index.html file directory, but it doesn't check if all links
         * are actually active.
         *
         */
        public static void CreateHtmlIndexForArchive()
        {
            ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
            FileArchive fileArchive = new(archive, VcsShaderModelType._40, useModularLookup: true);

            // ARCHIVE archive = ARCHIVE.alyx_hlvr_vulkan_v64;
            // ARCHIVE archive = ARCHIVE.dota_game_vulkan_v65;
            // FileArchive fileArchive = new(archive, useModularLookup: true);

            string filenamepath = $"Z:\\dev\\www\\vcs.codecreation.dev\\{fileArchive.archive}\\index.html";
            FileWriter fileWriter = new FileWriter(filenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader($"{fileArchive.archive}", $"{fileArchive.archive}");
            foreach (FileVcsTokens fileVcs in fileArchive.GetFileVcsTokens())
            {
                // Debug.WriteLine($"<a href='{fileVcs.GetServerFileUrl("summary2")}'>{fileVcs}</a>");
                fileWriter.WriteLine($"<a href='{fileVcs.GetServerFileUrl("summary2")}'>{fileVcs}</a>");
            }
            fileWriter.CloseStreamWriter();
            fileWriter.Dispose();
        }

        public static void SaveAllServerFilesFromArchive()
        {
            // ARCHIVE archive = ARCHIVE.alyx_hlvr_vulkan_v64;
            // ARCHIVE archive = ARCHIVE.dota_game_vulkan_v65;
            //FileArchive fileArchive = new(archive, useModularLookup: true);

            ARCHIVE archive = ARCHIVE.dota_core_pcgl_v64;
            // ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
            // ARCHIVE archive = ARCHIVE.dota_game_pc_v65;
            FileArchive fileArchive = new(archive, VcsShaderModelType._40, useModularLookup: true);

            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                SaveAllServerFiles(archive, fileArchive.GetFileVcsTokens(i).filename, 5, 5, saveGpuByteDetail: false);
                //if (i==2)
                //    {
                //        break;
                //       }
            }
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

            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 20);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 20);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 20);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "hero_vulkan_40_psrs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 20);

            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "multiblend_vulkan_40_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_vulkan_v65, "multiblend_vulkan_40_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);

            // SaveAllServerFiles(ARCHIVE.dota_game_pc_v65, "multiblend_pc_40_ps.vcs", zframesToPrint: 10, gpuSourcesToPrint: 10, saveGpuByteDetail: false);
            // SaveAllServerFiles(ARCHIVE.dota_game_pc_v65, "multiblend_pc_40_vs.vcs", zframesToPrint: 10, gpuSourcesToPrint: 10, saveGpuByteDetail: false);

            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "crystal_pcgl_40_vs.vcs", zframesToPrint: 10000, gpuSourcesToPrint: 10000);
            SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "crystal_pcgl_40_ps.vcs", zframesToPrint: 10000, gpuSourcesToPrint: 10000);

            // SaveAllServerFiles(ARCHIVE.dota_game_pc_v64, "multiblend_pc_30_ps.vcs", zframesToPrint: 30, gpuSourcesToPrint: 30);
            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "spritecard_pcgl_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.dota_game_pcgl_v64, "spritecard_pcgl_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);

            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "bilateral_blur_pc_30_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "blend_pc_41_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_features.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_ps.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            // SaveAllServerFiles(ARCHIVE.the_lab_v62, "aerial_perspective_pc_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);

            // SaveAllServerFiles(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_xen_foliage_vulkan_50_features.vcs", zframesToPrint: 30, gpuSourcesToPrint: 30);
            // SaveAllServerFiles(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_xen_foliage_vulkan_50_vs.vcs", zframesToPrint: 30, gpuSourcesToPrint: 30);
            // SaveAllServerFiles(ARCHIVE.alyx_hlvr_vulkan_v64, "vr_xen_foliage_vulkan_50_ps.vcs", zframesToPrint: 30, gpuSourcesToPrint: 30);
        }

        public static void SaveAllServerFiles(ARCHIVE archive, string filename, int zframesToPrint, int gpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveAllServerFiles(zframesToPrint, gpuSourcesToPrint, saveGpuByteDetail);
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
            // SaveGpuSources(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs", zframeId: 0, gpuSourcesToPrint: 2);

            // SaveGpuSources(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs", zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: true);
            // SaveGpuSources(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs", zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: false);

            SaveGpuSources(ARCHIVE.dota_game_vulkan_v65, "blur_cs_vulkan_50_cs.vcs", zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: true);
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

        public static void SaveGpuSources(ARCHIVE archive, string filename, int zframeId, int gpuSourcesToPrint, bool saveGpuByteSource = false)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveGpuSourcesToHtml(zframeId, gpuSourcesToPrint, saveGpuByteSource);
        }
    }
}

