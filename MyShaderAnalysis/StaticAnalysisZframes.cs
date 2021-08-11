using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis {


    public class StaticAnalysisZframes {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";
        const string SERVER_BASEDIR = @"Z:\dev\www\vcs.codecreation.dev";
        const string OUTPUT_SUB_DIR = @"\GEN-output";


        public static void RunTrials() {
            Trial1();




            // SurveryHeaderParams();
            // SurveryBytesInLeadingDataload();
            //SurveryLeadingDataSingleFile();



            // Debug.WriteLine($"{123:x}");

            PrintReport(showCount: false);
            CloseStreamWriter();
        }






        static void Trial1() {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}\refract_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_CORE}\visualize_cloth_pcgl_40_ps.vcs";

            ShaderFile shaderFile = new(filenamepath);
            // ZFrameFile zframeFile = shaderFile.GetZFrameFile(0x24);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(0xa);
            // ZFrameFile zframeFile = shaderFile.GetZFrameFile(0x3b);

            // ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(5);

            // Debug.WriteLine($"{zframeFile.leadingData.h0}");
            // Debug.WriteLine($"{zframeFile.leadingData.h1}");
            // Debug.WriteLine($"{zframeFile.leadingData.h2}");
            // Debug.WriteLine($"{DataReader.BytesToString(zframeFile.leadingData.dataload)}");

            // zframeFile.ShowZFrameHeader();
            // zframeFile.ShowLeadSummary();
            // zframeFile.ShowDatablocks();
            // zframeFile.ShowTailSummary();
            // zframeFile.ShowGlslSources();

            zframeFile.ShowEndBlocks();

        }






        static void SurveryHeaderParams() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {

                    // zframeFile.ShowZFrameHeader();
                    foreach (var zparam in zframeFile.zframeParams) {
                        CollectStringValue(zparam.ToString());
                    }
                }
            }

        }



        static void SurveryBytesInLeadingDataload() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {
                    for (int i = 3; i < zframeFile.leadingData.dataload.Length; i += 4) {
                        if (zframeFile.leadingData.dataload[i] == 0) {
                            CollectStringValue($"{zframeFile.leadingData.dataload[i - 1]:X02} {zframeFile.leadingData.dataload[i]:X02}");
                            if (zframeFile.leadingData.dataload[i - 1] == 0x35 && zframeFile.leadingData.dataload[i] == 0) {
                                Debug.WriteLine($"{zframeFile.filenamepath}");
                                Debug.WriteLine($"{zframeFile.zframeId}");
                                goto breakhere;
                            }
                        }
                    }
                }
            }
            breakhere: Debug.WriteLine("");
        }



        /*
         * A big problem with surverying zframes is that it creates insane workloads
         * decompressing and parsing hero_pcgl_30_ps.vcs taking 20 GB of data.
         * I've setup a selection that chooses files with 4000 zframes or less only, which is pretty manageable
         *
         *
         */
        static void SurveryLeadingDataSingleFile() {

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {
                    CollectStringValue($"{zframeFile.leadingData.h0,2} {zframeFile.leadingData.h1,2} {zframeFile.leadingData.h2,2}");
                }
            }
        }





        static List<string> GetFileSelectionWithLimitedZframes() {
            List<string> vcsFiles = new();
            List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, 30);
            foreach (string checkVcsFile in selectedFiles) {
                ShaderFile shaderFile = new ShaderFile(checkVcsFile);
                if (shaderFile.GetZFrameCount() < 4000 && shaderFile.GetZFrameCount() > 0) {
                    vcsFiles.Add(checkVcsFile);
                }
            }

            return vcsFiles;
        }

        // needs work! currently returning null. perhaps add interesting test cases as I go along
        static List<string> GetManualFileSelection() {
            // don't do this one yet!
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\hero_pcgl_30_ps.vcs";
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\grasstile_pcgl_41_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\grasstile_pcgl_41_vs.vcs");
            return null;
        }







        private static StreamWriter sw = null;
        private static bool DisableOutput = false;
        private static bool WriteAsHtml = false;
        private static bool swWriterAlreadyClosed = false;


        // This basestream != null is nonsense, it doesn't check if the file is open
        private static void CloseStreamWriter() {
            if (WriteAsHtml && !swWriterAlreadyClosed) {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed) {
                sw.Close();
            }
        }

        public static void OutputWrite(string text) {
            if (!DisableOutput) {
                Debug.Write(text);
            }
            if (sw != null) {
                sw.Write(text);
            }
        }
        public static void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }



}








