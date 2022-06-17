using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct Road
    {
        public int roadType;

        // Unknown flags
        public int a;
        public int b;
        public int c;
        public int d;
        public int e;
        public int f;
        
        public static Road Parse(ref ReadOnlySpan<byte> span)
        {
            var r = new Road
            {
                roadType = ReadUInt16(ref span),
                // Flags of some kind
                a = ReadByte(ref span),
                b = ReadByte(ref span),
                c = ReadByte(ref span),
                d = ReadByte(ref span),
                e = ReadByte(ref span),
                f = ReadByte(ref span)
            };

            return r;
        }
    }
}