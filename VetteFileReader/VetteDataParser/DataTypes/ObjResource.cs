using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
	[Serializable]
	public class ObjResource : ResourceBase
	{
		public int dataLength;
		public VertexArray vertices;
		public PolygonArray polygons;
		
		public override void Parse(ref ReadOnlySpan<byte> span)
		{
			dataLength = ReadUInt16(ref span);
			vertices = VertexArray.Parse(ref span);
			polygons = PolygonArray.Parse(ref span);
		}
	}
}

