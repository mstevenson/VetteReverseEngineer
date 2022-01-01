using System.Collections.Generic;

namespace VetteFileReader
{
	public class StreetNames : IResource
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
