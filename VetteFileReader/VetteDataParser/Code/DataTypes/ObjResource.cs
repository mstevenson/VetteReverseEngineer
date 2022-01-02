using System;
using MacResourceFork;

namespace Vette
{
	[Serializable]
	public struct ObjResource : IResource
	{
		public int Id { get; set; }
		public string Name { get; set; }
		
		public int dataLength;
		public VertexArray vertices;
		public PolygonArray polygons;
		
		public void Parse(BinaryReaderBigEndian reader)
		{
			dataLength = reader.ReadUInt16();
			vertices = VertexArray.Parse(reader);
			polygons = PolygonArray.Parse(reader);
		}
	}
}

