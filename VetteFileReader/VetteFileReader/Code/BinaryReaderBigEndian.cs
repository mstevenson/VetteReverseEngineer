using System;
using System.IO;

namespace VetteFileReader
{
	public class BinaryReaderBigEndian : BinaryReader
	{
		// http://stackoverflow.com/questions/8620885/c-sharp-binary-reader-in-big-endian

		byte[] a16 = new byte[2];
		byte[] a32 = new byte[4];
		byte[] a64 = new byte[8];

		public BinaryReaderBigEndian (System.IO.Stream stream) : base(stream) {}

		public new Int16 ReadInt16 ()
		{
			base.Read (a16, 0, 2);
			Array.Reverse (a16);
			return BitConverter.ToInt16 (a16, 0);
		}

		public new Int32 ReadInt32 ()
		{
			var a32 = base.ReadBytes (4);
			Array.Reverse (a32);
			return BitConverter.ToInt32 (a32,0);
		}

		public new Int64 ReadInt64 ()
		{
			base.Read (a64, 0, 8);
			Array.Reverse (a64);
			return BitConverter.ToInt64 (a64, 0);
		}

		public new UInt16 ReadUInt16 ()
		{
			base.Read (a16, 0, 2);
			Array.Reverse (a16);
			return BitConverter.ToUInt16 (a16, 0);
		}

		public new UInt32 ReadUInt32 ()
		{
			base.Read (a32, 0, 2);
			Array.Reverse (a32);
			return BitConverter.ToUInt32 (a32, 0);
		}

	}
}

