using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using MacResourceFork;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct QuadDescriptor
    {
        public uint unknown;
        public uint collisionModel;

        public List<Road> roads;
        public List<ObjInstance> objects;
        
        public enum CollisionModel
        {
            Water = 0x3F
        }

        public static QuadDescriptor Parse(ref ReadOnlySpan<byte> span)
        {
            var quad = new QuadDescriptor
            {
                unknown = ReadUInt16(ref span),
                collisionModel = ReadUInt16(ref span),
                roads = new List<Road>(),
                objects = new List<ObjInstance>()
            };

            // Console.WriteLine($"QUAD ({reader.BaseStream.Position:X4}):  {quad.collisionModel}");
            // Console.WriteLine("  road search");
            
            while (span.Length > 0 && !DetectDelimiter(ref span))
            {
                var road = Road.Parse(ref span);
                quad.roads.Add(road);
                // Console.WriteLine($"    read road: {road.roadType}");
            }
            
            // Console.WriteLine("  obj search");
            
            while (span.Length > 0 &&!DetectDelimiter(ref span))
            {
                var obj = ObjInstance.Parse(ref span);
                quad.objects.Add(obj);
                // Console.WriteLine("    read obj: " + obj.objectId);
            }

            return quad;
        }
        
        private static bool DetectDelimiter(ref ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(span) == 0xFFFF;
        }
    }
}