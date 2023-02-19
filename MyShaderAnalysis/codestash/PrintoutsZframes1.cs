using System;
using System.IO;
using System.Collections.Generic;

using ConfigMappingSParams = ValveResourceFormat.CompiledShader.ConfigMappingSParams;
using DBlock = ValveResourceFormat.CompiledShader.DBlock;
using GlslSource = ValveResourceFormat.CompiledShader.GlslSource;
using GpuSource = ValveResourceFormat.CompiledShader.GpuSource;
using PrintZFrameSummary = ValveResourceFormat.CompiledShader.PrintZFrameSummary;
using ShaderFile = ValveResourceFormat.CompiledShader.ShaderFile;
using ShaderParserException = ValveResourceFormat.CompiledShader.ShaderParserException;
using VcsProgramType = ValveResourceFormat.CompiledShader.VcsProgramType;
using ZDataBlock = ValveResourceFormat.CompiledShader.ZDataBlock;
using ZFrameFile = ValveResourceFormat.CompiledShader.ZFrameFile;

using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;


/*
 *
 * Printouts formatted zframes - replaced by production file PrintZframeSummary
 *
 *
 *
 */
namespace MyShaderAnalysis.codestash
{


    public class PrintoutsZframes1
    {

        // const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        // const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        //const string PC_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        //const string PC_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        // const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        // const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";
        //const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        // const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";
        //const string SERVER_BASEDIR = @"Z:/dev/www/vcs.codecreation.dev";
        //const string OUTPUT_SUB_DIR = @"/GEN-output";


        public static void RunTrials()
        {

            WriteBunchOfZframes();
            // PrintSingleFileSummary();

            PrintReport();
            CloseStreamWriter();
        }


        static void PrintSingleFileSummary()
        {

            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "bilateral_blur_pcgl_30_vs.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0x2, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0xa9, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "multiblend_pcgl_30_ps.vcs", 0xc9, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "hero_pcgl_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "depth_only_pcgl_30_vs.vcs", 0x68, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "refract_pcgl_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "visualize_cloth_pcgl_40_ps.vcs", 0x10, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "multiblend_pcgl_30_vs.vcs", 0xab, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "panorama_fancyquad_pcgl_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "blur_pcgl_30_ps.vcs", 0x1, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "copytexture_pcgl_30_vs.vcs", 0x1, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "copytexture_pcgl_30_vs.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.artifact_classiccore_pc, "generic_pc_30_ps.vcs", 0x10, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dota_core_gles, "copytexture_mobile_gles_30_ps.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.artifact_classiccore_pc, "depth_only_pc_40_vs.vcs", 0x0, writeFile: true, disableOutput: true);
            // WriteBunchOfZframes();



            // ZFileSummary(ARCHIVE_OLD.dotagame_pcgl, "visualize_physics_pcgl_40_gs.vcs", 0x0, writeFile: true);


            ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "visualize_physics_pcgl_40_gs.vcs", 0x0, writeFile: true);
            // ZFileSummary(ARCHIVE_OLD.dotacore_pcgl, "debug_show_texture_pcgl_30_vs.vcs", 0x0, writeFile: true);



            // PrintZframeFileDirectory(SERVER_OUTPUT_DIR, writeFile: true);
        }



        static void WriteBunchOfZframes()
        {
            int NUM_TO_PRINT = 2;
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(vcsFiles);
            // List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(DOTA_GAME_PCGL_SOURCE, DOTA_CORE_PCGL_SOURCE, -1);
            // List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(DOTA_GAME_PCGL_SOURCE, DOTA_CORE_PCGL_SOURCE, 30);
            // List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, -1);
            // List<FileTriple> triples = FileTriple.GetFeaturesVsPsFileTriple(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, -1);


            int count = 0;

            foreach (var triple in triples)
            {
                int zframeCount = 0;
                ShaderFile shaderFile = InstantiateShaderFile(triple.vsFile.filenamepath);
                foreach (var item in shaderFile.ZframesLookup)
                {
                    ZFileSummary(triple.vsFile, item.Key, writeFile: true, disableOutput: true);
                    CloseStreamWriter();
                    zframeCount++;
                    if (zframeCount == NUM_TO_PRINT)
                    {
                        break;
                    }
                }
                zframeCount = 0;
                shaderFile = InstantiateShaderFile(triple.psFile.filenamepath);
                foreach (var item in shaderFile.ZframesLookup)
                {
                    ZFileSummary(triple.psFile, item.Key, writeFile: true, disableOutput: true);
                    CloseStreamWriter();
                    zframeCount++;
                    if (zframeCount == NUM_TO_PRINT)
                    {
                        break;
                    }
                }
                //count++;
                //if (count == 2)
                //{
                //    break;
                //}
            }
            swWriterAlreadyClosed = true;

        }


        static void ZFileSummary(ARCHIVE_OLD archive, string filename, long zframeId, bool writeFile = false, bool disableOutput = false)
        {
            FileTokensOld vcsFile = new(archive, filename);
            ZFileSummary(vcsFile, zframeId, writeFile, disableOutput);
        }



        static void ZFileSummary(FileTokensOld vcsFile, long zframeId, bool writeFile = false, bool disableOutput = false)
        {
            // writeFile = false;
            // DisableOutput = true;
            ShaderFile shaderFile = InstantiateShaderFile(vcsFile.filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(zframeId);
            if (writeFile)
            {
                vcsFile.CreateZFramesDirectory();
                string outputFilenamepath = vcsFile.GetZFrameHtmlFilenamepath(zframeId, "summary");
                ConfigureOutputFile(outputFilenamepath, disableOutput);
                WriteHtmlFile($"Z 0x{zframeId:x}", vcsFile.GetZFrameHtmlFilename(zframeId, "")[0..^5]);
            }


            // print referring files
            // OutputWriteLine($"<a href='{vcsFile.GetBestPath()}'>{vcsFile.RemoveBaseDir()}</a>");
            OutputWriteLine($"<a href='{vcsFile.GetServerFileUrl("summary2")}'>{vcsFile.RemoveBaseDir()}</a>");
            // PrintZframeByteCodeLink(shaderFile.filenamepath, zframeId);
            OutputWriteLine($"<a href='{vcsFile.GetZFrameLink(zframeId, "bytes")}'>{vcsFile.GetZFrameHtmlFilename(zframeId, "")[0..^5]}-databytes</a>");
            OutputWriteLine("");


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



        static Dictionary<int, GpuSource> GetBlockIdToSource(ZFrameFile zframeFile)
        {
            Dictionary<int, GpuSource> blockIdToSource = new();
            if (zframeFile.VcsProgramType == VcsProgramType.VertexShader || zframeFile.VcsProgramType == VcsProgramType.GeometryShader)
            {
                foreach (ZFrameFile.VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIdToSource.Add(vsEndBlock.BlockIdRef, zframeFile.GpuSources[vsEndBlock.SourceRef]);
                }
            } else
            {
                foreach (ZFrameFile.PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
                {
                    blockIdToSource.Add(psEndBlock.BlockIdRef, zframeFile.GpuSources[psEndBlock.SourceRef]);
                }
            }
            return blockIdToSource;
        }



        static void PrintSourceSummary(ZFrameFile zframeFile)
        {
            string headerText = "source summary";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            int b0 = zframeFile.Flags0[0];
            int b1 = zframeFile.Flags0[1];
            int b2 = zframeFile.Flags0[2];
            int b3 = zframeFile.Flags0[3];
            OutputWrite($"{b0:X02} {b1:X02} {b2:X02} {b3:X02}");
            OutputWriteLine($"    // possible flags {ByteToBinary(b0)} {ByteToBinary(b1)}");
            OutputWriteLine($"{zframeFile.Flagbyte0}              // values seen 0,1");
            OutputWriteLine($"{zframeFile.GpuSourceCount,-11}    // nr of source files");
            OutputWriteLine($"{zframeFile.Flagbyte1}              // values seen 0,1");
            OutputWriteLine("");
            OutputWriteLine("");

        }

        static string ByteToBinary(int b0)
        {
            string byteString = "";
            byteString += $"{Convert.ToString(b0 >> 4, 2).PadLeft(4, '0')}";
            byteString += " ";
            byteString += $"{Convert.ToString(b0 & 0xf, 2).PadLeft(4, '0')}";
            return byteString;
        }



        static void PrintLeadSummary(ZFrameFile zframeFile)
        {
            OutputWriteLine(PrintZFrameSummary.SummarizeBytes(zframeFile.LeadingSummary));
            OutputWriteLine("");
        }
        static void PrintTailSummary(ZFrameFile zframeFile)
        {
            OutputWriteLine(PrintZFrameSummary.SummarizeBytes(zframeFile.TrailingSummary));
            OutputWriteLine("");
        }


        static void PrintFrameLeadingArgs(ZFrameFile zframeFile)
        {
            string headerText = "Frame Header";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            OutputWrite(zframeFile.ZFrameHeaderStringDescription());
            if (zframeFile.ZframeParams.Count == 0)
            {
                OutputWriteLine("[empty frameheader]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }





        static void PrintDataBlocks3(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences)
        {
            FileTokensOld fileTokens = new FileTokensOld(shaderFile.FilenamePath);

            Dictionary<int, GpuSource> blockIdToSource = GetBlockIdToSource(zframeFile);
            string configHeader = $"D-Param configurations ({blockIdToSource.Count})";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            PrintAbbreviations(shaderFile);
            List<int> activeBlockIds = GetActiveBlockIds(zframeFile);


            List<string> dParamNames = new();
            foreach (DBlock dBlock in shaderFile.DBlocks)
            {
                dParamNames.Add(ShortenShaderParam(dBlock.Name0).ToLower());
            }
            string configNames = CombineStringsSpaceSep(dParamNames.ToArray(), 6);
            configNames = $"{new string(' ', 5)}{configNames}";
            int dBlockCount = 0;


            foreach (int blockId in activeBlockIds)
            {
                if (dBlockCount % 100 == 0)
                {
                    OutputWriteLine($"{configNames}");
                }
                dBlockCount++;
                int[] dBlockConfig = shaderFile.GetDBlockConfig(blockId);
                string configStr = CombineIntsSpaceSep(dBlockConfig, 6);
                string writeSeqText = $"WRITESEQ[{writeSequences[blockId]}]";
                if (writeSequences[blockId] == -1)
                {
                    writeSeqText = "[empty]";
                }
                OutputWrite($"[{blockId:X02}] {configStr}        {writeSeqText,-12}");
                GpuSource blockSource = blockIdToSource[blockId];

                if (blockSource is GlslSource source)
                {
                    string urlText = $"source[{blockSource.GetEditorRefIdAsString()}]";
                    string sourceLink = $"<a href='{fileTokens.GetGlslHtmlUrl(blockSource.GetEditorRefIdAsString())}'>{urlText}</a>";
                    OutputWriteLine($"    {sourceLink} {blockSource.Sourcebytes.Length,12}  (bytes)");
                } else
                {
                    // todo - doesn't show id along with block name
                    OutputWriteLine($"  {blockSource.GetBlockName().PadRight(20)} {blockSource.Sourcebytes.Length,12} (bytes)");
                }

            }
            OutputWriteLine("");
            OutputWriteLine("");
        }


        static List<int> GetActiveBlockIds(ZFrameFile zframeFile)
        {
            List<int> blockIds = new();
            if (zframeFile.VcsProgramType == VcsProgramType.VertexShader || zframeFile.VcsProgramType == VcsProgramType.GeometryShader)
            {
                foreach (ZFrameFile.VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIds.Add(vsEndBlock.BlockIdRef);
                }
            } else
            {
                foreach (ZFrameFile.PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
                {
                    blockIds.Add(psEndBlock.BlockIdRef);
                }
            }
            return blockIds;
        }




        static void PrintDataBlocks2(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences)
        {
            Dictionary<int, GpuSource> blockIdToSource = GetBlockIdToSource(zframeFile);
            string configHeader = $"D-Param configurations ({blockIdToSource.Count})";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            PrintAbbreviations(shaderFile);
            OutputWriteLine("");
            List<string> dParamNames = new();
            foreach (DBlock dBlock in shaderFile.DBlocks)
            {
                dParamNames.Add(ShortenShaderParam(dBlock.Name0).ToLower());
            }
            string configNames = CombineStringsSpaceSep(dParamNames.ToArray(), 6);
            configNames = $"{new string(' ', 5)}{configNames}";
            int dBlockCount = 0;
            for (int blockId = 0; blockId < zframeFile.DataBlocks.Count; blockId++)
            {
                if (zframeFile.DataBlocks[blockId].H0 == 0)
                {
                    continue;
                }
                if (dBlockCount % 100 == 0)
                {
                    OutputWriteLine($"{configNames}");
                }
                dBlockCount++;
                int[] dBlockConfig = shaderFile.GetDBlockConfig(blockId);
                string configStr = CombineIntsSpaceSep(dBlockConfig, 6);
                string writeSeqText = $"WRITESEQ[{writeSequences[blockId]}]";
                OutputWrite($"[{blockId:X02}] {configStr}    {writeSeqText,14}");
                GpuSource blockSource = blockIdToSource[blockId];
                if (blockSource is GlslSource)
                {
                    OutputWriteLine($"    {GetSourceLink(shaderFile.FilenamePath, (GlslSource)blockSource)} {blockSource.Sourcebytes.Length,12}  (bytes)");
                }
            }
            OutputWriteLine("");
        }


        static string GetSourceLink(string filenamepath, GlslSource blockSource)
        {
            string glslId = blockSource.GetEditorRefIdAsString();
            string token = GetCoreOrDotaString(filenamepath);
            string fileName = @$"/vcs-all/{token}/zsource/glsl-{glslId}.html";
            string urlText = $"source[{glslId}]";
            return $"<a href='{fileName}'>{urlText}</a>";
        }


        static void PrintAbbreviations(ShaderFile shaderFile)
        {
            List<string> abbreviations = new();
            foreach (var dBlock in shaderFile.DBlocks)
            {
                string abbreviation = $"{dBlock.Name0}({ShortenShaderParam(dBlock.Name0).ToLower()})";
                abbreviations.Add(abbreviation);
            }
            if (abbreviations.Count == 0)
            {
                return;
            }

            string[] breakabbreviations = CombineValuesBreakString(abbreviations.ToArray(), 120);
            if (breakabbreviations.Length == 1 && breakabbreviations[0] == "")
            {
                return;
            }
            foreach (string abbr in breakabbreviations)
            {
                OutputWriteLine(abbr.Replace("(", "<span style='color: blue'>(").Replace(")", "</span>)"));
                OutputWriteLine("");
            }
        }



        static void PrintWriteSequences(ShaderFile shaderFile, ZFrameFile zframeFile, SortedDictionary<int, int> writeSequences)
        {
            string headerText = "Parameter glsl mapping";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));

            int lastseq = writeSequences[-1];
            if (zframeFile.LeadingData.H0 > 0)
            {
                OutputWriteLine("");
            }


            string seqName = $"WRITESEQ[{lastseq}] (default)";
            ValveResourceFormat.CompiledShader.ZDataBlock leadData = zframeFile.LeadingData;
            PrintParamWriteSequence(shaderFile, leadData.Dataload, leadData.H0, leadData.H1, leadData.H2, seqName: seqName);
            OutputWriteLine("");
            foreach (var item in writeSequences)
            {
                if (item.Value > lastseq)
                {
                    lastseq = item.Value;
                    ZDataBlock zBlock = zframeFile.DataBlocks[item.Key];
                    seqName = $"WRITESEQ[{lastseq}]";
                    PrintParamWriteSequence(shaderFile, zBlock.Dataload, zBlock.H0, zBlock.H1, zBlock.H2, seqName: seqName);
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
        static SortedDictionary<int, int> GetWriteSequences(ZFrameFile zframeFile)
        {
            Dictionary<string, int> writeSequences = new();
            SortedDictionary<int, int> sequencesMap = new();
            int seqCount = 0;
            // IMP the first entry is always set 0 regardless of whether the leading datablock carries any data
            sequencesMap.Add(zframeFile.LeadingData.BlockId, 0);
            if (zframeFile.LeadingData.H0 == 0)
            {
                writeSequences.Add("", seqCount++);
            } else
            {
                writeSequences.Add(BytesToString(zframeFile.LeadingData.Dataload, -1), seqCount++);
            }
            foreach (ZDataBlock zBlock in zframeFile.DataBlocks)
            {
                if (zBlock.Dataload == null)
                {
                    sequencesMap.Add(zBlock.BlockId, -1);
                    continue;
                }
                string dataloadStr = BytesToString(zBlock.Dataload, -1);
                int seq = writeSequences.GetValueOrDefault(dataloadStr, -1);
                if (seq == -1)
                {
                    writeSequences.Add(dataloadStr, seqCount);
                    sequencesMap.Add(zBlock.BlockId, seqCount);
                    seqCount++;
                } else
                {
                    sequencesMap.Add(zBlock.BlockId, seq);
                }

            }
            return sequencesMap;
        }


        static void PrintParamWriteSequence(ShaderFile shaderFile, byte[] dataload, int h0, int h1, int h2, string seqName = "")
        {
            if (h0 == 0)
            {
                OutputWriteLine("[empty writesequence]");
                return;
            }
            string b2Desc = "dest";
            string b3Desc = "control";
            // string dataBlockHeader = $"{new string(' ', 5)} {new string(' ', 28)} {b2Desc,-11} {b3Desc}";
            string dataBlockHeader = $"{seqName,-35} {b2Desc,-11} {b3Desc}";
            OutputWriteLine(dataBlockHeader);
            for (int i = 0; i < h0; i++)
            {
                int paramId = dataload[i * 4];
                int b2 = dataload[i * 4 + 2];
                int b3 = dataload[i * 4 + 3];
                string b2Text = $"{b2,3} ({b2:X02})";
                if (b2 == 0xff)
                {
                    b2Text = $"  _ ({b2:X02})";
                }
                string b3Text = $"{b3,3} ({b3:X02})";
                if (b3 == 0xff)
                {
                    b3Text = $"  _ ({b2:X02})";
                }
                OutputWrite($"[{paramId,3}] {shaderFile.ParamBlocks[paramId].Name0,-30} {b2Text,-14} {b3Text}");
                if (i + 1 == h0 && h0 != h2)
                {
                    OutputWrite($"   // {h0}");
                }
                if (i + 1 == h1)
                {
                    OutputWrite($"   // {h1}");
                }
                if (i + 1 == h2)
                {
                    OutputWrite($"   // {h2}");
                }
                OutputWriteLine("");
            }
        }




        static void PrintDataBlocks1(ShaderFile shaderFile, ZFrameFile zframeFile)
        {
            PrintZBlock(shaderFile, zframeFile.LeadingData);
            for (int i = 0; i < zframeFile.DataBlocks.Count; i++)
            {
                if (i > 0 && zframeFile.DataBlocks[i].H0 > 0 && zframeFile.DataBlocks[i - 1].H0 == 0)
                {
                    OutputWriteLine("");
                }
                PrintZBlock(shaderFile, zframeFile.DataBlocks[i]);
            }
        }

        static void PrintZBlock(ShaderFile shaderFile, ZDataBlock zBlock)
        {
            int h0 = zBlock.H0;
            int h1 = zBlock.H1;
            int h2 = zBlock.H2;
            string blockName = zBlock.BlockId == -1 ? "leading data" : $"data-block[{zBlock.BlockId}]";
            OutputWriteLine($"{blockName} ({h0},{h1},{h2})");
            if (zBlock.BlockId > -1 && h0 == 0)
            {
                return;
            }
            PrintParamWriteSequence(shaderFile, zBlock.Dataload, h0, h1, h2);
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



        static void PrintConfigurationState(ShaderFile shaderFile, long zframeId)
        {
            string configHeader = "Configuration";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            ConfigMappingSParams configGen = new(shaderFile);
            int[] configState = configGen.GetConfigState(zframeId);
            for (int i = 0; i < configState.Length; i++)
            {
                // if (configState[i] > 0) {
                OutputWriteLine($"{shaderFile.SfBlocks[i].Name0,-30} {configState[i]}");
                // }
            }
            if (configState.Length == 0)
            {
                OutputWriteLine("[no static params]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }


        // todo - remove this, functionality should be overed by FileTokens class
        static void PrintZframeByteCodeLink(string filenamepath, long zframeId)
        {
            string token = GetCoreOrDotaString(filenamepath);
            string htmlFileName = $"{Path.GetFileName(filenamepath[0..^4])}-ZFRAME{zframeId:x08}.html";
            string htmlLink = $"<a href='/vcs-all/{token}/zsource/{htmlFileName}'>{htmlFileName[0..^5]} databytes</a>";
            OutputWrite(htmlLink);
            OutputWriteLine("");
        }




        static void ShowEndBlocks(ShaderFile shaderFile, ZFrameFile zframeFile)
        {
            string headerText = "End blocks";
            OutputWriteLine($"{headerText}");
            OutputWriteLine(new string('-', headerText.Length));

            VcsProgramType vcsFiletype = shaderFile.VcsProgramType;
            if (vcsFiletype == VcsProgramType.VertexShader || vcsFiletype == VcsProgramType.GeometryShader)
            {
                OutputWriteLine($"{zframeFile.VsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.VsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (ZFrameFile.VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    OutputWriteLine($"block-ref         {vsEndBlock.BlockIdRef}");
                    OutputWriteLine($"arg0              {vsEndBlock.Arg0}");
                    OutputWriteLine($"source-ref        {vsEndBlock.SourceRef}");
                    OutputWriteLine($"source-pointer    {vsEndBlock.SourcePointer}");
                    OutputWriteLine($"{BytesToString(vsEndBlock.Databytes)}");
                    OutputWriteLine("");
                }
            } else
            {
                OutputWriteLine($"{zframeFile.PsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.PsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (ZFrameFile.PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
                {
                    OutputWriteLine($"block-ref         {psEndBlock.BlockIdRef}");
                    OutputWriteLine($"arg0              {psEndBlock.Arg0}");
                    OutputWriteLine($"source-ref        {psEndBlock.SourceRef}");
                    OutputWriteLine($"source-pointer    {psEndBlock.SourcePointer}");
                    OutputWriteLine($"has data ({psEndBlock.HasData0},{psEndBlock.HasData1},{psEndBlock.HasData2})");
                    if (psEndBlock.HasData0)
                    {
                        OutputWriteLine("// data-section 0");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data0)}");
                    }
                    if (psEndBlock.HasData1)
                    {
                        OutputWriteLine("// data-section 1");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data1)}");
                    }
                    if (psEndBlock.HasData2)
                    {
                        OutputWriteLine("// data-section 2");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[0..3])}");
                        // OutputWriteLine($"{BytesToString(psEndBlock.data2[3..11])}");
                        // OutputWriteLine($"{BytesToString(psEndBlock.data2[11..75])}");

                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[3..27])}");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[27..51])}");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[51..75])}");
                    }
                    OutputWriteLine("");
                }
            }
        }





        static void PrintZframeFileDirectory(string outputDir = null, bool writeFile = false, bool disableOutput = false)
        {
            string outputFilenamepath = $"{outputDir}/zframelisting.html";
            if (outputDir != null && writeFile)
            {
                ConfigureOutputFile(outputFilenamepath, disableOutput);
                WriteHtmlFile("Zframes", "Zframes");
            }

            Dictionary<FileTokensOld, List<long>> zframesFound = new();
            List<string> coreFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, null, VcsProgramType.Undetermined, 30);
            foreach (var filenamepath in coreFiles)
            {
                FileTokensOld vcsFile = new(ARCHIVE_OLD.dotacore_pcgl, filenamepath);
                List<long> zframeIds = new();
                zframesFound.Add(vcsFile, zframeIds);
                foreach (var item in vcsFile.GetZFrameListing())
                {
                    long zframeId = Convert.ToInt64(item[^13..^5], 16);
                    zframeIds.Add(zframeId);
                }
            }

            List<string> gameFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, null, VcsProgramType.Undetermined, 30);
            foreach (var filenamepath in gameFiles)
            {
                FileTokensOld vcsFile = new(ARCHIVE_OLD.dotagame_pcgl, filenamepath);
                List<long> zframeIds = new();
                zframesFound.Add(vcsFile, zframeIds);
                foreach (var item in vcsFile.GetZFrameListing())
                {
                    long zframeId = Convert.ToInt64(item[^13..^5], 16);
                    zframeIds.Add(zframeId);
                }
            }

            foreach (var item in zframesFound)
            {
                FileTokensOld vcsFile = item.Key;
                List<long> zframeIds = item.Value;

                string htmlName = $"[{vcsFile.RemoveBaseDir()}]".PadRight(60);
                string htmlLink = vcsFile.GetBestPath();
                if (htmlLink.Length > 0)
                {
                    htmlName = htmlName.Replace("[", $"<a href='{htmlLink}'>");
                    htmlName = htmlName.Replace("]", "</a>");
                } else
                {
                    htmlName = htmlName.Replace("[", "");
                    htmlName = htmlName.Replace("]", "");
                }
                OutputWrite($"{htmlName}");

                int listCount = 0;
                string zFrameListing = "";
                foreach (long zframeId in zframeIds)
                {
                    string zframeName = $" [{zframeId:x}]";
                    string zframeLink = zframeName.Replace("[", $"<a href='{vcsFile.GetZFrameLink(zframeId, "summary")}'>");
                    zframeLink = zframeLink.Replace("]", "</a>");

                    // OutputWrite(vcsFile.GetZFrameLink(zframeId));
                    OutputWrite(zframeLink);
                    listCount++;
                    if (listCount == 40)
                    {
                        break;
                    }
                }
                OutputWriteLine("");
            }
        }




        private static StreamWriter sw = null;
        private static bool DisableOutput = false;
        private static bool WriteAsHtml = false;
        private static bool swWriterAlreadyClosed = false;


        private static void WriteHtmlFile(string htmlTitle, string htmlHeader)
        {
            if (sw == null)
            {
                throw new ShaderParserException("StreamWriter needs to be setup before calling this");
            }
            WriteAsHtml = true;
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
        }


        private static void ConfigureOutputFile(string filepathname, bool disableOutput = false)
        {
            DisableOutput = disableOutput;
            Console.WriteLine($"writing to {filepathname}");
            sw = new StreamWriter(filepathname);
        }


        // This basestream != null is nonsense, it doesn't check if the file is open
        private static void CloseStreamWriter()
        {
            if (WriteAsHtml && !swWriterAlreadyClosed)
            {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed)
            {
                sw.Close();
            }
        }

        public static void OutputWrite(string text)
        {
            if (!DisableOutput)
            {
                Console.Write(text);
            }
            if (sw != null)
            {
                sw.Write(text);
            }
        }
        public static void OutputWriteLine(string text)
        {
            OutputWrite(text + "\n");
        }


    }



}








