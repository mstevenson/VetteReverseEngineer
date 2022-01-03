using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class Vector
    {
        public short x;
        public short y;
        public short z;
        
        public static Vector Parse(BinaryReaderBigEndian reader)
        {
            return new Vector()
            {
                x = reader.ReadInt16(),
                y = reader.ReadInt16(),
                z = reader.ReadInt16(),
            };
        }
    }
}