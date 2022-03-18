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

namespace MyShaderAnalysis
{
    public class RunPrintoutsSingleFile
    {
        const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";


        // todo - need some kind of sw writer here
        // static OutputWriter output = new();




        // todo - my old sw setup
        //if (outputFilenamepath != null && writeFile)
        //{
        //    output.SetOutputFile(outputFilenamepath);
        //    output.WriteAsHtml(fileTokens.GetAbbreviatedName(), $"{ShortHandName(filenamepath)}");
        //    if (disableOutput)
        //    {
        //        output.DisableOutput();
        //    }
        //}


        public static void RunTrials()
        {

            // PrintAllFiles();
            // TestBatchPrinting(); // (works for single file)
            PrintAndSaveSingleFile();
            // PrintSingleFileToConsole();


            // todo - need some kind of sw writer here
            // output.CloseStreamWriter();
        }


        static void PrintAllFiles()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsProgramType.Undetermined, -1);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
                string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";

                // PrintSingleFileFullSummary(filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
                // output.CloseStreamWriter();
            }

        }


        static void TestBatchPrinting()
        {

            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, null, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, null, VcsFileType.Any, -1);

            List<string> vcsFiles = new();
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/water_dota_pcgl_30_ps.vcs");
            // vcsFiles.Add($"{DOTA_GAME_PCGL_SOURCE}/visualize_physics_pcgl_40_gs.vcs");
            vcsFiles.Add($"{DOTA_CORE_PCGL_SOURCE}/visualize_physics_pcgl_40_gs.vcs");
            // vcsFiles.Add($"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/depth_only_pc_40_vs.vcs");


            FileTokens fileTokens = new FileTokens(vcsFiles[0]);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";

            // PrintSingleFileFullSummary(fileTokens.filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
        }



        static void PrintAndSaveSingleFile()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_psrs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_ps.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/msaa_resolve_cs_pcgl_50_features.vcs"; // strange file that doesn't contain any data


            FileWriter fileWriter = new FileWriter($"{SERVER_OUTPUT_DIR}/testfile.html");
            fileWriter.WriteHtmlHeader(GetShortName(filenamepath), RemoveBaseDir(filenamepath));
            new PrintoutSingleFile(filenamepath, fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void PrintSingleFileToConsole()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            // new PrintoutSingleFile(filenamepath, $"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            new PrintoutSingleFile(filenamepath);
        }
    }
}
