using System;
using MacResourceFork;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct Polygon
    {
        public DrawingMode drawingMode;
        public int patternIndex;
        public int vertexCount;
        public int[] vertexIndices;

        private PatternName PatternName => (PatternName) patternIndex;

        public static Polygon Parse(ref ReadOnlySpan<byte> span)
        {
            var p = new Polygon
            {
                drawingMode = (DrawingMode) ReadInt16(ref span),
                patternIndex = ReadInt16(ref span),
                vertexCount = ReadInt16(ref span) + 1
            };

            p.vertexIndices = new int[p.vertexCount];

            // The list of vertex indices are shifted by 4 bits to the left for an unknown reason.
            var vertexIndexData = ReadBytes(ref span, 2 * p.vertexCount);
            var shiftedBytes = ByteShift.ShiftLeft(ref vertexIndexData, 4);

            for (int i = 0; i < p.vertexCount; i++)
            {
                var indexBytes = new[] {shiftedBytes[2 * i], shiftedBytes[2 * i + 1]};
                p.vertexIndices[i] = BitConverter.ToInt16(indexBytes, 0);
            }

            var delimiter = ReadBytes(ref span, 2);
            if (delimiter[0] != 0xFF || delimiter[1] != 0xFF)
            {
                // TODO exception
            }

            return p;
        }
    }
}