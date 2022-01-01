using System.Collections.Generic;
using MacResourceFork;

namespace VetteFileReader
{
    public class PatternsResource : IResource
    {
        public List<Pattern> patterns = new List<Pattern>();
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var pattern = Pattern.Parse(reader);
                patterns.Add(pattern);
            }
        }
    }
    
    public struct Pattern
    {
        // 8x8 grid
        public int[] pixelColorIndexes;
        
        private const int PixelCount = 64;
        
        public static Pattern Parse(BinaryReaderBigEndian reader)
        {
            var pattern = new Pattern
            {
                pixelColorIndexes = new int[PixelCount]
            };
            
            // Need to read 4 bits
            
            for (int i = 0; i < PixelCount; i++)
            {
                if (i % 2 == 0)
                {
                    // Unpack into two half bytes representing one of 16 colors
                    var b = reader.ReadByte();
                    pattern.pixelColorIndexes[i] = b >> 1;
                    pattern.pixelColorIndexes[i + 1] = b & 0xF;
                }
            }

            return pattern;
        }
    }
}