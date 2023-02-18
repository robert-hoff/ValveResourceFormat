using System;
using System.Collections.Generic;
using System.IO;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace ValveResourceFormat.CompiledShader
{
    public class PrintZFrameSummary
    {
        public HandleOutputWrite OutputWriter { get; set; }
        private readonly ShaderFile shaderFile;
        private readonly ZFrameFile zframeFile;
        private readonly bool showRichTextBoxLinks;

        // If OutputWriter is left as null; output will be written to Console.
        // Otherwise output is directed to the passed HandleOutputWrite object (defined by the calling application, for example GUI element or file)
        public PrintZFrameSummary(ShaderFile shaderFile, ZFrameFile zframeFile,
            HandleOutputWrite outputWriter = null, bool showRichTextBoxLinks = false)
        {
            this.shaderFile = shaderFile;
            this.zframeFile = zframeFile;
            OutputWriter = outputWriter ?? ((x) => { Console.Write(x); });

            if (zframeFile.VcsProgramType == VcsProgramType.Features)
            {
                OutputWriteLine("Zframe byte data (encoding for features files has not been determined)");
                zframeFile.DataReader.BaseStream.Position = 0;
                var zframeBytes = zframeFile.DataReader.ReadBytesAsString((int)zframeFile.DataReader.BaseStream.Length);
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
            PrintAttributes();
            var writeSequences = GetWriteSequences();
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
            var configHeader = "Configuration";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            OutputWriteLine("The static configuration this zframe belongs to (zero or more static parameters)\n");
            ConfigMappingSParams configGen = new(shaderFile);
            var configState = configGen.GetConfigState(zframeFile.ZframeId);
            for (var i = 0; i < configState.Length; i++)
            {
                OutputWriteLine($"{shaderFile.SfBlocks[i].Name,-30} {configState[i]}");
            }
            if (configState.Length == 0)
            {
                OutputWriteLine("[no static params]");
            }
            OutputWriteLine("");
            OutputWriteLine("");
        }

        private void PrintAttributes()
        {
            var headerText = "Attributes";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            OutputWrite(zframeFile.AttributesStringDescription());
            if (zframeFile.Attributes.Count == 0)
            {
                OutputWriteLine("[no attributes]");
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
            var seqCount = 0;
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

            foreach (var zBlock in zframeFile.DataBlocks)
            {
                if (zBlock.Dataload == null)
                {
                    sequencesMap.Add(zBlock.BlockId, -1);
                    continue;
                }
                var dataloadStr = BytesToString(zBlock.Dataload, -1);
                var seq = writeSequences.GetValueOrDefault(dataloadStr, -1);
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
            var headerText = "Parameter write sequences";
            OutputWriteLine(headerText);
            OutputWriteLine(new string('-', headerText.Length));
            OutputWriteLine(
                "This data (thought to be buffer write sequences) appear to be linked to the dynamic (D-param) configurations;\n" +
                "each configuration points to exactly one sequence. WRITESEQ[0] is always defined.");

            if (zframeFile.LeadingData.H0 > 0)
            {
                OutputWriteLine("");
            }

            OutputFormatterTabulatedData tabulatedData = new(OutputWriter);
            tabulatedData.DefineHeaders(new string[] { "segment", "", "dest", "control" });
            tabulatedData.AddTabulatedRow(new string[] { "", "", "", "" });

            tabulatedData.AddTabulatedRow(new string[] { "WRITESEQ[0]", "", "", "" });
            var dataBlock0 = zframeFile.LeadingData;
            PrintParamWriteSequence(shaderFile, dataBlock0.Dataload, dataBlock0.H0, dataBlock0.H1, dataBlock0.H2, tabulatedData);
            tabulatedData.AddTabulatedRow(new string[] { "", "", "", "" });

            var lastSeq = writeSequences[-1];
            foreach (var item in writeSequences)
            {
                if (item.Value > lastSeq)
                {
                    lastSeq = item.Value;
                    var dataBlock = zframeFile.DataBlocks[item.Key];
                    tabulatedData.AddTabulatedRow(new string[] { $"WRITESEQ[{lastSeq}]", "", "", "" });
                    PrintParamWriteSequence(shaderFile, dataBlock.Dataload, dataBlock.H0, dataBlock.H1, dataBlock.H2, tabulatedData);
                    tabulatedData.AddTabulatedRow(new string[] { "", "", "", "" });
                }
            }
            tabulatedData.PrintTabulatedValues(spacing: 2);
            OutputWriteLine("");
        }

        private void PrintParamWriteSequence(ShaderFile shaderFile, byte[] dataload, int h0, int h1, int h2,
            OutputFormatterTabulatedData tabulatedData)
        {
            if (h0 == 0)
            {
                OutputWriteLine("[empty seq.]");
                return;
            }
            PrintParamWriteSequenceSegment(dataload, segStart: 0, segEnd: h1, segId: 0, tabulatedData);
            PrintParamWriteSequenceSegment(dataload, segStart: h1, segEnd: h2, segId: 1, tabulatedData);
            PrintParamWriteSequenceSegment(dataload, segStart: h2, segEnd: h0, segId: 2, tabulatedData);
        }

        private void PrintParamWriteSequenceSegment(byte[] dataload, int segStart, int segEnd, int segId,
            OutputFormatterTabulatedData tabulatedData)
        {
            if (segEnd > segStart)
            {
                for (int i = segStart; i < segEnd; i++)
                {
                    var paramId = dataload[i * 4] + 256 * dataload[i * 4 + 1];
                    var arg0 = dataload[i * 4 + 2];
                    var arg1 = dataload[i * 4 + 3];
                    var segmentDesc = i == segStart ? $"seg_{segId}" : "";
                    var paramDesc = $"{shaderFile.ParamBlocks[paramId].Name}[{paramId}]";
                    var arg0Desc = arg0 == 0xff ? $"{"_",4}" : $"{arg0,4}";
                    var arg1Desc = arg1 == 0xff ? $"{"_",7}" : $"{arg1,7}";
                    tabulatedData.AddTabulatedRow(new string[] { segmentDesc, paramDesc, arg0Desc, arg1Desc });
                }
            }
            else
            {
                tabulatedData.AddTabulatedRow(new string[] { $"seg_{segId}", "[empty]", "", "" });
            }
        }

        private void PrintDataBlocks(SortedDictionary<int, int> writeSequences)
        {
            var blockIdToSource = GetBlockIdToSource(zframeFile);
            var configHeader = $"Dynamic (D-Param) configurations ({blockIdToSource.Count} defined)";
            OutputWriteLine(configHeader);
            OutputWriteLine(new string('-', configHeader.Length));
            OutputWriteLine(
                "Each dynamic parameters has 1 or more defined states. The disabled state (0) is shown as '_'\n" +
                "All permitted configurations are listed with their matching write sequence and GPU source (there is exactly\n" +
                "one of these for each configuration). To save space, the parameter names (original names starting with D_)\n" +
                "are shortened to 3-5 length strings (shown in parenthesis).\n");
            PrintAbbreviations();
            var activeBlockIds = GetActiveBlockIds();
            List<string> dParamNames = new();
            foreach (var dBlock in shaderFile.DBlocks)
            {
                dParamNames.Add(ShortenShaderParam(dBlock.Name).ToLowerInvariant());
            }
            var configNames = CombineStringsSpaceSep(dParamNames.ToArray(), 6);
            configNames = $"{new string(' ', 5)}{configNames}";
            var dBlockCount = 0;
            foreach (var blockId in activeBlockIds)
            {
                if (dBlockCount % 100 == 0)
                {
                    OutputWriteLine($"{configNames}");
                }
                dBlockCount++;
                var dBlockConfig = shaderFile.GetDBlockConfig(blockId);
                var configStr = CombineIntsSpaceSep(dBlockConfig, 6);
                var writeSeqText = $"WRITESEQ[{writeSequences[blockId]}]";
                if (writeSequences[blockId] == -1)
                {
                    writeSeqText = "[empty]";
                }
                OutputWrite($"[{blockId:X02}] {configStr}   {writeSeqText,-12}");
                var blockSource = blockIdToSource[blockId];

                if (showRichTextBoxLinks)
                {
                    OutputWriteLine($"{blockSource.GetBlockName()}[{blockSource.SourceId}] \\\\source\\{blockSource.SourceId}");
                }
                else
                {
                    var sourceDesc = $"{blockSource.GetBlockName()}[{blockSource.GetEditorRefIdAsString()}]";
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
                var abbreviation = $"{dBlock.Name}({ShortenShaderParam(dBlock.Name).ToLowerInvariant()})";
                abbreviations.Add(abbreviation);
            }
            if (abbreviations.Count == 0)
            {
                return;
            }

            var breakabbreviations = CombineValuesBreakString(abbreviations.ToArray(), 120);
            if (breakabbreviations.Length == 1 && breakabbreviations[0].Length == 0)
            {
                return;
            }
            foreach (var abbr in breakabbreviations)
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
                foreach (var vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIds.Add(vsEndBlock.BlockIdRef);
                }
            }
            else
            {
                foreach (var psEndBlock in zframeFile.PsEndBlocks)
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
                foreach (var vsEndBlock in zframeFile.VsEndBlocks)
                {
                    blockIdToSource.Add(vsEndBlock.BlockIdRef, zframeFile.GpuSources[vsEndBlock.SourceRef]);
                }
            }
            else
            {
                foreach (var psEndBlock in zframeFile.PsEndBlocks)
                {
                    blockIdToSource.Add(psEndBlock.BlockIdRef, zframeFile.GpuSources[psEndBlock.SourceRef]);
                }
            }
            return blockIdToSource;
        }


        public static string SummarizeBytes(int[] bytes)
        {
            var summaryDesc = "";
            for (var i = 0; i < bytes.Length; i++)
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
            var headerText = "source bytes/flags";
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
            OutputWriteLine($"{zframeFile.Flagbyte1}       // added with v66");
            OutputWriteLine($"{zframeFile.GpuSourceCount,-6}  // nr of source files");
            OutputWriteLine($"{zframeFile.Flagbyte2}       // values seen 0,1");
            OutputWriteLine("");
            OutputWriteLine("");
        }

        /*
        private static string ByteToBinary(int b0)
        {
            var byteString = "";
            byteString += $"{Convert.ToString(b0 >> 4, 2).PadLeft(4, '0')}";
            byteString += " ";
            byteString += $"{Convert.ToString(b0 & 0xf, 2).PadLeft(4, '0')}";
            return byteString;
        }
        */

        private void PrintEndBlocks()
        {
            var headerText = "End blocks";
            OutputWriteLine($"{headerText}");
            OutputWriteLine(new string('-', headerText.Length));

            var vcsFiletype = shaderFile.VcsProgramType;
            if (vcsFiletype == VcsProgramType.VertexShader || vcsFiletype == VcsProgramType.GeometryShader ||
                vcsFiletype == VcsProgramType.ComputeShader || vcsFiletype == VcsProgramType.DomainShader ||
                vcsFiletype == VcsProgramType.HullShader)
            {
                OutputWriteLine($"{zframeFile.VsEndBlocks.Count:X02} 00 00 00   // end blocks ({zframeFile.VsEndBlocks.Count})");
                OutputWriteLine("");
                foreach (var vsEndBlock in zframeFile.VsEndBlocks)
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
                foreach (var psEndBlock in zframeFile.PsEndBlocks)
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
