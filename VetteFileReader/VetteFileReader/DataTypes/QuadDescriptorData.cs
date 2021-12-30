using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
    public struct QuadDescriptorData : IParsable
    {
        public int fileLength;
        List<QuadDescriptor> quads;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();

            quads = new List<QuadDescriptor>();
            while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
            {
                var quad = new QuadDescriptor();
                quad.Parse(reader);
                quads.Add(quad);
            }
        }
    }
    
    public struct QuadDescriptor : IParsable
    {
        public int unknown;
        public int collisionModel;

        public List<Road> roads;
        public List<ObjInstance> objects;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            // Parse header
            unknown = reader.ReadUInt16();
            collisionModel = reader.ReadUInt16();

            roads = new List<Road>();
            objects = new List<ObjInstance>();
            
            while (!DetectDelimiter(reader))
            {
                var road = new Road();
                road.Parse(reader);
                roads.Add(road);
            }
            
            while (!DetectDelimiter(reader))
            {
                var obj = new ObjInstance();
                obj.Parse(reader);
                objects.Add(obj);
            }
        }
        
        public bool DetectDelimiter(BinaryReader reader)
        {
            var foundDelimiter = reader.ReadUInt16() == 0xFFFF;
            if (!foundDelimiter)
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
            }
            return foundDelimiter;
        }
    }

    public struct Road : IParsable
    {
        public int roadType;

        // Unknown flags
        public int a;
        public int b;
        public int c;
        public int d;
        public int e;
        public int f;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            roadType = reader.ReadUInt16();

            // Flags of some kind
            a = reader.ReadByte();
            b = reader.ReadByte();
            c = reader.ReadByte();
            d = reader.ReadByte();
            e = reader.ReadByte();
            f = reader.ReadByte();
        }
    }

    public struct ObjInstance : IParsable
    {
        // Corresponds with OBJ resource ID
        public float objectId;
        public Vector position;

        public void Parse(BinaryReaderBigEndian reader)
        {
            objectId = reader.ReadUInt16();
            position = new Vector();
            position.Parse(reader);
        }
    }

    public enum CollisionModel
    {
        Water = 0x3F
    }
}