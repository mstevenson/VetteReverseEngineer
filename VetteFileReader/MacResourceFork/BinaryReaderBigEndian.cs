using System.Buffers.Binary;

namespace MacResourceFork
{
	public static class BinaryReaderBigEndian
	{
		public static short ReadInt16(ref ReadOnlySpan<byte> span)
		{
			var value = BinaryPrimitives.ReadInt16BigEndian(span);
			span = span[sizeof(short)..];
			return value;
		}

		public static int ReadInt32(ref ReadOnlySpan<byte> span)
		{
			var value = BinaryPrimitives.ReadInt32BigEndian(span);
			span = span[sizeof(int)..];
			return value;
		}

		public static long ReadInt64(ref ReadOnlySpan<byte> span)
		{
			var value = BinaryPrimitives.ReadInt64BigEndian(span);
			span = span[sizeof(long)..];
			return value;
		}

		public static ushort ReadUInt16(ref ReadOnlySpan<byte> span)
		{
			var value = BinaryPrimitives.ReadUInt16BigEndian(span);
			span = span[sizeof(ushort)..];
			return value;
		}

		public static uint ReadUInt32(ref ReadOnlySpan<byte> span)
		{
			var value = BinaryPrimitives.ReadUInt32BigEndian(span);
			span = span[sizeof(uint)..];
			return value;
		}
		
		public static byte ReadByte(ref ReadOnlySpan<byte> span)
		{
			var value = span[0];
			span = span[1..];
			return value;
		}
		
		public static ReadOnlySpan<byte> ReadBytes(ref ReadOnlySpan<byte> span, int count)
		{
			var value = span[..count];
			span = span[count..];
			return value;
		}
	}
}

