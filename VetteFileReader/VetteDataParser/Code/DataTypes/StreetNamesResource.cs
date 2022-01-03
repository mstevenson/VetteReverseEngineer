using System;
using System.Collections.Generic;
using System.Text;
using MacResourceFork;

namespace Vette
{
	[Serializable]
	public class StreetNamesResource : ResourceBase
	{
		public List<string> names = new List<string>();
		
		public override void Parse(BinaryReaderBigEndian reader)
		{
			// header
			reader.ReadByte();
			
			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				var length = reader.ReadByte();
				var bytes = reader.ReadBytes(length);
				var streetName = Encoding.GetEncoding(10000).GetString(bytes);
				names.Add(streetName);
			}
		}
	}
}
