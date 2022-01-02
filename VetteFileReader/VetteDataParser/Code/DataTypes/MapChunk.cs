using System;
using System.IO;
using MacResourceFork;

namespace Vette
{
    [Serializable]
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
}