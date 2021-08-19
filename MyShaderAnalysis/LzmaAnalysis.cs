using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.vcsparsing;
using System.Collections.Generic;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using MyValveResourceFormat.ThirdParty;
using System;

namespace MyShaderAnalysis {


    public class LzmaAnalysis {



        public static void RunTrials() {
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial4();
            // Trial5();
            // Trial6();
            // Trial7();
            // Trial8();
            // Trial9();
            // Trial10();
            Trial11();
        }


        static void Trial11() {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            ShaderFile shaderFile = new(filenamepath);
            // ZFrameFile zframeFile = shaderFile.GetZFrameFile(zId);


            Debug.WriteLine($"{shaderFile.GetZFrameCount()}");


        }



        static void Trial10() {
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs"; long zId = 0x1a1;
            // string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs"; long zId = 0;
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/bloom_dota_pc_40_vs.vcs"; long zId = 0;
            // string filenamepath = $"{ARTIFACT_CLASSIC_DCG_PC_SOURCE}/visualize_physics_pc_30_ps.vcs"; long zId = 0x10;
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(zId);
            DataReaderZFrameByteAnalysis datareader = new(zframeDatabytes, GetVcsFileType(filenamepath), GetVcsSourceType(filenamepath));
            datareader.PrintByteAnalysis();

        }




        static void Trial9() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            VcsSourceType vcsSourceType = GetVcsSourceType(filenamepath);
            Debug.WriteLine($"{vcsSourceType}");

        }


        static void Trial8() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            shaderByteAnalysis.SetShortenOutput(false);
            shaderByteAnalysis.PrintByteAnalysis();
        }



        static void Trial7() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x1a1);


            DataReaderZFramePcFile datareader = new(zframeDatabytes, VcsFileType.PixelShader);
            datareader.PrintByteAnalysis();


            int blockCountOutput = datareader.ReadInt16AtPosition();
            datareader.ShowBytes(2, breakLine: false);
            datareader.TabComment($"nr of data-blocks ({blockCountOutput})", 1);

            datareader.ShowBytes(blockCountOutput * 2);
            datareader.BreakLine();

            byte flagbyte = datareader.ReadByteAtPosition();
            datareader.ShowBytes(1, $"possible control byte ({flagbyte}) or flags ({Convert.ToString(flagbyte, 2).PadLeft(8, '0')})");
            datareader.ShowBytes(1, "values seen (0,1,2)");
            datareader.ShowBytes(1, "always 0");
            datareader.ShowBytes(1, "always 0");
            datareader.ShowBytes(1, "values seen (0,1)");
            datareader.BreakLine();

            datareader.ShowByteCount($"Start of source section, {datareader.offset} is the base offset for end-section source pointers");
            int hlsl_source_count = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"nr of DXIL sources ({hlsl_source_count})");
            datareader.ShowBytes(1);
            datareader.BreakLine();




            for (int i = 0; i < hlsl_source_count; i++) {
                int sourceOffset = datareader.ReadIntAtPosition();
                datareader.ShowByteCount();
                datareader.ShowBytes(4, $"offset to end of source {sourceOffset} (taken from {datareader.offset + 4})");
                datareader.ShowBytes(4);
                uint unknown_prog_uint16 = datareader.ReadUInt16AtPosition(2);
                uint unknown_prog_byte = datareader.ReadByteAtPosition(2);
                datareader.ShowBytes(4, $"({unknown_prog_uint16}) the first ({unknown_prog_uint16} * 4) bytes look like header data that may need to be processed");
                datareader.ShowByteCount($"DXIL-SOURCE[{i}]");
                int sourceSize = sourceOffset - 8;
                // datareader.ShowBytesAtPosition(0, (int) (unknown_prog_uint16-unknown_prog_byte));
                // datareader.ShowBytesAtPosition((int) (unknown_prog_uint16-unknown_prog_byte), (int) unknown_prog_byte);
                // datareader.ShowBytesAtPosition((int) (unknown_prog_uint16+unknown_prog_byte), 100);
                datareader.ShowBytesAtPosition(0, (int)(unknown_prog_uint16 * 4));
                datareader.ShowBytesAtPosition((int)(unknown_prog_uint16 * 4), 100);
                datareader.ShowByteCount();
                datareader.Comment($"... {sourceSize - 100 - (unknown_prog_uint16 * 4)} bytes of data not shown");
                datareader.offset += sourceSize;
                datareader.BreakLine();
                datareader.ShowByteCount();
                datareader.ShowBytes(16, "DXIL(hlsl) Source-Id");
                datareader.BreakLine();
            }


            int end_block_count = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"nr of end blocks ({end_block_count})");


            for (int i = 0; i < end_block_count; i++) {
                datareader.ShowByteCount($"End-block[{i}]");
                int blockId = datareader.ReadInt16AtPosition();
                datareader.ShowBytes(4, breakLine: false);
                datareader.TabComment($"blockId ref ({blockId})");
                datareader.ShowBytes(4, breakLine: false);
                datareader.TabComment("always 0");
                int sourceReference = datareader.ReadInt16AtPosition();
                datareader.ShowBytes(4, breakLine: false);
                datareader.TabComment($"source ref ({sourceReference})");
                uint glslPointer = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(4, breakLine: false);
                datareader.TabComment($"DXIL(hlsl) source pointer ({glslPointer})");
                datareader.ShowBytes(3, breakLine: false);
                bool hasData0 = datareader.databytes[datareader.offset - 3] == 0;
                bool hasData1 = datareader.databytes[datareader.offset - 2] == 0;
                bool hasData2 = datareader.databytes[datareader.offset - 1] == 0;
                datareader.TabComment($"(data0={hasData0}, data1={hasData1}, data2={hasData2})", 7);
                if (hasData0) {
                    datareader.OutputWriteLine("// data-section 0");
                    datareader.ShowBytes(16);
                }
                if (hasData1) {
                    datareader.OutputWriteLine("// data-section 1");
                    datareader.ShowBytes(20);
                }
                if (hasData2) {
                    datareader.OutputWriteLine("// data-section 2");
                    datareader.ShowBytes(3);
                    datareader.ShowBytes(8);
                    datareader.ShowBytes(64, 32);
                }
                datareader.OutputWriteLine("");
            }


            datareader.BreakLine();
            datareader.EndOfFile();
        }




        static void Trial6() {
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x1a1);


            DataReaderZFramePcFile datareader = new(zframeDatabytes, VcsFileType.PixelShader);
            datareader.PrintByteAnalysis();


            int blockCountOutput = datareader.ReadInt16AtPosition();
            datareader.ShowBytes(2, breakLine: false);
            datareader.TabComment($"nr of data-blocks ({blockCountOutput})", 1);

            datareader.ShowBytes(blockCountOutput * 2);
            datareader.BreakLine();

            byte flagbyte = datareader.ReadByteAtPosition();
            datareader.ShowBytes(1, $"possible control byte ({flagbyte}) or flags ({Convert.ToString(flagbyte, 2)})");
            datareader.ShowBytes(1, "values seen (0,1,2)");
            datareader.ShowBytes(1, "always 0");
            datareader.ShowBytes(1, "always 0");
            datareader.ShowBytes(1, "values seen (0,1)");
            datareader.BreakLine();

            datareader.ShowByteCount($"Start of source section, {datareader.offset} is the base for end-section source pointers");
            int hlsl_source_count = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"nr of DXIL sources ({hlsl_source_count})");
            datareader.ShowBytes(1);
            datareader.BreakLine();

            // datareader.ShowBytes(4);
            // int sourceSize = datareader.ReadIntAtPosition();
            int sourceOffset = datareader.ReadIntAtPosition();
            datareader.ShowByteCount();
            //datareader.ShowBytes(4, $"offset to end of source (File ID) {sourceOffset} " +
            //    $"({datareader.offset+4}+{sourceOffset}={datareader.offset+4+sourceOffset})");
            datareader.ShowBytes(4, $"offset to end of source {sourceOffset} (taken from {datareader.offset + 4})");
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.BreakLine();
            datareader.ShowByteCount("DXIL-SOURCE[0]");
            int sourceSize = sourceOffset - 8;
            // datareader.ShowBytes(sourceOffset-8);
            datareader.ShowBytesAtPosition(0, 100);
            datareader.ShowByteCount();
            datareader.Comment($"... {sourceSize - 100} bytes of data not shown");
            datareader.offset += sourceSize;
            datareader.BreakLine();
            datareader.ShowByteCount();
            datareader.ShowBytes(16, "DXIL(hlsl) Source-Id");

            datareader.BreakLine();




            datareader.ShowBytes(100);


        }



        /*
         *
         * I think I'm breaking this source at the right locations, but it only has 2 hlsl sources
         * which is a bit thin
         *
         *
         *
         */
        static void Trial5() {
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/blur_pc_30_ps.vcs";
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);


            // byte[] zframeDatabytes = shaderFile.GetDecompressedZFrameByIndex(0);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x28);


            DataReaderZFramePcFile datareader = new(zframeDatabytes, VcsFileType.PixelShader);
            datareader.PrintByteAnalysis();


            int blockCountOutput = datareader.ReadInt16AtPosition();
            datareader.ShowBytes(2, breakLine: false);
            datareader.TabComment($"nr of data-blocks ({blockCountOutput})", 1);

            datareader.ShowBytes(blockCountOutput * 2);
            datareader.BreakLine();


            datareader.BreakLine();
            datareader.ShowBytes(4);
            datareader.ShowBytes(1);
            datareader.ShowBytes(4);
            datareader.ShowBytes(1);
            datareader.BreakLine();

            // datareader.ShowBytes(4);
            datareader.PrintIntWithValue();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.BreakLine();



            datareader.ShowBytes(1552);
            datareader.BreakLine();
            datareader.BreakLine();
            datareader.PrintIntWithValue();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.BreakLine();
            datareader.ShowBytes(1540);
            datareader.BreakLine();
            datareader.BreakLine();


            datareader.ShowBytes(4, "nr end-blocks (2)");
            datareader.BreakLine();

            datareader.ShowByteCount();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(3);
            datareader.ShowBytes(16);
            datareader.ShowBytes(3);
            datareader.ShowBytes(8);
            datareader.ShowBytes(32);
            datareader.ShowBytes(32);
            datareader.BreakLine();


            datareader.ShowByteCount();
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(4);
            datareader.ShowBytes(3);
            datareader.ShowBytes(16);
            datareader.ShowBytes(3);
            datareader.ShowBytes(8);
            datareader.ShowBytes(32);
            datareader.ShowBytes(32);
            datareader.BreakLine();
            datareader.BreakLine();

            datareader.EndOfFile();



        }




        /*
         * Using lzma decompression here.
         * It seems to be giving something that looks like a zframe, but the zframebyteanalysis is
         * not managing to read it
         *
         *
         *
         */
        static void Trial4() {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            DataReaderVcsByteAnalysis datareader = new(filenamepath);

            datareader.offset = 9504;


            // it seems the decompression maybe be correct, but reading the frame below isn't working
            byte[] zframeDatabytes = ReadShaderChunk(datareader);


            // DataReaderZFrameByteAnalysis zframeData = new(zframeDatabytes, FILETYPE.ps_file);
            // zframeData.PrintByteAnalysis();
            // zframeData.ShowBytes(500);


            // Debug.WriteLine($"{lzmaId:x}");


        }


        private static byte[] ReadShaderChunk(DataReader datareader) {

            int chunkSize = datareader.ReadInt();
            uint lzmaId = datareader.ReadUInt(); // should equal 0x414D5A4C
            int uncompressedSize = datareader.ReadInt();
            int compressedSize = datareader.ReadInt();
            var decoder = new Decoder();
            decoder.SetDecoderProperties(datareader.ReadBytes(5));
            var compressedBuffer = datareader.ReadBytes(compressedSize);

            using (var inputStream = new MemoryStream(compressedBuffer))
            using (var outStream = new MemoryStream((int)uncompressedSize)) {
                decoder.Code(inputStream, outStream, compressedBuffer.Length, uncompressedSize, null);
                return outStream.ToArray();
            }
        }




        /*
         *
         * These numbers kind of looked like floats to me, but don't seem to be converted into
         * anything recognizable
         *
         * 00 05 80 36      3.8152793E-06
         * 00 03 80 37      1.5260186E-05
         *
         *
         *
         */
        static void Trial3() {
            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            DataReaderVcsByteAnalysis reader = new(filenamepath);

            // reader.offset = 9525;
            reader.offset = 10340;

            float num = reader.ReadFloatAtPosition();
            // int num = reader.ReadIntAtPosition();
            Debug.WriteLine($"{num}");
            reader.ShowBytes(4);
        }

        /*
         * FD FF FF FF read as an int evaluates as -3
         *
         *
         */
        static void Trial2() {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            DataReaderVcsByteAnalysis reader = new(filenamepath);
            reader.offset = 2032;
            int num = reader.ReadIntAtPosition();
            Debug.WriteLine($"{num}");
            reader.ShowBytes(4);

        }

        /*
         *
         * file ending with lzma compression
         *
         */
        static void Trial1() {


            string filenamepath = $"{ARTIFACT_CLASSIC_CORE_PC_SOURCE}/aerial_perspective_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";

            // Debug.WriteLine($"{filenamepath}");
            // ShaderFile shaderfile = new ShaderFile(filenamepath);

            DataReaderVcsByteAnalysis reader = new DataReaderVcsByteAnalysis(filenamepath);
            reader.PrintByteAnalysis();

            // reader.ShowBytes(300);

            reader.ShowByteCount();
            int offset0 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"zframe[0] offset {offset0}");
            int offset1 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"zframe[1] offset {offset1}");
            int offset2 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"EOF {offset2}");
            reader.BreakLine();


            reader.ShowByteCount();
            int chunkOffset = reader.offset;
            int chunkSize = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"{chunkSize}");
            reader.ShowBytes(4);
            reader.PrintIntWithValue();
            reader.PrintIntWithValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.offset = chunkOffset + chunkSize + 4;
            reader.ShowByteCount();
            int chunkOffset2 = reader.offset;
            int chunkSize2 = reader.ReadIntAtPosition();
            reader.ShowBytes(4, $"{chunkSize2}");
            reader.ShowBytes(4);
            reader.PrintIntWithValue();
            reader.PrintIntWithValue();
            reader.ShowBytes(5, "decoder properties");
            reader.ShowBytesAtPosition(0, 100);
            reader.BreakLine();

            reader.offset = chunkOffset2 + chunkSize2 + 4;

            reader.EndOfFile();
            // reader.ShowByteCount();
            // reader.ShowBytes(4);




        }





    }


}





