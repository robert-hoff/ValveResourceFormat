using System;
using System.IO;
using System.Collections.Generic;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystemOld;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using MyShaderAnalysis.vcsanalysis;


namespace MyShaderAnalysis.codestash
{
    public class ParseVcsFilesOld
    {

        public static void RunTrials()
        {
            // -- these methods uses largely redundant code, direct parsing to the production code
            RunBatchPrintVcsFiles();
            // RunPrintVcsFile();
        }



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
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader(fileTokens.GetShortName(), fileTokens.GetBaseName());
            new PrintoutSingleFile(filenamepath, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


    }
}
