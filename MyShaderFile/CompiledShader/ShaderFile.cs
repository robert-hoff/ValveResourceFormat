using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ZstdSharp;
using LzmaDecoder = SevenZip.Compression.LZMA.Decoder;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;

namespace ValveResourceFormat.CompiledShader
{
    public class ShaderFile : IDisposable
    {
        public const int MAGIC = 0x32736376; // "vcs2"
        public const uint ZSTD_DELIM = 0xFFFFFFFD;
        public const uint LZMA_DELIM = 0x414D5A4C;
        public const int UNCOMPRESSED = 0;
        public const int ZSTD_COMPRESSION = 1;
        public const int LZMA_COMPRESSION = 2;
        public const uint PI_MURMURSEED = 0x31415926;
        public ShaderDataReader datareader { get; set; }

        /// <summary>
        /// Releases binary reader.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && datareader != null)
            {
                datareader.Dispose();
                datareader = null;
            }
        }

        /// <summary>
        /// Opens and reads the given filename.
        /// The file is held open until the object is disposed.
        /// </summary>
        /// <param name="filenamepath">The file to open and read.</param>
        public void Read(string filenamepath)
        {
            var fs = new FileStream(filenamepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Read(filenamepath, fs);
        }

        /// <summary>
        /// Reads the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="filename">The filename <see cref="string"/>.</param>
        /// <param name="input">The input <see cref="Stream"/> to read from.</param>
        public void Read(string filenamepath, Stream input)
        {
            datareader = new ShaderDataReader(input);
            this.filenamepath = filenamepath;
            ParseFile();
        }

        public void PrintSummary(HandleOutputWrite OutputWriter = null, bool showRichTextBoxLinks = false, List<string> relatedfiles = null)
        {
            PrintVcsFileSummary fileSummary = new PrintVcsFileSummary(this, OutputWriter, showRichTextBoxLinks, relatedfiles);
        }

        public string filenamepath { get; private set; }
        public VcsProgramType vcsProgramType { get; private set; }
        public VcsPlatformType vcsPlatformType { get; private set; }
        public VcsShaderModelType vcsShaderModelType { get; private set; }
        public FeaturesHeaderBlock featuresHeader { get; private set; }
        public VsPsHeaderBlock vspsHeader { get; private set; }
        public int vcsVersion { get; private set; }
        public int possibleEditorDescription { get; private set; } // 17 for all up to date files. 14 seen in old test files
        public List<SfBlock> sfBlocks { get; private set; } = new();
        public List<SfConstraintsBlock> sfConstraintsBlocks { get; private set; } = new();
        public List<DBlock> dBlocks { get; private set; } = new();
        public List<DConstraintsBlock> dConstraintsBlocks { get; private set; } = new();
        public List<ParamBlock> paramBlocks { get; private set; } = new();
        public List<MipmapBlock> mipmapBlocks { get; private set; } = new();
        public List<BufferBlock> bufferBlocks { get; private set; } = new();
        public List<VertexSymbolsBlock> symbolBlocks { get; private set; } = new();

        // Zframe data assigned to the ZFrameDataDescription class are key pieces of
        // information needed to decompress and retrieve zframes (to save processing zframes are only
        // decompressed on request). This information is organised in zframesLookup by their zframeId's.
        // Because the zframes appear in the file in ascending order, storing their data in a
        // sorted dictionary enables retrieval based on the order they are seen; by calling
        // zframesLookup.ElementAt(zframeIndex). We also retrieve them based on their id using
        // zframesLookup[zframeId]. Both methods are useful in different contexts (be aware not to mix them up).
        public SortedDictionary<long, ZFrameDataDescription> zframesLookup { get; } = new();
        private ConfigMappingDParams dBlockConfigGen;

        private void ParseFile()
        {
            var vcsFileProperties = ComputeVCSFileName(filenamepath);
            vcsProgramType = vcsFileProperties.Item1;
            vcsPlatformType = vcsFileProperties.Item2;
            vcsShaderModelType = vcsFileProperties.Item3;
            // There's a chance HullShader, DomainShader and RaytracingShader work but they haven't been tested
            if (vcsProgramType == VcsProgramType.Features)
            {
                featuresHeader = new FeaturesHeaderBlock(datareader);
                vcsVersion = featuresHeader.vcsFileVersion;
            } else if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.PixelShader
                   || vcsProgramType == VcsProgramType.GeometryShader || vcsProgramType == VcsProgramType.PixelShaderRenderState
                   || vcsProgramType == VcsProgramType.ComputeShader || vcsProgramType == VcsProgramType.HullShader
                   || vcsProgramType == VcsProgramType.DomainShader || vcsProgramType == VcsProgramType.RaytracingShader)
            {
                vspsHeader = new VsPsHeaderBlock(datareader);
                vcsVersion = vspsHeader.vcsFileVersion;
            } else
            {
                throw new ShaderParserException($"Can't parse this filetype: {vcsProgramType}");
            }
            possibleEditorDescription = datareader.ReadInt32();
            int sfBlockCount = datareader.ReadInt32();
            for (int i = 0; i < sfBlockCount; i++)
            {
                SfBlock nextSfBlock = new(datareader, i);
                sfBlocks.Add(nextSfBlock);
            }
            int sfConstraintsBlockCount = datareader.ReadInt32();
            for (int i = 0; i < sfConstraintsBlockCount; i++)
            {
                SfConstraintsBlock nextSfConstraintsBlock = new(datareader, i);
                sfConstraintsBlocks.Add(nextSfConstraintsBlock);
            }
            int dBlockCount = datareader.ReadInt32();
            for (int i = 0; i < dBlockCount; i++)
            {
                DBlock nextDBlock = new(datareader, i);
                dBlocks.Add(nextDBlock);
            }
            int dConstraintsBlockCount = datareader.ReadInt32();
            for (int i = 0; i < dConstraintsBlockCount; i++)
            {
                DConstraintsBlock nextDConstraintsBlock = new(datareader, i);
                dConstraintsBlocks.Add(nextDConstraintsBlock);
            }

            // This is needed for the zframes to determine their source mapping
            // it must be instantiated after the D-blocks have been read
            dBlockConfigGen = new ConfigMappingDParams(this);

            int paramBlockCount = datareader.ReadInt32();
            for (int i = 0; i < paramBlockCount; i++)
            {
                ParamBlock nextParamBlock = new(datareader, i, vcsVersion);
                paramBlocks.Add(nextParamBlock);
            }
            int mipmapBlockCount = datareader.ReadInt32();
            for (int i = 0; i < mipmapBlockCount; i++)
            {
                MipmapBlock nextMipmapBlock = new(datareader, i);
                mipmapBlocks.Add(nextMipmapBlock);
            }
            int bufferBlockCount = datareader.ReadInt32();
            for (int i = 0; i < bufferBlockCount; i++)
            {
                BufferBlock nextBufferBlock = new(datareader, i);
                bufferBlocks.Add(nextBufferBlock);
            }
            if (vcsProgramType == VcsProgramType.Features || vcsProgramType == VcsProgramType.VertexShader)
            {
                int sybmolsBlockCount = datareader.ReadInt32();
                for (int i = 0; i < sybmolsBlockCount; i++)
                {
                    VertexSymbolsBlock nextSymbolsBlock = new(datareader, i);
                    symbolBlocks.Add(nextSymbolsBlock);
                }
            }

            List<long> zframeIds = new();
            int zframesCount = datareader.ReadInt32();
            if (zframesCount == 0)
            {
                // if zframes = 0 there's nothing more to do
                if (datareader.BaseStream.Position != datareader.BaseStream.Length)
                {
                    throw new ShaderParserException($"Reader contains more data, but EOF expected");
                }
                return;
            }
            for (int i = 0; i < zframesCount; i++)
            {
                zframeIds.Add(datareader.ReadInt64());
            }

            List<(long, int)> zframeIdsAndOffsets = new();
            foreach (long zframeId in zframeIds)
            {
                zframeIdsAndOffsets.Add((zframeId, datareader.ReadInt32()));
            }

            int offsetToEndOffile = datareader.ReadInt32();
            if (offsetToEndOffile != (int)datareader.BaseStream.Length)
            {
                throw new ShaderParserException($"Pointer to end of file expected, value read = {offsetToEndOffile}");
            }

            foreach (var item in zframeIdsAndOffsets)
            {
                long zframeId = item.Item1;
                int offsetToZframeHeader = item.Item2;
                datareader.BaseStream.Position = offsetToZframeHeader;
                uint chunkSizeOrZframeDelim = datareader.ReadUInt32();
                int compressionType = chunkSizeOrZframeDelim == ZSTD_DELIM ? ZSTD_COMPRESSION : LZMA_COMPRESSION;

                int uncompressedLength = 0;
                int compressedLength = 0;
                if (chunkSizeOrZframeDelim != ZSTD_DELIM)
                {
                    uint lzmaDelimOrStartOfData = datareader.ReadUInt32();
                    if (lzmaDelimOrStartOfData != LZMA_DELIM)
                    {
                        uncompressedLength = (int) chunkSizeOrZframeDelim;
                        compressionType = UNCOMPRESSED;
                    }
                }

                if (compressionType == ZSTD_COMPRESSION || compressionType == LZMA_COMPRESSION)
                {
                    uncompressedLength = datareader.ReadInt32();
                    compressedLength = datareader.ReadInt32();
                }

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

        public ZFrameFile GetZFrameFile(long zframeId, HandleOutputWrite OutputWriter = null, bool omitParsing = false)
        {
            return new ZFrameFile(GetDecompressedZFrame(zframeId), filenamepath, zframeId,
                vcsProgramType, vcsPlatformType, vcsShaderModelType, omitParsing, OutputWriter);
        }

        public ZFrameFile GetZFrameFileByIndex(int zframeIndex, HandleOutputWrite OutputWriter = null, bool omitParsing = false)
        {
            long zframeId = zframesLookup.ElementAt(zframeIndex).Key;
            return GetZFrameFile(zframeId, OutputWriter: OutputWriter, omitParsing: omitParsing);
        }
#pragma warning restore CA1024


        private uint zFrameCount;
        const int SKIP_ZFRAMES_IF_MORE_THAN = 10;

        public void PrintByteAnalysis(bool shortenOutput = true, HandleOutputWrite OutputWriter = null)
        {
            datareader.OutputWriter = OutputWriter ?? ((x) => { Console.Write(x); });
            datareader.BaseStream.Position = 0;
            if (vcsProgramType == VcsProgramType.Features)
            {
                featuresHeader.PrintAnnotatedBytestream();
            } else if (vcsProgramType == VcsProgramType.VertexShader || vcsProgramType == VcsProgramType.PixelShader
                   || vcsProgramType == VcsProgramType.GeometryShader || vcsProgramType == VcsProgramType.PixelShaderRenderState
                   || vcsProgramType == VcsProgramType.ComputeShader || vcsProgramType == VcsProgramType.HullShader
                   || vcsProgramType == VcsProgramType.DomainShader || vcsProgramType == VcsProgramType.RaytracingShader)
            {
                vspsHeader.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            int possible_editor_desc = datareader.ReadInt32AtPosition();
            datareader.ShowBytes(4, $"({possible_editor_desc}) possible editor description");
            int lastEditorRef = vcsProgramType == VcsProgramType.Features ? featuresHeader.editorIDs.Count - 1 : 1;
            datareader.TabComment($"value appears to be linked to the last Editor reference (Editor ref. ID{lastEditorRef})", 15);
            datareader.ShowByteCount();
            uint sfBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{sfBlockCount} SF blocks (usually 152 bytes each)");
            datareader.BreakLine();
            foreach (var sfBlock in sfBlocks)
            {
                sfBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint sfConstraintsBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{sfConstraintsBlockCount} S-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var sfConstraintsBlock in sfConstraintsBlocks)
            {
                sfConstraintsBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint dBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{dBlockCount} D-blocks (152 bytes each)");
            datareader.BreakLine();
            foreach (var dBlock in dBlocks)
            {
                dBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint dConstraintsBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{dConstraintsBlockCount} D-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var dConstraintBlock in dConstraintsBlocks)
            {
                dConstraintBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint paramBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{paramBlockCount} Param-Blocks");
            datareader.BreakLine();
            foreach (var paramBlock in paramBlocks)
            {
                paramBlock.PrintAnnotatedBytestream(vcsVersion);
            }
            datareader.ShowByteCount();
            uint mipmapBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{mipmapBlockCount} Mipmap blocks (280 bytes each)");
            datareader.BreakLine();
            foreach (var mipmapBlock in mipmapBlocks)
            {
                mipmapBlock.PrintAnnotatedBytestream();
            }
            datareader.ShowByteCount();
            uint bufferBlockCount = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{bufferBlockCount} Buffer blocks (variable length)");
            datareader.BreakLine();
            foreach (var bufferBlock in bufferBlocks)
            {
                bufferBlock.PrintAnnotatedBytestream();
            }
            if (vcsProgramType == VcsProgramType.Features || vcsProgramType == VcsProgramType.VertexShader)
            {
                datareader.ShowByteCount();
                uint symbolBlockCount = datareader.ReadUInt32AtPosition();
                datareader.ShowBytes(4, $"{symbolBlockCount} symbol/names blocks");
                foreach (var symbolBlock in symbolBlocks)
                {
                    datareader.BreakLine();
                    symbolBlock.PrintAnnotatedBytestream();
                }
                datareader.BreakLine();
            }

            PrintZframes(shortenOutput);
            if (shortenOutput && zFrameCount > SKIP_ZFRAMES_IF_MORE_THAN)
            {
                datareader.Comment("rest of data contains compressed zframes");
                datareader.BreakLine();
                return;
            }

            datareader.ShowEndOfFile();
        }

        public int[] GetDBlockConfig(int blockId)
        {
            return dBlockConfigGen.GetConfigState(blockId);
        }

        private void PrintZframes(bool shortenOutput)
        {
            datareader.ShowByteCount();
            zFrameCount = datareader.ReadUInt32AtPosition();
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
                uint zframeId = datareader.ReadUInt32AtPosition();
                datareader.ShowBytes(8, breakLine: false);
                datareader.TabComment($"{getZFrameIdString(zframeId)}    {Convert.ToString(zframeId, 2).PadLeft(20, '0')}");
                zFrameIndexes.Add(zframeId);
            }
            datareader.BreakLine();
            if (shortenOutput && zFrameCount > SKIP_ZFRAMES_IF_MORE_THAN)
            {
                return;
            }
            datareader.ShowByteCount("zFrame file offsets");
            foreach (uint zframeId in zFrameIndexes)
            {
                uint zframe_offset = datareader.ReadUInt32AtPosition();
                datareader.ShowBytes(4, $"{zframe_offset} offset of {getZFrameIdString(zframeId)}");
            }
            uint total_size = datareader.ReadUInt32AtPosition();
            datareader.ShowBytes(4, $"{total_size} - end of file");
            datareader.OutputWriteLine("");
            foreach (uint zframeId in zFrameIndexes)
            {
                PrintCompressedZFrame(zframeId);
            }
        }

        int MAX_ZFRAME_BYTES_TO_SHOW = 96;
        public void PrintCompressedZFrame(uint zframeId)
        {
            datareader.OutputWriteLine($"[{datareader.BaseStream.Position}] {getZFrameIdString(zframeId)}");
            bool isLzma = false;
            uint zstdDelimOrChunkSize = datareader.ReadUInt32AtPosition();
            if (zstdDelimOrChunkSize == ZSTD_DELIM)
            {
                datareader.ShowBytes(4, $"Zstd delim (0x{ZSTD_DELIM:x08})");
            } else
            {
                datareader.ShowBytes(4, $"Chunk size {zstdDelimOrChunkSize}");
                uint lzmaDelim = datareader.ReadUInt32AtPosition();
                if (lzmaDelim != LZMA_DELIM)
                {
                    datareader.Comment($"neither ZStd or Lzma found (frame appears to be uncompressed)");
                    datareader.ShowBytes((int)zstdDelimOrChunkSize);
                    datareader.BreakLine();
                    return;
                }
                isLzma = true;
                datareader.ShowBytes(4, $"Lzma delim (0x{LZMA_DELIM:x08})");
            }
            int uncompressed_length = datareader.ReadInt32AtPosition();
            datareader.ShowBytes(4, $"{uncompressed_length,-8} uncompressed length");
            int compressed_length = datareader.ReadInt32AtPosition();
            datareader.ShowBytes(4, $"{compressed_length,-8} compressed length");
            if (isLzma)
            {
                datareader.ShowBytes(5, "Decoder properties");
            }
            datareader.ShowBytesAtPosition(0, compressed_length > MAX_ZFRAME_BYTES_TO_SHOW ? MAX_ZFRAME_BYTES_TO_SHOW : compressed_length);
            if (compressed_length > MAX_ZFRAME_BYTES_TO_SHOW)
            {
                datareader.Comment($"... ({compressed_length - MAX_ZFRAME_BYTES_TO_SHOW} bytes not shown)");
            }
            datareader.BaseStream.Position += compressed_length;
            datareader.BreakLine();
        }

        private static string getZFrameIdString(uint zframeId)
        {
            return $"zframe[0x{zframeId:x08}]";
        }
    }

    // Lzma also comes with a 'chunk-size' field, which is not needed
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
            datareader.BaseStream.Position = offsetToZFrameHeader;

            if (compressionType == ShaderFile.UNCOMPRESSED)
            {
                datareader.BaseStream.Position += 4;
                return datareader.ReadBytes(uncompressedLength);
            }

            if (compressionType == ShaderFile.ZSTD_COMPRESSION)
            {
                datareader.BaseStream.Position += 12;
                byte[] compressedZframe = datareader.ReadBytes(compressedLength);
                using var zstdDecoder = new Decompressor();
                zstdDecoder.LoadDictionary(ZStdDictionary.GetDictionary());
                Span<byte> zframeUncompressed = zstdDecoder.Unwrap(compressedZframe);
                if (zframeUncompressed.Length != uncompressedLength)
                {
                    throw new ShaderParserException("Decompressed zframe doesn't match expected size");
                }
                zstdDecoder.Dispose();
                return zframeUncompressed.ToArray();
            }

            if (compressionType == ShaderFile.LZMA_COMPRESSION)
            {
                var lzmaDecoder = new LzmaDecoder();
                datareader.BaseStream.Position += 16;
                lzmaDecoder.SetDecoderProperties(datareader.ReadBytes(5));
                var compressedBuffer = datareader.ReadBytes(compressedLength);
                using (var inputStream = new MemoryStream(compressedBuffer))
                using (var outStream = new MemoryStream((int)uncompressedLength))
                {
                    lzmaDecoder.Code(inputStream, outStream, compressedBuffer.Length, uncompressedLength, null);
                    return outStream.ToArray();
                }
            }

            throw new ShaderParserException($"Unknown compression type or compression type not determined {compressionType}");
        }

        public override string ToString()
        {
            string comprDesc = compressionType switch
            {
                ShaderFile.UNCOMPRESSED => "uncompressed",
                ShaderFile.ZSTD_COMPRESSION => "ZSTD",
                ShaderFile.LZMA_COMPRESSION => "LZMA",
                _ => "undetermined"
            };
            return $"zframeId[0x{zframeId:x08}] {comprDesc} offset={offsetToZFrameHeader,8} " +
                $"compressedLength={compressedLength,7} uncompressedLength={uncompressedLength,9}";
        }
    }
}
