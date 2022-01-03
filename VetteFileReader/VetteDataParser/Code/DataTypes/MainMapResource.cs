using System;
using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class MainMapResource : ResourceBase
    {
        public int fileLength;
        public List<MapChunk> chunks = new List<MapChunk>();
        
        public override void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var chunk = MapChunk.Parse(reader);
                chunks.Add(chunk);
            }
        }
    }
}