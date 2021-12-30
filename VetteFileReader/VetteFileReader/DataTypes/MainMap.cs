using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
    public struct MainMap : IParsable
    {
        public int fileLength;
        public List<MapChunk> chunks;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            
            chunks = new List<MapChunk>();
            
            while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
            {
                var chunk = new MapChunk();
                chunk.Parse(reader);
                chunks.Add(chunk);
            }
        }
    }

    public struct MapChunk : IParsable
    {
        public int headerA1;
        public int headerA2;
        public int headerB1;
        public int headerB2;
        public int headerC1;
        public int headerC2;
        public int headerD1;
        public int headerD2;
        
        public QuadInstance[] quads;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            reader.BaseStream.Seek(1, SeekOrigin.Current);
            headerA1 = reader.ReadByte();
            headerA2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            headerB1 = reader.ReadByte();
            headerB2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            headerC1 = reader.ReadByte();
            headerC2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            headerD1 = reader.ReadByte();
            headerD2 = reader.ReadByte();
            reader.BaseStream.Seek(1, SeekOrigin.Current);
            
            quads = new QuadInstance[48];
            
            for (int i = 0; i < quads.Length; i++)
            {
                var quad = new QuadInstance();
                quad.Parse(reader);
                quads[i] = quad;
            }
        }
    }

    public struct QuadInstance : IParsable
    {
        public int quadDescriptorIndex;
        public int flagA;
        public int flagB;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            quadDescriptorIndex = reader.ReadUInt16();
            flagA = reader.ReadByte();
            flagB = reader.ReadByte();
        }
    }
}