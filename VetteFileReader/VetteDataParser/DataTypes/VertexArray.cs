using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct VertexArray
    {
        public int vertexCount;
        
        // The first four verts have an unknown effect.
        // When changing them on car models, parts of the car disappear.
        public Vector[] vertices;
        
        public static VertexArray Parse(ref ReadOnlySpan<byte> span)
        {
            var v = new VertexArray();
			
            v.vertexCount = ReadUInt16(ref span) + 1; // number of vertices
            v.vertices = new Vector[v.vertexCount];
			
            for (int i = 0; i < v.vertexCount; i++)
            {
                // 0xFFFF padding at beginning
                ReadBytes(ref span, 2);
                v.vertices[i] = Vector.Parse(ref span); // y axis is inverted
            }

            return v;
        }
    }
}