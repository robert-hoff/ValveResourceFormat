using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using MyShaderAnalysis.vcsanalysis;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using System.Diagnostics;
using System.Globalization;

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
 *
 */
namespace MyShaderAnalysis
{
    public class ParseVcsFiles
    {
        // const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        // const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";

        public static void RunTrials()
        {
            // PrintAndSaveSingleFilePostProcessResult();

            // RunPrintVcsByteDetail();
            // RunPrintVcsSummary();
            // RunPrintZFrameSummary();
            // RunPrintZFrameBytes();
            RunPrintGpuSource();

            // RunBatchPrintVcsFiles();
            // RunPrintVcsFile();


        }


        // March 2022 - new method to allow using the PrintVcsFileSummary class
        static void PrintAndSaveSingleFilePostProcessResult()
        {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_vs.vcs";
            ShaderFile shaderFile = shaderFile = InstantiateShaderFile(filenamepath);
            FileTokens fileTokens = new FileTokens(filenamepath);

            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            shaderFile.PrintSummary(buffer.Write, showRichTextBoxLinks: true);


            // Console.WriteLine(buffer.ToString());

            new PostProcessVcsFile(fileTokens, buffer.ToString());
            // new PostProccessVcsFile(buffer);

        }


        static void RunPrintVcsSummary()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            PrintVcsSummary(filenamepath);
        }

        static void RunPrintVcsByteDetail()
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            PrintVcsByteDetail(filenamepath);
        }


        static void RunPrintZFrameSummary()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokens fileTokens = new FileTokens(filenamepath);

            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintZFrameSummary(shaderFile, zframe0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintZFrameSummary(shaderFile, zframe1, fileTokens);
        }

        static void RunPrintZFrameBytes()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokens fileTokens = new FileTokens(filenamepath);

            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintZFrameBytes(zframe0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintZFrameBytes(zframe1, fileTokens);
        }


        static void PrintVcsSummary(string filenamepath)
        {
            FileTokens fileTokens = new FileTokens(filenamepath);
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
            FileTokens fileTokens = new FileTokens(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("bytes", createDirs: true)}";

            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            string htmlTitle = fileTokens.GetShortName();
            string htmlHeader = $"{fileTokens.GetBaseName()} (byte detail)";
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);

            shaderFile.PrintByteAnalysis(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void PrintZFrameSummary(ShaderFile shaderFile, ZFrameFile zframeFile, FileTokens fileTokens = null)
        {
            if (fileTokens == null)
            {
                fileTokens = new FileTokens(zframeFile.filenamepath);
            }
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "summary", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId)[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            // zframe.PrintByteAnalysis(outputWriter: fileWriter.GetOutputWriter());
            new PrintZFrameSummary(shaderFile, zframeFile, outputWriter: fileWriter.GetOutputWriter(), showRichTextBoxLinks: true);
            fileWriter.CloseStreamWriter();
        }


        static void PrintZFrameBytes(ZFrameFile zframeFile, FileTokens fileTokens = null)
        {
            if (fileTokens == null)
            {
                fileTokens = new FileTokens(zframeFile.filenamepath);
            }
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "bytes", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes")[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            zframeFile.PrintByteAnalysis(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void RunPrintGpuSource()
        {
            int ZFRAME_INDEX0 = 0;
            int ZFRAME_INDEX1 = 1;
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokens fileTokens = new FileTokens(filenamepath);
            ZFrameFile zframe0 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX0);
            PrintGpuSource(zframe0, 0, fileTokens);
            ZFrameFile zframe1 = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX1);
            PrintGpuSource(zframe1, 1, fileTokens);
        }

        static void PrintGpuSource(ZFrameFile zframeFile, int sourceId, FileTokens fileTokens = null)
        {
            var glslSource = zframeFile.gpuSources[sourceId];
            string glslServerDir = fileTokens.GetGlslServerDir(createDirs: true);
            string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename((GlslSource)glslSource)}";

            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            string htmlTitleAndHeader = Path.GetFileName(outputFilenamepath)[0..^5];
            fileWriter.WriteHtmlHeader(htmlTitleAndHeader, htmlTitleAndHeader);
            zframeFile.PrintGpuSource(sourceId, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }





        // -- replace these methods

        static void RunBatchPrintVcsFiles()
        {
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            BatchPrintVcsFiles(vcsFiles);
        }

        static void BatchPrintVcsFiles(List<string> vcsFiles)
        {
            foreach (var filenamepath in vcsFiles)
            {
                PrintVcsFile(filenamepath);
            }
        }
        static void RunPrintVcsFile()
        {
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            PrintVcsFile(filenamepath);


        }
        static void PrintVcsFile(string filenamepath)
        {
            FileTokens fileTokens = new FileTokens(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader(fileTokens.GetShortName(), fileTokens.GetBaseName());
            new PrintoutSingleFile(filenamepath, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }




    }
}


