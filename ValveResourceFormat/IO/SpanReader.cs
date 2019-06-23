using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace ValveResourceFormat.IO
{
    /// <summary>
    /// Wraps a Span and provides convenient read functionality for strings and primitive types.
    /// Based on System.IO.BinaryReader.
    /// </summary>
    public ref struct SpanReader
    {
        public ReadOnlySpan<byte> BaseSpan { get; }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanReader"/> struct.
        /// Reads primitive data types as binary values.
        /// </summary>
        /// <param name="input">The input span.</param>
        public SpanReader(ReadOnlySpan<byte> input)
        {
            BaseSpan = input;
            Position = 0;
        }

        public byte ReadByte() => InternalReadByte();
        public sbyte ReadSByte() => (sbyte)InternalReadByte();
        public bool ReadBoolean() => InternalReadByte() != 0;
        public short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(InternalRead(2));
        public ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(InternalRead(2));
        public int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(InternalRead(4));
        public uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(InternalRead(4));
        public long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(InternalRead(8));
        public ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(InternalRead(8));
        public float ReadSingle() => BitConverter.ToSingle(InternalRead(4).ToArray(), 0); // TODO: BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian is in netstandard2.1
        public double ReadDouble() => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalRead(8)));

        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return InternalRead(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySpan<byte> InternalRead(int numBytes)
        {
            return BaseSpan.Slice(Position += numBytes, numBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte InternalReadByte()
        {
            return BaseSpan[Position++];
        }
    }
}
