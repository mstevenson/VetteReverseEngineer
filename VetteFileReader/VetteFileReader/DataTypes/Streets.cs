using System;
using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
	public struct Streets : IParsable
	{
		public List<string> streetNames;

		public void Parse (BinaryReaderBigEndian reader)
		{
			streetNames = new List<string> ();

			while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
			{
				var name = reader.ReadString();
				streetNames.Add(name);
			}
		}
	}
}

