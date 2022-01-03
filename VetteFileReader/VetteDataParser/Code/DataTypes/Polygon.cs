using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class Polygon
    {
        public DrawingMode drawingMode;
        public int patternIndex;
        public int vertexCount;
        public int[] vertexIndices;

        private PatternName PatternName => (PatternName)patternIndex;
        
        public static Polygon Parse(BinaryReaderBigEndian reader)
        {
            var p = new Polygon
            {
                drawingMode = (DrawingMode)reader.ReadInt16(),
                patternIndex = reader.ReadInt16(),
                vertexCount = reader.ReadInt16() + 1
            };

            p.vertexIndices = new int[p.vertexCount];

            // The list of vertex indices are shifted by 4 bits to the left for an unknown reason.
            byte[] vertexIndexData = reader.ReadBytes(2 * p.vertexCount);
            byte[] shiftedBytes = vertexIndexData.ShiftLeft(4);

            for (int i = 0; i < p.vertexCount; i++)
            {
                var indexBytes = new[] { shiftedBytes[2*i], shiftedBytes[2*i+1]};
                p.vertexIndices[i] = BitConverter.ToInt16(indexBytes, 0);
            }

            var delimiter = reader.ReadBytes(2);
            if (delimiter[0] != 0xFF || delimiter[1] != 0xFF)
            {
                // TODO exception
            }

            return p;
        }
    }
}