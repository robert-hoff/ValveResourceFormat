using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ZstdSharp;
using static TestVRF.vcsparsing.UtilHelpers;


namespace TestVRF.vcsparsing {

    public class ShaderFile {
        DataReader datareader;
        public string filenamepath;
        public FILETYPE vcsFiletype = FILETYPE.unknown;
        DataBlockFeaturesHeader featuresHeader = null;
        DataBlockVsPsHeader vspsHeader = null;
        List<DataBlockSfBlock> sfBlocks = new();
        List<CompatibilityBlock> compatibilityBlocks = new();
        List<DBlock> dBlocks = new();
        List<UnknownBlock> unknownBlocks = new();
        List<ParamBlock> paramBlocks = new();
        List<MipmapBlock> mipmapBlocks = new();
        List<BufferBlock> bufferBlocks = new();
        List<SymbolsBlock> symbolBlocks = new();
        SortedDictionary<long, int> zframesLookup = new(); // (frameID to offset)


        public ShaderFile(string filenamepath) {
            this.filenamepath = filenamepath;
            vcsFiletype = GetVcsFileType(filenamepath);
            datareader = new DataReader(File.ReadAllBytes(filenamepath));

            int magic = datareader.ReadInt();
            if (magic != 0x32736376) {
                throw new ShaderParserException($"wrong file id {magic:x}");
            }
            int version = datareader.ReadInt();
            if (version != 64) {
                throw new ShaderParserException($"wrong version {version}, expecting 64");
            }

            if (vcsFiletype == FILETYPE.features_file) {
                featuresHeader = new DataBlockFeaturesHeader(datareader, datareader.offset);
            } else if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.ps_file
                   || vcsFiletype == FILETYPE.gs_file || vcsFiletype == FILETYPE.psrs_file) {
                vspsHeader = new DataBlockVsPsHeader(datareader, datareader.offset);

            } else {
                throw new ShaderParserException($"can't parse this filetype: {vcsFiletype}");
            }



            int block_delim = datareader.ReadInt();
            if (block_delim != 17) {
                throw new ShaderParserException($"unexpected value for block_delom = {block_delim}, expecting 17");
            }
            int sfBlockCount = datareader.ReadInt();
            for (int i = 0; i < sfBlockCount; i++) {
                DataBlockSfBlock nextSfBlock = new(datareader, datareader.offset);
                sfBlocks.Add(nextSfBlock);
            }
            // always 472 bytes
            int compatBlockCount = datareader.ReadInt();
            for (int i = 0; i < compatBlockCount; i++) {
                CompatibilityBlock nextCompatibilityBlock = new(datareader, datareader.offset);
                compatibilityBlocks.Add(nextCompatibilityBlock);
                datareader.offset += 472;

            }



            // always 152 bytes
            int dBlockCount = datareader.ReadInt();
            for (int i = 0; i < dBlockCount; i++) {
                DBlock nextDBlock = new(datareader, datareader.offset);
                dBlocks.Add(nextDBlock);
                datareader.offset += 152;
            }

            // always 472 bytes
            int unknownBlockCount = datareader.ReadInt();
            for (int i = 0; i < unknownBlockCount; i++) {
                UnknownBlock nextUnknownBlock = new(datareader, datareader.offset);
                unknownBlocks.Add(nextUnknownBlock);
                datareader.offset += 472;
            }

            int paramBlockCount = datareader.ReadInt();
            for (int i = 0; i < paramBlockCount; i++) {
                ParamBlock nextParamBlock = new(datareader, datareader.offset);
                paramBlocks.Add(nextParamBlock);
            }

            // always 280 bytes
            int mipmapBlockCount = datareader.ReadInt();
            for (int i = 0; i < mipmapBlockCount; i++) {
                MipmapBlock nextMipmapBlock = new(datareader, datareader.offset);
                mipmapBlocks.Add(nextMipmapBlock);
                datareader.offset += 280;
            }

            int bufferBlockCount = datareader.ReadInt();
            for (int i = 0; i < bufferBlockCount; i++) {
                BufferBlock nextBufferBlock = new(datareader, datareader.offset);
                bufferBlocks.Add(nextBufferBlock);
            }



            if (vcsFiletype == FILETYPE.features_file || vcsFiletype == FILETYPE.vs_file) {
                int sybmolsBlockCount = datareader.ReadInt();
                for (int i = 0; i < sybmolsBlockCount; i++) {
                    SymbolsBlock nextSymbolsBlock = new(datareader, datareader.offset);
                    symbolBlocks.Add(nextSymbolsBlock);
                }
            }

            List<long> zframeIDs = new();
            int zframesCount = datareader.ReadInt();
            for (int i = 0; i < zframesCount; i++) {
                zframeIDs.Add(datareader.ReadLong());
            }

            foreach (long zframeID in zframeIDs) {
                zframesLookup.Add(zframeID, datareader.ReadInt());
            }

            if (zframesCount>0) {
                datareader.offset = datareader.ReadInt();
            }
            if (datareader.offset != datareader.databytes.Length) {
                throw new ShaderParserException("End of file not reached!");
            }
        }



        public int GetZFrameCount() {
            return zframesLookup.Count;
        }

        public long GetZFrameIdByIndex(int zframeIndex) {
            return zframesLookup.ElementAt(zframeIndex).Key;
        }

        public byte[] GetDecompressedZFrameByIndex(int zframeIndex) {
            var zframeBlock = zframesLookup.ElementAt(zframeIndex);
            datareader.offset = zframeBlock.Value;
            uint delim = datareader.ReadUInt();
            if (delim != 0xfffffffd) {
                throw new ShaderParserException("unexpected zframe delimiter");
            }
            int uncompressed_length = datareader.ReadInt();
            int compressed_length = datareader.ReadInt();
            byte[] compressedZframe = datareader.ReadBytes(compressed_length);
            using var decompressor = new Decompressor();
            decompressor.LoadDictionary(GetZFrameDictionary());
            Span<byte> zframeUncompressed = decompressor.Unwrap(compressedZframe);
            if (zframeUncompressed.Length != uncompressed_length) {
                throw new ShaderParserException("zframe length mismatch!");
            }
            // decompressor.Dispose(); // dispose or not?
            return zframeUncompressed.ToArray();
        }
    }


    public class ShaderParserException : Exception {
        public ShaderParserException() { }
        public ShaderParserException(string message) : base(message) { }
        public ShaderParserException(string message, Exception innerException) : base(message, innerException) { }
    }

    public enum FILETYPE {
        unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
    };


}






