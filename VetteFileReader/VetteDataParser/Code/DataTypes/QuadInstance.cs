using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public struct QuadInstance
    {
        public int quadDescriptorIndex;
        public int flagA;
        public int flagB;
        
        public static QuadInstance Parse(BinaryReaderBigEndian reader)
        {
            return new QuadInstance
            {
                quadDescriptorIndex = reader.ReadUInt16(),
                flagA = reader.ReadByte(),
                flagB = reader.ReadByte()
            };
        }
    }
}