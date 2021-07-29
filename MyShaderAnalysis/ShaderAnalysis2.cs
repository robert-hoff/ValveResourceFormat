using MyShaderAnalysis.readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis {
    class ShaderAnalysis2 {

        const string ANALYSIS_DIR1 = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string ANALYSIS_DIR2 = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";

        const string GLOW_OUPUT_PCGL_30_FEATURES = ANALYSIS_DIR1 + @"\glow_output_pcgl_30_features.vcs";
        const string GLOW_OUPUT_PCGL_30_PS = ANALYSIS_DIR1 + @"\glow_output_pcgl_30_ps.vcs";
        const string GLOW_OUPUT_PCGL_30_VS = ANALYSIS_DIR1 + @"\glow_output_pcgl_30_vs.vcs";
        const string GLOW_OUPUT_PCGL_40_PS = ANALYSIS_DIR1 + @"\glow_output_pcgl_40_ps.vcs";
        const string GLOW_OUPUT_PCGL_40_VS = ANALYSIS_DIR1 + @"\glow_output_pcgl_40_vs.vcs";
        const string MULTIBLEND_PCGL_30_PS = ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs";
        const string MULTIBLEND_PCGL_30_VS = ANALYSIS_DIR1 + @"\multiblend_pcgl_30_vs.vcs";
        const string REFRACT_PCGL_30_FEATURES = ANALYSIS_DIR1 + @"\refract_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_FEATURES = ANALYSIS_DIR1 + @"\ui_twotexture_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_PS = ANALYSIS_DIR1 + @"\ui_twotexture_pcgl_30_ps.vcs";
        const string UI_TWOTEXTURE_PCGL_30_VS = ANALYSIS_DIR1 + @"\ui_twotexture_pcgl_30_vs.vcs";


        public static void RunTrials() {
            // Trial3();
            // Trial2();
            // Trial1ScanHeaderData();
            Trial1ZFrame05();
            // Trial1ZFrame02();
            // Trial1ZFrame01();
            // Trial1ZFrame00();
        }


        static void Trial4() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < 3335; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < 3335; i++) {
                // readers[i].ShowBytesAtPositionNoLineBreak(0, 200);
                // string bytebte = readers[i].ReadBytesAsStringAtPosition(0, 200);
                // Debug.WriteLine("");
                // sw.WriteLine(bytebte);

                int int0 = readers[i].ReadIntAtPosition(0);
                int int1 = readers[i].ReadIntAtPosition(4);
                int int2 = readers[i].ReadIntAtPosition(8);
                Debug.WriteLine($"{int0,3} {int1,3} {int2,3} ");


            }


            sw.Flush();
            sw.Close();

        }



        static void Trial3() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");
            List<SortedRow> entries = new();

            int NUMROWS = 3335;


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < NUMROWS; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < NUMROWS; i++) {
                string bytebyte = readers[i].ReadBytesAsStringAtPosition(0, 2000);
                int v1 = readers[i].ReadIntAtPosition(0);
                int v2 = readers[i].ReadIntAtPosition(4);
                int v3 = readers[i].ReadIntAtPosition(8);
                entries.Add(new SortedRow(v1, v2, v3, bytebyte));
            }

            entries.Sort();
            for (int i = 0; i < NUMROWS; i++) {
                sw.WriteLine(entries[i].bytebyte);
            }


            sw.Flush();
            sw.Close();
        }



        class SortedRow : IComparable<SortedRow> {

            public int v1;
            public int v2;
            public int v3;
            public string bytebyte;

            public SortedRow(int v1, int v2, int v3, string bytebyte) {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
                this.bytebyte = bytebyte;


                //Debug.WriteLine($"{v1}  {v2}  {v3}");

            }

            public int CompareTo(SortedRow other) {
                if (v2 != other.v2) {
                    return v2 - other.v2;
                }
                if (v1 != other.v1) {
                    return v1 - other.v1;
                }
                return v3 - other.v3;
            }
        }




        static void Trial2() {
            StreamWriter sw = new StreamWriter(@"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump\!OUTPUT.txt");
            List<string> entries = new();


            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            List<DataReader> readers = new();
            for (int i = 0; i < 3335; i++) {
                readers.Add(shaderReader.getZframeDataReader(i));
            }
            for (int i = 0; i < 3335; i++) {
                string bytebyte = readers[i].ReadBytesAsStringAtPosition(0, 400);
                entries.Add(bytebyte);
            }

            entries.Sort();
            for (int i = 0; i < 3335; i++) {
                sw.WriteLine(entries[i]);
            }


            sw.Flush();
            sw.Close();
        }





        static void Trial1ScanHeaderData() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            for (int i = 0; i < 3335; i++) {
                doScan(shaderReader, i);
            }


            // doScan(shaderReader, 3);

        }



        static void doScan(ShaderReader shaderReader, int zframeID) {


            DataReader datareader = shaderReader.getZframeDataReader(zframeID);

            datareader.DisableOutput = true;


            datareader.ShowZDataSection(-1);
            datareader.ShowByteCount("ZFrame header");
            datareader.ShowBytes(2);
            int nr_arguments = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            //if (nr_arguments != 6) {
            //    Debug.WriteLine($"{nr_arguments}    {zframeID}");
            //}

            datareader.ShowMurmurString();
            datareader.ShowBytes(3);
            datareader.ShowMurmurString();
            datareader.ShowBytes(8);
            datareader.ShowMurmurString();
            int dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);

            if (nr_arguments == 6) {
                datareader.ShowMurmurString();
                // datareader.ShowZDataDelim();
                datareader.ShowBytes(11);
            }
            // Debug.WriteLine("");


            datareader.ShowBytesNoLineBreak(2);
            int nr_of_blocks = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);

            // Debug.WriteLine(nr_of_blocks);
            if (nr_of_blocks != 64) {
                Debug.WriteLine($"{nr_of_blocks}    {zframeID}");
            }



        }



        static void Trial1ZFrame05() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(5);
            // datareader.DisableOutput = true;

            datareader.ShowZDataSection(-1);
            datareader.ShowByteCount("ZFrame header");
            datareader.ShowBytes(2);
            int nr_arguments = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);


            datareader.ShowMurmurString();
            datareader.ShowBytes(3);
            datareader.ShowMurmurString();
            datareader.ShowBytes(8);
            datareader.ShowMurmurString();
            int dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);
            datareader.ShowMurmurString();
            dynExpLen = datareader.ReadIntAtPosition(datareader.offset + 3);
            datareader.ShowBytes(7);
            datareader.ShowDynamicExpression(dynExpLen);

            if (nr_arguments == 6) {
                datareader.ShowMurmurString();
                // datareader.ShowZDataDelim();
                datareader.ShowBytes(11);
            }
            // Debug.WriteLine("");


            datareader.ShowBytesNoLineBreak(2);
            int nr_of_blocks = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            datareader.TabPrintComment($"nr of blocks ({nr_of_blocks})");

            for (int i = 0; i < nr_of_blocks; i++) {
                datareader.ShowZDataSection(i);
            }


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            int blockEntryCount = (int)datareader.ReadUInt16AtPosition(datareader.offset - 2);
            int blocksRegistered = datareader.ShowBlocksRegistered(blockEntryCount);
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");


            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            Debug.WriteLine("");




            for (int i = 0; i < blocksRegistered; i++) {
                datareader.ShowZSourceOffsets();
                datareader.ShowZGlslSourceSummary(i);
                datareader.ShowByteCount();
                datareader.ShowBytesNoLineBreak(16);
                datareader.TabPrintComment($"File ID");
                Debug.WriteLine("");
            }


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            int nr_end_blocks = datareader.ReadIntAtPosition(datareader.offset - 4);
            datareader.TabPrintComment($"nr end blocks ({nr_end_blocks})");
            Debug.WriteLine("");


            for (int i = 0; i < nr_end_blocks; i++) {
                datareader.ShowZEndBlock(i);
            }

            datareader.EndOfFile();


            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            //datareader.ShowBytesAtPosition(datareader.offset, 1000);
            //Debug.WriteLine("");





        }



        static void Trial1ZFrame02() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(2);
            datareader.ShowZDataSection(-1);
            datareader.ShowZFrameHeader();
            datareader.ShowZDataDelim();
            for (int i = 0; i < 64; i++) {
                datareader.ShowZDataSection(i);
            }


            // this section may be showing what blocks are described in this file
            Debug.WriteLine("");
            datareader.ShowByteCount();
            int datasize = (int)datareader.ReadUInt16AtPosition(datareader.offset);
            datareader.ShowBytes(2);
            datareader.ShowBytes(datasize * 2);
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");



            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);



            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(1);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");



            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"nr end blocks ({datareader.ReadIntAtPosition(datareader.offset - 4)})");

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            datareader.EndOfFile();


        }







        static void Trial1ZFrame01() {
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");
            DataReader datareader = shaderReader.getZframeDataReader(1);


            datareader.ShowZDataSection(-1);
            datareader.ShowZFrameHeader();
            datareader.ShowZDataDelim();
            for (int i = 0; i < 64; i++) {
                datareader.ShowZDataSection(i);
            }

            // this section may be showing what blocks are described in this file
            Debug.WriteLine("");
            datareader.ShowByteCount();
            uint datasize = datareader.ReadUInt16AtPosition(datareader.offset);
            datareader.ShowBytes(2);

            for (int i = 0; i < datasize; i++) {
                datareader.ShowBytesNoLineBreak(2);
                datareader.TabPrintComment($"{i}");
            }


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(2);
            datareader.TabPrintComment("control, always 1C 02");
            Debug.WriteLine("");


            // int datasize = datareader.ReadIntAtPosition(datareader.offset);
            // datareader.ShowBytes(4);
            // datareader.ShowBytes(datasize * 2);
            // Debug.WriteLine("");




            datareader.ShowByteCount("flags");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount("glsl source offsets");
            datareader.PrintIntWithValue();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment("always 3");
            datareader.PrintIntWithValue();

            int sourceOffset = datareader.offset + datareader.ReadIntAtPosition(datareader.offset - 4);


            Debug.WriteLine("");
            datareader.ShowByteCount("SOURCE[0]");
            datareader.ShowBytes(100);
            Debug.WriteLine($"// ... ({sourceOffset - datareader.offset} bytes of data not shown)");
            datareader.offset = sourceOffset;
            Debug.WriteLine("");


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(1);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(2);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");


            datareader.ShowZSourceOffsets();
            datareader.ShowZGlslSourceSummary(3);
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(16);
            datareader.TabPrintComment($"File ID");
            Debug.WriteLine("");

            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"nr end blocks ({datareader.ReadIntAtPosition(datareader.offset - 4)})");

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.ShowBytes(12, 4);
            datareader.ShowBytes(80, 16);



            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 1000);
            Debug.WriteLine("");




        }





        // ZFRAME0
        static void Trial1ZFrame00() {
            // new ShaderReader(ANALYSIS_DIR1 + @"\glow_output_pcgl_30_ps.vcs");
            ShaderReader shaderReader = new ShaderReader(ANALYSIS_DIR1 + @"\multiblend_pcgl_30_ps.vcs");

            DataReader datareader = shaderReader.getZframeDataReader(0);
            datareader.ShowByteCount();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            Debug.WriteLine("");


            // uint count = datareader.ReadUIntAtPosition(datareader.offset);
            datareader.ShowByteCount();
            for (int i = 0; i < 8; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 2; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 4; i++) {
                datareader.ShowBytes(4);
            }

            Debug.WriteLine("");
            datareader.ShowByteCount();
            for (int i = 0; i < 4; i++) {
                datareader.ShowBytes(4);
            }


            Debug.WriteLine("");
            datareader.ShowByteCount(); // [88] 06 00 pattern
            datareader.ShowBytes(2);

            Debug.WriteLine("");
            datareader.ShowMurmurString();


            uint someval = datareader.ReadUInt16AtPosition(datareader.offset);
            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(3);
            datareader.TabPrintComment($"{someval} (?)    (14,74)"); // [116] 0E 4A 00

            Debug.WriteLine("");
            datareader.ShowMurmurString(); // SupportsMappingDimensions

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3); // [149] 09 FF FF
            datareader.ShowBytes(4);
            datareader.ShowBytes(1);

            Debug.WriteLine("");
            datareader.ShowMurmurString(); // DoNotReflect

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3);

            int dynExpLen0 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen0}");

            string dynExpStr = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen0);
            string dynExp0 = getDynamicExpression(dynExpStr);
            Debug.WriteLine($"// {dynExp0}");
            datareader.ShowBytes(dynExpLen0);


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowMurmurString(); // DoNotCastShadows
            Debug.WriteLine("");


            datareader.ShowByteCount();
            datareader.ShowBytesNoLineBreak(3); // 09 FF FF control pattern?
            Debug.WriteLine("");


            int dynExpLen1 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen1}");
            string dynExpStr1 = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen1);
            string dynExp1 = getDynamicExpression(dynExpStr1);
            Debug.WriteLine($"// {dynExp1}");
            datareader.ShowBytes(dynExpLen1);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowMurmurString(); // ShadowsOnly

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(3); // [267] 09 FF FF


            int dynExpLen2 = datareader.ReadIntAtPosition(datareader.offset);
            datareader.ShowBytesNoLineBreak(4);
            datareader.TabPrintComment($"dyn-exp len = {dynExpLen2}");
            string dynExpStr2 = datareader.ReadBytesAsStringAtPosition(datareader.offset, dynExpLen2);
            string dynExp2 = getDynamicExpression(dynExpStr2);
            Debug.WriteLine($"// {dynExp2}");
            datareader.ShowBytes(dynExpLen2);


            Debug.WriteLine("");
            datareader.ShowMurmurString(); // DepthPassBatchID


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(8); // [316] 05 FF FF 00 00 00 00 01
            datareader.ShowBytes(5);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            // datareader.ShowBytes(12, 4);
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(18 * 4, 4);


            Debug.WriteLine("");
            datareader.ShowByteCount(); // 756 null bytes follow
            datareader.ShowBytes(756);


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(128);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(2);
            datareader.PrintUInt16WithValue();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);

            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();
            datareader.PrintIntWithValue();

            Debug.WriteLine("");
            datareader.ShowByteCount();
            Debug.WriteLine("// source file");
            datareader.ShowBytes(500);
            Debug.WriteLine("// ...");

            datareader.offset = 9361;


            Debug.WriteLine("");
            datareader.ShowByteCount();
            datareader.ShowBytes(16);


            Debug.WriteLine("");
            datareader.ShowByteCount();


            datareader.ShowBytes(2);


            Debug.WriteLine("");
            datareader.ShowBytes(96, 8);



            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");
            datareader.ShowByteCount("----------------------------------------------------------------------------------------");
            datareader.ShowBytesAtPosition(datareader.offset, 10000);
            Debug.WriteLine("");



        }


        static ParseDynamicExpressionShader myDynParser = null;


        static string getDynamicExpression(string bytesAsString) {
            byte[] databytes = ParseString(bytesAsString);
            if (myDynParser == null) {
                myDynParser = new ParseDynamicExpressionShader();
            }
            myDynParser.ParseExpression(databytes);
            if (myDynParser.errorWhileParsing) {
                // throw new ShaderParserException("error in dynamic expression!");

                Debug.WriteLine(myDynParser.errorMessage);

            }
            return myDynParser.dynamicExpressionResult;
        }




        static byte[] ParseString(String bytestring) {
            string[] tokens = bytestring.Trim().Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }




    }


}