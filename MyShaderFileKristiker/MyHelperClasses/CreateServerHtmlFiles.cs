using MyShaderFileKristiker.MyHelperClasses.ProgEntries;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class CreateServerHtmlFiles
    {
        //private static ARCHIVE archive = ARCHIVE.alyx_hlvr_vulkan_v64;
        //private static ARCHIVE archive = ARCHIVE.dota_game_vulkan_v65;
        //private static ARCHIVE archive = ARCHIVE.dota_game_pc_v65;
        //private static ARCHIVE archive = ARCHIVE.dota_core_pcgl_v64;
        private static ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
        private const int ZCOUNT = 1;
        private const int GPU_COUNT = 1;
        private const bool CLEAR_DIRECTORY = true;

        public static void RunTrials()
        {
            // SaveAllServerFilesForArchive();
            SaveVcsCollection(ARCHIVE.dota_game_pcgl_v64, "crystal_pcgl_40", zFramesToPrint: ZCOUNT, gpuSourcesToPrint: GPU_COUNT);
            // CreateServerHtmlFiles.SaveServerSets();
            // CreateServerHtmlFiles.SaveAllServerFilesFromArchive(limitFileCount: 2);
        }

        public static void SaveAllServerFilesForArchive(ARCHIVE archive, int limitFileCount = 1000)
        {
            FileArchive fileArchive = new(archive, VcsProgramType.Features, VcsShaderModelType._40);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < fileCount; i++)
            {
                SaveVcsCollection(archive, fileArchive.GetFileVcsTokens(i).filename, 1, 1, saveGpuByteDetail: false);
                if (i == limitFileCount)
                {
                    break;
                }
            }
        }

        /*
         * shaderName is in the form 'multiblend_pcgl_30'
         *
         */
        public static void SaveVcsCollection(
            ARCHIVE archive,
            string shaderName,
            int zFramesToPrint = ZCOUNT,
            int gpuSourcesToPrint = GPU_COUNT,
            bool saveGpuByteDetail = false,
            bool clearDirectory = CLEAR_DIRECTORY)
        {
            List<FileVcsTokens> relatedVcs = FileVcsCollection.GetRelatedVcs(archive, shaderName);
            if (clearDirectory)
            {
                RunSnippets.DeleteDirectoryContent(relatedVcs[0].GetServerFileDir());
            }
            foreach (FileVcsTokens vcsTokens in relatedVcs)
            {
                ParseVcsFile vcsFile = new ParseVcsFile(vcsTokens, convertLinksToHtml: true);
                vcsFile.SaveAllServerFiles(zFramesToPrint, gpuSourcesToPrint, saveGpuByteDetail);
            }
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

        public static void SingleFileExamples2()
        {
            // FileVcsTokens vcsTokens = new(ARCHIVE.the_lab_pc_v62, "bilateral_blur_pc_30_features.vcs");
            FileVcsTokens vcsTokens = new(ARCHIVE.dota_game_pcgl_v64, "multiblend_pc_30_vs.vcs");

            ParseVcsFile vcsFile = new ParseVcsFile(vcsTokens);
            // vcsFile.SaveVcsSummaryToHtml();
            vcsFile.SaveVcsByteSummaryToHtml();
        }

        public static void SingleFileExamples1()
        {
            // FileVcsTokens vcsTokens = new(ARCHIVE.dota_game_pcgl_v64, "multiblend_pc_30_vs.vcs");
            FileVcsTokens vcsTokens = new(ARCHIVE.dota_game_vulkan_v65, "blur_cs_vulkan_50_cs.vcs");

            SaveVcsBytesAndSummary(vcsTokens);
            //SaveZframeSummary(vcsTokens, zframeId: 0);
            //SaveZframeSummaries(vcsTokens, zframesToPrint: 20);
            //SaveGpuSource(vcsTokens, zframeId: 0, gpuSourceId: 0);
            //SaveGpuSources(vcsTokens, zframeId: 0, gpuSourcesToPrint: 2);
            //SaveGpuSources(vcsTokens, zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: true);
            //SaveGpuSources(vcsTokens, zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: false);
            //SaveGpuSources(vcsTokens, zframeId: 0, gpuSourcesToPrint: 1, saveGpuByteSource: true);
        }


        public static void SaveVcsBytesAndSummary(FileVcsTokens vcsFileTokens)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveVcsByteSummaryToHtml();
            vcsFile.SaveVcsSummaryToHtml();
        }

        // save ONE zframe
        public static void SaveZframeSummary(FileVcsTokens vcsFileTokens, int zframeId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveZframeByteSummaryToHtml(zframeId);
            vcsFile.SaveZframeSummaryToHtml(zframeId);
        }

        // save more than one
        public static void SaveZframeSummaries(FileVcsTokens vcsFileTokens, int zframesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveZframeSummaries(zframesToPrint);
        }

        // save ONE gpu source
        public static void SaveGpuSource(FileVcsTokens vcsFileTokens, int zframeId, int gpuSourceId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveGpuSourceToHtml(zframeId, gpuSourceId);
        }

        // save more than one
        public static void SaveGpuSources(FileVcsTokens vcsFileTokens, int zframeId, int gpuSourcesToPrint, bool saveGpuByteSource = false)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveGpuSourcesToHtml(zframeId, gpuSourcesToPrint, saveGpuByteSource);
        }
    }
}

