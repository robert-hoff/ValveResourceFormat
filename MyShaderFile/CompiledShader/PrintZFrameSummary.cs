using System;
using System.Collections.Generic;
using System.IO;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ZFrameFile;

namespace ValveResourceFormat.CompiledShader
{
    public class PrintZFrameSummary
    {
        public HandleOutputWrite OutputWriter { get; set; }
        private ShaderFile shaderFile;
        private ZFrameFile zframeFile;
        private bool showRichTextBoxLinks;

        // If OutputWriter is left as null; output will be written to Console.
        // Otherwise output is directed to the passed HandleOutputWrite object (defined by the calling application, for example GUI element or file)
        public PrintZFrameSummary(ShaderFile shaderFile, ZFrameFile zframeFile,
            HandleOutputWrite outputWriter = null, bool showRichTextBoxLinks = false)
        {
            this.shaderFile = shaderFile;
            this.zframeFile = zframeFile;
            this.OutputWriter = outputWriter ?? ((x) => { Console.Write(x); });

            if (zframeFile.VcsProgramType == VcsProgramType.Features)
            {
                OutputWriteLine("Zframe byte data (encoding for features files has not been determined)");
                zframeFile.DataReader.BaseStream.Position = 0;
                string zframeBytes = zframeFile.DataReader.ReadBytesAsString((int)zframeFile.DataReader.BaseStream.Length);
                OutputWriteLine(zframeBytes);
                return;
            }

            this.showRichTextBoxLinks = showRichTextBoxLinks;
            if (showRichTextBoxLinks)
            {
                OutputWriteLine($"View byte detail \\\\{Path.GetFileName(shaderFile.FilenamePath)}-ZFRAME{zframeFile.ZframeId:x08}-databytes");
                OutputWriteLine("");
            }
            PrintConfigurationState();
            PrintFrameLeadingArgs();
            SortedDictionary<int, int> writeSequences = GetWriteSequences();
            PrintWriteSequences(writeSequences);
            PrintDataBlocks(writeSequences);

            if (zframeFile.VcsProgramType == VcsProgramType.VertexShader)
            {
                OutputWriteLine($"// configuration states ({zframeFile.LeadingSummary.Length}), leading summary\n");
                OutputWriteLine(SummarizeBytes(zframeFile.LeadingSummary) + "\n");
            }
            OutputWriteLine($"// configuration states ({zframeFile.TrailingSummary.Length}), trailing summary\n");
            OutputWriteLine(SummarizeBytes(zframeFile.TrailingSummary) + "\n");
            OutputWrite("\n");

            PrintSourceSummary();
            PrintEndBlocks();
        }

        private void PrintConfigurationState()
        {
            string configHeader = "Configuration";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            OutputWriteLine("The static configuration this zframe belongs to (zero or more static parameters)\n");
            ConfigMappingSParams configGen = new(shaderFile);
            int[] configState = configGen.GetConfigState(zframeFile.ZframeId);
            for (int i = 0; i < configState.Length; i++)
            {
                OutputWriteLine($"{shaderFile.SfBlocks[i].Name0,-30} {configState[i]}");
            }
            if (configState.Length == 0)
            {
                OutputWriteLine("[no static params]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }

        private void PrintFrameLeadingArgs()
        {
            string headerText = "ZFrame Headers";
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

        /*
         * Occasionally leadingData.h0 (leadingData is the first datablock, always present) is 0 we create the empty
         * write sequence WRITESEQ[0] (configurations may refer to it) otherwise sequences assigned -1 mean the write
         * sequence doesn't contain any data and not needed.
         *
         */
        private SortedDictionary<int, int> GetWriteSequences()
        {
            Dictionary<string, int> writeSequences = new();
            SortedDictionary<int, int> sequencesMap = new();
            int seqCount = 0;
            // IMP the first entry is always set 0 regardless of whether the leading datablock carries any data
            sequencesMap.Add(zframeFile.LeadingData.BlockId, 0);
            if (zframeFile.LeadingData.H0 == 0)
            {
                writeSequences.Add("", seqCount++);
            }
            else
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
                }
                else
                {
                    sequencesMap.Add(zBlock.BlockId, seq);
                }
            }
            return sequencesMap;
        }

        private void PrintWriteSequences(SortedDictionary<int, int> writeSequences)
        {
            string headerText = "Parameter write sequences";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            OutputWriteLine(
                "This data (thought to be buffer write sequences) appear to be linked to the dynamic (D-param) configurations;\n" +
                "each configuration points to exactly one sequence. WRITESEQ[0] is always defined and considered 'default'.\n");

            int lastseq = writeSequences[-1];
            if (zframeFile.LeadingData.H0 > 0)
            {
                OutputWriteLine("");
            }
            string seqName = $"WRITESEQ[{lastseq}] (default)";
            ZDataBlock leadData = zframeFile.LeadingData;
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

        private void PrintParamWriteSequence(ShaderFile shaderFile, byte[] dataload, int h0, int h1, int h2, string seqName = "")
        {
            string b2Desc = "dest";
            string b3Desc = "control";
            string dataBlockHeader = $"{seqName,-35} {b2Desc,-11} {b3Desc}";
            OutputWriteLine(dataBlockHeader);
            if (h0 == 0)
            {
                OutputWriteLine("[empty writesequence]");
                return;
            }
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

        private void PrintDataBlocks(SortedDictionary<int, int> writeSequences)
        {
            Dictionary<int, GpuSource> blockIdToSource = GetBlockIdToSource(zframeFile);
            string configHeader = $"Dynamic (D-Param) configurations ({blockIdToSource.Count} defined)";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            OutputWriteLine(
                "Each dynamic parameters has 1 or more defined states. The disabled state (0) is shown as '_'\n" +
                "All permitted configurations are listed with their matching write sequence and GPU source (there is exactly\n" +
                "one of these for each configuration). To save space, the parameter names (original names starting with D_)\n" +
                "are shortened to 3-5 length strings (shown in parenthesis).\n");
            PrintAbbreviations();
            List<int> activeBlockIds = GetActiveBlockIds();
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
                OutputWrite($"[{blockId:X02}] {configStr}   {writeSeqText,-12}");
                GpuSource blockSource = blockIdToSource[blockId];

                if (showRichTextBoxLinks)
                {
                    OutputWriteLine($"{blockSource.GetBlockName()}[{blockSource.SourceId}] \\\\source\\{blockSource.SourceId}");
                }
                else
                {
                    string sourceDesc = $"{blockSource.GetBlockName()}[{blockSource.GetEditorRefIdAsString()}]";
                    OutputWriteLine(sourceDesc);
                }
            }
            OutputWriteLine("\n");
        }

        private void PrintAbbreviations()
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
            if (breakabbreviations.Length == 1 && breakabbreviations[0].Length == 0)
            {
                return;
            }
            foreach (string abbr in breakabbreviations)
            {
                OutputWriteLine(abbr);
            }
            OutputWriteLine("");
        }

        private List<int> GetActiveBlockIds()
        {
            List<int> blockIds = new();
            if (zframeFile.VcsProgramType == VcsProgramType.VertexShader || zframeFile.VcsProgramType == VcsProgramType.GeometryShader ||
                zframeFile.VcsProgramType == VcsProgramType.ComputeShader || zframeFile.VcsProgramType == VcsProgramType.DomainShader ||
                zframeFile.VcsProgramType == VcsProgramType.HullShader)
            {
                foreach (VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIds.Add(vsEndBlock.BlockIdRef);
                }
            }
            else
            {
                foreach (PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
                {
                    blockIds.Add(psEndBlock.BlockIdRef);
                }
            }
            return blockIds;
        }

        static Dictionary<int, GpuSource> GetBlockIdToSource(ZFrameFile zframeFile)
        {
            Dictionary<int, GpuSource> blockIdToSource = new();
            if (zframeFile.VcsProgramType == VcsProgramType.VertexShader || zframeFile.VcsProgramType == VcsProgramType.GeometryShader ||
                zframeFile.VcsProgramType == VcsProgramType.ComputeShader || zframeFile.VcsProgramType == VcsProgramType.DomainShader ||
                zframeFile.VcsProgramType == VcsProgramType.HullShader)
            {
                foreach (VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIdToSource.Add(vsEndBlock.BlockIdRef, zframeFile.GpuSources[vsEndBlock.SourceRef]);
                }
            }
            else
            {
                foreach (PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
                {
                    blockIdToSource.Add(psEndBlock.BlockIdRef, zframeFile.GpuSources[psEndBlock.SourceRef]);
                }
            }
            return blockIdToSource;
        }


        public static string SummarizeBytes(int[] bytes)
        {
            string summaryDesc = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && i % 16 == 0)
                {
                    summaryDesc += "\n";
                }
                summaryDesc += bytes[i] > -1 ? $"{bytes[i],-8}" : "_  ".PadRight(8);
            }
            return summaryDesc.Trim();
        }


        private void PrintSourceSummary()
        {
            string headerText = "source bytes/flags";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            int b0 = zframeFile.Flags0[0];
            int b1 = zframeFile.Flags0[1];
            int b2 = zframeFile.Flags0[2];
            int b3 = zframeFile.Flags0[3];
            OutputWriteLine($"{b0:X02}      // possible control byte ({b0}) or flags ({Convert.ToString(b0, 2).PadLeft(8, '0')})");
            OutputWriteLine($"{b1:X02}      // values seen (0,1,2)");
            OutputWriteLine($"{b2:X02}      // always 0");
            OutputWriteLine($"{b3:X02}      // always 0");
            OutputWriteLine($"{zframeFile.Flagbyte0}       // values seen 0,1");
            OutputWriteLine($"{zframeFile.GpuSourceCount,-6}  // nr of source files");
            OutputWriteLine($"{zframeFile.Flagbyte1}       // values seen 0,1");
            OutputWriteLine("");
            OutputWriteLine("");
        }

        private static string ByteToBinary(int b0)
        {
            string byteString = "";
            byteString += $"{Convert.ToString(b0 >> 4, 2).PadLeft(4, '0')}";
            byteString += " ";
            byteString += $"{Convert.ToString(b0 & 0xf, 2).PadLeft(4, '0')}";
            return byteString;
        }

        private void PrintEndBlocks()
        {
            string headerText = "End blocks";
            OutputWriteLine($"{headerText}");
            OutputWriteLine(new string('-', headerText.Length));

            VcsProgramType vcsFiletype = shaderFile.VcsProgramType;
            if (vcsFiletype == VcsProgramType.VertexShader || vcsFiletype == VcsProgramType.GeometryShader ||
                vcsFiletype == VcsProgramType.ComputeShader || vcsFiletype == VcsProgramType.DomainShader ||
                vcsFiletype == VcsProgramType.HullShader)
            {
                OutputWriteLine($"{zframeFile.VsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.VsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (VsEndBlock vsEndBlock in zframeFile.VsEndBlocks)
                {
                    OutputWriteLine($"block-ref         {vsEndBlock.BlockIdRef}");
                    OutputWriteLine($"arg0              {vsEndBlock.Arg0}");
                    OutputWriteLine($"source-ref        {vsEndBlock.SourceRef}");
                    OutputWriteLine($"source-pointer    {vsEndBlock.SourcePointer}");
                    if (vcsFiletype == VcsProgramType.HullShader)
                    {
                        OutputWriteLine($"hs-arg            {vsEndBlock.HullShaderArg}");
                    }
                    OutputWriteLine($"{BytesToString(vsEndBlock.Databytes)}");
                    OutputWriteLine("");
                }
            }
            else
            {
                OutputWriteLine($"{zframeFile.PsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.PsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (PsEndBlock psEndBlock in zframeFile.PsEndBlocks)
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
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[3..27])}");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[27..51])}");
                        OutputWriteLine($"{BytesToString(psEndBlock.Data2[51..75])}");
                    }
                    OutputWriteLine("");
                }
            }
        }

        public void OutputWrite(string text)
        {
            OutputWriter(text);
        }

        public void OutputWriteLine(string text)
        {
            OutputWrite(text + "\n");
        }
    }
}
