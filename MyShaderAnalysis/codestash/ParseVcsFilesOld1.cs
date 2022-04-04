using System;
using System.IO;
using System.Collections.Generic;

using VcsProgramType = ValveResourceFormat.CompiledShader.VcsProgramType;

using static MyShaderAnalysis.utilhelpers.FileSystemOld;
using static MyShaderAnalysis.utilhelpers.MyTrashUtilHelpers;


namespace MyShaderAnalysis.utilhelpers
{
    public class ParseVcsFilesOld1
    {

        public static void RunTrials()
        {
            // -- these methods uses largely redundant code, direct parsing to the production code
            RunBatchPrintVcsFiles();
            // RunPrintVcsFile();
        }



        static void RunBatchPrintVcsFiles()
        {
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 5);
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
            string outputFile = "Z:/dev/www/vcs.codecreation.dev/GEN-output/testfile.html";
            Console.WriteLine($"writing {filenamepath}");
            StreamWriter sw = new StreamWriter(outputFile); // DON'T MIX UP INPUT AND OUTPUT - will overwrite files that I can't afford to lose
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string htmlTitle = fileTokens.GetShortName();
            string htmlHeader = fileTokens.GetBaseName();
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
            new PrintoutSingleFile(filenamepath, sw.Write);
            sw.WriteLine(GetHtmlFooter());
            sw.Close();
        }



        private static string GetHtmlHeader(string browserTitle, string pageHeader)
        {
            string html_header = "" +
                $"<!DOCTYPE html>\n<html>\n<head>\n  <title>{browserTitle}</title>\n" +
                $"  <link href='/includes/styles.css' rel='stylesheet' type='text/css' />\n" +
                $"</head>\n<body>\n<b>{pageHeader}</b>\n<pre>";
            return html_header;
        }
        private static string GetHtmlFooter()
        {
            return "</pre>\n</html>";
        }

    }
}
