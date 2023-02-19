using System;
using System.Globalization;
using System.IO;

namespace ValveResourceFormat.CompiledShader
{
    public delegate void HandleOutputWrite(string s);
    public class ShaderDataReader : BinaryReader
    {
        public HandleOutputWrite OutputWriter { get; set; }

        // pass an OutputWriter to direct output somewhere else, Console.Write is assigned by default
        public ShaderDataReader(Stream input, HandleOutputWrite outputWriter = null) : base(input)
        {
            OutputWriter = outputWriter ?? ((x) => { Console.Write(x); });
        }

        public byte ReadByteAtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var b0 = ReadByte();
            BaseStream.Position = savedPosition;
            return b0;
        }

        public uint ReadUInt16AtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            uint uint0 = ReadUInt16();
            BaseStream.Position = savedPosition;
            return uint0;
        }

        public int ReadInt16AtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var s0 = ReadInt16();
            BaseStream.Position = savedPosition;
            return s0;
        }

        public uint ReadUInt32AtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var uint0 = ReadUInt32();
            BaseStream.Position = savedPosition;
            return uint0;
        }

        public int ReadInt32AtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var int0 = ReadInt32();
            BaseStream.Position = savedPosition;
            return int0;
        }

        public float ReadSingleAtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var float0 = ReadSingle();
            BaseStream.Position = savedPosition;
            return float0;
        }

        public byte[] ReadBytesAtPosition(long ind, int len, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var bytes0 = ReadBytes(len);
            BaseStream.Position = savedPosition;
            return bytes0;
        }

        public string ReadBytesAsString(int len)
        {
            var bytes0 = ReadBytes(len);
            return ShaderUtilHelpers.BytesToString(bytes0);
        }

        public string ReadNullTermString()
        {
            var str = "";
            var b0 = ReadByte();
            while (b0 > 0)
            {
                str += (char)b0;
                b0 = ReadByte();
            }
            return str;
        }

        public string ReadNullTermStringAtPosition(long ind = 0, bool rel = true)
        {
            var savedPosition = BaseStream.Position;
            BaseStream.Position = rel ? BaseStream.Position + ind : ind;
            var str = ReadNullTermString();
            BaseStream.Position = savedPosition;
            return str;
        }

        public void ShowEndOfFile()
        {
            if (BaseStream.Position != BaseStream.Length)
            {
                throw new ShaderParserException("End of file not reached");
            }
            ShowByteCount();
            OutputWriteLine("EOF");
            BreakLine();
        }

        public void ShowBytesWithIntValue()
        {
            var intval = ReadInt32AtPosition();
            ShowBytes(4, breakLine: false);
            TabComment(intval.ToString(CultureInfo.InvariantCulture));
        }

        public void ShowByteCount(string message = null)
        {
            OutputWrite($"[{BaseStream.Position}]{(message != null ? " " + message : "")}\n");
        }

        public void ShowBytes(int len, string message = null, int tabLen = 4, bool use_slashes = true, bool breakLine = true)
        {
            ShowBytes(len, 32, message, tabLen, use_slashes, breakLine);
        }

        public void ShowBytes(int len, int breakLen, string message = null, int tabLen = 4, bool use_slashes = true, bool breakLine = true)
        {
            var bytes0 = ReadBytes(len);
            var byteString = ShaderUtilHelpers.BytesToString(bytes0, breakLen);
            OutputWrite(byteString);
            if (message != null)
            {
                TabComment(message, tabLen, use_slashes);
            }
            if (message == null && breakLine)
            {
                BreakLine();
            }
        }

        public void ShowBytesAtPosition(int ind, int len, int breakLen = 32, bool rel = true)
        {
            var bytes0 = ReadBytesAtPosition(ind, len, rel);
            var bytestr = ShaderUtilHelpers.BytesToString(bytes0, breakLen);
            OutputWriteLine(bytestr);
        }

        public void BreakLine()
        {
            OutputWriter("\n");
        }

        public void Comment(string message)
        {
            TabComment(message, 0, true);
        }

        public void TabComment(string message, int tabLen = 4, bool useSlashes = true)
        {
            OutputWriter($"{"".PadLeft(tabLen)}{(useSlashes ? "// " : "")}{message}\n");
        }

        public void OutputWrite(string text)
        {
            OutputWriter(text);
        }

        public void OutputWriteLine(string text)
        {
            OutputWriter(text + "\n");
        }
    }
}
