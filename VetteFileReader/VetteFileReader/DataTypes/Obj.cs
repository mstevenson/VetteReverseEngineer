using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace VetteFileReader
{
	public struct Obj : IParsable
	{
		public int fileLength;
		public Vertices vertices;
		public GeoElements geoElements;

		public void Parse (BinaryReaderBigEndian reader)
		{
			fileLength = reader.ReadUInt16 ();

			vertices = new Vertices ();
			vertices.Parse (reader);

			geoElements = new GeoElements ();
			geoElements.Parse (reader);
		}
	}

	public struct Vertices : IParsable
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

	public struct GeoElements : IParsable
	{
		public int geoCount;
		public Geo[] geos;

		public void Parse (BinaryReaderBigEndian reader)
		{
			geoCount = reader.ReadInt16 () + 1; // length of 0 indicates one quad
			geos = new Geo[geoCount];

			for (int i = 0; i < geoCount; i++) {
				geos [i] = new Geo ();
				geos [i].Parse (reader);
			}
		}
	}

	public enum DrawMode {
		Triangle = 0, // ???
		Quad = 1,
		Line = 4,
	}

	public struct Geo : IParsable
	{
		public DrawMode drawMode;
		public Poly line;

		public void Parse (BinaryReaderBigEndian reader)
		{
			drawMode = (DrawMode)reader.ReadInt16 ();

			switch (drawMode) {
			case DrawMode.Line:
				line = new Poly ();
				line.Parse (reader);
				Console.WriteLine (line.vertIndexA);
				break;
			case DrawMode.Quad:
				reader.ReadBytes (14);
				break;
			}

			//padding 0xFFFF
			reader.ReadBytes (2);
		}
	}

	public struct Poly : IParsable
	{
		public int unknown;
		public int unknown2; // seems often to be 0x0002 when drawing lines, like Hwy1 obj

		public int vertIndexA;
		public int vertIndexB;
		public int vertIndexC;

		public void Parse (BinaryReaderBigEndian reader)
		{
			unknown = reader.ReadInt16 ();
			unknown2 = reader.ReadInt16 ();

			// Now things get weird. There are 3 Int16s representing vertex indices that
			// form a line segment connecting vertices A-B-A, but these Int16s are shifted
			// by 4 bits to the left for an unknown reason.

			var bytes = reader.ReadBytes (6);
			var shiftedBytes = bytes.ShiftLeft (4);

			// Index of each vertex forming the line segment
			vertIndexA = BitConverter.ToInt16 (shiftedBytes, 0);
			vertIndexB = BitConverter.ToInt16 (shiftedBytes, 2);

			// When drawing lines, this is always the same value as vertIndexA so it can be ignored
			vertIndexC = BitConverter.ToInt16 (shiftedBytes, 4);
		}


		// http://stackoverflow.com/questions/1275572/bit-shifting-n-bits


	}
}

