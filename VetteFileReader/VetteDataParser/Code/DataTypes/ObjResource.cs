using System;
using MacResourceFork;

namespace Vette
{
	[Serializable]
	public class ObjResource : ResourceBase
	{
		public int dataLength;
		public VertexArray vertices;
		public PolygonArray polygons;
		
		public override void Parse(BinaryReaderBigEndian reader)
		{
			dataLength = reader.ReadUInt16();
			vertices = VertexArray.Parse(reader);
			polygons = PolygonArray.Parse(reader);
		}
	}
}

