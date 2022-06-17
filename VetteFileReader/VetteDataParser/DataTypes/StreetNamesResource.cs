using System;
using System.Collections.Generic;
using System.Text;
using MacResourceFork;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
	[Serializable]
	public class StreetNamesResource : ResourceBase
	{
		public List<string> names = new();
		
		public override void Parse(ref ReadOnlySpan<byte> span)
		{
			// header
			ReadUInt16(ref span);
			
			while (span.Length > 0)
			{
				var length = ReadByte(ref span);
				var bytes = ReadBytes(ref span, length);
				var streetName = Encoding.GetEncoding(10000).GetString(bytes);
				names.Add(streetName);
			}
		}
	}
}
