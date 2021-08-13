using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static MyShaderAnalysis.vcsparsing.ZFrameFile;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using MyShaderAnalysis.utilhelpers;

namespace MyShaderAnalysis {


    public class StaticAnalysisZframes2 {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        //const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        //const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        //const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";
        //const string SERVER_BASEDIR = @"Z:\dev\www\vcs.codecreation.dev";
        //const string OUTPUT_SUB_DIR = @"\GEN-output";


        public static void RunTrials() {
            Trial1();


            PrintReport();
            CloseStreamWriter();
        }






        static void Trial1() {

            // ZFileSummary(ARCHIVE.dotacore_pcgl, "bilateral_blur_pcgl_30_vs.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0x2, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0xa9, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0xc9, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "hero_pcgl_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE.dotacore_pcgl, "depth_only_pcgl_30_vs.vcs", 0x68, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "refract_pcgl_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE.dotacore_pcgl, "visualize_cloth_pcgl_40_ps.vcs", 0x10, writeFile: true);
            // ZFileSummary(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", 0xab, writeFile: true);



            // WriteBunchOfZframes();
            PrintZframeFileDirectory(SERVER_OUTPUT_DIR, writeFile: true);


        }



        static void WriteBunchOfZframes() {
            int NUM_TO_PRINT = 30;
            List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, 30);

            foreach (var triple in triples) {
                int zframeCount = 0;
                ShaderFile shaderFile = new(triple.vsFile.filenamepath);
                foreach (var item in shaderFile.zframesLookup) {
                    ZFileSummary(triple.vsFile, item.Key, writeFile: true, disableOutput: true);
                    CloseStreamWriter();
                    zframeCount++;
                    if (zframeCount == NUM_TO_PRINT) {
                        break;
                    }
                }
                zframeCount = 0;
                shaderFile = new(triple.psFile.filenamepath);
                foreach (var item in shaderFile.zframesLookup) {
                    ZFileSummary(triple.psFile, item.Key, writeFile: true, disableOutput: true);
                    CloseStreamWriter();
                    zframeCount++;
                    if (zframeCount == NUM_TO_PRINT) {
                        break;
                    }
                }
            }
            swWriterAlreadyClosed = true;

        }


        static void ZFileSummary(ARCHIVE archive, string filename, long zframeId, bool writeFile = false, bool disableOutput = false) {
            FileTokens vcsFile = new(archive, filename);
            ZFileSummary(vcsFile, zframeId, writeFile, disableOutput);
        }



        static void ZFileSummary(FileTokens vcsFile, long zframeId, bool writeFile = false, bool disableOutput = false) {
            ShaderFile shaderFile = new(vcsFile.filenamepath);

            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zframeId);
            if (writeFile) {
                vcsFile.CreateZFramesDirectory();
                string outputFilenamepath = vcsFile.GetZFrameHtmlFilenamepath(zframeId);
                ConfigureOutputFile(outputFilenamepath, disableOutput);
                WriteHtmlFile($"Z 0x{zframeId:x}", vcsFile.GetZFrameHtmlFilename(zframeId)[0..^5]);
            }
            PrintConfigurationState(shaderFile, zframeId);
            // PrintDataBlocks1(shaderFile, zframeFile);
            PrintFrameLeadingArgs(zframeFile);
            SortedDictionary<int, int> writeSequences = GetWriteSequences(zframeFile);
            PrintWriteSequences(shaderFile, zframeFile, writeSequences);
            // PrintDataBlocks2(shaderFile, zframeFile, writeSequences);
            PrintDataBlocks3(shaderFile, zframeFile, writeSequences);
            PrintLeadSummary(zframeFile);
            PrintTailSummary(zframeFile);
            PrintSourceSummary(zframeFile);
            ShowEndBlocks(shaderFile, zframeFile);

        }



        static Dictionary<int, GlslSource> GetBlockIdToSource(ZFrameFile zframeFile) {
            Dictionary<int, GlslSource> blockIdToSource = new();
            if (zframeFile.vcsFiletype == FILETYPE.vs_file) {
                foreach (VsEndBlock vsEndBlock in zframeFile.vsEndBlocks) {
                    blockIdToSource.Add(vsEndBlock.blockIdRef, zframeFile.glslSources[vsEndBlock.sourceRef]);
                }
            } else {
                foreach (PsEndBlock psEndBlock in zframeFile.psEndBlocks) {
                    blockIdToSource.Add(psEndBlock.blockIdRef, zframeFile.glslSources[psEndBlock.sourceRef]);
                }
            }
            return blockIdToSource;
        }



        static void PrintSourceSummary(ZFrameFile zframeFile) {
            string headerText = "source summary";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            int b0 = zframeFile.flags0[0];
            int b1 = zframeFile.flags0[1];
            int b2 = zframeFile.flags0[2];
            int b3 = zframeFile.flags0[3];
            OutputWrite($"{b0:X02} {b1:X02} {b2:X02} {b3:X02}");
            OutputWriteLine($"    // possible flags {ByteToBinary(b0)} {ByteToBinary(b1)}");
            OutputWriteLine($"{zframeFile.flagbyte0}              // values seen 0,1");
            OutputWriteLine($"{zframeFile.glslSourceCount,-11}    // nr of source files");
            OutputWriteLine($"{zframeFile.flagbyte1}              // values seen 0,1");
            OutputWriteLine("");
            OutputWriteLine("");

        }

        static string ByteToBinary(int b0) {
            string byteString = "";
            byteString += $"{Convert.ToString(b0 >> 4, 2).PadLeft(4, '0')}";
            byteString += " ";
            byteString += $"{Convert.ToString(b0 & 0xf, 2).PadLeft(4, '0')}";
            return byteString;
        }



        static void PrintLeadSummary(ZFrameFile zframeFile) {
            if (zframeFile.vcsFiletype != FILETYPE.vs_file) {
                return;
            }
            OutputWriteLine(zframeFile.GetLeadSummary());
            OutputWriteLine("");
        }

        static void PrintTailSummary(ZFrameFile zframeFile) {
            OutputWriteLine(zframeFile.GetTailSummary());
            OutputWriteLine("");
        }


        static void PrintFrameLeadingArgs(ZFrameFile zframeFile) {
            string headerText = "Frame Header";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            OutputWrite(zframeFile.GetZFrameHeaderStringDescription());
            if (zframeFile.zframeParams.Count == 0) {
                OutputWriteLine("[empty frameheader]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }





        static void PrintDataBlocks3(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences) {
            Dictionary<int, GlslSource> blockIdToSource = GetBlockIdToSource(zframeFile);
            string configHeader = $"D-Param configurations ({blockIdToSource.Count})";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            PrintAbbreviations(shaderFile);
            List<int> activeBlockIds = GetActiveBlockIds(zframeFile);
            OutputWriteLine("");

            List<string> dParamNames = new();
            foreach (DBlock dBlock in shaderFile.dBlocks) {
                dParamNames.Add(ShortenShaderParam(dBlock.name0).ToLower());
            }
            string configNames = CombineStringsSpaceSep(dParamNames.ToArray(), 6);
            configNames = $"{new string(' ', 5)}{configNames}";
            int dBlockCount = 0;


            foreach (int blockId in activeBlockIds) {
                //if (zframeFile.dataBlocks[blockId].h0 == 0) {
                //    continue;
                //}
                if (dBlockCount % 100 == 0) {
                    OutputWriteLine($"{configNames}");
                }
                dBlockCount++;
                int[] dBlockConfig = shaderFile.GetDBlockConfig(blockId);
                string configStr = CombineIntsSpaceSep(dBlockConfig, 6);
                string writeSeqText = $"WRITESEQ[{writeSequences[blockId]}]";
                if (writeSequences[blockId] == -1) {
                    writeSeqText = "[empty writeseq]";
                }
                OutputWrite($"[{blockId:X02}] {configStr}        {writeSeqText,-12}");
                GlslSource blockSource = blockIdToSource[blockId];
                OutputWriteLine($"    {GetSourceLink(shaderFile.filenamepath, blockSource)} {blockSource.offset1,12}  (bytes)");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }


        static List<int> GetActiveBlockIds(ZFrameFile zframeFile) {
            List<int> blockIds = new();
            if (zframeFile.vcsFiletype == FILETYPE.vs_file) {
                foreach (VsEndBlock vsEndBlock in zframeFile.vsEndBlocks) {
                    blockIds.Add(vsEndBlock.blockIdRef);
                }
            } else {
                foreach (PsEndBlock psEndBlock in zframeFile.psEndBlocks) {
                    blockIds.Add(psEndBlock.blockIdRef);
                }
            }
            return blockIds;
        }




        static void PrintDataBlocks2(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences) {
            Dictionary<int, GlslSource> blockIdToSource = GetBlockIdToSource(zframeFile);
            string configHeader = $"D-Param configurations ({blockIdToSource.Count})";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            PrintAbbreviations(shaderFile);
            OutputWriteLine("");
            List<string> dParamNames = new();
            foreach (DBlock dBlock in shaderFile.dBlocks) {
                dParamNames.Add(ShortenShaderParam(dBlock.name0).ToLower());
            }
            string configNames = CombineStringsSpaceSep(dParamNames.ToArray(), 6);
            configNames = $"{new string(' ', 5)}{configNames}";
            int dBlockCount = 0;
            for (int blockId = 0; blockId < zframeFile.dataBlocks.Count; blockId++) {
                if (zframeFile.dataBlocks[blockId].h0 == 0) {
                    continue;
                }
                if (dBlockCount % 100 == 0) {
                    OutputWriteLine($"{configNames}");
                }
                dBlockCount++;
                int[] dBlockConfig = shaderFile.GetDBlockConfig(blockId);
                string configStr = CombineIntsSpaceSep(dBlockConfig, 6);
                string writeSeqText = $"WRITESEQ[{writeSequences[blockId]}]";
                OutputWrite($"[{blockId:X02}] {configStr}    {writeSeqText,14}");
                GlslSource blockSource = blockIdToSource[blockId];
                OutputWriteLine($"    {GetSourceLink(shaderFile.filenamepath, blockSource)} {blockSource.offset1,12}  (bytes)");
            }
            OutputWriteLine("");
        }


        static string GetSourceLink(string filenamepath, GlslSource blockSource) {
            string glslId = blockSource.GetStringId();
            string token = GetCoreOrDotaString(filenamepath);
            string fileName = @$"/vcs-all/{token}/zsource/glsl-{glslId}.html";
            string urlText = $"source[{glslId}]";
            return $"<a href='{fileName}'>{urlText}</a>";
        }


        static void PrintAbbreviations(ShaderFile shaderFile) {
            List<string> abbreviations = new();
            foreach (var dBlock in shaderFile.dBlocks) {
                string abbreviation = $"{dBlock.name0}({ShortenShaderParam(dBlock.name0).ToLower()})";
                abbreviations.Add(abbreviation);
            }
            string[] breakabbreviations = CombineValuesBreakString(abbreviations.ToArray(), 120);
            if (breakabbreviations.Length == 1 && breakabbreviations[0] == "") {
                return;
            }
            foreach (string abbr in breakabbreviations) {
                OutputWriteLine(abbr.Replace("(", "<span style='color: blue'>(").Replace(")", "</span>)"));
            }
        }



        static void PrintWriteSequences(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences) {
            string headerText = "Parameter glsl mapping";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));

            int lastseq = writeSequences[-1];
            if (zframeFile.leadingData.h0 > 0) {
                OutputWriteLine("");
            }


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
            OutputWriteLine("");
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
            string dataBlockHeader = $"{seqName,-35} {b2Desc,-11} {b3Desc}";
            OutputWriteLine(dataBlockHeader);
            for (int i = 0; i < h0; i++) {
                int paramId = dataload[i * 4];
                int b2 = dataload[i * 4 + 2];
                int b3 = dataload[i * 4 + 3];
                string b2Text = $"{b2,3} ({b2:X02})";
                if (b2 == 0xff) {
                    b2Text = $"  _ ({b2:X02})";
                }
                string b3Text = $"{b3,3} ({b3:X02})";
                if (b3 == 0xff) {
                    b3Text = $"  _ ({b2:X02})";
                }
                OutputWrite($"[{paramId,3}] {shaderFile.paramBlocks[paramId].name0,-30} {b2Text,-14} {b3Text}");
                if (i + 1 == h0 && h0 != h2) {
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




        static void PrintDataBlocks1(ShaderFile shaderFile, ZFrameFile zframeFile) {
            PrintZBlock(shaderFile, zframeFile.leadingData);
            for (int i = 0; i < zframeFile.dataBlocks.Count; i++) {
                if (i > 0 && zframeFile.dataBlocks[i].h0 > 0 && zframeFile.dataBlocks[i - 1].h0 == 0) {
                    OutputWriteLine("");
                }
                PrintZBlock(shaderFile, zframeFile.dataBlocks[i]);
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
            // FIXME - needs rethink
            PrintReferingFileLink(shaderFile.filenamepath);
            PrintZframeByteCodeLink(shaderFile.filenamepath, zframeId);

            OutputWriteLine("");
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

            if (zframeId == 0) {
                OutputWriteLine("[all static params 0]");
            } else if (configState.Length == 1) {
                OutputWriteLine("[no static params defined]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }


        static void PrintZframeByteCodeLink(string filenamepath, long zframeId) {
            string token = GetCoreOrDotaString(filenamepath);
            string htmlFileName = $"{Path.GetFileName(filenamepath[0..^4])}-ZFRAME{zframeId:x08}.html";
            string htmlLink = $"<a href='/vcs-all/{token}/zsource/{htmlFileName}'>{htmlFileName[0..^5]} databytes</a>";
            OutputWrite(htmlLink);
            OutputWriteLine("");
        }


        static void PrintReferingFileLink(string filenamepath) {
            string token = GetCoreOrDotaString(filenamepath);
            string htmlFileName = $"{Path.GetFileName(filenamepath[0..^4])}-summary.html";
            string htmlLink = $"<a href='/GEN-output/sf-summaries/{token}/{htmlFileName}'>{ShortHandName(filenamepath)}</a>";
            OutputWrite(htmlLink);
            OutputWriteLine("");
        }



        static void ShowEndBlocks(ShaderFile shaderFile, ZFrameFile zframeFile) {
            string headerText = "End blocks";
            OutputWriteLine($"{headerText}");
            OutputWriteLine(new string('-', headerText.Length));

            FILETYPE vcsFiletype = shaderFile.vcsFiletype;
            if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.gs_file) {
                OutputWriteLine($"{zframeFile.vsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.vsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (VsEndBlock vsEndBlock in zframeFile.vsEndBlocks) {
                    OutputWriteLine($"block-ref         {vsEndBlock.blockIdRef}");
                    OutputWriteLine($"arg0              {vsEndBlock.arg0}");
                    OutputWriteLine($"source-ref        {vsEndBlock.sourceRef}");
                    OutputWriteLine($"source-pointer    {vsEndBlock.sourcePointer}");
                    OutputWriteLine($"{DataReader.BytesToString(vsEndBlock.databytes)}");
                    OutputWriteLine("");
                }
            } else {
                OutputWriteLine($"{zframeFile.psEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.psEndBlocks.Count})");
                OutputWriteLine("");
                foreach (PsEndBlock psEndBlock in zframeFile.psEndBlocks) {
                    OutputWriteLine($"block-ref         {psEndBlock.blockIdRef}");
                    OutputWriteLine($"arg0              {psEndBlock.arg0}");
                    OutputWriteLine($"source-ref        {psEndBlock.sourceRef}");
                    OutputWriteLine($"source-pointer    {psEndBlock.sourcePointer}");
                    OutputWriteLine($"has data ({psEndBlock.hasData0},{psEndBlock.hasData1},{psEndBlock.hasData2})");
                    if (psEndBlock.hasData0) {
                        OutputWriteLine("// data-section 0");
                        OutputWriteLine($"{DataReader.BytesToString(psEndBlock.data0)}");
                    }
                    if (psEndBlock.hasData1) {
                        OutputWriteLine("// data-section 1");
                        OutputWriteLine($"{DataReader.BytesToString(psEndBlock.data1)}");
                    }
                    if (psEndBlock.hasData2) {
                        OutputWriteLine("// data-section 2");
                        OutputWriteLine($"{DataReader.BytesToString(psEndBlock.data2[0..3])}");
                        OutputWriteLine($"{DataReader.BytesToString(psEndBlock.data2[3..11])}");
                        OutputWriteLine($"{DataReader.BytesToString(psEndBlock.data2[11..75])}");
                    }
                    OutputWriteLine("");
                }
            }
        }





        static void PrintZframeFileDirectory(string outputDir = null, bool writeFile = false, bool disableOutput = false) {
            string outputFilenamepath = $"{outputDir}/zframelisting.html";
            if (outputDir != null && writeFile) {
                ConfigureOutputFile(outputFilenamepath, disableOutput);
                WriteHtmlFile("Zframes", "Zframes");
            }

            Dictionary<FileTokens, List<long>> zframesFound = new();
            List<string> coreFiles = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.any, 30);
            foreach (var filenamepath in coreFiles) {
                FileTokens vcsFile = new(ARCHIVE.dotacore_pcgl, filenamepath);
                List<long> zframeIds = new();
                zframesFound.Add(vcsFile, zframeIds);
                foreach (var item in vcsFile.GetZFrameListing()) {
                    long zframeId = Convert.ToInt64(item[^13..^5], 16);
                    zframeIds.Add(zframeId);
                }
            }

            List<string> gameFiles = GetVcsFiles(PCGL_DIR_NOT_CORE, null, FILETYPE.any, 30);
            foreach (var filenamepath in gameFiles) {
                FileTokens vcsFile = new(ARCHIVE.dotagame_pcgl, filenamepath);
                List<long> zframeIds = new();
                zframesFound.Add(vcsFile, zframeIds);
                foreach (var item in vcsFile.GetZFrameListing()) {
                    long zframeId = Convert.ToInt64(item[^13..^5], 16);
                    zframeIds.Add(zframeId);
                }
            }

            foreach (var item in zframesFound) {
                FileTokens vcsFile = item.Key;
                List<long> zframeIds = item.Value;

                string htmlName = $"[{vcsFile.RemoveBaseDir()}]".PadRight(60);
                htmlName = htmlName.Replace("[", $"<a href='{vcsFile.GetBestPath()}'>");
                htmlName = htmlName.Replace("]", "</a>");
                OutputWrite($"{htmlName}");

                string zFrameListing = "";
                foreach (long zframeId in zframeIds) {
                    string zframeName = $" [{zframeId:x}]";
                    string zframeLink  = zframeName.Replace("[", $"<a href='{vcsFile.GetZFrameLink(zframeId)}'>");
                    zframeLink = zframeLink.Replace("]", "</a>");

                    // OutputWrite(vcsFile.GetZFrameLink(zframeId));
                    OutputWrite(zframeLink);
                }
                OutputWriteLine("");
            }
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








