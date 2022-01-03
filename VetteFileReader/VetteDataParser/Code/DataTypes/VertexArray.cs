using System;
using System.IO;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class VertexArray
    {
        public int vertexCount;
        
        // The first four verts have an unknown effect.
        // When changing them on car models, parts of the car disappear.
        public Vector[] vertices;
        
        public static VertexArray Parse(BinaryReaderBigEndian reader)
        {
            var v = new VertexArray();
			
            v.vertexCount = reader.ReadUInt16() + 1; // number of vertices
            v.vertices = new Vector[v.vertexCount];
			
            for (int i = 0; i < v.vertexCount; i++)
            {
                // 0xFFFF padding at beginning
                reader.BaseStream.Seek(2, SeekOrigin.Current);
                v.vertices[i] = Vector.Parse(reader); // y axis is inverted
            }

            return v;
        }
    }
}