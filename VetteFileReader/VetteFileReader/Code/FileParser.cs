using System;
using System.IO;

namespace VetteFileReader
{
	public enum Endianness {
		Big,
		Little
	}

	public class FileParser
	{
		public static T Parse<T> (string dataPath)
			where T : IParsable, new()
		{
			using (var reader = new BinaryReaderBigEndian (File.Open (dataPath, FileMode.Open)))
			{
				T obj = new T ();
				obj.Parse (reader);
				return obj;
			}
		}

//		public static int ReadUInt16 (BinaryReader reader, Endianness endianness = Endianness.Big)
//		{
//			var bytes = new byte[2];
//			reader.Read (bytes, 0, 2);
//			if (endianness == Endianness.Big) {
//				int i = (bytes[0] << 8) | bytes[1];
//				return i;
//			} else {
//				int i = bytes[0] | (bytes[1] << 8);
//				return i;
//			}
//		}
//
//		public static int ReadInt16 (BinaryReader reader, Endianness endianness = Endianness.Big)
//		{
//			var bytes = new byte[2];
//			reader.Read (bytes, 0, 2);
//			if (endianness == Endianness.Big) {
//				int i = (bytes[0] << 8) | bytes[1];
//				return i;
//			} else {
//				int i = bytes[0] | (bytes[1] << 8);
//				return i;
//			}
//		}
	}
}

