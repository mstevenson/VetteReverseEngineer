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
				
				// TODO why is this check required in order to parse correctly?
				// Do I need to do something for big endian?
				// The raw data always has 11 bytes preceded by 0x0B
				if (length != 0x0B)
				{
					continue;
				}
				var bytes = reader.ReadBytes(length);
				var streetName = Encoding.GetEncoding(10000).GetString(bytes);
				names.Add(streetName);
			}
		}
	}
}
