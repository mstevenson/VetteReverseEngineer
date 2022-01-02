using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
	public class StreetNamesResource : IResource
	{
		public int Id { get; set; }
		public string Name { get; set; }
		
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
