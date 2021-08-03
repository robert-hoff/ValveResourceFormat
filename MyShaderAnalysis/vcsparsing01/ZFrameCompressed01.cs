using System;
using ZstdSharp;


namespace MyShaderAnalysis.vcsparsing01 {

    public class ZFrameCompressed01 : DataPart01 {

        int uncompressed_length;
        int compressed_length;
        byte[] zframe;


        public ZFrameCompressed01(byte[] databytes, int offset, int length) : base(databytes, offset, length) {
            uint checkbytes = datareader.ReadUInt();
            if (checkbytes != 0xFFFFFFFD) {
                throw new ShaderParserException01("wrong checkbytes!");
            }

            uncompressed_length = datareader.ReadInt();
            compressed_length = datareader.ReadInt();

            if (compressed_length != length-12) {
                throw new ShaderParserException01("wrong length!");
            }

            zframe = datareader.ReadBytes(compressed_length);
        }



        // couldn't get this one to work (and can't find an api, or method calls)
        // https://github.com/Crauzer/ZstdSharp

        // using
        // https://github.com/oleg-st/ZstdSharp
        public byte[] DecompressFrame(byte[] zStdDictionary) {
            using var decompressor = new Decompressor();
            decompressor.LoadDictionary(zStdDictionary);

            // R: it manages the decompression without knowing the length of the result
            // Note, interestingly, it will arrive at the same length even with a false dictionary
            Span<byte> zframeUncompressed = decompressor.Unwrap(zframe);
            if (zframeUncompressed.Length != uncompressed_length) {
                throw new ShaderParserException01("zframe length mismatch!");
            }

            decompressor.Dispose();

            return zframeUncompressed.ToArray();
        }


    }



}






