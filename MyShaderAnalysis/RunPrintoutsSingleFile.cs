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
 *
 *
 *
 *
 */
namespace MyShaderAnalysis
{
    public class RunPrintoutsSingleFile
    {
        const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";


        public static void RunTrials()
        {
            // updated March 2022

            TestBatchPrintVcsFiles();
            // TestPrintVcsFile();
            // CreateFileDirs();
            // PrintAndSaveSingleFilePostProcessResult();

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


        static void TestBatchPrintVcsFiles()
        {
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            BatchPrintVcsFiles(vcsFiles);
        }

        static void BatchPrintVcsFiles(List<string> vcsFiles) {
            foreach (var filenamepath in vcsFiles)
            {
                PrintVcsFile(filenamepath);
            }
        }


        static void TestPrintVcsFile()
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
            fileWriter.WriteHtmlHeader(GetShortName(filenamepath), RemoveBaseDir(filenamepath));
            new PrintoutSingleFile(filenamepath, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }




    }
}


