using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct PolygonArray
    {
        public int polyCount;
        public Polygon[] polys;

        public static PolygonArray Parse(ref ReadOnlySpan<byte> span)
        {
            var p = new PolygonArray();
			
            p.polyCount = ReadInt16(ref span) + 1;
            p.polys = new Polygon[p.polyCount];

            for (int i = 0; i < p.polyCount; i++)
            {
                p.polys[i] = Polygon.Parse(ref span);
            }

            return p;
        }
    }
}