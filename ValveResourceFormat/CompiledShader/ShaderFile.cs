using System;
using System.Collections.Generic;
using System.Linq;
using ZstdSharp;
using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1024 // Use properties where appropriate
namespace ValveResourceFormat.ShaderParser
{
    public class ShaderFile
    {
        private ShaderDataReader datareader;
        public string filenamepath;
        public VcsFileType vcsFiletype = VcsFileType.Undetermined;
        public FeaturesHeaderBlock featuresHeader;
        public VsPsHeaderBlock vspsHeader;
        public List<SfBlock> sfBlocks = new();
        public List<SfConstraintsBlock> sfRuleBlocks = new();
        public List<DBlock> dBlocks = new();
        public List<DConstraintsBlock> dRuleBlocks = new();
        public List<ParamBlock> paramBlocks = new();
        public List<MipmapBlock> mipmapBlocks = new();
        public List<BufferBlock> bufferBlocks = new();
        public List<SymbolsBlock> symbolBlocks = new();
        public SortedDictionary<long, int> zframesLookup = new(); // (frameID to offset)

        public ShaderFile(string filenamepath, ShaderDataReader datareader)
        {
            this.filenamepath = filenamepath;
            vcsFiletype = GetVcsFileType(filenamepath);
            this.datareader = datareader;

            if (vcsFiletype == VcsFileType.Features)
            {
                featuresHeader = new FeaturesHeaderBlock(datareader, datareader.GetOffset());
            } else if (vcsFiletype == VcsFileType.VertexShader || vcsFiletype == VcsFileType.PixelShader
                   || vcsFiletype == VcsFileType.GeometryShader || vcsFiletype == VcsFileType.PotentialShadowReciever)
            {
                vspsHeader = new VsPsHeaderBlock(datareader, datareader.GetOffset());

            } else
            {
                throw new ShaderParserException($"Can't parse this filetype: {vcsFiletype}");
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
            // always 472 bytes
            int sfRuleBlockCount = datareader.ReadInt();
            for (int i = 0; i < sfRuleBlockCount; i++)
            {
                SfConstraintsBlock nextSfRuleBlock = new(datareader, datareader.GetOffset(), i);
                sfRuleBlocks.Add(nextSfRuleBlock);
            }
            // always 152 bytes
            int dBlockCount = datareader.ReadInt();
            for (int i = 0; i < dBlockCount; i++)
            {
                DBlock nextDBlock = new(datareader, datareader.GetOffset(), i);
                dBlocks.Add(nextDBlock);
            }
            // always 472 bytes
            int dRuleBlockCount = datareader.ReadInt();
            for (int i = 0; i < dRuleBlockCount; i++)
            {
                DConstraintsBlock nextDRuleBlock = new(datareader, datareader.GetOffset(), i);
                dRuleBlocks.Add(nextDRuleBlock);
            }
            int paramBlockCount = datareader.ReadInt();
            for (int i = 0; i < paramBlockCount; i++)
            {
                ParamBlock nextParamBlock = new(datareader, datareader.GetOffset(), i);
                paramBlocks.Add(nextParamBlock);
            }
            // always 280 bytes
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
            // only features and vs files observe symbol blocks
            if (vcsFiletype == VcsFileType.Features || vcsFiletype == VcsFileType.VertexShader)
            {
                int sybmolsBlockCount = datareader.ReadInt();
                for (int i = 0; i < sybmolsBlockCount; i++)
                {
                    SymbolsBlock nextSymbolsBlock = new(datareader, datareader.GetOffset(), i);
                    symbolBlocks.Add(nextSymbolsBlock);
                }
            }
            List<long> zframeIDs = new();
            int zframesCount = datareader.ReadInt();
            for (int i = 0; i < zframesCount; i++)
            {
                zframeIDs.Add(datareader.ReadLong());
            }
            foreach (long zframeID in zframeIDs)
            {
                zframesLookup.Add(zframeID, datareader.ReadInt());
            }
            if (zframesCount > 0)
            {
                int offsetToEndOfFile = datareader.ReadInt();
                datareader.SetPosition(offsetToEndOfFile);
            }
             datareader.ThrowIfNotAtEOF();
        }

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
            var zframeBlock = zframesLookup.ElementAt(zframeIndex);
            return GetDecompressedZFrame(zframeBlock.Key);
        }

        public byte[] GetDecompressedZFrame(long zframeId)
        {
            datareader.SetPosition(zframesLookup[zframeId]);
            uint delim = datareader.ReadUInt();
            if (delim != 0xfffffffd)
            {
                throw new ShaderParserException("unexpected zframe delimiter");
            }
            int uncompressed_length = datareader.ReadInt();
            int compressed_length = datareader.ReadInt();
            byte[] compressedZframe = datareader.ReadBytes(compressed_length);
            using var decompressor = new Decompressor();
            decompressor.LoadDictionary(GetZFrameDictionary());
            Span<byte> zframeUncompressed = decompressor.Unwrap(compressedZframe);
            if (zframeUncompressed.Length != uncompressed_length)
            {
                throw new ShaderParserException("zframe length mismatch!");
            }
            // decompressor.Dispose(); // dispose or not?
            return zframeUncompressed.ToArray();
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

        public void PrintByteAnalysis()
        {
            datareader.SetPosition(0);
            if (vcsFiletype == VcsFileType.Features)
            {
                featuresHeader.PrintAnnotatedBytestream();
            } else if (vcsFiletype == VcsFileType.VertexShader || vcsFiletype == VcsFileType.PixelShader
                  || vcsFiletype == VcsFileType.GeometryShader || vcsFiletype == VcsFileType.PotentialShadowReciever)
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
            uint sfRuleBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{sfRuleBlockCount} S-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var sfRuleBlock in sfRuleBlocks)
            {
                sfRuleBlock.PrintAnnotatedBytestream();
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
            uint dRuleBlockCount = datareader.ReadUIntAtPosition();
            datareader.ShowBytes(4, $"{dRuleBlockCount} D-configuration constraint blocks (472 bytes each)");
            datareader.BreakLine();
            foreach (var dRuleBlock in dRuleBlocks)
            {
                dRuleBlock.PrintAnnotatedBytestream();
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
            if (vcsFiletype == VcsFileType.Features || vcsFiletype == VcsFileType.VertexShader)
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
                datareader.ThrowIfNotAtEOF();
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
            datareader.moveOffset(compressed_length);
            datareader.BreakLine();
        }

        private static string getZFrameIdString(uint zframeId)
        {
            return $"zframe[0x{zframeId:x08}]";
        }

    }
}