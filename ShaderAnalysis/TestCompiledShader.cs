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

        static string TEST_SHADERS_DIR = $"X:/checkouts/ValveResourceFormat/Tests/Files/Shaders";
        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx"; // i.e. Dota2 specific
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"";
        const string ALYX_VULKAN_CORE = @"X:\hl2alyx-export\alyx-vulkan-core";



        public static void RunTrials()
        {
            // Trial2();
            // Trial3();
            // Trial4();
            Trial5();
            // Trial6();
            // Trial7();
            // Trial8();
        }


        static void Trial6()
        {
            // string filenamepath = $"{PCGL_DIR_NOT_CORE}/grasstile_pcgl_30_ps.vcs";
            string filenamepath = $"{PCGL_DIR_NOT_CORE}/grasstile_preview_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
        }




        static void Trial5()
        {
            string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_vs.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0, omitParsing: true);
            zframeFile.datareader.WriteToDebug = true;
            zframeFile.datareader.WriteToConsole = false;

            zframeFile.PrintByteAnalysis();
        }


        static void Trial4()
        {
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_ps.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_vs.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_ps.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_pcgl_40_vs.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_features.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_ps.vcs";
            // string filenamepath = $"{TEST_SHADERS_DIR}/error_vulkan_40_vs.vcs";
            // string filenamepath = $"{ALYX_VULKAN_CORE}/brushsplat_vulkan_40_ps.vcs";
            string filenamepath = $"{PC_DIR_CORE}/debugoverlay_wireframe_pc_40_ps.vcs";
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            shaderFile.PrintByteAnalysis(shortenOutput: false);
        }

        static void Trial3()
        {
            string filenamepath = $"{TEST_SHADERS_DIR}/error_pc_40_features.vcs";
            new DataReaderVcsTesting(filenamepath);
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

