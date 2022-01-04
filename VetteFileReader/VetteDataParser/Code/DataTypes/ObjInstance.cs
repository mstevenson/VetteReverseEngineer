using System;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public struct ObjInstance
    {
        // Corresponds with OBJ resource ID
        public int objectId;
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