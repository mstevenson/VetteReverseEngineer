using System;
using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
	public struct Streets : IParsable
	{
//		public List<string> streets = new List<string> ();

		const long dataStart = 0x01;
		const int nameLength = 11;

		public void Parse (BinaryReaderBigEndian stream)
		{
//			long position = dataStart;
//			while (position < stream.Length) {
//				Char[] chars = new Char[nameLength];
//				for (int i = 0; i < nameLength; i++) {
//					chars[i] = accessor.ReadChar (position);
//				}
//				streets.Add (new string (chars));
//				position += nameLength + 1;
//			}
		}
	}
}

