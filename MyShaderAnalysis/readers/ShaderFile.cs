using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyShaderAnalysis.utilhelpers.UtilHelpers;


namespace MyShaderAnalysis.readers {


    public enum FILETYPE {
        unknown, any, features_file, vs_file, ps_file, gs_file, psrs_file
    };


    public class ShaderFile : DataReader {

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
        List<ZFrameBlock> zframeBlocks = new();


        public ShaderFile(string filenamepath) : base(File.ReadAllBytes(filenamepath)) {
            this.filenamepath = filenamepath;
            filename = Path.GetFileName(filenamepath);
            vcsFiletype = GetVcsFileType(filenamepath);

            int magic = ReadInt();
            if (magic != 0x32736376) {
                throw new ShaderParserException($"wrong file id {magic:x}");
            }
            int version = ReadInt();
            if (version != 64) {
                throw new ShaderParserException($"wrong version {version}, expecting 64");
            }


            if (vcsFiletype == FILETYPE.features_file) {
                featuresHeader = new DataBlockFeaturesHeader(databytes, 8);
                offset = featuresHeader.GetFileOffset();
            }
             else if (vcsFiletype == FILETYPE.vs_file || vcsFiletype == FILETYPE.ps_file
                  || vcsFiletype == FILETYPE.gs_file || vcsFiletype == FILETYPE.psrs_file) {
                vspsHeader = new DataBlockVsPsHeader(databytes, 8);
                offset += 36;
            } else {
                throw new ShaderParserException($"can't parse this filetype: {vcsFiletype}");
            }

            int block_delim = ReadInt();
            if (block_delim != 17) {
                throw new ShaderParserException($"unexpected value for block_delom = {block_delim}, expecting 17");
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

            int sybmolsBlockCount = ReadInt();
            for (int i = 0; i < sybmolsBlockCount; i++) {
                SymbolsBlock nextSymbolsBlock = new(databytes, offset);
                symbolBlocks.Add(nextSymbolsBlock);
                offset = nextSymbolsBlock.GetFileOffset();
            }

            ShowBytes(4);

            //int zframesBlockCount = ReadInt();
            //for (int i = 0; i < zframesBlockCount; i++) {
            //    ZFrameBlock nextZFrameBlockk = new(databytes, offset);
            //}



        }








        private static FILETYPE GetVcsFileType(string filenamepath) {
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

            throw new ShaderParserException($"don't know what this file is {filenamepath}");
        }




    }





    public class ShaderParserException : Exception {
        public ShaderParserException() { }
        public ShaderParserException(string message) : base(message) {}
        public ShaderParserException(string message, Exception innerException) : base(message, innerException) {}
    }



}


