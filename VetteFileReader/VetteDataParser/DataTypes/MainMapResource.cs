using System;
using System.Collections.Generic;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public class MainMapResource : ResourceBase
    {
        public int fileLength;
        public List<MapRow> rows = new();
        
        public override void Parse(ref ReadOnlySpan<byte> span)
        {
            fileLength = ReadUInt16(ref span);
            
            while (span.Length > 0)
            {
                // Main Map rows go from south to north. Columns go from west to east.
                var row = MapRow.Parse(ref span);
                rows.Add(row);
            }
        }
    }
}