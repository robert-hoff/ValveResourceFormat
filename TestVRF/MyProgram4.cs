using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Decompiler;
using SkiaSharp;
using ValveResourceFormat;


namespace TestVRF
{



    class MyProgram4
    {
        static void Mainz()
        {
            // RunTheDecompiler();
            decompressDataAndGenerateBitmap();
            // analyseData();
        }




        static void analyseData()
        {
            // the data is exactly 2^17 !!!
            // length of the data is 131072

            // 2^17 = 128 * 128 * 8


            Span<byte> dataspan = getTheDataPart2();
            Span<byte> blockStorage = dataspan.Slice(0, 8);

            // Debug.WriteLine(blockStorage.ToArray().Length);

            Debug.WriteLine(dataspan.Length);

        }



        static void decompressDataAndGenerateBitmap()
        {
            int w = 512;
            int h = 512;

            var imageInfo = new SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Unpremul);

            Span<byte> dataspan = getTheDataPart2();
            // var datadecompressed = new Span<byte>(new byte[512*512*4]);
            var datadecompressed = imageInfo.PeekPixels().GetPixelSpan<byte>();

            var offset = 0;
            var blockCountX = (w + 3) / 4;          // 128
            var blockCountY = (h + 3) / 4;          // 128
            var imageWidth = 512;
            var rowBytes = imageWidth * 4;          // 2048 (bytes per row)

            for (var j = 0; j < blockCountY; j++)
            {
                for (var i = 0; i < blockCountX; i++)
                {
                    var blockStorage = dataspan.Slice(offset, 8);       // simply returns [offset,offset+8)
                    offset += 8;
                    DecompressBlockDXT1(i * 4, j * 4, imageWidth, blockStorage, datadecompressed, rowBytes);
                }
            }
            uint crc32 = Crc32.Compute(datadecompressed.ToArray());
            Debug.WriteLine("0x{0:x8}", crc32);


            Span<byte> data;
            // R: outputting to bmp or jpeg works but other formats offered by SKEncodedImageFormat produce
            // empty files (of 0 bytes)
            using (var ms = new MemoryStream())
            {
                imageInfo.PeekPixels().Encode(ms, SKEncodedImageFormat.Jpeg, 100);
                data = ms.ToArray();
                Debug.WriteLine("Uncompressed size = {0}", data.Length);
            }

            string outputPath = @"X:\checkouts\ValveResourceFormat\output\hello.jpg";
            File.WriteAllBytes(outputPath, data.ToArray());
        }



        private static void DecompressBlockDXT1(int x, int y, int width, Span<byte> blockStorage, Span<byte> pixels, int stride)
        {
            var color0 = (ushort) (blockStorage[0] | blockStorage[1] << 8);
            var color1 = (ushort) (blockStorage[2] | blockStorage[3] << 8);

            ConvertRgb565ToRgb888(color0, out var r0, out var g0, out var b0);
            ConvertRgb565ToRgb888(color1, out var r1, out var g1, out var b1);

            uint c1 = blockStorage[4];
            var c2 = (uint)blockStorage[5] << 8;
            var c3 = (uint)blockStorage[6] << 16;
            var c4 = (uint)blockStorage[7] << 24;
            var code = c1 | c2 | c3 | c4;

            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                    var positionCode = (byte)((code >> (2 * ((4 * j) + i))) & 0x03);

                    byte finalR = 0, finalG = 0, finalB = 0;

                    switch (positionCode)
                    {
                        case 0:
                            finalR = r0;
                            finalG = g0;
                            finalB = b0;
                            break;

                        case 1:
                            finalR = r1;
                            finalG = g1;
                            finalB = b1;
                            break;

                        case 2:
                            if (color0 > color1)
                            {
                                finalR = (byte)(((2 * r0) + r1) / 3);
                                finalG = (byte)(((2 * g0) + g1) / 3);
                                finalB = (byte)(((2 * b0) + b1) / 3);
                            } else
                            {
                                finalR = (byte)((r0 + r1) / 2);
                                finalG = (byte)((g0 + g1) / 2);
                                finalB = (byte)((b0 + b1) / 2);
                            }
                            break;

                        case 3:
                            if (color0 < color1)
                            {
                                break;
                            }

                            finalR = (byte)(((2 * r1) + r0) / 3);
                            finalG = (byte)(((2 * g1) + g0) / 3);
                            finalB = (byte)(((2 * b1) + b0) / 3);
                            break;
                    }

                    var pixelIndex = ((y + j) * stride) + ((x + i) * 4);
                    if (x + i < width && pixels.Length > pixelIndex + 3)
                    {
                        pixels[pixelIndex] = finalB;
                        pixels[pixelIndex + 1] = finalG;
                        pixels[pixelIndex + 2] = finalR;
                        pixels[pixelIndex + 3] = byte.MaxValue;
                    }


                }
            }
        }



        private static void ConvertRgb565ToRgb888(ushort color, out byte r, out byte g, out byte b)
        {
            int temp;
            temp = ((color >> 11) * 255) + 16;
            r = (byte)(((temp / 32) + temp) / 32);
            temp = (((color & 0x07E0) >> 5) * 255) + 32;
            g = (byte)(((temp / 64) + temp) / 64);
            temp = ((color & 0x001F) * 255) + 16;
            b = (byte)(((temp / 32) + temp) / 32);
        }




        static Span<byte> getTheDataPart2()
        {
            int start = 1764;
            int DATASIZE = 131072;
            string filepath = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c";
            FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader myReader = new BinaryReader(filestream);
            myReader.BaseStream.Position = start;
            var databytes = new Span<byte>(new byte[DATASIZE]);
            myReader.Read(databytes);
            return databytes;
        }



        /*
         * crc32 = 0x348f965b
         *
         */
        static byte[] getTheDataPart1()
        {
            int start = 1764;
            int DATASIZE = 131072;

            string filepath = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c";
            FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader myReader = new BinaryReader(filestream);

            myReader.BaseStream.Position = start;
            byte[] databytes = myReader.ReadBytes(DATASIZE);

            // uint crc32 = Crc32.Compute(databyes);
            // Debug.WriteLine("0x{0:x8}", crc32);
            return databytes;
        }



        /*
         * Produces
         * X:\checkouts\ValveResourceFormat\output\output.png
         *
         * Arguments are
         *
         *   -o
         *   X:\checkouts\ValveResourceFormat\output\output.png
         *   -i
         *   X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c
         *
         *
         *
         */
        static void RunTheDecompiler()
        {
            String[] args = new string[4];
            // args[0] = "--stats";
            args[0] = "-o";
            args[1] = @"X:\checkouts\ValveResourceFormat\output\output.png";
            args[2] = "-i";
            args[3] = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c";
            Decompiler.Decompiler.Main(args);
        }




    }





}
