using System;
using System.Collections.Generic;
using System.IO;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class QuadDescriptorDataResource : IResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public uint fileLength;
        public List<QuadDescriptor> quads;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            quads = new List<QuadDescriptor>();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                try
                {
                    var quad = QuadDescriptor.Parse(reader);
                    quads.Add(quad);
                }
                catch (EndOfStreamException e)
                {
                    // nothing
                }
            }
        }
    }
}