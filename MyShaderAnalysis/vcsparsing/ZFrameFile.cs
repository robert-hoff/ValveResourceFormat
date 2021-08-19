using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;

namespace MyShaderAnalysis.vcsparsing
{
    public class ZFrameFile : IDisposable
    {
        private ShaderDataReader datareader;
        public string filenamepath { get; }
        public VcsFileType vcsFileType { get; }
        public VcsSourceType vcsSourceType { get; }
        public long zframeId { get; }
        public ZDataBlock leadingData { get; }
        public List<ZFrameParam> zframeParams { get; }
        public int[] leadSummary { get; } = null;
        public List<ZDataBlock> dataBlocks { get; } = new();
        public int[] tailSummary { get; } = null;
        public byte[] flags0 { get; }
        public int flagbyte0 { get; }
        public int gpuSourceCount { get; }
        public int flagbyte1 { get; }
        // which of these are filled depends on vcsSourceType
        public List<GpuSource> gpuSources { get; } = new();
        public List<VsEndBlock> vsEndBlocks { get; } = new();
        public List<PsEndBlock> psEndBlocks { get; } = new();
        public int nrEndBlocks { get; }
        public int nonZeroDataBlockCount { get; } = 0;

        public ZFrameFile(byte[] databytes, string filenamepath, long zframeId, VcsFileType vcsFileType, VcsSourceType vcsSourceType)
        {
            this.filenamepath = filenamepath;
            this.vcsFileType = vcsFileType;
            this.vcsSourceType = vcsSourceType;
            datareader = new ShaderDataReader(databytes);
            this.zframeId = zframeId;
            leadingData = new ZDataBlock(datareader, datareader.GetOffset(), -1);
            zframeParams = new();
            int paramCount = datareader.ReadInt16();
            for (int i = 0; i < paramCount; i++)
            {
                ZFrameParam zParam = new(datareader);
                zframeParams.Add(zParam);
            }
            if (this.vcsFileType == VcsFileType.VertexShader)
            {
                int summaryLength = datareader.ReadInt16();
                leadSummary = new int[summaryLength];
                for (int i = 0; i < summaryLength; i++)
                {
                    leadSummary[i] = datareader.ReadInt16();
                }
            }
            int dataBlockCount = datareader.ReadInt16();
            for (int blockId = 0; blockId < dataBlockCount; blockId++)
            {
                ZDataBlock dataBlock = new(datareader, datareader.GetOffset(), blockId);
                if (dataBlock.h0 > 0)
                {
                    nonZeroDataBlockCount++;
                }
                dataBlocks.Add(dataBlock);
            }
            int tailSummaryLength = datareader.ReadInt16();
            tailSummary = new int[tailSummaryLength];
            for (int i = 0; i < tailSummaryLength; i++)
            {
                tailSummary[i] = datareader.ReadInt16();
            }
            flags0 = datareader.ReadBytes(4);
            flagbyte0 = datareader.ReadByte();
            gpuSourceCount = datareader.ReadInt();
            flagbyte1 = datareader.ReadByte();
            switch (vcsSourceType)
            {
                case VcsSourceType.Glsl:
                    ReadGlslSources(gpuSourceCount);
                    break;
                case VcsSourceType.DXIL:
                    ReadDxilSources(gpuSourceCount);
                    break;
                case VcsSourceType.DXBC:
                    ReadDxbcSources(gpuSourceCount);
                    break;
            }
            nrEndBlocks = datareader.ReadInt();
            for (int i = 0; i < nrEndBlocks; i++)
            {
                if (this.vcsFileType == VcsFileType.VertexShader || this.vcsFileType == VcsFileType.GeometryShader)
                {
                    VsEndBlock vsEndBlock = new(datareader);
                    vsEndBlocks.Add(vsEndBlock);
                } else
                {
                    PsEndBlock psEndBlock = new(datareader);
                    psEndBlocks.Add(psEndBlock);
                }
            }
            if (!datareader.CheckPositionIsAtEOF())
            {
                throw new ShaderParserException("End of file not reached!");
            }
        }

        private void ReadGlslSources(int glslSourceCount)
        {
            for (int sourceId = 0; sourceId < glslSourceCount; sourceId++)
            {
                GlslSource glslSource = new(datareader, datareader.GetOffset(), sourceId);
                gpuSources.Add(glslSource);
            }
        }
        private void ReadDxilSources(int dxilSourceCount)
        {
            for (int sourceId = 0; sourceId < dxilSourceCount; sourceId++)
            {
                DxilSource dxilSource = new(datareader, datareader.GetOffset(), sourceId);
                gpuSources.Add(dxilSource);
            }
        }
        private void ReadDxbcSources(int dxbcSourceCount)
        {
            for (int sourceId = 0; sourceId < dxbcSourceCount; sourceId++)
            {
                DxbcSource dxbcSource = new(datareader, datareader.GetOffset(), sourceId);
                gpuSources.Add(dxbcSource);
            }
        }
        public ZDataBlock GetDataBlock(int blockId)
        {
            return blockId == -1 ? leadingData : dataBlocks[blockId];
        }

        public void ShowZFrameHeader()
        {
            foreach (ZFrameParam zParam in zframeParams)
            {
                Debug.WriteLine($"{zParam}");
            }
        }

        public string GetZFrameHeaderStringDescription()
        {
            string zframeHeaderString = "";
            foreach (ZFrameParam zParam in zframeParams)
            {
                zframeHeaderString += $"{zParam}\n";
            }
            return zframeHeaderString;
        }

        public void ShowLeadSummary()
        {
            Debug.WriteLine(GetLeadSummary());
            Debug.WriteLine($"");
        }

        public string GetLeadSummary()
        {
            if (vcsFileType != VcsFileType.VertexShader)
            {
                return "only vs files have this section";
            }
            string leadSummaryDesc = $"{leadSummary.Length:X02} 00   // configuration states ({leadSummary.Length}), lead summary\n";
            for (int i = 0; i < leadSummary.Length; i++)
            {
                if (i > 0 && i % 16 == 0)
                {
                    leadSummaryDesc += "\n";
                }
                leadSummaryDesc += leadSummary[i] > -1 ? $"{leadSummary[i],-3}" : "_  ";
            }
            return leadSummaryDesc.Trim();
        }

        public void ShowDatablocks()
        {
            foreach (ZDataBlock dataBlock in dataBlocks)
            {
                Debug.WriteLine($"// data-block[{dataBlock.blockId}]");
                Debug.WriteLine($"{dataBlock.h0},{dataBlock.h1},{dataBlock.h2}");
                if (dataBlock.dataload != null)
                {
                    Debug.WriteLine($"{ShaderDataReader.BytesToString(dataBlock.dataload)}");
                    Debug.WriteLine("");
                }
            }
        }

        public void ShowTailSummary()
        {
            Debug.WriteLine(GetTailSummary());
            Debug.WriteLine($"");
        }

        public string GetTailSummary()
        {
            string tailSummaryDesc = $"{tailSummary.Length:X02} 00   // configuration states ({tailSummary.Length}), tail summary\n";
            for (int i = 0; i < tailSummary.Length; i++)
            {
                if (i > 0 && i % 16 == 0)
                {
                    tailSummaryDesc += "\n";
                }
                tailSummaryDesc += tailSummary[i] > -1 ? $"{tailSummary[i],-10}" : "_  ".PadRight(10);
            }
            return tailSummaryDesc.Trim();
        }

        public void ShowGlslSources()
        {
            foreach (GpuSource gpuSource in gpuSources)
            {
                Debug.WriteLine(gpuSource.GetBlockName());
                if (gpuSource.sourcebytes.Length > 0)
                {
                    Debug.WriteLine($"{gpuSource.sourcebytes.Length}");
                    // Debug.WriteLine($"{DataReader.BytesToString(glslSource.sourcebytes)}");
                } else
                {
                    Debug.WriteLine($"// empty source");
                }
                Debug.WriteLine($"{ShaderDataReader.BytesToString(gpuSource.editorRefId)}  // File ID");
            }
        }

        public void ShowEndBlocks()
        {
            if (vcsFileType == VcsFileType.VertexShader || vcsFileType == VcsFileType.GeometryShader)
            {
                Debug.WriteLine($"{vsEndBlocks.Count:X02} 00 00 00   // nr of end blocks ({vsEndBlocks.Count})");
                foreach (VsEndBlock vsEndBlock in vsEndBlocks)
                {
                    Debug.WriteLine($"{ShaderDataReader.BytesToString(vsEndBlock.databytes)}");
                }
            } else
            {
                Debug.WriteLine($"{psEndBlocks.Count:X02} 00 00 00   // nr of end blocks ({vsEndBlocks.Count})");
                foreach (PsEndBlock psEndBlock in psEndBlocks)
                {
                    Debug.WriteLine($"blockId Ref {psEndBlock.blockIdRef}");
                    Debug.WriteLine($"arg0 {psEndBlock.arg0}");
                    Debug.WriteLine($"source ref {psEndBlock.sourceRef}");
                    Debug.WriteLine($"source pointer {psEndBlock.sourcePointer}");
                    Debug.WriteLine($"has data ({psEndBlock.hasData0},{psEndBlock.hasData1},{psEndBlock.hasData2})");
                    if (psEndBlock.hasData0)
                    {
                        Debug.WriteLine("// data-section 0");
                        Debug.WriteLine($"{ShaderDataReader.BytesToString(psEndBlock.data0)}");
                    }
                    if (psEndBlock.hasData1)
                    {
                        Debug.WriteLine("// data-section 1");
                        Debug.WriteLine($"{ShaderDataReader.BytesToString(psEndBlock.data1)}");
                    }
                    if (psEndBlock.hasData2)
                    {
                        Debug.WriteLine("// data-section 2");
                        Debug.WriteLine($"{ShaderDataReader.BytesToString(psEndBlock.data2[0..3])}");
                        Debug.WriteLine($"{ShaderDataReader.BytesToString(psEndBlock.data2[3..11])}");
                        Debug.WriteLine($"{ShaderDataReader.BytesToString(psEndBlock.data2[11..75])}");
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                datareader.Dispose();
                datareader = null;
            }
        }


        public class ZFrameParam
        {
            public string name0 { get; }
            public uint murmur32 { get; }
            public byte[] code { get; }
            public byte headerOperator { get; }
            public int dynExpLen { get; } = -1;
            public byte[] dynExpression { get; } = null;
            public string dynExpEvaluated { get; } = null;
            public int operatorVal { get; } = int.MinValue;

            public ZFrameParam(ShaderDataReader datareader)
            {
                name0 = datareader.ReadNullTermString();
                murmur32 = datareader.ReadUInt();
                uint murmurCheck = MurmurHashPiSeed(name0.ToLower());
                if (murmur32 != murmurCheck)
                {
                    throw new ShaderParserException("not a murmur string!");
                }
                code = datareader.ReadBytes(3);
                headerOperator = code[0];
                if (headerOperator == 0x0e)
                {
                    return;
                }
                dynExpLen = datareader.ReadInt();
                if (dynExpLen > 0)
                {
                    dynExpression = datareader.ReadBytes(dynExpLen);
                    dynExpEvaluated = GetDynamicExpression(dynExpression);
                    return;
                }
                if (headerOperator == 1 || headerOperator == 9)
                {
                    operatorVal = datareader.ReadByte();
                    return;
                }
                if (headerOperator == 5)
                {
                    operatorVal = datareader.ReadInt(); ;
                    return;
                }
                throw new ShaderParserException("unexpected data!");
            }

            public override string ToString()
            {
                if (dynExpLen > 0)
                {
                    return $"{name0,-40} 0x{murmur32:x08}     {ShaderDataReader.BytesToString(code)}   {dynExpEvaluated}";
                } else
                {
                    return $"{name0,-40} 0x{murmur32:x08}     {ShaderDataReader.BytesToString(code)}   {(operatorVal != int.MinValue ? $"{operatorVal}" : "")}";
                }

            }

            private VfxEval myDynParser = new();
            private string GetDynamicExpression(byte[] dynExpDatabytes)
            {
                if (myDynParser == null)
                {
                    myDynParser = new VfxEval();
                }
                myDynParser.ParseExpression(dynExpDatabytes);
                if (myDynParser.errorWhileParsing)
                {
                    string errorMessage = $"problem occured parsing dynamic expression {myDynParser.errorMessage}";
                    Debug.WriteLine(errorMessage);
                    return errorMessage;
                }
                return myDynParser.dynamicExpressionResult;
            }
        }

        public class VsEndBlock
        {
            public byte[] databytes { get; }
            public int blockIdRef { get; }
            public int arg0 { get; }
            public int sourceRef { get; }
            public int sourcePointer { get; }

            public VsEndBlock(ShaderDataReader datareader)
            {
                databytes = datareader.ReadBytesAtPosition(0, 16);
                blockIdRef = datareader.ReadInt();
                arg0 = datareader.ReadInt();
                sourceRef = datareader.ReadInt();
                sourcePointer = datareader.ReadInt();
            }
        }

        public class PsEndBlock
        {
            public int blockIdRef { get; }
            public int arg0 { get; }
            public int sourceRef { get; }
            public int sourcePointer { get; }
            public bool hasData0 { get; }
            public bool hasData1 { get; }
            public bool hasData2 { get; }
            public byte[] data0 { get; } = null;
            public byte[] data1 { get; } = null;
            public byte[] data2 { get; } = null;

            public PsEndBlock(ShaderDataReader datareader)
            {
                blockIdRef = datareader.ReadInt();
                arg0 = datareader.ReadInt();
                sourceRef = datareader.ReadInt();
                sourcePointer = datareader.ReadInt();
                int flag0 = datareader.ReadByte();
                int flag1 = datareader.ReadByte();
                int flag2 = datareader.ReadByte();

                if (flag0 != 0 && flag0 != 1 || flag1 != 0 && flag1 != 1 || flag2 != 0 && flag2 != 1)
                {
                    throw new ShaderParserException("unexpected data");
                }
                hasData0 = flag0 == 0;
                hasData1 = flag1 == 0;
                hasData2 = flag2 == 0;

                if (hasData0)
                {
                    data0 = datareader.ReadBytes(16);
                }
                if (hasData1)
                {
                    data1 = datareader.ReadBytes(20);
                }
                if (hasData2)
                {
                    data2 = datareader.ReadBytes(75);
                }
            }
        }

    }
}
