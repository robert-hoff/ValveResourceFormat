using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ZstdSharp;
using System.Diagnostics;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
using LzmaDecoder = SevenZip.Compression.LZMA.Decoder;


namespace MyShaderAnalysis.vcsparsing {

    public class ShaderFile {
        private ShaderDataReader datareader;
        public string filenamepath;
        public VcsFileType vcsFileType = VcsFileType.Undetermined;
        public VcsSourceType vcsSourceType;
        public DataBlockFeaturesHeader featuresHeader = null;
        public DataBlockVsPsHeader vspsHeader = null;
        public List<DataBlockSfBlock> sfBlocks = new();
        public List<CompatibilityBlock> compatibilityBlocks = new();
        public List<DBlock> dBlocks = new();
        public List<UnknownBlock> unknownBlocks = new();
        public List<ParamBlock> paramBlocks = new();
        public List<MipmapBlock> mipmapBlocks = new();
        public List<BufferBlock> bufferBlocks = new();
        public List<SymbolsBlock> symbolBlocks = new();


        public const int ZSTD_COMPRESSION = 1;
        public const int LZMA_COMPRESSION = 2;
        public const uint ZSTD_DELIM = 0xFFFFFFFD;
        public const uint LZMA_DELIM = 0x414D5A4C;

        // zframe data is sorted by the order they appear in the file
        // their Id (which is different and describes the configuration they are intended for) is the dictionary key
        // both their index and Id are used in different contexts
        public SortedDictionary<long, ZFrameDataDescription> zframesLookup = new();
        private DBlockConfigurationMap dBlockConfigGen;


        public ShaderFile(string filenamepath) {
            this.filenamepath = filenamepath;
            vcsFileType = GetVcsFileType(filenamepath);
            vcsSourceType = GetVcsSourceType(filenamepath);
            datareader = new ShaderDataReader(File.ReadAllBytes(filenamepath));

            int magic = datareader.ReadInt();
            if (magic != 0x32736376) {
                throw new ShaderParserException($"wrong file id {magic:x}");
            }
            int version = datareader.ReadInt();
            if (version != 64) {
                throw new ShaderParserException($"wrong version {version}, expecting 64. {filenamepath}");
            }

            if (vcsFileType == VcsFileType.Features) {
                featuresHeader = new DataBlockFeaturesHeader(datareader, datareader.offset);
            } else if (vcsFileType == VcsFileType.VertexShader || vcsFileType == VcsFileType.PixelShader
                   || vcsFileType == VcsFileType.GeometryShader || vcsFileType == VcsFileType.PotentialShadowReciever) {
                vspsHeader = new DataBlockVsPsHeader(datareader, datareader.offset);

            } else {
                throw new ShaderParserException($"can't parse this filetype: {vcsFileType}");
            }



            int block_delim = datareader.ReadInt();
            if (block_delim != 17) {
                throw new ShaderParserException($"unexpected value for block_delim = {block_delim}, expecting 17");
            }
            int sfBlockCount = datareader.ReadInt();
            for (int i = 0; i < sfBlockCount; i++) {
                DataBlockSfBlock nextSfBlock = new(datareader, datareader.offset, i);
                sfBlocks.Add(nextSfBlock);
            }
            // always 472 bytes
            int compatBlockCount = datareader.ReadInt();
            for (int i = 0; i < compatBlockCount; i++) {
                CompatibilityBlock nextCompatibilityBlock = new(datareader, datareader.offset, i);
                compatibilityBlocks.Add(nextCompatibilityBlock);
            }



            // always 152 bytes
            int dBlockCount = datareader.ReadInt();
            for (int i = 0; i < dBlockCount; i++) {
                DBlock nextDBlock = new(datareader, datareader.offset, i);
                dBlocks.Add(nextDBlock);
            }

            // always 472 bytes
            int druleBlockCount = datareader.ReadInt();
            for (int i = 0; i < druleBlockCount; i++) {
                UnknownBlock nextUnknownBlock = new(datareader, datareader.offset, i);
                unknownBlocks.Add(nextUnknownBlock);
            }

            // This is needed for the zframes to generate their configuratio mapping
            // and must be instantiated after the D-blocks have been read.
            dBlockConfigGen = new DBlockConfigurationMap(this);


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

            if (vcsFileType == VcsFileType.Features || vcsFileType == VcsFileType.VertexShader) {
                int sybmolsBlockCount = datareader.ReadInt();
                for (int i = 0; i < sybmolsBlockCount; i++) {
                    SymbolsBlock nextSymbolsBlock = new(datareader, datareader.offset);
                    symbolBlocks.Add(nextSymbolsBlock);
                }
            }


            List<long> zframeIds = new();
            int zframesCount = datareader.ReadInt();
            if (zframesCount == 0) {
                // if zframes = 0 here there's nothing more to do
                if (datareader.offset != datareader.databytes.Length) {
                    throw new ShaderParserException($"Reader contains more data, but EOF expected");
                }
                return;
            }


            for (int i = 0; i < zframesCount; i++) {
                zframeIds.Add(datareader.ReadLong());
            }


            List<(long, int)> zframeIdsAndOffsets = new();
            foreach (long zframeId in zframeIds) {
                // zframesLookup.Add(zframeID, datareader.ReadInt());
                zframeIdsAndOffsets.Add((zframeId, datareader.ReadInt()));
            }

            int offsetToEndOffile = datareader.ReadInt();
            if (offsetToEndOffile != datareader.databytes.Length) {
                throw new ShaderParserException($"Pointer to end of file expected, value read = {offsetToEndOffile}");
            }

            foreach (var item in zframeIdsAndOffsets) {
                long zframeId = item.Item1;
                int offsetToZframeHeader = item.Item2;
                datareader.offset = offsetToZframeHeader;
                uint chunkSizeOrZframeDelim = datareader.ReadUInt();
                int compressionType = chunkSizeOrZframeDelim == ZSTD_DELIM ? ZSTD_COMPRESSION : LZMA_COMPRESSION;
                if (chunkSizeOrZframeDelim != ZSTD_DELIM) {
                    if (datareader.ReadUInt() != LZMA_DELIM) {
                        throw new ShaderParserException("Unknown compression, neither ZStd nor Lzma found");
                    }
                }
                int uncompressedLength = datareader.ReadInt();
                int compressedLength = datareader.ReadInt();

                ZFrameDataDescription zframeDataDesc = new ZFrameDataDescription(zframeId, offsetToZframeHeader,
                    compressionType, uncompressedLength, compressedLength, datareader);
                zframesLookup.Add(zframeId, zframeDataDesc);
            }
        }


        public int GetZFrameCount() {
            return zframesLookup.Count;
        }

        public long GetZFrameIdByIndex(int zframeIndex) {
            return zframesLookup.ElementAt(zframeIndex).Key;
        }

        public byte[] GetDecompressedZFrameByIndex(int zframeIndex) {
            return zframesLookup.ElementAt(zframeIndex).Value.GetDecompressedZFrame();
        }

        public byte[] GetDecompressedZFrame(long zframeId) {
            return zframesLookup[zframeId].GetDecompressedZFrame();
        }

        public ZFrameFile GetZFrameFile(long zframeId) {
            return new ZFrameFile(GetDecompressedZFrame(zframeId), filenamepath, zframeId, vcsFileType, vcsSourceType);
        }

        public ZFrameFile GetZFrameFileByIndex(int zframeIndex) {
            long zframeId = zframesLookup.ElementAt(zframeIndex).Key;
            return GetZFrameFile(zframeId);
        }

        public int[] GetDBlockConfig(int blockId) {
            return dBlockConfigGen.GetConfigState(blockId);
        }

        public void ShowZFrames() {
            foreach (var zframeData in zframesLookup) {
                Debug.WriteLine($"{zframeData}");
            }
        }

    }





    // Lzma also comes with 'chunk-size', but doesn't seem to be needed
    // (possibly the idea with the chunk size is an an aid for navigating the data)
    public class ZFrameDataDescription {
        public long zframeId;
        public int offsetToZFrameHeader;
        public int compressionType;
        public int compressedLength;
        public int uncompressedLength;
        ShaderDataReader datareader;
        public ZFrameDataDescription(long zframeId, int offsetToZFrameHeader, int compressionType,
            int uncompressedLength, int compressedLength, ShaderDataReader datareader) {
            this.zframeId = zframeId;
            this.offsetToZFrameHeader = offsetToZFrameHeader;
            this.compressionType = compressionType;
            this.uncompressedLength = uncompressedLength;
            this.compressedLength = compressedLength;
            this.datareader = datareader;
        }
        public byte[] GetDecompressedZFrame() {
            datareader.offset = offsetToZFrameHeader;
            if (compressionType == ShaderFile.ZSTD_COMPRESSION) {
                datareader.offset += 12;
                byte[] compressedZframe = datareader.ReadBytes(compressedLength);
                using var zstdDecoder = new Decompressor();
                zstdDecoder.LoadDictionary(GetZFrameDictionary());
                Span<byte> zframeUncompressed = zstdDecoder.Unwrap(compressedZframe);
                if (zframeUncompressed.Length != uncompressedLength) {
                    throw new ShaderParserException("Decompressed zframe doesn't match expected size");
                }
                zstdDecoder.Dispose();
                return zframeUncompressed.ToArray();
            }

            if (compressionType == ShaderFile.LZMA_COMPRESSION) {
                var lzmaDecoder = new LzmaDecoder();
                datareader.offset += 16;
                lzmaDecoder.SetDecoderProperties(datareader.ReadBytes(5));
                var compressedBuffer = datareader.ReadBytes(compressedLength);
                using (var inputStream = new MemoryStream(compressedBuffer))
                using (var outStream = new MemoryStream((int)uncompressedLength)) {
                    lzmaDecoder.Code(inputStream, outStream, compressedBuffer.Length, uncompressedLength, null);
                    return outStream.ToArray();
                }
            }

            throw new ShaderParserException("This point cannot be reached, compressionType should be either ZSTD or LZMA");
        }



        public override string ToString() {
            string comprDesc = compressionType == ShaderFile.ZSTD_COMPRESSION ? "ZSTD" : "LZMA";
            return $"zframeId[0x{zframeId:x08}] {comprDesc} offset={offsetToZFrameHeader,8} " +
                $"compressedLength={compressedLength,7} uncompressedLength={uncompressedLength,9}";
        }






    }
}



