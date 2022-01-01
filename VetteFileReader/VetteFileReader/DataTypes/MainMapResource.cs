using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
    public class MainMapResource : IResource
    {
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
    
    public struct MapChunk
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
        
        public static MapChunk Parse(BinaryReaderBigEndian reader)
        {
            var m = new MapChunk();
            
            reader.BaseStream.Seek(1, SeekOrigin.Current);
            m.headerA1 = reader.ReadByte();
            m.headerA2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            m.headerB1 = reader.ReadByte();
            m.headerB2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            m.headerC1 = reader.ReadByte();
            m.headerC2 = reader.ReadByte();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            m.headerD1 = reader.ReadByte();
            m.headerD2 = reader.ReadByte();
            reader.BaseStream.Seek(1, SeekOrigin.Current);
            
            m.quads = new QuadInstance[48];
            
            for (int i = 0; i < m.quads.Length; i++)
            {
                var quad = QuadInstance.Parse(reader);
                m.quads[i] = quad;
            }

            return m;
        }
    }

    public struct QuadInstance
    {
        public int quadDescriptorIndex;
        public int flagA;
        public int flagB;
        
        public static QuadInstance Parse(BinaryReaderBigEndian reader)
        {
            return new QuadInstance
            {
                quadDescriptorIndex = reader.ReadUInt16(),
                flagA = reader.ReadByte(),
                flagB = reader.ReadByte()
            };
        }
    }
}