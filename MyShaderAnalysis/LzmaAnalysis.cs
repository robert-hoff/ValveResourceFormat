using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.vcsparsing;
using System.Collections.Generic;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using MyValveResourceFormat.ThirdParty;

namespace MyShaderAnalysis {


    public class LzmaAnalysis {



        public static void RunTrials() {
            // Trial1();
            // Trial2();
            // Trial3();
            // Trial4();
            Trial5();
        }




        static void Trial5() {
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/blur_pc_30_ps.vcs";
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            ShaderFile shaderFile = new ShaderFile(filenamepath);


            // byte[] zframeDatabytes = shaderFile.GetDecompressedZFrameByIndex(0);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(0x28);


            DataReaderZFramePcFile datareader = new(zframeDatabytes, FILETYPE.ps_file);
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
            datareader.BreakLine();

            // datareader.ShowBytes(482);
            // datareader.ShowBytes(2866);
            // datareader.ShowBytes(3348);
            datareader.ShowBytes(3128);
            datareader.BreakLine();
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





