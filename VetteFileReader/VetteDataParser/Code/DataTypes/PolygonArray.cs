using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class PolygonArray
    {
        public int polyCount;
        public Polygon[] polys;

        public static PolygonArray Parse(BinaryReaderBigEndian reader)
        {
            var p = new PolygonArray();
			
            p.polyCount = reader.ReadInt16() + 1;
            p.polys = new Polygon[p.polyCount];

            for (int i = 0; i < p.polyCount; i++)
            {
                p.polys[i] = Polygon.Parse(reader);
            }

            return p;
        }
    }
}