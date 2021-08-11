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


        public ZFrameFile(byte[] databytes, string filenamepath, long zframeId) {
            this.filenamepath = filenamepath;
            vcsFiletype = GetVcsFileType(filenamepath);
            datareader = new DataReader(databytes);
            this.zframeId = zframeId;
            leadingData = new ZDataBlock(datareader, datareader.offset);

            zframeParams = new();
            int paramCount = datareader.ReadInt16();
            for (int i = 0; i < paramCount; i++) {
                ZFrameParam zParam = new(datareader);
                zframeParams.Add(zParam);
            }

        }



        public void ShowZFrameHeader() {
            foreach (ZFrameParam zParam in zframeParams) {
                Debug.WriteLine($"{zParam}");
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






    }
}




