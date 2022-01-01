using System.Collections.Generic;
using MacResourceFork;

namespace VetteFileReader
{
	public class StreetNamesResource : IResource
	{
		public List<string> names = new List<string>();
		
		public void Parse(BinaryReaderBigEndian reader)
		{
			while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
			{
				var name = reader.ReadString();
				names.Add(name);
			}
		}
	}
}
