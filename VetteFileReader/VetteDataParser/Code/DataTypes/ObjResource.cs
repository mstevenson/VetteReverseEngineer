using System;
using System.IO;
using MacResourceFork;

namespace VetteFileReader
{
	public struct ObjResource : IResource
	{
		public int fileLength;
		public VertexArray vertices;
		public PolygonArray polygons;

		public void Parse (BinaryReaderBigEndian reader)
		{
			fileLength = reader.ReadUInt16();
			vertices = VertexArray.Parse(reader);
			polygons = PolygonArray.Parse(reader);
		}
	}
	
	public struct VertexArray
	{
		public int vertexCount;
		// vert1: pivot point
		// vert2: x scale
		// vert3: z scale
		// vert4: y scale
		// vertN: mesh vertex
		public Vector[] vertices;

		public static VertexArray Parse(BinaryReaderBigEndian reader)
		{
			var v = new VertexArray();
			
			v.vertexCount = reader.ReadUInt16() + 1; // the length starts at 0 to indicate one vertex
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

	// each vertex begins with FF FF and is 6 bytes of data

	public struct PolygonArray
	{
		public int polyCount;
		public Polygon[] polys;

		public static PolygonArray Parse(BinaryReaderBigEndian reader)
		{
			var p = new PolygonArray();
			
			p.polyCount = reader.ReadInt16() + 1; // length of 0 indicates one quad
			p.polys = new Polygon[p.polyCount];

			for (int i = 0; i < p.polyCount; i++)
			{
				p.polys[i] = Polygon.Parse(reader);
			}

			return p;
		}
	}

	public enum DrawMode
	{
		Triangle = 0, // ???
		Quad = 1,
		Line = 4,
	}

	public struct Polygon
	{
		public DrawMode drawMode;
		public int unknown;
		public int vertexCount;
		public int[] vertexIndices;

		public static Polygon Parse(BinaryReaderBigEndian reader)
		{
			var p = new Polygon
			{
				drawMode = (DrawMode)reader.ReadInt16(),
				unknown = reader.ReadInt16(),
				vertexCount = reader.ReadInt16() + 1 // lengths always start with 0 to represent 1 element
			};

			p.vertexIndices = new int[p.vertexCount];

			// The list of vertex indices are shifted by 4 bits to the left for an unknown reason.
			byte[] vertexIndexData = reader.ReadBytes(2 * p.vertexCount);
			byte[] shiftedBytes = vertexIndexData.ShiftLeft(4);

			for (int i = 0; i < p.vertexCount; i++)
			{
				var indexBytes = new[] { shiftedBytes[(2*i)], shiftedBytes[(2*i)+1]};
				p.vertexIndices[i] = BitConverter.ToInt16(indexBytes, 0);
			}

			var delimiter = reader.ReadBytes(2);
			if (delimiter[0] != 0xFF || delimiter[1] != 0xFF)
			{
				// TODO exception
			}

			return p;
		}
	}
}

