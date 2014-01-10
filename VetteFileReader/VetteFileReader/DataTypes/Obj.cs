using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace VetteFileReader
{
	public struct Obj : IParsable
	{
		public int fileLength;
		public VertexArray vertices;
		public PolygonArray polygons;

		public void Parse (BinaryReaderBigEndian reader)
		{
			fileLength = reader.ReadUInt16 ();

			vertices = new VertexArray ();
			vertices.Parse (reader);

			polygons = new PolygonArray ();
			polygons.Parse (reader);
		}
	}

	public struct VertexArray : IParsable
	{
		public int vertexCount;
		// vert1: pivot point
		// vert2: x scale
		// vert3: z scale
		// vert4: y scale
		// vertN: mesh vertex
		public Vertex[] vertices;

		public void Parse (BinaryReaderBigEndian reader)
		{
			vertexCount = reader.ReadUInt16 () + 1; // the length starts at 0 to indicate one vertex
			vertices = new Vertex[vertexCount];

			for (int i = 0; i < vertexCount; i++) {
				vertices [i] = new Vertex ();
				vertices [i].Parse (reader);
			}
		}
	}

	// each vertex begins with FF FF and is 6 bytes of data
	public struct Vertex : IParsable
	{
		public int x;
		public int y; // y is inverted
		public int z;

		public void Parse (BinaryReaderBigEndian reader)
		{
			// 0xFFFF padding at beginning
			reader.ReadBytes (2);
			x = reader.ReadInt16 ();
			y = reader.ReadInt16 ();
			z = reader.ReadInt16 ();
		}
	}

	public struct PolygonArray : IParsable
	{
		public int polyCount;
		public Polygon[] polys;

		public void Parse (BinaryReaderBigEndian reader)
		{
			polyCount = reader.ReadInt16 () + 1; // length of 0 indicates one quad
			polys = new Polygon[polyCount];

			for (int i = 0; i < polyCount; i++) {
				polys [i] = new Polygon ();
				polys [i].Parse (reader);
			}
		}
	}

	public enum DrawMode {
		Triangle = 0, // ???
		Quad = 1,
		Line = 4,
	}

	public struct Polygon : IParsable
	{
		public DrawMode drawMode;
		public int unknown;
		public int vertexCount;
		public int[] vertexIndices;

		public void Parse (BinaryReaderBigEndian reader)
		{
			drawMode = (DrawMode)reader.ReadInt16 ();

			unknown = reader.ReadInt16 ();

			vertexCount = reader.ReadInt16 () + 1; // lengths always start with 0 to represent 1 element
			vertexIndices = new int[vertexCount];

			// The list of vertex indices are shifted by 4 bits to the left for an unknown reason.
			byte[] vertexIndexData = reader.ReadBytes (2 * vertexCount);
			byte[] shiftedBytes = vertexIndexData.ShiftLeft (4);

			for (int i = 0; i < vertexCount; i++) {
				var indexBytes = new byte[] { shiftedBytes[(2*i)], shiftedBytes[(2*i)+1]};
				vertexIndices[i] = BitConverter.ToInt16 (indexBytes, 0);
			}

			var terminator = reader.ReadBytes (2);
			if (terminator[0] != 0xFF || terminator[1] != 0xFF) {
				// TODO exception
			}
		}
	}
}

