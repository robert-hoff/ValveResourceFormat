using MyShaderFileKristiker.MyHelperClasses.ProgEntries;
using System.Diagnostics;
using System.IO.Enumeration;
using ValveResourceFormat.CompiledShader;
using static MyShaderFileKristiker.MyHelperClasses.ARCHIVE;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class CreateServerHtmlFiles
    {
        //private static ARCHIVE archive = alyx_hlvr_vulkan_v64;
        //private static ARCHIVE archive = dota_game_vulkan_v65;
        //private static ARCHIVE archive = dota_game_pc_v65;
        //private static ARCHIVE archive = dota_core_pcgl_v64;
        // static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "crystal_pcgl_40_vs.vcs";
        // static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "crystal_pcgl_40_ps.vcs"; static int zFrameId = 4;
        static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "hero_pcgl_40_ps.vcs"; static int zFrameId = 0x018cbfb1;
        // static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "bloom_dota_pcgl_40_vs.vcs"; static int zFrameId = 0;
        // static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "multiblend_pcgl_40_vs.vcs";
        // static ARCHIVE archive = dota_game_pcgl_v64; static string filename = "spring_meteor_pcgl_40_vs.vcs"; static int zFrameId = 0;

        // -- constants applied throughout
        private const int COLLECTION_LIMIT = 1;
        private const int ZCOUNT = 1;
        private const int GPU_COUNT = 0;
        private const bool CLEAR_DIRECTORY = true;
        private const string USE_BASE_FOLDER = "GEN-output2";
        // private const string USE_BASE_FOLDER = "";
        private const int ZFRAME_ID = 3;
        private const bool PRINT_ZFRAME_BYTES = false;
        private const bool PRINT_GPU_BYTES = false;
        private const bool PRINT_INDEX = true;

        private static int[] printFilesfor = { ZFRAME, COLLECTION, ALL, LIST_DIR };

        // --
        private static int DO_OPERATION = printFilesfor[ZFRAME];
        // --

        public static void RunTrials()
        {
            switch (DO_OPERATION)
            {
                case ZFRAME:
                    FileVcsTokens vcsTokens = new FileVcsTokens(archive, filename);
                    SaveZframeSummary(
                        vcsFileTokens: vcsTokens,
                        zframeId: zFrameId >= 0 ? zFrameId : ZFRAME_ID,
                        gpuCount: GPU_COUNT,
                        useBaseFolder: USE_BASE_FOLDER,
                        printZframeByteVersion: PRINT_ZFRAME_BYTES);
                    break;

                case COLLECTION:
                    SaveVcsCollection(
                        archive,
                        filename,
                        zFramesToPrint: ZCOUNT,
                        gpuSourcesToPrint: GPU_COUNT,
                        saveGpuByteDetail: false,
                        clearDirectory: CLEAR_DIRECTORY,
                        useBaseFolder: USE_BASE_FOLDER);
                    break;

                case ALL:
                    SaveAllServerFilesForArchive(
                        archive,
                        vcsCollectionLimit: COLLECTION_LIMIT,
                        zFramesToPrint: ZCOUNT,
                        gpuSourcesToPrint: GPU_COUNT);
                    break;

                case LIST_DIR:
                    FileArchive fileArchive = new FileArchive(archive, VcsProgramType.Features, VcsShaderModelType._40);
                    foreach (FileVcsTokens f in fileArchive.GetFileVcsTokens())
                    {
                        Debug.WriteLine($"{f}");
                    }
                    break;
            }
        }

        // save ONE zframe
        public static void SaveZframeSummary(
            FileVcsTokens vcsFileTokens,
            int zframeId,
            int gpuCount = 5,
            string useBaseFolder = "",
            bool printZframeByteVersion = PRINT_ZFRAME_BYTES,
            bool printGpuByteVersion = PRINT_GPU_BYTES)
        {
            if (useBaseFolder.Length > 0)
            {
                vcsFileTokens.targetsavedir = useBaseFolder;
            }
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            ZFrameFile zFrame = vcsFileTokens.GetZframeFile(zframeId);
            // byte version
            if (printZframeByteVersion)
            {
                if (printGpuByteVersion)
                {
                    for (int i = 0; i < Math.Min(gpuCount, zFrame.GpuSourceCount); i++)
                    {
                        vcsFile.SaveGpuByteSourceToHtml(zframeId, i);
                    }
                }
                vcsFile.SaveZframeByteSummaryToHtml(zframeId, useBaseFolder: useBaseFolder);
            }
            // normal version
            for (int i = 0; i < Math.Min(gpuCount, zFrame.GpuSourceCount); i++)
            {
                vcsFile.SaveGpuSourceToHtml(zframeId, i);
            }
            vcsFile.SaveZframeSummaryToHtml(zFrame, useBaseFolder: useBaseFolder);
        }

        public static void SaveAllServerFilesForArchive(
            ARCHIVE archive,
            int vcsCollectionLimit = 5,
            int zFramesToPrint = 20,
            int gpuSourcesToPrint = 20)
        {
            FileArchive fileArchive = new(archive, VcsProgramType.Features, VcsShaderModelType._40);
            int fileCount = fileArchive.GetFileCount();
            for (int i = 0; i < Math.Min(vcsCollectionLimit, fileCount); i++)
            {
                SaveVcsCollection(
                    archive,
                    fileArchive.GetFileVcsTokens(i).filename,
                    zFramesToPrint,
                    zFramesToPrint,
                    saveGpuByteDetail: false);
            }
            if (PRINT_INDEX)
            {
                CreateHtmlIndexForArchive(archive);
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
            bool clearDirectory = CLEAR_DIRECTORY,
            string useBaseFolder = USE_BASE_FOLDER)
        {
            List<FileVcsTokens> relatedVcs = FileVcsCollection.GetRelatedVcs(archive, shaderName);

            if (useBaseFolder.Length > 0)
            {
                foreach (FileVcsTokens vcsTokens in relatedVcs)
                {
                    vcsTokens.targetsavedir = useBaseFolder;
                }
            }
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
        public static void CreateHtmlIndexForArchive(ARCHIVE archive)
        {
            FileArchive fileArchive = new(archive, VcsShaderModelType._40, useModularLookup: true);
            string filenamepath = $"Z:\\dev\\www\\vcs.codecreation.dev\\{fileArchive.archive}\\index.html";
            FileWriter fileWriter = new FileWriter(filenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader($"{fileArchive.archive}", $"{fileArchive.archive}");
            foreach (FileVcsTokens fileVcs in fileArchive.GetFileVcsTokens())
            {
                fileWriter.WriteLine($"<a href='{fileVcs.GetServerFileUrl()}'>{fileVcs}</a>");
            }
            fileWriter.CloseStreamWriter();
            fileWriter.Dispose();
        }

        public static void SaveVcsBytesAndSummary(FileVcsTokens vcsFileTokens)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(vcsFileTokens, convertLinksToHtml: true);
            vcsFile.SaveVcsByteSummaryToHtml();
            vcsFile.SaveVcsSummaryToHtml();
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

        private const int ZFRAME = 0;
        private const int COLLECTION = 1;
        private const int ALL = 2;
        private const int LIST_DIR = 3;
        private const string DEFAULT_BASE_FOLDER = "GEN-output";
    }
}

