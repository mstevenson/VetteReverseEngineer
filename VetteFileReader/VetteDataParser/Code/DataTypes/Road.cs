using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class Road
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
}