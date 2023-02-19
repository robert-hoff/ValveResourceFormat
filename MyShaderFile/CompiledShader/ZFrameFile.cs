using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ValveResourceFormat.ThirdParty;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;

namespace ValveResourceFormat.CompiledShader
{
    public class ZFrameFile : IDisposable
    {
        public ShaderDataReader DataReader { get; private set; }
        public string FilenamePath { get; }
        public VcsProgramType VcsProgramType { get; }
        public VcsPlatformType VcsPlatformType { get; }
        public VcsShaderModelType VcsShaderModelType { get; }
        public long ZframeId { get; }
        public ZDataBlock LeadingData { get; }
        public List<ZFrameParam> ZframeParams { get; } = new();
        public int[] LeadingSummary { get; } = Array.Empty<int>();
        public List<ZDataBlock> DataBlocks { get; } = new();
        public int[] TrailingSummary { get; }
        public byte[] Flags0 { get; }
        public int Flagbyte0 { get; }
        public int GpuSourceCount { get; }
        public int Flagbyte1 { get; }
        public List<GpuSource> GpuSources { get; } = new();
        public List<VsEndBlock> VsEndBlocks { get; } = new();
        public List<PsEndBlock> PsEndBlocks { get; } = new();
        public int NrEndBlocks { get; }
        public int NonZeroDataBlockCount { get; }

        public ZFrameFile(byte[] databytes, string filenamepath, long zframeId, VcsProgramType vcsProgramType,
            VcsPlatformType vcsPlatformType, VcsShaderModelType vcsShaderModelType, bool omitParsing = false, HandleOutputWrite outputWriter = null)
        {
            this.FilenamePath = filenamepath;
            this.VcsProgramType = vcsProgramType;
            this.VcsPlatformType = vcsPlatformType;
            this.VcsShaderModelType = vcsShaderModelType;
            DataReader = new ShaderDataReader(new MemoryStream(databytes), outputWriter);
            this.ZframeId = zframeId;

            // in case of failure; enable omitParsing and use the datareader directly
            // the zframe encoding for Features files has not been determined (only found in v62 files)
            if (omitParsing || vcsProgramType == VcsProgramType.Features)
            {
                return;
            }

            LeadingData = new ZDataBlock(DataReader, -1);
            int paramCount = DataReader.ReadInt16();
            for (int i = 0; i < paramCount; i++)
            {
                ZFrameParam zParam = new(DataReader);
                ZframeParams.Add(zParam);
            }
            // this data is applicable to vertex shaders
            if (this.VcsProgramType == VcsProgramType.VertexShader)
            {
                int summaryLength = DataReader.ReadInt16();
                LeadingSummary = new int[summaryLength];
                for (int i = 0; i < summaryLength; i++)
                {
                    LeadingSummary[i] = DataReader.ReadInt16();
                }
            }
            int dataBlockCount = DataReader.ReadInt16();
            for (int blockId = 0; blockId < dataBlockCount; blockId++)
            {
                ZDataBlock dataBlock = new(DataReader, blockId);
                if (dataBlock.h0 > 0)
                {
                    NonZeroDataBlockCount++;
                }
                DataBlocks.Add(dataBlock);
            }
            int tailSummaryLength = DataReader.ReadInt16();
            TrailingSummary = new int[tailSummaryLength];
            for (int i = 0; i < tailSummaryLength; i++)
            {
                TrailingSummary[i] = DataReader.ReadInt16();
            }
            Flags0 = DataReader.ReadBytes(4);
            Flagbyte0 = DataReader.ReadByte();
            GpuSourceCount = DataReader.ReadInt32();
            Flagbyte1 = DataReader.ReadByte();

            if (vcsPlatformType == VcsPlatformType.PC)
            {
                switch (vcsShaderModelType)
                {
                    case VcsShaderModelType._20:
                    case VcsShaderModelType._2b:
                    case VcsShaderModelType._30:
                    case VcsShaderModelType._31:
                        ReadDxilSources(GpuSourceCount);
                        break;
                    case VcsShaderModelType._40:
                    case VcsShaderModelType._41:
                    case VcsShaderModelType._50:
                    case VcsShaderModelType._60:
                        ReadDxbcSources(GpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported model type {vcsPlatformType} {vcsShaderModelType}");
                }
            }
            else
            {
                switch (vcsPlatformType)
                {
                    case VcsPlatformType.PCGL:
                    case VcsPlatformType.MOBILE_GLES:
                        ReadGlslSources(GpuSourceCount);
                        break;
                    case VcsPlatformType.VULKAN:
                    case VcsPlatformType.ANDROID_VULKAN:
                    case VcsPlatformType.IOS_VULKAN:
                        ReadVulkanSources(GpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported source type {vcsPlatformType}");
                }
            }
            NrEndBlocks = DataReader.ReadInt32();
            for (int i = 0; i < NrEndBlocks; i++)
            {
                if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.GeometryShader ||
                    vcsProgramType == VcsProgramType.ComputeShader || vcsProgramType == VcsProgramType.DomainShader ||
                    vcsProgramType == VcsProgramType.HullShader)
                {
                    VsEndBlock vsEndBlock = new(DataReader, hullShader: vcsProgramType == VcsProgramType.HullShader);
                    VsEndBlocks.Add(vsEndBlock);
                }
                else
                {
                    PsEndBlock psEndBlock = new(DataReader);
                    PsEndBlocks.Add(psEndBlock);
                }
            }
            if (DataReader.BaseStream.Position != DataReader.BaseStream.Length)
            {
                throw new ShaderParserException("End of file expected");
            }
        }

        private void ReadGlslSources(int glslSourceCount)
        {
            for (int sourceId = 0; sourceId < glslSourceCount; sourceId++)
            {
                GlslSource glslSource = new(DataReader, sourceId);
                GpuSources.Add(glslSource);
            }
        }
        private void ReadDxilSources(int dxilSourceCount)
        {
            for (int sourceId = 0; sourceId < dxilSourceCount; sourceId++)
            {
                DxilSource dxilSource = new(DataReader, sourceId);
                GpuSources.Add(dxilSource);
            }
        }
        private void ReadDxbcSources(int dxbcSourceCount)
        {
            for (int sourceId = 0; sourceId < dxbcSourceCount; sourceId++)
            {
                DxbcSource dxbcSource = new(DataReader, sourceId);
                GpuSources.Add(dxbcSource);
            }
        }
        private void ReadVulkanSources(int vulkanSourceCount)
        {
            for (int sourceId = 0; sourceId < vulkanSourceCount; sourceId++)
            {
                VulkanSource vulkanSource = new(DataReader, sourceId);
                GpuSources.Add(vulkanSource);
            }
        }

        public ZDataBlock GetDataBlock(int blockId)
        {
            return blockId == -1 ? LeadingData : DataBlocks[blockId];
        }

        public string ZFrameHeaderStringDescription()
        {
            string zframeHeaderString = "";
            foreach (ZFrameParam zParam in ZframeParams)
            {
                zframeHeaderString += $"{zParam}\n";
            }
            return zframeHeaderString;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && DataReader != null)
            {
                DataReader.Dispose();
                DataReader = null;
            }
        }

        /*
         * Prints the GPU source as text (GLSL) or bytes (DXIL, DXBC, Vulkan)
         *
         * Method accepts a HandleOutputWrite to redirect output, this needed by the GUI
         * when opening data in different window tabs.
         *
         */
        public void PrintGpuSource(int sourceId, HandleOutputWrite outputWriter = null)
        {
            outputWriter ??= (x) => { Console.Write(x); };
            if (GpuSources[sourceId].sourcebytes.Length == 0)
            {
                outputWriter($"{GpuSources[sourceId].GetSourceDetails()}\n");
                outputWriter("[empty source]");
                return;
            }
            if (GpuSources[sourceId] is GlslSource)
            {
                string glslSourceFile = Encoding.UTF8.GetString(GpuSources[sourceId].sourcebytes);
                outputWriter(glslSourceFile);
            }
            else if (GpuSources[sourceId] is VulkanSource)
            {
                VulkanSource vulkanSource = (VulkanSource)GpuSources[sourceId];
                outputWriter($"{vulkanSource.GetSourceDetails()}\n");
                outputWriter($"// Spirv bytecode ({vulkanSource.metadataOffset})\n");
                outputWriter($"[0]\n");
                outputWriter($"{BytesToString(vulkanSource.GetSpirvBytes())}\n\n");
                outputWriter($"// Source metadata (unknown encoding) ({vulkanSource.metadataLength})\n");
                outputWriter($"[{vulkanSource.metadataOffset}]\n");
                outputWriter($"{BytesToString(vulkanSource.GetMetadataBytes())}");
            }
            else
            {
                outputWriter($"{GpuSources[sourceId].GetSourceDetails()}\n");
                outputWriter($"{BytesToString(GpuSources[sourceId].sourcebytes)}");
            }
        }

        public class ZFrameParam
        {
            public string Name0 { get; }
            public uint Murmur32 { get; }
            public byte HeaderOperator { get; }
            public byte[] HeaderCode { get; }
            public int DynExpLen { get; } = -1;
            public byte[] DynExpression { get; }
            public string DynExpEvaluated { get; }
            public bool HasOperatorVal { get; }
            public int OperatorVal { get; }

            public ZFrameParam(ShaderDataReader datareader)
            {
                Name0 = datareader.ReadNullTermString();
                Murmur32 = datareader.ReadUInt32();
                uint murmurCheck = MurmurHash2.Hash(Name0.ToLower(), ShaderFile.PI_MURMURSEED);
                if (Murmur32 != murmurCheck)
                {
                    throw new ShaderParserException("Murmur check failed on header name");
                }
                HeaderCode = datareader.ReadBytes(3);
                HeaderOperator = HeaderCode[0];
                if (HeaderOperator == 0x0e)
                {
                    return;
                }
                DynExpLen = datareader.ReadInt32();
                if (DynExpLen > 0)
                {
                    DynExpression = datareader.ReadBytes(DynExpLen);
                    DynExpEvaluated = ParseDynamicExpression(DynExpression);
                }
                else if (HeaderOperator == 1 || HeaderOperator == 5)
                {
                    OperatorVal = datareader.ReadInt32();
                    HasOperatorVal = true;
                }
                else if (HeaderOperator == 9)
                {
                    OperatorVal = datareader.ReadByte();
                    HasOperatorVal = true;
                }
                else
                {
                    throw new ShaderParserException($"Unknown header operator {HeaderOperator}");
                }
            }

            public override string ToString()
            {
                if (DynExpLen > 0)
                {
                    return $"{Name0,-40} 0x{Murmur32:x08}     {BytesToString(HeaderCode)}   {DynExpEvaluated}";
                }
                else
                {
                    var toByteString = (byte[] b) => $"{b[0]:x02} {b[1]:x02} {b[2]:x02} {b[3]:x02}";
                    string operatorDesc = HasOperatorVal ? OperatorVal switch
                    {
                        > 0x01000000 => $"{toByteString(BitConverter.GetBytes(OperatorVal))} (unusual data, note 0x40 = '@')",
                        _ => $"{OperatorVal}"
                    } : "";
                    return $"{Name0,-40} 0x{Murmur32:x08}     {BytesToString(HeaderCode)}   {operatorDesc}";
                }
            }
        }

        public class VsEndBlock
        {
            public byte[] Databytes { get; }
            public int BlockIdRef { get; }
            public int Arg0 { get; }
            public int SourceRef { get; }
            public int SourcePointer { get; }
            public int HullShaderArg { get; } = -1;
            public VsEndBlock(ShaderDataReader datareader, bool hullShader = false)
            {
                Databytes = datareader.ReadBytesAtPosition(0, 16 + (hullShader ? 1 : 0));
                BlockIdRef = datareader.ReadInt32();
                Arg0 = datareader.ReadInt32();
                SourceRef = datareader.ReadInt32();
                SourcePointer = datareader.ReadInt32();
                if (hullShader)
                {
                    HullShaderArg = datareader.ReadByte();
                }
            }
        }

        public class PsEndBlock
        {
            public int BlockIdRef { get; }
            public int Arg0 { get; }
            public int SourceRef { get; }
            public int SourcePointer { get; }
            public bool HasData0 { get; }
            public bool HasData1 { get; }
            public bool HasData2 { get; }
            public byte[] Data0 { get; }
            public byte[] Data1 { get; }
            public byte[] Data2 { get; }
            public PsEndBlock(ShaderDataReader datareader)
            {
                BlockIdRef = datareader.ReadInt32();
                Arg0 = datareader.ReadInt32();
                SourceRef = datareader.ReadInt32();
                SourcePointer = datareader.ReadInt32();
                int flag0 = datareader.ReadByte();
                int flag1 = datareader.ReadByte();
                int flag2 = datareader.ReadByte();

                if (flag0 != 0 && flag0 != 1 || flag1 != 0 && flag1 != 1 || flag2 != 0 && flag2 != 1)
                {
                    throw new ShaderParserException("unexpected data");
                }
                HasData0 = flag0 == 0;
                HasData1 = flag1 == 0;
                HasData2 = flag2 == 0;

                if (HasData0)
                {
                    Data0 = datareader.ReadBytes(16);
                }
                if (HasData1)
                {
                    Data1 = datareader.ReadBytes(20);
                }
                if (HasData2)
                {
                    Data2 = datareader.ReadBytes(75);
                }
            }
        }

        public void PrintByteDetail(HandleOutputWrite outputWriter = null)
        {
            if (outputWriter != null)
            {
                DataReader.outputWriter = outputWriter;
            }
            DataReader.BaseStream.Position = 0;
            if (VcsProgramType == VcsProgramType.Features)
            {
                DataReader.Comment("Zframe byte data (encoding for features files has not been determined)");
                DataReader.ShowBytes((int)DataReader.BaseStream.Length);
                return;
            }

            ShowZDataSection(-1);
            ShowZFrameHeader();
            // this applies only to vs files (ps, gs and psrs files don't have this section)
            if (VcsProgramType == VcsProgramType.VertexShader)
            {
                int blockCountInput = DataReader.ReadInt16AtPosition();
                DataReader.ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to configurations (block IDs)");
                DataReader.ShowBytes(2, breakLine: false);
                DataReader.TabComment($"nr of data-blocks ({blockCountInput})");
                DataReader.ShowBytes(blockCountInput * 2);
                DataReader.OutputWriteLine("");
            }
            int blockCount = DataReader.ReadInt16AtPosition();
            DataReader.ShowByteCount("Data blocks");
            DataReader.ShowBytes(2, breakLine: false);
            DataReader.TabComment($"nr of data-blocks ({blockCount})");
            DataReader.OutputWriteLine("");
            for (int i = 0; i < blockCount; i++)
            {
                ShowZDataSection(i);
            }
            DataReader.BreakLine();
            DataReader.ShowByteCount("Unknown additional parameters, non 'FF FF' entries point to active block IDs");
            int blockCountOutput = DataReader.ReadInt16AtPosition();
            DataReader.ShowBytes(2, breakLine: false);
            DataReader.TabComment($"nr of data-blocks ({blockCountOutput})", 1);
            DataReader.ShowBytes(blockCountOutput * 2);
            DataReader.BreakLine();
            DataReader.ShowByteCount();
            byte flagbyte = DataReader.ReadByteAtPosition();
            DataReader.ShowBytes(1, $"possible control byte ({flagbyte}) or flags ({Convert.ToString(flagbyte, 2).PadLeft(8, '0')})");
            DataReader.ShowBytes(1, "values seen (0,1,2)");
            DataReader.ShowBytes(1, "always 0");
            DataReader.ShowBytes(1, "always 0");
            DataReader.ShowBytes(1, "values seen (0,1)");
            DataReader.BreakLine();
            DataReader.ShowByteCount($"Start of source section, {DataReader.BaseStream.Position} is " +
                $"the base offset for end-section source pointers");
            int gpuSourceCount = DataReader.ReadInt32AtPosition();
            DataReader.ShowBytes(4, $"gpu source files ({gpuSourceCount})");
            DataReader.ShowBytes(1, "unknown boolean, values seen 0,1", tabLen: 13);
            DataReader.BreakLine();

            if (VcsPlatformType == VcsPlatformType.PC)
            {
                switch (VcsShaderModelType)
                {
                    case VcsShaderModelType._20:
                    case VcsShaderModelType._2b:
                    case VcsShaderModelType._30:
                    case VcsShaderModelType._31:
                        ShowDxilSources(gpuSourceCount);
                        break;
                    case VcsShaderModelType._40:
                    case VcsShaderModelType._41:
                    case VcsShaderModelType._50:
                    case VcsShaderModelType._60:
                        ShowDxbcSources(gpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported model type {VcsPlatformType} {VcsShaderModelType}");
                }
            }
            else
            {
                switch (VcsPlatformType)
                {
                    case VcsPlatformType.PCGL:
                    case VcsPlatformType.MOBILE_GLES:
                        ShowGlslSources(gpuSourceCount);
                        break;
                    case VcsPlatformType.VULKAN:
                    case VcsPlatformType.ANDROID_VULKAN:
                    case VcsPlatformType.IOS_VULKAN:
                        ShowVulkanSources(gpuSourceCount);
                        break;
                    default:
                        throw new ShaderParserException($"Unknown or unsupported platform type {VcsPlatformType}");
                }
            }

            //  End blocks for vs, gs, cs, ds and hs files
            if (VcsProgramType == VcsProgramType.VertexShader || VcsProgramType == VcsProgramType.GeometryShader ||
                VcsProgramType == VcsProgramType.ComputeShader || VcsProgramType == VcsProgramType.DomainShader ||
                VcsProgramType == VcsProgramType.HullShader)
            {
                ShowZAllEndBlocksTypeVs(hullShader: VcsProgramType == VcsProgramType.HullShader);
                DataReader.BreakLine();
            }
            //  End blocks for ps and psrs files
            if (VcsProgramType == VcsProgramType.PixelShader || VcsProgramType == VcsProgramType.PixelShaderRenderState)
            {
                DataReader.ShowByteCount();
                int nrEndBlocks = DataReader.ReadInt32AtPosition();
                DataReader.ShowBytes(4, breakLine: false);
                DataReader.TabComment($"nr of end blocks ({nrEndBlocks})");
                DataReader.OutputWriteLine("");
                for (int i = 0; i < nrEndBlocks; i++)
                {
                    DataReader.ShowByteCount($"End-block[{i}]");
                    int blockId = DataReader.ReadInt16AtPosition();
                    DataReader.ShowBytes(4, breakLine: false);
                    DataReader.TabComment($"blockId ref ({blockId})");
                    DataReader.ShowBytes(4, breakLine: false);
                    DataReader.TabComment("always 0");
                    int sourceReference = DataReader.ReadInt16AtPosition();
                    DataReader.ShowBytes(4, breakLine: false);
                    DataReader.TabComment($"source ref ({sourceReference})");
                    uint glslPointer = DataReader.ReadUInt32AtPosition();
                    DataReader.ShowBytes(4, breakLine: false);
                    DataReader.TabComment($"glsl source pointer ({glslPointer})");
                    bool hasData0 = DataReader.ReadByteAtPosition(0) == 0;
                    bool hasData1 = DataReader.ReadByteAtPosition(1) == 0;
                    bool hasData2 = DataReader.ReadByteAtPosition(2) == 0;
                    DataReader.ShowBytes(3, breakLine: false);
                    DataReader.TabComment($"(data0={hasData0}, data1={hasData1}, data2={hasData2})", 7);
                    if (hasData0)
                    {
                        DataReader.OutputWriteLine("// data-section 0");
                        DataReader.ShowBytes(16);
                    }
                    if (hasData1)
                    {
                        DataReader.OutputWriteLine("// data-section 1");
                        DataReader.ShowBytes(20);
                    }
                    if (hasData2)
                    {
                        DataReader.OutputWriteLine("// data-section 2");
                        DataReader.ShowBytes(3);
                        DataReader.ShowBytes(8);
                        DataReader.ShowBytes(64, 32);
                    }
                    DataReader.OutputWriteLine("");
                }
            }
            DataReader.ShowEndOfFile();
        }

        private bool prevBlockWasZero;
        public void ShowZDataSection(int blockId)
        {
            int blockSize = ShowZBlockDataHeader(blockId);
            ShowZBlockDataBody(blockSize);
        }
        public int ShowZBlockDataHeader(int blockId)
        {
            int arg0 = DataReader.ReadInt32AtPosition();
            int arg1 = DataReader.ReadInt32AtPosition(4);
            int arg2 = DataReader.ReadInt32AtPosition(8);

            if (blockId != -1 && arg0 == 0 && arg1 == 0 && arg2 == 0)
            {
                DataReader.ShowBytes(12, breakLine: false);
                DataReader.TabComment($"data-block[{blockId}]");
                return 0;
            }
            string comment = "";
            if (blockId == -1)
            {
                comment = $"leading data";
            }
            if (blockId >= 0)
            {
                comment = $"data-block[{blockId}]";
            }
            int blockSize = DataReader.ReadInt32AtPosition();
            if (prevBlockWasZero)
            {
                DataReader.OutputWriteLine("");
            }
            DataReader.ShowByteCount(comment);
            DataReader.ShowBytesWithIntValue();
            DataReader.ShowBytesWithIntValue();
            DataReader.ShowBytesWithIntValue();
            if (blockId == -1 && arg0 == 0 && arg1 == 0 && arg2 == 0)
            {
                DataReader.BreakLine();
            }
            return blockSize * 4;
        }
        public void ShowZBlockDataBody(int byteSize)
        {
            if (byteSize == 0)
            {
                prevBlockWasZero = true;
                return;
            }
            else
            {
                prevBlockWasZero = false;
            }
            DataReader.Comment($"{byteSize / 4}*4 bytes");
            DataReader.ShowBytes(byteSize);
            DataReader.BreakLine();
        }
        public void ShowZFrameHeader()
        {
            DataReader.ShowByteCount("Frame header");
            uint nrArgs = DataReader.ReadUInt16AtPosition();
            DataReader.ShowBytes(2, breakLine: false);
            DataReader.TabComment($"nr of arguments ({nrArgs})");
            DataReader.OutputWriteLine("");

            for (int i = 0; i < nrArgs; i++)
            {
                ShowMurmurString();
                int headerOperator = DataReader.ReadByteAtPosition();
                DataReader.ShowBytes(3, "header-code");
                if (headerOperator == 0x0e)
                {
                    DataReader.BreakLine();
                    continue;
                }
                int dynExpLen = DataReader.ReadInt32AtPosition();
                DataReader.ShowBytes(4, $"dynamic expression length = {dynExpLen}");
                if (dynExpLen > 0)
                {
                    ShowDynamicExpression(dynExpLen);
                    DataReader.BreakLine();
                    continue;
                }
                if (headerOperator == 1)
                {
                    DataReader.ShowBytes(4, "header argument\n");
                }
                if (headerOperator == 5)
                {
                    DataReader.ShowBytes(4, "header argument\n");
                }
                if (headerOperator == 9)
                {
                    DataReader.ShowBytes(1, "header argument\n");
                }
            }
            if (nrArgs > 0)
            {
                DataReader.BreakLine();
            }
        }

        const int SOURCE_BYTES_TO_SHOW = 96;
        private void ShowDxilSources(int dxilSourceCount)
        {
            for (int i = 0; i < dxilSourceCount; i++)
            {
                int sourceOffset = DataReader.ReadInt32AtPosition();
                DataReader.ShowByteCount();
                DataReader.ShowBytes(4, $"offset to end of source {sourceOffset} (taken from {DataReader.BaseStream.Position + 4})");
                int additionalSourceBytes = 0;
                if (sourceOffset > 0)
                {
                    DataReader.ShowBytes(4);
                    int unknown_prog_uint16 = (int)DataReader.ReadUInt16AtPosition(2);
                    DataReader.ShowBytes(4, $"({unknown_prog_uint16}) the first ({unknown_prog_uint16} * 4) " +
                        $"bytes look like header data that may need to be processed");
                    DataReader.BreakLine();
                    DataReader.ShowByteCount($"DXIL-SOURCE[{i}]");
                    int sourceSize = sourceOffset - 8;
                    if (unknown_prog_uint16 > 0)
                    {
                        DataReader.ShowBytes(unknown_prog_uint16 * 4);
                    }
                    additionalSourceBytes = sourceSize - unknown_prog_uint16 * 4;
                }
                int endOfSource = (int)DataReader.BaseStream.Position + additionalSourceBytes;
                if (additionalSourceBytes > SOURCE_BYTES_TO_SHOW)
                {
                    DataReader.ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    DataReader.OutputWrite(" ");
                    int remainingBytes = endOfSource - (int)DataReader.BaseStream.Position;
                    if (remainingBytes < 50)
                    {
                        DataReader.ShowBytes(remainingBytes);
                    }
                    else
                    {
                        DataReader.Comment($"... ({endOfSource - DataReader.BaseStream.Position} bytes of data not shown)");
                    }
                }
                else if (additionalSourceBytes <= SOURCE_BYTES_TO_SHOW && additionalSourceBytes > 0)
                {
                    DataReader.ShowBytes(additionalSourceBytes);
                }
                else
                {
                    DataReader.OutputWriteLine("// no source present");
                }
                DataReader.BaseStream.Position = endOfSource;
                DataReader.BreakLine();
                DataReader.ShowByteCount();
                DataReader.ShowBytes(16, "DXIL(hlsl) Editor ref.");
                DataReader.BreakLine();
            }
        }
        private void ShowDxbcSources(int dxbcSourceCount)
        {
            for (int sourceId = 0; sourceId < dxbcSourceCount; sourceId++)
            {
                int sourceSize = DataReader.ReadInt32AtPosition();
                DataReader.ShowByteCount();
                DataReader.ShowBytes(4, $"Source size, {sourceSize} bytes");
                DataReader.BreakLine();
                int endOfSource = (int)DataReader.BaseStream.Position + sourceSize;
                DataReader.ShowByteCount($"DXBC-SOURCE[{sourceId}]");
                if (sourceSize == 0)
                {
                    DataReader.OutputWriteLine("// no source present");
                }
                if (sourceSize > SOURCE_BYTES_TO_SHOW)
                {
                    DataReader.ShowBytes(SOURCE_BYTES_TO_SHOW, breakLine: false);
                    DataReader.OutputWrite(" ");
                    DataReader.Comment($"... ({endOfSource - DataReader.BaseStream.Position} bytes of data not shown)");
                }
                else if (sourceSize <= SOURCE_BYTES_TO_SHOW && sourceSize > 0)
                {
                    DataReader.ShowBytes(sourceSize);
                }
                DataReader.BaseStream.Position = endOfSource;
                DataReader.BreakLine();
                DataReader.ShowByteCount();
                DataReader.ShowBytes(16, "DXBC(hlsl) Editor ref.");
                DataReader.BreakLine();
            }
        }

        const int VULKAN_SOURCE_BYTES_TO_SHOW = 192;
        private void ShowVulkanSources(int vulkanSourceCount)
        {
            for (int i = 0; i < vulkanSourceCount; i++)
            {
                int offsetToEditorId = DataReader.ReadInt32AtPosition();
                if (offsetToEditorId == 0)
                {
                    DataReader.ShowBytes(4);
                    DataReader.OutputWriteLine("// no source present");
                    DataReader.BreakLine();
                }
                else
                {
                    DataReader.ShowByteCount();
                    DataReader.ShowBytes(4, $"({offsetToEditorId}) offset to Editor ref. ID ");
                    int endOfSourceOffset = (int)DataReader.BaseStream.Position + offsetToEditorId;
                    int arg0 = DataReader.ReadInt32AtPosition();
                    DataReader.ShowBytes(4, $"({arg0}) values seen for Vulkan sources are (2,3)");
                    int offset2 = DataReader.ReadInt32AtPosition();
                    DataReader.ShowBytes(4, $"({offset2}) - looks like an offset, unknown significance");
                    DataReader.BreakLine();
                    DataReader.ShowByteCount($"VULKAN-SOURCE[{i}]");
                    int sourceSize = offsetToEditorId - 8;
                    int bytesToShow = VULKAN_SOURCE_BYTES_TO_SHOW > sourceSize ? sourceSize : VULKAN_SOURCE_BYTES_TO_SHOW;
                    DataReader.ShowBytes(bytesToShow);
                    int bytesNotShown = sourceSize - bytesToShow;
                    if (bytesNotShown > 0)
                    {
                        DataReader.Comment($"... {bytesNotShown} bytes of data not shown)");
                    }
                    DataReader.BreakLine();
                    DataReader.BaseStream.Position = endOfSourceOffset;
                }
                DataReader.ShowByteCount();
                DataReader.ShowBytes(16, "Vulkan Editor ref. ID");
                DataReader.BreakLine();
            }
        }

        private void ShowGlslSources(int glslSourceCount)
        {
            for (int sourceId = 0; sourceId < glslSourceCount; sourceId++)
            {
                int sourceSize = ShowGlslSourceOffsets();
                int sourceOffset = (int)DataReader.BaseStream.Position;
                ShowZGlslSourceSummary(sourceId);
                DataReader.ShowByteCount();
                byte[] fileIdBytes = DataReader.ReadBytes(16);
                string fileIdStr = BytesToString(fileIdBytes);
                DataReader.OutputWrite(fileIdStr);
                DataReader.TabComment($" Editor ref.");
                DataReader.BreakLine();
            }
        }
        public int ShowGlslSourceOffsets()
        {
            DataReader.ShowByteCount("glsl source offsets");
            uint offset1 = DataReader.ReadUInt32AtPosition();
            DataReader.ShowBytesWithIntValue();
            if (offset1 == 0)
            {
                return 0;
            }
            DataReader.ShowBytes(4, breakLine: false);
            DataReader.TabComment("always 3");
            int sourceSize = DataReader.ReadInt32AtPosition() - 1; // one less because of null-term
            DataReader.ShowBytesWithIntValue();
            DataReader.BreakLine();
            return sourceSize;
        }
        public void ShowZGlslSourceSummary(int sourceId)
        {
            int bytesToRead = DataReader.ReadInt32AtPosition(-4);
            int endOfSource = (int)DataReader.BaseStream.Position + bytesToRead;
            DataReader.ShowByteCount($"GLSL-SOURCE[{sourceId}]");
            if (bytesToRead == 0)
            {
                DataReader.OutputWriteLine("// no source present");
            }
            if (bytesToRead > SOURCE_BYTES_TO_SHOW)
            {
                DataReader.ShowBytes(SOURCE_BYTES_TO_SHOW);
                DataReader.Comment($"... ({endOfSource - DataReader.BaseStream.Position} bytes of data not shown)");
            }
            else if (bytesToRead <= SOURCE_BYTES_TO_SHOW && bytesToRead > 0)
            {
                DataReader.ShowBytes(bytesToRead);
            }
            DataReader.BaseStream.Position = endOfSource;
            DataReader.BreakLine();
        }
        public void ShowZAllEndBlocksTypeVs(bool hullShader = false)
        {
            DataReader.ShowByteCount();
            int nr_end_blocks = DataReader.ReadInt32AtPosition();
            DataReader.ShowBytes(4, breakLine: false);
            DataReader.TabComment($"nr end blocks ({nr_end_blocks})");
            DataReader.BreakLine();
            for (int i = 0; i < nr_end_blocks; i++)
            {
                DataReader.ShowBytes(16 + (hullShader ? 1 : 0));
            }
        }
        private void ShowMurmurString()
        {
            string nulltermstr = DataReader.ReadNullTermStringAtPosition();
            uint murmur32 = DataReader.ReadUInt32AtPosition(nulltermstr.Length + 1);
            uint murmurCheck = MurmurHash2.Hash(nulltermstr.ToLower(), ShaderFile.PI_MURMURSEED);
            if (murmur32 != murmurCheck)
            {
                throw new ShaderParserException("not a murmur string!");
            }
            DataReader.Comment($"{nulltermstr} | 0x{murmur32:x08}");
            DataReader.ShowBytes(nulltermstr.Length + 1 + 4);
        }
        private void ShowDynamicExpression(int dynExpLen)
        {
            byte[] dynExpDatabytes = DataReader.ReadBytesAtPosition(0, dynExpLen);
            string dynExp = ParseDynamicExpression(dynExpDatabytes);
            DataReader.OutputWriteLine($"// {dynExp}");
            DataReader.ShowBytes(dynExpLen);
        }
    }

}
