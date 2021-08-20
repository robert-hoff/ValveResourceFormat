using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using ValveResourceFormat.ShaderParser;
using ShaderAnalysis.utilhelpers;
using static ShaderAnalysis.utilhelpers.FileHelpers;
// using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;

namespace ShaderAnalysis
{
    public class TestCompiledShader
    {


        public static void RunTrials()
        {
            Trial2();
        }


        static void Trial2()
        {
            // string filenamepath = $"{FileSystem.DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_DCG_PC_SOURCE}/bloom_dota_pc_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/generic_pc_30_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/generic_pc_30_vs.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/occluder_vis_pc_40_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/refract_pc_30_ps.vcs";
            // string filenamepath = $"{FileSystem.ARTIFACT_CLASSIC_CORE_PC_SOURCE}/refract_pc_30_features.vcs";
            string filenamepath = $"{FileSystem.DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_40_ps.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();

            // Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
            shaderFile.PrintByteAnalysis(shortenOutput: false);

            // Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
            shaderFile.GetZFrameFile(0).PrintByteAnalysis();

        }



        static void Trial1()
        {
            List<string> vcsFiles = GetVcsFiles(FileSystem.DOTA_CORE_PCGL_SOURCE, FileSystem.DOTA_GAME_PCGL_SOURCE, VcsFileType.Features, -1);

            foreach (var item in vcsFiles)
            {
                // ShaderFile shaderFile = new(item);
            }


        }




    }




}

