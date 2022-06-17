using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct Pattern
    {
        // 8x8 grid
        public byte[] pixelColorIndexes;
        
        private const int PixelCount = 64;
        
        public static Pattern Parse(ref ReadOnlySpan<byte> span)
        {
            var pattern = new Pattern
            {
                pixelColorIndexes = new byte[PixelCount]
            };
            
            // Need to read 4 bits
            
            for (int i = 0; i < PixelCount; i++)
            {
                if (i % 2 != 0)
                {
                    continue;
                }
                
                // Unpack into two half bytes representing one of 16 colors
                var b = ReadByte(ref span);
                pattern.pixelColorIndexes[i] = (byte)((b & 0xF0) >> 4);
                pattern.pixelColorIndexes[i + 1] = (byte)(b & 0x0F);
            }

            return pattern;
        }
    }
}