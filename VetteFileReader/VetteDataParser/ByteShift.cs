namespace Vette
{
    // http://stackoverflow.com/questions/1275572/bit-shifting-n-bits

    public static class ByteShift
    {
        public static byte[] ShiftLeft(ref ReadOnlySpan<byte> value, int bitCount)
        {
            var temp = new byte[value.Length];
            value.CopyTo(temp);
            
            if (bitCount >= 8)
            {
                value[(bitCount / 8)..].CopyTo(temp.AsSpan()[..^(bitCount / 8)]);
                // Array.Copy(value, bitcount / 8, temp, 0, temp.Length - (bitcount / 8));
            }
            else
            {
                value.CopyTo(temp);
                // Array.Copy(value, temp, temp.Length);
            }
            if (bitCount % 8 != 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] <<= bitCount % 8;
                    if (i < temp.Length - 1)
                    {
                        temp[i] |= (byte)(temp[i + 1] >> 8 - bitCount % 8);
                    }
                }
            }
            return temp;
        }
        
        public static byte[] ShiftRight(ref ReadOnlySpan<byte> value, int bitCount)
        {
            var temp = new byte[value.Length];
            if (bitCount >= 8)
            {
                value.CopyTo(temp.AsSpan()[..^(bitCount / 8)]);
                // Array.Copy(value, 0, temp, bitcount / 8, temp.Length - (bitcount / 8));
            }
            else
            {
                value.CopyTo(temp);
                // Array.Copy(value, temp, temp.Length);
            }
            if (bitCount % 8 != 0)
            {
                for (int i = temp.Length - 1; i >= 0; i--)
                {
                    temp[i] >>= bitCount % 8;
                    if (i > 0)
                    {
                        temp[i] |= (byte)(temp[i - 1] << 8 - bitCount % 8);
                    }
                }
            }
            return temp;
        }
    }
}

