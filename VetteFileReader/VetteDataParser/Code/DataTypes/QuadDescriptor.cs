using System;
using System.Collections.Generic;
using System.IO;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class QuadDescriptor
    {
        public uint unknown;
        public uint collisionModel;

        public List<Road> roads;
        public List<ObjInstance> objects;
        
        public enum CollisionModel
        {
            Water = 0x3F
        }

        public static QuadDescriptor Parse(BinaryReaderBigEndian reader)
        {
            var quad = new QuadDescriptor
            {
                unknown = reader.ReadUInt16(),
                collisionModel = reader.ReadUInt16(),
                roads = new List<Road>(),
                objects = new List<ObjInstance>()
            };

            // Console.WriteLine($"QUAD ({reader.BaseStream.Position:X4}):  {quad.collisionModel}");
            // Console.WriteLine("  road search");
            
            while (!DetectDelimiter(reader))
            {
                var road = Road.Parse(reader);
                quad.roads.Add(road);
                // Console.WriteLine($"    read road: {road.roadType}");
            }
            
            // Console.WriteLine("  obj search");
            
            while (!DetectDelimiter(reader))
            {
                var obj = ObjInstance.Parse(reader);
                quad.objects.Add(obj);
                // Console.WriteLine("    read obj: " + obj.objectId);
            }

            return quad;
        }
        
        private static bool DetectDelimiter(BinaryReader reader)
        {
            var originalPosition = reader.BaseStream.Position;
            var foundDelimiter = reader.ReadUInt16() == 0xFFFF;
            if (!foundDelimiter)
            {
                reader.BaseStream.Position = originalPosition;
            }
            return foundDelimiter;
        }
    }
}