using System;

namespace MyShaderAnalysis.vcsparsing {

    public class ZDataBlock : ShaderDataBlock {

        public int blockId;
        public int h0;
        public int h1;
        public int h2;
        public byte[] dataload = null;

        public ZDataBlock(ShaderDataReader datareader, int start, int blockId) : base(datareader, start) {
            this.blockId = blockId;
            h0 = datareader.ReadInt();
            h1 = datareader.ReadInt();
            h2 = datareader.ReadInt();
            if (h0 > 0) {
                dataload = datareader.ReadBytes(h0 * 4);
            }
        }
    }


    public abstract class GpuSource : ShaderDataBlock {
        public int sourceId;
        public int offset;
        public byte[] sourcebytes = Array.Empty<byte>();
        public byte[] editorRefId;
        protected GpuSource(ShaderDataReader datareader, int start, int sourceId) : base(datareader, start) {
            this.sourceId = sourceId;
        }
        public string GetEditorRefIdAsString() {
            string stringId = ShaderDataReader.BytesToString(editorRefId);
            stringId = stringId.Replace(" ", "").ToLower();
            return stringId;
        }

        public abstract string GetBlockName();
    }


    public class GlslSource : GpuSource {
        public int arg0; // always 3
        // offset2, if present, always observes offset2 == offset + 8
        // offset2 can also be interpreted as the source-size
        public int offset2 = -1;
        public GlslSource(ShaderDataReader datareader, int start, int sourceId) : base(datareader, start, sourceId) {
            offset = datareader.ReadInt();
            if (offset > 0) {
                arg0 = datareader.ReadInt();
                //uint glslDelim = datareader.ReadUInt();
                //if (glslDelim != 0x00000003) {
                //    throw new ShaderParserException($"Unexpected Glsl source id {glslDelim:x08}");
                //}
                offset2 = datareader.ReadInt();
                sourcebytes = datareader.ReadBytes(offset2);
            }
            editorRefId = datareader.ReadBytes(16);
        }
        public override string GetBlockName() {
            return $"GLSL-SOURCE[{sourceId}]";
        }
    }

    public class DxilSource : GpuSource {
        public int arg0; // always 3
        public int arg1; // always 0xFFFF or 0xFFFE
        public int headerBytes;

        public DxilSource(ShaderDataReader datareader, int start, int sourceId) : base(datareader, start, sourceId) {
            offset = datareader.ReadInt();
            if (offset > 0) {
                arg0 = datareader.ReadInt16();
                arg1 = (int)datareader.ReadUInt16();
                uint dxilDelim = datareader.ReadUInt16();
                if (dxilDelim != 0xFFFE) {
                    throw new ShaderParserException($"Unexpected DXIL source id {dxilDelim:x08}");
                }

                headerBytes = (int) datareader.ReadUInt16() * 4; // size is given as a 4-byte count
                sourcebytes = datareader.ReadBytes(offset-8); // size of source equals offset-8
            }
            editorRefId = datareader.ReadBytes(16);
        }
        public override string GetBlockName() {
            return $"DXIL-SOURCE[{sourceId}]";
        }
    }

    /*
     * DXBC sources don't show the same kind of headers like Glsl or DXIL sources
     * It only has one header, the offset (which in this case happens to be equal to the source size)
     */
    public class DxbcSource : GpuSource {
        public DxbcSource(ShaderDataReader datareader, int start, int sourceId) : base(datareader, start, sourceId) {
            this.sourceId = sourceId;
            this.offset = datareader.ReadInt();
            if (offset>0) {
                sourcebytes = datareader.ReadBytes(offset);
            }
            editorRefId = datareader.ReadBytes(16);
        }
        public override string GetBlockName() {
            return $"DXBC-SOURCE[{sourceId}]";
        }
    }
}









