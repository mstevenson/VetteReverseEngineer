using System;
namespace Vette
{
	// http://stackoverflow.com/questions/1275572/bit-shifting-n-bits

	public static class ByteArrayExtension
	{
		public static byte[] ShiftLeft (this byte[] value, int bitcount)
		{
			byte[] temp = new byte[value.Length];
			if (bitcount >= 8) {
				Array.Copy(value, bitcount / 8, temp, 0, temp.Length - (bitcount / 8));
			} else {
				Array.Copy(value, temp, temp.Length);
			}
			if (bitcount % 8 != 0) {
				for (int i = 0; i < temp.Length; i++) {
					temp[i] <<= bitcount % 8;
					if (i < temp.Length - 1) {
						temp[i] |= (byte)(temp[i + 1] >> 8 - bitcount % 8);
					}
				}
			}
			return temp;
		}
		
		public static byte[] ShiftRight (this byte[] value, int bitcount)
		{
			byte[] temp = new byte[value.Length];
			if (bitcount >= 8) {
				Array.Copy(value, 0, temp, bitcount / 8, temp.Length - (bitcount / 8));
			} else {
				Array.Copy(value, temp, temp.Length);
			}
			if (bitcount % 8 != 0) {
				for (int i = temp.Length - 1; i >= 0; i--) {
					temp[i] >>= bitcount % 8;
					if (i > 0) {
						temp[i] |= (byte)(temp[i - 1] << 8 - bitcount % 8);
					}
				}
			}
			return temp;
		}
	}
}

