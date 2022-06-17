using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct MapRow
    {
        public int headerA1;
        public int headerA2;
        public int headerB1;
        public int headerB2;
        public int headerC1;
        public int headerC2;
        public int headerD1;
        public int headerD2;
        
        public QuadInstance[] quads;
        
        public static MapRow Parse(ref ReadOnlySpan<byte> span)
        {
            var m = new MapRow();

            ReadByte(ref span);
            m.headerA1 = ReadByte(ref span);
            m.headerA2 = ReadByte(ref span);
            ReadBytes(ref span, 2);
            m.headerB1 = ReadByte(ref span);
            m.headerB2 = ReadByte(ref span);
            ReadBytes(ref span, 2);
            m.headerC1 = ReadByte(ref span);
            m.headerC2 = ReadByte(ref span);
            ReadBytes(ref span, 2);
            m.headerD1 = ReadByte(ref span);
            m.headerD2 = ReadByte(ref span);
            ReadByte(ref span);
            
            m.quads = new QuadInstance[48];
            
            for (int i = 0; i < m.quads.Length; i++)
            {
                var quad = QuadInstance.Parse(ref span);
                m.quads[i] = quad;
            }

            return m;
        }
    }
}