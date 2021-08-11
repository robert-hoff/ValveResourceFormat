using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ZstdSharp;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using System.Diagnostics;

namespace MyShaderAnalysis.vcsparsing {


    public class ZFrameFile {
        private DataReader datareader;
        public string filenamepath;
        public FILETYPE vcsFiletype = FILETYPE.unknown;
        public long zframeId;

        public ZDataBlock leadingData;
        public List<ZFrameParam> zframeParams;
        public int[] leadSummary = null;
        public List<ZDataBlock> dataBlocks = new();
        public int[] tailSummary = null;
        public int flags0;
        public int flagbyte0;
        public int glslSourceCount;
        public int flagbyte1;
        public List<GlslSource> glslSources = new();

        public List<VsEndBlock> vsEndBlocks = new();
        public List<PsEndBlock> psEndBlocks = new();
        public int nrEndBlocks;
        public int nonZeroDataBlockCount = 0;




        public ZFrameFile(byte[] databytes, string filenamepath, long zframeId) {
            this.filenamepath = filenamepath;
            vcsFiletype = GetVcsFileType(filenamepath);
            datareader = new DataReader(databytes);
            this.zframeId = zframeId;
            leadingData = new ZDataBlock(datareader, datareader.offset, -1);

            zframeParams = new();
            int paramCount = datareader.ReadInt16();
            for (int i = 0; i < paramCount; i++) {
                ZFrameParam zParam = new(datareader);
                zframeParams.Add(zParam);
            }

            if (vcsFiletype == FILETYPE.vs_file) {
                int summaryLength = datareader.ReadInt16();
                leadSummary = new int[summaryLength];
                for (int i = 0; i < summaryLength; i++) {
                    leadSummary[i] = datareader.ReadInt16();
                }
            }

            int dataBlockCount = datareader.ReadInt16();
            for (int blockId = 0; blockId < dataBlockCount; blockId++) {
                ZDataBlock dataBlock = new(datareader, datareader.offset, blockId);
                if (dataBlock.h0 > 0) {
                    nonZeroDataBlockCount++;
                }
                dataBlocks.Add(dataBlock);
            }

            int tailSummaryLength = datareader.ReadInt16();
            tailSummary = new int[tailSummaryLength];
            for (int i = 0; i < tailSummaryLength; i++) {
                tailSummary[i] = datareader.ReadInt16();
            }

            flags0 = datareader.ReadInt();
            flagbyte0 = datareader.ReadByte();
            glslSourceCount = datareader.ReadInt();
            flagbyte1 = datareader.ReadByte();

            for (int sourceId = 0; sourceId < glslSourceCount; sourceId++) {
                GlslSource glslSource = new GlslSource(datareader, datareader.offset, sourceId);
                glslSources.Add(glslSource);
            }

            nrEndBlocks = datareader.ReadInt();
            for (int i = 0; i < nrEndBlocks; i++) {
                if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.gs_file) {
                    VsEndBlock vsEndBlock = new(datareader);
                    vsEndBlocks.Add(vsEndBlock);
                } else {
                    PsEndBlock psEndBlock = new(datareader);
                    psEndBlocks.Add(psEndBlock);
                }
            }

            if (datareader.offset != datareader.databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }

        }



        public void ShowZFrameHeader() {
            foreach (ZFrameParam zParam in zframeParams) {
                Debug.WriteLine($"{zParam}");
            }
        }

        public string GetZFrameHeaderStringDescription() {
            string zframeHeaderString = "";
            foreach (ZFrameParam zParam in zframeParams) {
                zframeHeaderString += $"{zParam}\n";
            }
            return zframeHeaderString;
        }


        public void ShowLeadSummary() {
            if (vcsFiletype != FILETYPE.vs_file) {
                Debug.WriteLine("only vs files have this section");
                return;
            }
            Debug.WriteLine($"{leadSummary.Length:X02} 00   // nr of datablocks ({leadSummary.Length})");
            for (int i = 0; i < leadSummary.Length; i++) {
                if (i > 0 && i % 16 == 0) {
                    Debug.WriteLine("");
                }
                Debug.Write(leadSummary[i] > -1 ? $"{leadSummary[i],-3}" : "_  ");
            }
            Debug.WriteLine($"");
        }

        public void ShowDatablocks() {
            foreach (ZDataBlock dataBlock in dataBlocks) {
                Debug.WriteLine($"// data-block[{dataBlock.blockId}]");
                Debug.WriteLine($"{dataBlock.h0},{dataBlock.h1},{dataBlock.h2}");
                if (dataBlock.dataload != null) {
                    Debug.WriteLine($"{DataReader.BytesToString(dataBlock.dataload)}");
                    Debug.WriteLine("");
                }
            }
        }

        public void ShowTailSummary() {
            Debug.WriteLine($"{tailSummary.Length:X02} 00   // nr of datablocks ({tailSummary.Length})");
            for (int i = 0; i < tailSummary.Length; i++) {
                if (i > 0 && i % 16 == 0) {
                    Debug.WriteLine("");
                }
                Debug.Write(tailSummary[i] > -1 ? $"{tailSummary[i],-3}" : "_  ");
            }
            Debug.WriteLine($"");
        }

        public void ShowGlslSources() {
            foreach (GlslSource glslSource in glslSources) {
                Debug.WriteLine($"GLSL-SOURCE[{glslSource.sourceId}]");
                if (glslSource.offset0 > 0) {
                    Debug.WriteLine($"{glslSource.offset1}");
                    // Debug.WriteLine($"{DataReader.BytesToString(glslSource.sourcebytes)}");
                } else {
                    Debug.WriteLine($"// empty source");
                }
                Debug.WriteLine($"{DataReader.BytesToString(glslSource.fileId)}  // File ID");
            }
        }

        public void ShowEndBlocks() {
            if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.gs_file) {
                Debug.WriteLine($"{vsEndBlocks.Count:X02} 00 00 00   // nr of end blocks ({vsEndBlocks.Count})");
                foreach (VsEndBlock vsEndBlock in vsEndBlocks) {
                    Debug.WriteLine($"{DataReader.BytesToString(vsEndBlock.databytes)}");
                }
            } else {
                Debug.WriteLine($"{psEndBlocks.Count:X02} 00 00 00   // nr of end blocks ({vsEndBlocks.Count})");
                foreach (PsEndBlock psEndBlock in psEndBlocks) {
                    Debug.WriteLine($"blockId Ref {psEndBlock.blockIdRef}");
                    Debug.WriteLine($"arg0 {psEndBlock.arg0}");
                    Debug.WriteLine($"source ref {psEndBlock.sourceRef}");
                    Debug.WriteLine($"source pointer {psEndBlock.sourcePointer}");
                    Debug.WriteLine($"has data ({psEndBlock.hasData0},{psEndBlock.hasData1},{psEndBlock.hasData2})");
                    if (psEndBlock.hasData0) {
                        Debug.WriteLine("// data-section 0");
                        Debug.WriteLine($"{DataReader.BytesToString(psEndBlock.data0)}");
                    }
                    if (psEndBlock.hasData1) {
                        Debug.WriteLine("// data-section 1");
                        Debug.WriteLine($"{DataReader.BytesToString(psEndBlock.data1)}");
                    }
                    if (psEndBlock.hasData2) {
                        Debug.WriteLine("// data-section 2");
                        Debug.WriteLine($"{DataReader.BytesToString(psEndBlock.data2[0..3])}");
                        Debug.WriteLine($"{DataReader.BytesToString(psEndBlock.data2[3..11])}");
                        Debug.WriteLine($"{DataReader.BytesToString(psEndBlock.data2[11..75])}");
                    }
                }
            }
        }



        public class ZFrameParam {
            public string name0;
            public uint murmur32;
            public byte[] code;
            public byte headerOperator;
            public int dynExpLen = -1;
            public byte[] dynExpression = null;
            public string dynExpEvaluated = null;
            public int operatorVal = -1;

            public ZFrameParam(DataReader datareader) {
                name0 = datareader.ReadNullTermString();
                murmur32 = datareader.ReadUInt();
                uint murmurCheck = MurmurHashPiSeed(name0.ToLower());
                if (murmur32 != murmurCheck) {
                    throw new ShaderParserException("not a murmur string!");
                }
                code = datareader.ReadBytes(3);
                headerOperator = code[0];
                if (headerOperator == 0x0e) {
                    return;
                }
                dynExpLen = datareader.ReadInt();
                if (dynExpLen > 0) {
                    dynExpression = datareader.ReadBytes(dynExpLen);
                    dynExpEvaluated = GetDynamicExpression(dynExpression);
                    return;
                }
                if (headerOperator == 1 || headerOperator == 9) {
                    operatorVal = datareader.ReadByte();
                    return;
                }
                if (headerOperator == 5) {
                    operatorVal = datareader.ReadInt(); ;
                    return;
                }
                throw new ShaderParserException("unexpected data!");
            }


            public override string ToString() {
                if (dynExpLen > 0) {
                    return $"{name0,-40} {murmur32:x08}   {DataReader.BytesToString(code)}   {dynExpEvaluated}";
                } else {
                    return $"{name0,-40} {murmur32:x08}   {DataReader.BytesToString(code)}   {operatorVal}";
                }

            }


            private ParseDynamicExpressionShader myDynParser = new();
            private string GetDynamicExpression(byte[] dynExpDatabytes) {
                if (myDynParser == null) {
                    myDynParser = new ParseDynamicExpressionShader();
                }
                myDynParser.ParseExpression(dynExpDatabytes);
                if (myDynParser.errorWhileParsing) {
                    string errorMessage = $"problem occured parsing dynamic expression {myDynParser.errorMessage}";
                    Debug.WriteLine(errorMessage);
                    return errorMessage;
                }
                return myDynParser.dynamicExpressionResult;
            }
        }



        /*
         * FIXME - needs a bit more work
         *
         */
        public class VsEndBlock {
            public byte[] databytes;
            public VsEndBlock(DataReader datareader) {
                databytes = datareader.ReadBytes(16);
            }
        }


        /*
         *
         * FIXME - needs a bit more work, data2 can be broken down
         *
         */
        public class PsEndBlock {
            public int blockIdRef;
            public int arg0;
            public int sourceRef;
            public int sourcePointer;
            public bool hasData0;
            public bool hasData1;
            public bool hasData2;
            public byte[] data0 = null;
            public byte[] data1 = null;
            public byte[] data2 = null;

            public PsEndBlock(DataReader datareader) {
                blockIdRef = datareader.ReadInt();
                arg0 = datareader.ReadInt();
                sourceRef = datareader.ReadInt();
                sourcePointer = datareader.ReadInt();
                int flag0 = datareader.ReadByte();
                int flag1 = datareader.ReadByte();
                int flag2 = datareader.ReadByte();

                if (flag0 != 0 && flag0 != 1 || flag1 != 0 && flag1 != 1 || flag2 != 0 && flag2 != 1) {
                    throw new ShaderParserException("unexpected data");
                }
                hasData0 = flag0 == 0;
                hasData1 = flag1 == 0;
                hasData2 = flag2 == 0;

                if (hasData0) {
                    data0 = datareader.ReadBytes(16);
                }
                if (hasData1) {
                    data1 = datareader.ReadBytes(20);
                }
                if (hasData2) {
                    data2 = datareader.ReadBytes(75);
                }

            }
        }





    }
}




