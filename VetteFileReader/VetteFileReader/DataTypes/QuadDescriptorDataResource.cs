using System.Collections.Generic;
using System.IO;

namespace VetteFileReader
{
    public class QuadDescriptorDataResource : IResource
    {
        public int fileLength;
        public List<QuadDescriptor> quads;
        
        public void Parse(BinaryReaderBigEndian reader)
        {
            fileLength = reader.ReadUInt16();
            quads = new List<QuadDescriptor>();

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                quads.Add(QuadDescriptor.Parse(reader));
            }
        }
    }
    
    public struct QuadDescriptor
    {
        public int unknown;
        public int collisionModel;

        public List<Road> roads;
        public List<ObjInstance> objects;
        
        public enum CollisionModel
        {
            Water = 0x3F
        }
        
        public static QuadDescriptor Parse(BinaryReaderBigEndian reader)
        {
            var q = new QuadDescriptor
            {
                unknown = reader.ReadUInt16(),
                collisionModel = reader.ReadUInt16(),
                roads = new List<Road>(),
                objects = new List<ObjInstance>()
            };
            
            while (!DetectDelimiter(reader))
            {
                var road = Road.Parse(reader);
                q.roads.Add(road);
            }
            
            while (!DetectDelimiter(reader))
            {
                var obj = ObjInstance.Parse(reader);
                q.objects.Add(obj);
            }

            return q;
        }
        
        private static bool DetectDelimiter(BinaryReader reader)
        {
            var foundDelimiter = reader.ReadUInt16() == 0xFFFF;
            if (!foundDelimiter)
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
            }
            return foundDelimiter;
        }
    }

    public struct Road
    {
        public int roadType;

        // Unknown flags
        public int a;
        public int b;
        public int c;
        public int d;
        public int e;
        public int f;
        
        public static Road Parse(BinaryReaderBigEndian reader)
        {
            var r = new Road
            {
                roadType = reader.ReadUInt16(),
                // Flags of some kind
                a = reader.ReadByte(),
                b = reader.ReadByte(),
                c = reader.ReadByte(),
                d = reader.ReadByte(),
                e = reader.ReadByte(),
                f = reader.ReadByte()
            };

            return r;
        }
    }

    public struct ObjInstance
    {
        // Corresponds with OBJ resource ID
        public float objectId;
        public Vector position;

        public static ObjInstance Parse(BinaryReaderBigEndian reader)
        {
            return new ObjInstance
            {
                objectId = reader.ReadUInt16(),
                position = Vector.Parse(reader)
            };
        }
    }
}