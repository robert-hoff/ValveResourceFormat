using MyShaderAnalysis.readers;
using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static MyShaderAnalysis.readers.ShaderFile;
using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis {


    class ShaderAnalysis {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string PC_DIR_PC_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_PC_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";


        public static void RunTrials() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\crystal_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\depth_only_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\grasstile_preview_pcgl_41_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\solidcolor_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\physics_wireframe_pcgl_30_ps.vcs"; // frame 16
            // string filenamepath = PCGL_DIR_CORE + @"\tools_solid_pcgl_30_ps.vcs"; // frame 7
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs"; // frame 5 has an empty glsl file reference
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_nav_pcgl_40_ps.vcs"; // frame 10
            // string filenamepath = PCGL_DIR_CORE + @"\tools_sprite_pcgl_40_gs.vcs"; // gs file
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_psrs.vcs"; // psrs file
            // string filenamepath = PCGL_DIR_CORE + @"\deferred_shading_pcgl_41_ps.vcs"; // interesting zframe content
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\grasstile_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\3dskyboxstencil_pcgl_30_features.vcs";
            string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs"; // this file has some very short zframes


            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";


            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_ps.vcs";


            // PrintByteAnalysis(filenamepath);
            // WriteByteAnalysisToFile(filenamepath);



            // List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.features_file, 30);
            // List<string> files = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.features_file, 30);
            // List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.psrs_file, -1);
            // List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.gs_file, -1);

        }


        static void PrintByteAnalysis(string filenamepath) {
            ShaderFile shaderFile = new(filenamepath);
            shaderFile.PrintByteAnalysis();
        }

        static void WriteByteAnalysisToFile(string filenamepath) {
            ShaderFile shaderFile = new(filenamepath);
            shaderFile.WriteByteAnalysisToFile(OUTPUT_DIR);
        }



        /*
         * Some files have shared names in the not-core and core directory, so some files
         * will be overwritten by entries in PCGL_DIR_NOT_CORE
         * the number of found files is 662 but the end result will be 631
         * These files exist in both folders
         *
         * blur_pcgl_30_features.vcs                blur_pcgl_30_ps.vcs             blur_pcgl_30_vs.vcs
         * blur_pcgl_40_features.vcs                blur_pcgl_40_ps.vcs             blur_pcgl_40_vs.vcs
         * cables_pcgl_30_features.vcs              cables_pcgl_30_ps.vcs           cables_pcgl_30_vs.vcs
         * cables_pcgl_40_features.vcs              cables_pcgl_40_ps.vcs           cables_pcgl_40_vs.vcs
         * spritecard_pcgl_30_features.vcs          spritecard_pcgl_30_ps.vcs       spritecard_pcgl_30_vs.vcs
         * ssao_pcgl_30_features.vcs                ssao_pcgl_30_ps.vcs             ssao_pcgl_30_vs.vcs
         * ssao_pcgl_40_features.vcs                ssao_pcgl_40_ps.vcs             ssao_pcgl_40_vs.vcs
         * grasstile_pcgl_41_features.vcs           grasstile_pcgl_41_ps.vcs        grasstile_pcgl_41_vs.vcs
         * grasstile_preview_pcgl_41_features.vcs
         * grasstile_preview_pcgl_41_ps.vcs
         * grasstile_preview_pcgl_41_vs.vcs
         * visualize_physics_pcgl_40_features.vcs
         * visualize_physics_pcgl_40_gs.vcs
         * visualize_physics_pcgl_40_ps.vcs
         * visualize_physics_pcgl_40_vs.vcs
         *
         *
         */
        static void ParseAndWriteAllVcsFiles() {
            List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            Debug.WriteLine($"found {files.Count} files");
            foreach (string filenamepath in files) {
                WriteByteAnalysisToFile(filenamepath);
            }
        }



        static List<string> GetVcsFiles(string dir1, FILETYPE fileType) {
            return GetVcsFiles(dir1, null, fileType, -1);
        }

        static List<string> GetVcsFiles(string dir1, string dir2, FILETYPE fileType, int numEnding) {
            List<string> filesFound = new();
            if (fileType == FILETYPE.features_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_features.vcs" : "features.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.vs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_vs.vcs" : "vs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.ps_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_ps.vcs" : "ps.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.gs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_gs.vcs" : "gs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.psrs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_psrs.vcs" : "psrs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            return filesFound;
        }


        static List<string> GetAllFilesWithEnding(string dir, string endsWith) {
            List<string> filesFound = new();
            if (dir == null) {
                return filesFound;
            }
            foreach (string filenamepath in Directory.GetFiles(dir)) {
                if (filenamepath.EndsWith(endsWith)) {
                    filesFound.Add(filenamepath);
                }
            }
            return filesFound;
        }





        static void ShowDuplicateFiles() {
            List<string> files = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            List<string> filenamesOnly = new();
            foreach (string s in files) {
                AddCollectValueString(Path.GetFileName(s));
            }
            List<string> stringvalues = new();
            foreach (var s in collectValuesString) {
                if (s.Value == 1) {
                    continue;
                }
                stringvalues.Add(s.Key);
            }
            stringvalues.Sort();
            foreach (string s in stringvalues) {
                Debug.WriteLine(s);
            }
        }


        static Dictionary<string, int> collectValuesString = new();
        static void AddCollectValueString(string val) {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;
        }







    }


}








