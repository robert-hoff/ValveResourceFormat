using System.IO;
using System.Collections.Generic;
using System.Globalization;

using PostProcessVcsFile = MyShaderAnalysis.vcsanalysis.PostProcessVcsFile;
using PostProcessZframeFile = MyShaderAnalysis.vcsanalysis.PostProcessZframeFile;

using ShaderFile = ValveResourceFormat.CompiledShader.ShaderFile;
using ZFrameFile = ValveResourceFormat.CompiledShader.ZFrameFile;
using PrintVcsFileSummary = ValveResourceFormat.CompiledShader.PrintVcsFileSummary;
using PrintZFrameSummary = ValveResourceFormat.CompiledShader.PrintZFrameSummary;

using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;


/*
 *
 * See TestUtilFunctions.cs for snippets relating to FileSysteam and FileTokens
 *
 * Write this file as the main entry point for batch parsing and printing.
 * A type of printing that makes sense is to declare a target vcs file and then indicate what associated
 * printouts to target. For example, whether to parse associated formatted summary, zframes, gpu-sources
 * and byte versions.
 *
 *
 *
 * NOTE - I wrote these methods after the methods ParseVcsFilesOld1.cs
 * they are relativately new and use the post-processing system for generating html links.
 * This was refactored and improved into the ParseVcsFile class (Mar 2022).
 *
 * There shouldn't be any functionality expressed in these methods that is not expressed in the ParseVcsFile class
 *
 *
 */
namespace MyShaderAnalysis.codestash
{
    public class ParseVcsFilesOld2
    {
        // const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        // const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";

        public static void RunTrials()
        {

            // RunPrintZFrameSummaryPostProcess();
            RunPrintVcsSummaryPostProcess();

            // RunPrintVcsByteDetail();
            // RunPrintVcsSummary();
            // RunPrintZFrameSummary();
            // RunPrintZFrameBytes();
            // RunPrintGpuSource();

        }



        static void RunPrintZFrameSummaryPostProcess()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_vs.vcs";

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, filenamepath);

            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintZFrameSummaryPostProcess(shaderFile, zframe0, fileTokens);
            // ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            // PrintZFrameSummaryPostProcess(shaderFile, zframe1, fileTokens);
        }



        static void PrintZFrameSummaryPostProcess(ShaderFile shaderFile, ZFrameFile zframeFile, FileVcsTokens fileTokens = null)
        {
            //if (fileTokens == null)
            //{
            //    fileTokens = new FileTokensOld(zframeFile.filenamepath);
            //}


            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintZFrameSummary(shaderFile, zframeFile, outputWriter: buffer.Write, showRichTextBoxLinks: true);

            string processedData = new PostProcessZframeFile(zframeFile, fileTokens).PostProcessZframeData(buffer.ToString());


            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "summary", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            // This emulates the zframe name in the VRF viewer - omitting {fileTokens.namelabel} (e.g. '3dskyboxstencil')
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId)[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            fileWriter.GetOutputWriter()(processedData);
            // -- (for testing) write the buffer without any post processing
            // fileWriter.GetOutputWriter()(buffer.ToString());
            fileWriter.CloseStreamWriter();
        }




        static void RunPrintVcsSummaryPostProcess()
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_vs.vcs";
            PrintVcsSummaryPostProcess(filenamepath);
        }


        static void PrintVcsSummaryPostProcess(string filenamepath)
        {
            FileVcsTokens fileTokens = new FileVcsTokens(ARCHIVE.dota_game_pcgl_v64, filenamepath);
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            // List<string> relatedFiles = GetRelatedFiles2(fileTokens.filenamepath);
            List<string> relatedFiles = GetRelatedFilesOld(ARCHIVE.dota_game_pcgl_v64, fileTokens.filenamepath);
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintVcsFileSummary(shaderFile, buffer.Write, showRichTextBoxLinks: true, relatedFiles);

            string processedData = new PostProcessVcsFile(fileTokens).PostProcessVcsData(buffer.ToString());

            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader(fileTokens.GetShortName(), fileTokens.GetBaseName());
            fileWriter.GetOutputWriter()(processedData);
            // -- write the buffer without any post processing
            // fileWriter.GetOutputWriter()(buffer.ToString());
            fileWriter.CloseStreamWriter();
        }

        /*
         * copy of method in FileVcsCollection
         *
         */
        private static List<string> GetRelatedFilesOld(ARCHIVE archive, string vcsCollectionName)
        {
            if (vcsCollectionName.EndsWith(".vcs"))
            {
                vcsCollectionName = Path.GetFileName(vcsCollectionName);
                vcsCollectionName = vcsCollectionName.Substring(0, vcsCollectionName.LastIndexOf('_'));
            }
            List<string> relatedFiles = new();
            string featuresFile = null;
            foreach (var vcsFile in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (Path.GetFileName(vcsFile).StartsWith(vcsCollectionName))
                {
                    if (vcsFile.EndsWith("features.vcs"))
                    {
                        featuresFile = Path.GetFileName(vcsFile);
                    } else if (vcsFile.EndsWith("vs.vcs"))
                    {
                        relatedFiles.Insert(0, Path.GetFileName(vcsFile));
                    } else
                    {
                        relatedFiles.Add(Path.GetFileName(vcsFile));
                    }
                }
            }
            if (featuresFile != null)
            {
                relatedFiles.Insert(0, featuresFile);
            }
            return relatedFiles;
        }






        // no post-processing performed here (therefore links won't be enabled)

        static void RunPrintVcsSummary()
        {
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            PrintVcsSummary(filenamepath);
        }

        static void RunPrintVcsByteDetail()
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_features.vcs";
            PrintVcsByteDetail(filenamepath);
        }


        static void RunPrintZFrameSummary()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/3dskyboxstencil_pcgl_30_vs.vcs";

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);

            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintZFrameSummary(shaderFile, zframe0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintZFrameSummary(shaderFile, zframe1, fileTokens);
        }



        static void RunPrintZFrameBytes()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);

            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintZFrameBytes(zframe0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintZFrameBytes(zframe1, fileTokens);
        }


        static void PrintVcsSummary(string filenamepath)
        {
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader(fileTokens.GetShortName(), fileTokens.GetBaseName());
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            List<string> relatedFiles = GetRelatedFiles2(fileTokens.filenamepath);
            new PrintVcsFileSummary(shaderFile, fileWriter.GetOutputWriter(), showRichTextBoxLinks: true, relatedFiles);
            fileWriter.CloseStreamWriter();
        }


        static void PrintVcsByteDetail(string filenamepath)
        {
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("bytes", createDirs: true)}";

            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            string htmlTitle = fileTokens.GetShortName();
            string htmlHeader = $"{fileTokens.GetBaseName()} (byte detail)";
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);

            shaderFile.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void PrintZFrameSummary(ShaderFile shaderFile, ZFrameFile zframeFile, FileTokensOld fileTokens = null)
        {
            if (fileTokens == null)
            {
                fileTokens = new FileTokensOld(zframeFile.filenamepath);
            }
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "summary", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            // string htmlTitle = $"{fileTokens.namelabel}-{fileTokens.vcstoken}[{zframeFile.zframeId:x}]";
            // This emulates the name in the VRF viewer - omitting {fileTokens.namelabel}
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId)[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            // zframe.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            new PrintZFrameSummary(shaderFile, zframeFile, outputWriter: fileWriter.GetOutputWriter(), showRichTextBoxLinks: true);
            fileWriter.CloseStreamWriter();
        }


        static void PrintZFrameBytes(ZFrameFile zframeFile, FileTokensOld fileTokens = null)
        {
            if (fileTokens == null)
            {
                fileTokens = new FileTokensOld(zframeFile.filenamepath);
            }
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "bytes", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes")[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            zframeFile.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void RunPrintGpuSource()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            string filenamepath = $"{FileSystemOld.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintGpuSource(zframe0, 0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintGpuSource(zframe1, 1, fileTokens);
        }

        static void PrintGpuSource(ZFrameFile zframeFile, int sourceId, FileTokensOld fileTokens = null)
        {
            var glslSource = zframeFile.gpuSources[sourceId];
            string glslServerDir = fileTokens.GetGlslServerDir(createDirs: true);
            string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename(glslSource.GetEditorRefIdAsString())}";

            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            string htmlTitleAndHeader = Path.GetFileName(outputFilenamepath)[0..^5];
            fileWriter.WriteHtmlHeader(htmlTitleAndHeader, htmlTitleAndHeader);
            zframeFile.PrintGpuSource(sourceId, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }






    }
}


