using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ZstdSharp;
using LzmaDecoder = SevenZip.Compression.LZMA.Decoder;
using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;

namespace ValveResourceFormat.ShaderParser
{
    public class ShaderFile
    {
        private ShaderDataReader datareader { get; }
        public string filenamepath { get; }
        public VcsFileType vcsFileType { get; }
        public VcsSourceType vcsSourceType { get; }
        public FeaturesHeaderBlock featuresHeader { get; }
        public VsPsHeaderBlock vspsHeader { get; }
        public List<SfBlock> sfBlocks { get; } = new();
        public List<SfConstraintsBlock> sfConstraintsBlocks { get; } = new();
        public List<DBlock> dBlocks { get; } = new();
        public List<DConstraintsBlock> dConstraintsBlocks { get; } = new();
        public List<ParamBlock> paramBlocks { get; } = new();
        public List<MipmapBlock> mipmapBlocks { get; } = new();
        public List<BufferBlock> bufferBlocks { get; } = new();
        public List<SymbolsBlock> symbolBlocks { get; } = new();

        // zframe data is sorted by the order they appear in the file
        // their Id (which is different) is the dictionary key
        // both their index and Id are used in different contexts
        public SortedDictionary<long, ZFrameDataDescription> zframesLookup { get; } = new();
        private DBlockConfigurationMapping dBlockConfigGen;

        public ShaderFile(string filenamepath, ShaderDataReader datareader)
        {
            this.filenamepath = filenamepath;
            vcsFileType = GetVcsFileType(filenamepath);
            vcsSourceType = GetVcsSourceType(filenamepath);
            this.datareader = datareader;

            if (vcsFileType == VcsFileType.Features)
            {
                featuresHeader = new FeaturesHeaderBlock(datareader, datareader.GetOffset());
            } else if (vcsFileType == VcsFileType.VertexShader || vcsFileType == VcsFileType.PixelShader
                   || vcsFileType == VcsFileType.GeometryShader || vcsFileType == VcsFileType.PotentialShadowReciever)
            {
                vspsHeader = new VsPsHeaderBlock(datareader, datareader.GetOffset());
            } else
            {
                throw new ShaderParserException($"Can't parse this filetype: {vcsFileType}");
            }
            int block_delim = datareader.ReadInt();
            if (block_delim != 17)
            {
                throw new ShaderParserException($"Unexpected value for block_delim = {block_delim}, expecting 17");
            }
            int sfBlockCount = datareader.ReadInt();
            for (int i = 0; i < sfBlockCount; i++)
            {
                SfBlock nextSfBlock = new(datareader, datareader.GetOffset(), i);
                sfBlocks.Add(nextSfBlock);
            }
            int sfConstraintsBlockCount = datareader.ReadInt();
            for (int i = 0; i < sfConstraintsBlockCount; i++)
            {
                SfConstraintsBlock nextSfConstraintsBlock = new(datareader, datareader.GetOffset(), i);
                sfConstraintsBlocks.Add(nextSfConstraintsBlock);
            }
            int dBlockCount = datareader.ReadInt();
            for (int i = 0; i < dBlockCount; i++)
            {
                DBlock nextDBlock = new(datareader, datareader.GetOffset(), i);
                dBlocks.Add(nextDBlock);
            }
            int dConstraintsBlockCount = datareader.ReadInt();
            for (int i = 0; i < dConstraintsBlockCount; i++)
            {
                DConstraintsBlock nextDConstraintsBlock = new(datareader, datareader.GetOffset(), i);
                dConstraintsBlocks.Add(nextDConstraintsBlock);
            }

            // This is needed for the zframes to determine their source mapping
            // it must be instantiated after the D-blocks have been read
            dBlockConfigGen = new DBlockConfigurationMapping(this);

            int paramBlockCount = datareader.ReadInt();
            for (int i = 0; i < paramBlockCount; i++)
            {
                ParamBlock nextParamBlock = new(datareader, datareader.GetOffset(), i);
                paramBlocks.Add(nextParamBlock);
            }
            int mipmapBlockCount = datareader.ReadInt();
            for (int i = 0; i < mipmapBlockCount; i++)
            {
                MipmapBlock nextMipmapBlock = new(datareader, datareader.GetOffset(), i);
                mipmapBlocks.Add(nextMipmapBlock);
            }
            int bufferBlockCount = datareader.ReadInt();
            for (int i = 0; i < bufferBlockCount; i++)
            {
                BufferBlock nextBufferBlock = new(datareader, datareader.GetOffset(), i);
                bufferBlocks.Add(nextBufferBlock);
            }
            if (vcsFileType == VcsFileType.Features || vcsFileType == VcsFileType.VertexShader)
            {
                int sybmolsBlockCount = datareader.ReadInt();
                for (int i = 0; i < sybmolsBlockCount; i++)
                {
                    SymbolsBlock nextSymbolsBlock = new(datareader, datareader.GetOffset(), i);
                    symbolBlocks.Add(nextSymbolsBlock);
                }
            }

            List<long> zframeIds = new();
            int zframesCount = datareader.ReadInt();
            if (zframesCount == 0)
            {
                // if zframes = 0 there's nothing more to do
                if (!datareader.CheckPositionIsAtEOF())
                {
                    throw new ShaderParserException($"Reader contains more data, but EOF expected");
                }
                return;
            }
            for (int i = 0; i < zframesCount; i++)
            {
                zframeIds.Add(datareader.ReadLong());
            }

            List<(long, int)> zframeIdsAndOffsets = new();
            foreach (long zframeId in zframeIds)
            {
                zframeIdsAndOffsets.Add((zframeId, datareader.ReadInt()));
            }

            int offsetToEndOffile = datareader.ReadInt();
            if (offsetToEndOffile != datareader.GetFileSize())
            {
                throw new ShaderParserException($"Pointer to end of file expected, value read = {offsetToEndOffile}");
            }

            foreach (var item in zframeIdsAndOffsets)
            {
                long zframeId = item.Item1;
                int offsetToZframeHeader = item.Item2;
                datareader.SetOffset(offsetToZframeHeader);
                uint chunkSizeOrZframeDelim = datareader.ReadUInt();
                int compressionType = chunkSizeOrZframeDelim == CompiledShader.ZSTD_DELIM ?
                    CompiledShader.ZSTD_COMPRESSION : CompiledShader.LZMA_COMPRESSION;
                if (chunkSizeOrZframeDelim != CompiledShader.ZSTD_DELIM)
                {
                    if (datareader.ReadUInt() != CompiledShader.LZMA_DELIM)
                    {
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


#pragma warning disable CA1024 // Use properties where appropriate
        public int GetZFrameCount()
        {
            return zframesLookup.Count;
        }

        public long GetZFrameIdByIndex(int zframeIndex)
        {
            return zframesLookup.ElementAt(zframeIndex).Key;
        }

        public byte[] GetDecompressedZFrameByIndex(int zframeIndex)
        {
            return zframesLookup.ElementAt(zframeIndex).Value.GetDecompressedZFrame();
        }

        public byte[] GetDecompressedZFrame(long zframeId)
        {
            return zframesLookup[zframeId].GetDecompressedZFrame();
        }

        public ZFrameFile GetZFrameFile(long zframeId)
        {
            return new ZFrameFile(GetDecompressedZFrame(zframeId), filenamepath, zframeId);
        }

        public ZFrameFile GetZFrameFileByIndex(int zframeIndex)
        {
            long zframeId = zframesLookup.ElementAt(zframeIndex).Key;
            return GetZFrameFile(zframeId);
        }
#pragma warning restore CA1024

        public void PrintByteAnalysis()
        {
            datareader.SetOffset(0);
            if (vcsFileType == VcsFileType.Features)
            {
                featuresHeader.PrintAnnotatedBytestream();
            } else if (vcsFileType == VcsFileType.VertexShader || vcsFileType == VcsFileType.PixelShader
                  || vcsFileType == VcsFileType.GeometryShader || vcsFileType == VcsFileType.PotentialShadowReciever)
            {
                vspsHeader.PrintAnnotatedBytestream();
            }
            uint blockDelim = datareader.ReadUIntAtPosition();
            if (blockDelim != 17)
            {
                throw new ShaderParserException($"unexpected block delim value! {blockDelim}");
            }
            datareader.ShowByteCount();
            datareader.ShowBytes(4, $"block DELIM always 17");
            datareader.BreakLine();
            datareader.ShowByteCount();
            uint sfBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{sfBlockCount} SF blocks (usually 152 bytes each)");
            datareader.BreakLine();
            foreach (var sfBlock in sfBlocks)
            {
                sfBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint sfConstraintsBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{sfConstraintsBlockCount} S-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var sfConstraintsBlock in sfConstraintsBlocks)
            {
                sfConstraintsBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint dBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{dBlockCount} D-blocks (152 bytes each)");
            datareader.BreakLine();
            foreach (var dBlock in dBlocks)
            {
                dBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint dConstraintsBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{dConstraintsBlockCount} D-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var dConstraintBlock in dConstraintsBlocks)
            {
                dConstraintBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint paramBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{paramBlockCount} Param-Blocks (may contain dynamic expressions)");
            datareader.BreakLine();
            foreach (var paramBlock in paramBlocks)
            {
                paramBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint mipmapBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            datareader.BreakLine();
            foreach (var mipmapBlock in mipmapBlocks)
            {
                mipmapBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint bufferBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{bufferBlockCount} Buffer blocks (variable length)");
            datareader.BreakLine();
            foreach (var bufferBlock in bufferBlocks)
            {
                bufferBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            if (vcsFileType == VcsFileType.Features || vcsFileType == VcsFileType.VertexShader)
            {
                uint symbolBlockCount = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(4, $"{symbolBlockCount} symbol/names blocks");
                foreach (var symbolBlock in symbolBlocks)
                {
                    datareader.BreakLine();
                    symbolBlock.PrintAnnotatedBytestream();
                }
                datareader.BreakLine();
            }

            PrintZframes();

            if (!DONT_SHOW_COMPRESSED_ZFRAMES)
            {
#pragma warning disable CS0162 // Unreachable code detected
                datareader.ShowEndOfFile();
#pragma warning restore CS0162 // Unreachable code detected
            }

        }

        const bool DONT_SHOW_COMPRESSED_ZFRAMES = true;
        private void PrintZframes()
        {
            datareader.ShowByteCount();
            uint zFrameCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{zFrameCount} zframes");
            datareader.BreakLine();
            if (zFrameCount == 0)
            {
                return;
            }
            List<uint> zFrameIndexes = new();
            datareader.ShowByteCount("zFrame IDs");
            for (int i = 0; i < zFrameCount; i++)
            {
                uint zframeId = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(8, breakLine: false);
                datareader.TabComment($"{getZFrameIdString(zframeId)}    {Convert.ToString(zframeId, 2).PadLeft(20, '0')}");
                zFrameIndexes.Add(zframeId);
            }
            datareader.BreakLine();
            if (DONT_SHOW_COMPRESSED_ZFRAMES)
            {
                datareader.Comment("rest of data contains compressed zframes");
                datareader.BreakLine();
                return;
            }
#pragma warning disable CS0162 // Unreachable code detected
            datareader.ShowByteCount("zFrame file offsets");
#pragma warning restore CS0162 // Unreachable code detected
            foreach (uint zframeId in zFrameIndexes)
            {
                uint zframe_offset = datareader.ReadUIntAtPosition();
                datareader.ShowBytes(4, $"{zframe_offset} offset of {getZFrameIdString(zframeId)}");
            }
            uint total_size = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{total_size} - end of file");
            datareader.OutputWriteLine("");
            foreach (uint zframeId in zFrameIndexes)
            {
                PrintCompressedZFrame(zframeId);
            }
        }

        public void PrintCompressedZFrame(uint zframeId)
        {
            datareader.OutputWriteLine($"[{datareader.GetOffset()}] {getZFrameIdString(zframeId)}");
            datareader.ShowBytes(4, "DELIM (0xfffffffd)");
            int uncompressed_length = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"{uncompressed_length,-8} uncompressed length");
            // TabPrintComment(uncompressed_length.ToString().PadRight(8));
            int compressed_length = datareader.ReadIntAtPosition();
            datareader.ShowBytes(4, $"{compressed_length,-8} compressed length");
            datareader.ShowBytesAtPosition(0, compressed_length > 96 ? 96 : compressed_length);
            if (compressed_length > 96)
            {
                datareader.Comment($"... ({compressed_length - 96} bytes not shown)");
            }
            datareader.MoveOffset(compressed_length);
            datareader.BreakLine();
        }

        private static string getZFrameIdString(uint zframeId)
        {
            return $"zframe[0x{zframeId:x08}]";
        }
    }




    // Lzma also comes with 'chunk-size' field, which is not needed
    public class ZFrameDataDescription
    {
        public long zframeId { get; }
        public int offsetToZFrameHeader { get; }
        public int compressionType { get; }
        public int compressedLength { get; }
        public int uncompressedLength { get; }
        private ShaderDataReader datareader { get; }
        public ZFrameDataDescription(long zframeId, int offsetToZFrameHeader, int compressionType,
            int uncompressedLength, int compressedLength, ShaderDataReader datareader)
        {
            this.zframeId = zframeId;
            this.offsetToZFrameHeader = offsetToZFrameHeader;
            this.compressionType = compressionType;
            this.uncompressedLength = uncompressedLength;
            this.compressedLength = compressedLength;
            this.datareader = datareader;
        }
        public byte[] GetDecompressedZFrame()
        {
            datareader.SetOffset(offsetToZFrameHeader);
            if (compressionType == CompiledShader.ZSTD_COMPRESSION)
            {
                datareader.MoveOffset(12);
                byte[] compressedZframe = datareader.ReadBytes(compressedLength);
                using var zstdDecoder = new Decompressor();
                zstdDecoder.LoadDictionary(GetZFrameDictionary());
                Span<byte> zframeUncompressed = zstdDecoder.Unwrap(compressedZframe);
                if (zframeUncompressed.Length != uncompressedLength)
                {
                    throw new ShaderParserException("Decompressed zframe doesn't match expected size");
                }
                zstdDecoder.Dispose();
                return zframeUncompressed.ToArray();
            }

            if (compressionType == CompiledShader.LZMA_COMPRESSION)
            {
                var lzmaDecoder = new LzmaDecoder();
                datareader.MoveOffset(16);
                lzmaDecoder.SetDecoderProperties(datareader.ReadBytes(5));
                var compressedBuffer = datareader.ReadBytes(compressedLength);
                using (var inputStream = new MemoryStream(compressedBuffer))
                using (var outStream = new MemoryStream((int)uncompressedLength))
                {
                    lzmaDecoder.Code(inputStream, outStream, compressedBuffer.Length, uncompressedLength, null);
                    return outStream.ToArray();
                }
            }

            throw new ShaderParserException("This point cannot be reached, compressionType should be either ZSTD or LZMA");
        }

        public override string ToString()
        {
            string comprDesc = compressionType == CompiledShader.ZSTD_COMPRESSION ? "ZSTD" : "LZMA";
            return $"zframeId[0x{zframeId:x08}] {comprDesc} offset={offsetToZFrameHeader,8} " +
                $"compressedLength={compressedLength,7} uncompressedLength={uncompressedLength,9}";
        }
    }


}
