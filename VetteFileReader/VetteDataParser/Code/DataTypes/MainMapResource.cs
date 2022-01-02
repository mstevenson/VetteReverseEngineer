using System;
using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class MainMapResource : IResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int fileLength;
        public List<MapChunk> chunks;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            chunks = new List<MapChunk>();

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var chunk = MapChunk.Parse(reader);
                chunks.Add(chunk);
            }
        }
    }
}