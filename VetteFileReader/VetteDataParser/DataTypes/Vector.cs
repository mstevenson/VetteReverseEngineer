using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct Vector
    {
        public short x;
        public short y;
        public short z;
        
        public static Vector Parse(ref ReadOnlySpan<byte> span)
        {
            return new Vector
            {
                x = ReadInt16(ref span),
                y = ReadInt16(ref span),
                z = ReadInt16(ref span),
            };
        }
    }
}