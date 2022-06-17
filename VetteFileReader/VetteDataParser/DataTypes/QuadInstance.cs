using System;
using MacResourceFork;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct QuadInstance
    {
        public int quadDescriptorIndex;
        public int flagA;
        public int flagB;
        
        public static QuadInstance Parse(ref ReadOnlySpan<byte> span)
        {
            return new QuadInstance
            {
                quadDescriptorIndex = ReadUInt16(ref span),
                flagA = ReadByte(ref span),
                flagB = ReadByte(ref span)
            };
        }
    }
}