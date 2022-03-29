using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using MyShaderAnalysis.vcsanalysis;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using System.Diagnostics;
using System.Globalization;

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
            // BackgroundExamples();
        }


        public static void SaveServerSets()
        {
            // SaveAllServerFiles(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
            SaveAllServerFiles(ARCHIVE.dotagame_pcgl, "3dskyboxstencil_pcgl_30_vs.vcs", zframesToPrint: 5, gpuSourcesToPrint: 5);
        }



        public static void BackgroundExamples()
        {
            SaveAllServerFiles(requestedZframesToPrint: 5, requestedGpuSourcesToPrint: 5);
            // SaveVcsBytesAndSummary();
            // SaveZframeSummary(0);
            // SaveZframeSummaries(20);
            // SaveGpuSource(zframeId: 0, gpuSourceId: 0);
            // SaveGpuSources(zframeId: 0, requestedGpuSourceToPrint: 20);
        }


        public static void SaveAllServerFiles(ARCHIVE archive, string filename, int zframesToPrint, int gpuSourcesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(archive, filename, convertLinksToHtml: true);
            vcsFile.SaveAllServerFiles(zframesToPrint, gpuSourcesToPrint);
        }



        public static void SaveAllServerFiles(int requestedZframesToPrint, int requestedGpuSourcesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            vcsFile.SaveAllServerFiles(requestedZframesToPrint, requestedGpuSourcesToPrint);
        }


        public static void SaveVcsBytesAndSummary()
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            vcsFile.SaveVcsByteSummaryToHtml();
            vcsFile.SaveVcsSummaryToHtml();
        }


        public static void SaveZframeSummary(int zframeId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            vcsFile.SaveZframeByteSummaryToHtml(zframeId);
            vcsFile.SaveZframeSummaryToHtml(zframeId);
        }


        public static void SaveZframeSummaries(int requestedZframesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            // vcsFile.SaveZframeByteSummaries(requestedZframesToPrint);
            vcsFile.SaveZframeSummaries(requestedZframesToPrint);
        }


        public static void SaveGpuSource(int zframeId, int gpuSourceId)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            vcsFile.SaveGpuSourceToHtml(zframeId, gpuSourceId);
        }


        public static void SaveGpuSources(int zframeId, int requestedGpuSourcesToPrint)
        {
            ParseVcsFile vcsFile = new ParseVcsFile(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", convertLinksToHtml: true);
            vcsFile.SaveGpuSources(zframeId, requestedGpuSourcesToPrint);
        }






    }
}




