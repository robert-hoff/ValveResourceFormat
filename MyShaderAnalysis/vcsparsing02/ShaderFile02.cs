using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZstdSharp;
using static MyShaderAnalysis.vcsparsing02.UtilHelpers02;


namespace MyShaderAnalysis.vcsparsing02 {

    public enum FILETYPE {
        unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
    };

    public class ShaderFile02 : DataReader02 {
        string filenamepath;
        string filename;
        private FILETYPE vcsFiletype = FILETYPE.unknown;
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
        Dictionary<long, int> zframesLookup = new(); // (frameID to offset)

        public ShaderFile02(string filenamepath) : base(File.ReadAllBytes(filenamepath)) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);

            int magic = ReadInt();
            if (magic != 0x32736376) {
                throw new ShaderParserException02($"wrong file id {magic:x}");
            }
            int version = ReadInt();
            if (version != 64) {
                throw new ShaderParserException02($"wrong version {version}, expecting 64");
            }

            if (vcsFiletype == FILETYPE.features_file) {
                featuresHeader = new DataBlockFeaturesHeader(databytes, 8);
                offset = featuresHeader.GetFileOffset();
            } else if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.ps_file
                   || vcsFiletype == FILETYPE.gs_file || vcsFiletype == FILETYPE.psrs_file) {
                vspsHeader = new DataBlockVsPsHeader(databytes, 8);
                offset += 36;
            } else {
                throw new ShaderParserException02($"can't parse this filetype: {vcsFiletype}");
            }

            int block_delim = ReadInt();
            if (block_delim != 17) {
                throw new ShaderParserException02($"unexpected value for block_delom = {block_delim}, expecting 17");
            }
            int sfBlockCount = ReadInt();
            for (int i = 0; i < sfBlockCount; i++) {
                DataBlockSfBlock nextSfBlock = new(databytes, offset);
                sfBlocks.Add(nextSfBlock);
                offset = nextSfBlock.GetFileOffset();
            }
            // always 472 bytes
            int compatBlockCount = ReadInt();
            for (int i = 0; i < compatBlockCount; i++) {
                CompatibilityBlock nextCompatibilityBlock = new(databytes, offset);
                compatibilityBlocks.Add(nextCompatibilityBlock);
                offset += 472;

            }
            // always 152 bytes
            int dBlockCount = ReadInt();
            for (int i = 0; i < dBlockCount; i++) {
                DBlock nextDBlock = new(databytes, offset);
                dBlocks.Add(nextDBlock);
                offset += 152;
            }

            // always 472 bytes
            int unknownBlockCount = ReadInt();
            for (int i = 0; i < unknownBlockCount; i++) {
                UnknownBlock nextUnknownBlock = new(databytes, offset);
                unknownBlocks.Add(nextUnknownBlock);
                offset += 472;
            }

            int paramBlockCount = ReadInt();
            for (int i = 0; i < paramBlockCount; i++) {
                ParamBlock nextParamBlock = new(databytes, offset);
                paramBlocks.Add(nextParamBlock);
                offset = nextParamBlock.GetFileOffset();
            }

            // always 280 bytes
            int mipmapBlockCount = ReadInt();
            for (int i = 0; i < mipmapBlockCount; i++) {
                MipmapBlock nextMipmapBlock = new(databytes, offset);
                mipmapBlocks.Add(nextMipmapBlock);
                offset += 280;
            }

            int bufferBlockCount = ReadInt();
            for (int i = 0; i < bufferBlockCount; i++) {
                BufferBlock nextBufferBlock = new(databytes, offset);
                bufferBlocks.Add(nextBufferBlock);
                offset = nextBufferBlock.GetFileOffset();
            }

            if (vcsFiletype == FILETYPE.features_file || vcsFiletype == FILETYPE.vs_file) {
                int sybmolsBlockCount = ReadInt();
                for (int i = 0; i < sybmolsBlockCount; i++) {
                    SymbolsBlock nextSymbolsBlock = new(databytes, offset);
                    symbolBlocks.Add(nextSymbolsBlock);
                    offset = nextSymbolsBlock.GetFileOffset();
                }
            }

            List<long> zframeIDs = new();
            int zframesCount = ReadInt();
            for (int i = 0; i < zframesCount; i++) {
                zframeIDs.Add(ReadLong());
            }

            foreach (long zframeID in zframeIDs) {
                zframesLookup.Add(zframeID, ReadInt());
            }
        }

        public void PrintDatabyteAnalysis() {
            OutputWriteLine("shaderfile hello");
            featuresHeader.PrintSummary();
        }


        public void WriteZFrameToFile(int zframeIndex, string outputdir) {
            long zframeId = zframesLookup.ElementAt(zframeIndex).Key;
            // string outputFilename = $"{filename[0..^4]}-ZFRAME{zframeId:x08}.txt";
            string outputFilename = GetZframeTxtFilename((uint) zframeId, filename);
            string outputFilenamepath = @$"{outputdir}\{outputFilename}";
            byte[] uncompressedZframe = GetDecompressedZFrameByIndex(zframeIndex);
            DataReaderZFrameByteAnalysis02 zFrameParser = new(uncompressedZframe, vcsFiletype);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.ParseFile();
            sw.Flush();
            sw.Close();
        }

        public void WriteAllZFramesToHtml(string outputDir, bool includeGlslSources) {
            for (int i = 0; i < zframesLookup.Count; i++) {
                WriteZFrameToHtml(i, outputDir, includeGlslSources);
            }
        }

        public void WriteZFrameToHtml(int zframeIndex, string outputDir, bool includeGlslSources) {
            long zframeId = zframesLookup.ElementAt(zframeIndex).Key;
            // string outputFilename = $"{filename[0..^4]}-ZFRAME{zframeId:x08}.txt";
            string outputFilename = GetZframeHtmlFilename((uint) zframeId, filename);
            string outputFilenamepath = @$"{outputDir}\{outputFilename}";

            byte[] uncompressedZframe = GetDecompressedZFrameByIndex(zframeIndex);
            DataReaderZFrameByteAnalysis02 zFrameParser = new(uncompressedZframe, vcsFiletype);

            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.RequestGlslFileSave(outputDir);

            string htmlHeader = GetHtmlHeader(outputFilename, outputFilename[0..^5]);
            sw.WriteLine($"{htmlHeader}");
            zFrameParser.ParseFile();
            sw.WriteLine($"{GetHtmlFooter()}");
            sw.Flush();
            sw.Close();
        }

        public int GetZFrameCount() {
            return zframesLookup.Count;
        }

        public long GetZFrameIdByIndex(int zframeIndex) {
            return zframesLookup.ElementAt(zframeIndex).Key;
        }

        public byte[] GetDecompressedZFrameByIndex(int zframeIndex) {
            var zframeBlock = zframesLookup.ElementAt(zframeIndex);
            offset = zframeBlock.Value;
            uint delim = ReadUInt();
            if (delim != 0xfffffffd) {
                throw new ShaderParserException02("unexpected zframe delimiter");
            }
            int uncompressed_length = ReadInt();
            int compressed_length = ReadInt();

            byte[] compressedZframe = ReadBytes(compressed_length);

            using var decompressor = new Decompressor();
            decompressor.LoadDictionary(getZFrameDictionary());

            Span<byte> zframeUncompressed = decompressor.Unwrap(compressedZframe);
            if (zframeUncompressed.Length != uncompressed_length) {
                throw new ShaderParserException02("zframe length mismatch!");
            }
            // unsure if we want to do this
            // decompressor.Dispose();
            return zframeUncompressed.ToArray();
        }

        public static FILETYPE GetVcsFileType(string filenamepath) {
            if (filenamepath.EndsWith("features.vcs")) {
                return FILETYPE.features_file;
            }
            if (filenamepath.EndsWith("vs.vcs")) {
                return FILETYPE.vs_file;
            }
            if (filenamepath.EndsWith("ps.vcs")) {
                return FILETYPE.ps_file;
            }
            if (filenamepath.EndsWith("psrs.vcs")) {
                return FILETYPE.psrs_file;
            }
            if (filenamepath.EndsWith("gs.vcs")) {
                return FILETYPE.gs_file;
            }
            throw new ShaderParserException02($"don't know what this file is {filenamepath}");
        }
    }





}









