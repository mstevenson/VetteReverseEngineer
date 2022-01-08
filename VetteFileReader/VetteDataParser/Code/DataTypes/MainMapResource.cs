using System;
using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class MainMapResource : ResourceBase
    {
        public int fileLength;
        public List<MapRow> rows = new List<MapRow>();
        
        public override void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                // Main Map rows go from south to north. Columns go from west to east.
                var row = MapRow.Parse(reader);
                rows.Add(row);
            }
        }
    }
}