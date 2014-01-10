using System;
using System.IO;

namespace VetteFileReader
{
	public interface IParsable
	{
		void Parse (BinaryReaderBigEndian reader);
	}
}

