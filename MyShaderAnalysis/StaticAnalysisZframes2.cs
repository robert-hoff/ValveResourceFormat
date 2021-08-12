using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis {


    public class StaticAnalysisZframes2 {

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


            PrintReport();
            CloseStreamWriter();
        }






        static void Trial1() {

            // string filenamepath = $@"{PCGL_DIR_CORE}\bilateral_blur_pcgl_30_vs.vcs"; int useZFrame = 0;
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs"; int useZFrame = 0xab;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs"; int useZFrame = 0xc9;
            // string filenamepath = $@"{PCGL_DIR_CORE}\depth_only_pcgl_30_vs.vcs"; int useZFrame = 0x68;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\refract_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_CORE}\visualize_cloth_pcgl_40_ps.vcs";



            ZFileSummary(filenamepath, useZFrame, $@"{SERVER_OUTPUT_DIR}\testrun2.html", writeFile: true);


        }



        static void ZFileSummary(string vcsFile, long zframeId, string outputFilenamepath = null, bool writeFile = false) {
            ShaderFile shaderFile = new(vcsFile);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zframeId);
            if (outputFilenamepath != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath);
                WriteHtmlFile($"Z 0x{zframeId:x}", GetZframeHtmlFilename((uint)zframeId, vcsFile)[0..^5]);
            }


            PrintConfigurationState(shaderFile, zframeId);



            SortedDictionary<int, int> writeSequences = GetWriteSequences(zframeFile);
            // zframeFile.leadingData.dataload
            PrintWriteSequences(shaderFile, zframeFile, writeSequences);





            //PrintZBlock(shaderFile, zframeFile.leadingData);
            //for (int i = 0; i < zframeFile.dataBlocks.Count; i++) {
            //    if (i > 0 && zframeFile.dataBlocks[i].h0 > 0 && zframeFile.dataBlocks[i - 1].h0 == 0) {
            //        OutputWriteLine("");
            //    }
            //    PrintZBlock(shaderFile, zframeFile.dataBlocks[i]);
            //}
        }




        static void PrintWriteSequences(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences) {
            int lastseq = writeSequences[-1];
            string seqName = $"WRITESEQ[{lastseq}] (default)";
            ZDataBlock leadData = zframeFile.leadingData;
            PrintParamWriteSequence(shaderFile, leadData.dataload, leadData.h0, leadData.h1, leadData.h2, seqName: seqName);
            OutputWriteLine("");
            foreach (var item in writeSequences) {
                if (item.Value > lastseq) {
                    lastseq = item.Value;
                    ZDataBlock zBlock = zframeFile.dataBlocks[item.Key];
                    seqName = $"WRITESEQ[{lastseq}]";
                    PrintParamWriteSequence(shaderFile, zBlock.dataload, zBlock.h0, zBlock.h1, zBlock.h2, seqName: seqName);
                    OutputWriteLine("");
                }
            }
        }




        /*
         *
         * usally writeseq -1 means empty writesequences, i.e. nothing to do
         * rarely if leadingData.h0 is 0 it is given a writeseq of 0 while the writesequence itself will be empty
         *
         *
         */
        static SortedDictionary<int, int> GetWriteSequences(ZFrameFile zframeFile) {
            Dictionary<string, int> writeSequences = new();
            SortedDictionary<int, int> sequencesMap = new();
            int seqCount = 0;
            // IMP the first entry is always set 0 regardless of whether the leading datablock carries any data
            sequencesMap.Add(zframeFile.leadingData.blockId, 0);
            if (zframeFile.leadingData.h0 == 0) {
                writeSequences.Add("", seqCount++);
            } else {
                writeSequences.Add(DataReader.BytesToString(zframeFile.leadingData.dataload, -1), seqCount++);
            }
            foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                if (zBlock.dataload == null) {
                    sequencesMap.Add(zBlock.blockId, -1);
                    continue;
                }
                string dataloadStr = DataReader.BytesToString(zBlock.dataload, -1);
                int seq = writeSequences.GetValueOrDefault(dataloadStr, -1);
                if (seq == -1) {
                    writeSequences.Add(dataloadStr, seqCount);
                    sequencesMap.Add(zBlock.blockId, seqCount);
                    seqCount++;
                } else {
                    sequencesMap.Add(zBlock.blockId, seq);
                }

            }
            return sequencesMap;
        }


        static void PrintParamWriteSequence(ShaderFile shaderFile, byte[] dataload, int h0, int h1, int h2, string seqName = "") {
            if (h0 == 0) {
                OutputWriteLine("[empty writesequence]");
                return;
            }
            string b2Desc = "dest";
            string b3Desc = "control";
            // string dataBlockHeader = $"{new string(' ', 5)} {new string(' ', 28)} {b2Desc,-11} {b3Desc}";
            string dataBlockHeader = $"{seqName,-34} {b2Desc,-11} {b3Desc}";
            OutputWriteLine(dataBlockHeader);
            for (int i = 0; i < h0; i++) {
                int paramId = dataload[i * 4];
                int b2 = dataload[i * 4 + 2];
                int b3 = dataload[i * 4 + 3];
                string b2Text = $"{b2,2} ({b2:X02})";
                string b3Text = $"{b3,2} ({b3:X02})";
                OutputWrite($"[{paramId,3}] {shaderFile.paramBlocks[paramId].name0,-30} {b2Text,-14} {b3Text}");
                if (i + 1 == h0) {
                    OutputWrite($"   // {h0}");
                }
                if (i + 1 == h1) {
                    OutputWrite($"   // {h1}");
                }
                if (i + 1 == h2) {
                    OutputWrite($"   // {h2}");
                }
                OutputWriteLine("");
            }
        }



        static void PrintZBlock(ShaderFile shaderFile, ZDataBlock zBlock) {
            int h0 = zBlock.h0;
            int h1 = zBlock.h1;
            int h2 = zBlock.h2;
            string blockName = zBlock.blockId == -1 ? "leading data" : $"data-block[{zBlock.blockId}]";
            OutputWriteLine($"{blockName} ({h0},{h1},{h2})");
            if (zBlock.blockId > -1 && h0 == 0) {
                return;
            }
            PrintParamWriteSequence(shaderFile, zBlock.dataload, h0, h1, h2);
            OutputWriteLine("");
        }






        //static void PrintZBlock(ZDataBlock zBlock) {
        //    int h0 = zframeFile.leadingData.h0;
        //    int h1 = zframeFile.leadingData.h1;
        //    int h2 = zframeFile.leadingData.h2;
        //    OutputWriteLine($"leading data ({h0},{h1},{h2})");

        //    string b2Desc = "dest";
        //    string b3Desc = "control";
        //    string dataBlockHeader = $"{new string(' ', 5)} {new string(' ', 28)} {b2Desc,-11} {b3Desc}";
        //    OutputWriteLine(dataBlockHeader);
        //    for (int i = 0; i < zframeFile.leadingData.h0; i++) {
        //        int paramId = zframeFile.leadingData.dataload[i * 4];
        //        int b2 = zframeFile.leadingData.dataload[i * 4 + 2];
        //        int b3 = zframeFile.leadingData.dataload[i * 4 + 3];
        //        string b2Text = $"{b2,2} ({b2:X02})";
        //        string b3Text = $"{b3,2} ({b3:X02})";
        //        OutputWrite($"[{paramId,3}] {shaderFile.paramBlocks[paramId].name0,-30} {b2Text,-14} {b3Text}");
        //        if (i+1 == h0) {
        //            OutputWrite($"   // h0 = {h0}");
        //        }
        //        if (i+1 == h1) {
        //            OutputWrite($"   // h1 = {h1}");
        //        }
        //        if (i+1 == h2) {
        //            OutputWrite($"   // h2 = {h2}");
        //        }
        //        OutputWriteLine("");
        //    }
        //}



        static void PrintConfigurationState(ShaderFile shaderFile, long zframeId) {
            string configHeader = "Configuration";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            CompatRulesGeneration configGen = new(shaderFile);
            int[] configState = configGen.GetConfigState(zframeId);
            for (int i = 0; i < configState.Length; i++) {
                if (configState[i] > 0) {
                    OutputWriteLine($"{shaderFile.sfBlocks[i].name0,-30} {configState[i]}");
                }
            }
            OutputWriteLine("");
        }






        static List<string> GetFileSelectionWithLimitedZframes() {
            List<string> vcsFiles = new();
            // List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, 30);
            List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.vs_file, 30);


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


        private static void WriteHtmlFile(string htmlTitle, string htmlHeader) {
            if (sw == null) {
                throw new ShaderParserException("StreamWriter needs to be setup before calling this");
            }
            WriteAsHtml = true;
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
        }


        private static void ConfigureOutputFile(string filepathname, bool disableOutput = false) {
            DisableOutput = disableOutput;
            Debug.WriteLine($"writing to {filepathname}");
            sw = new StreamWriter(filepathname);
        }


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








