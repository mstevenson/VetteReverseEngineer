using System;
using static MacResourceFork.BinaryReaderBigEndian;

namespace Vette
{
    [Serializable]
    public struct ObjInstance
    {
        // Corresponds with OBJ resource ID
        public int objectId;
        public Vector position;

        public static ObjInstance Parse(ref ReadOnlySpan<byte> span)
        {
            return new ObjInstance
            {
                objectId = ReadUInt16(ref span),
                position = Vector.Parse(ref span)
            };
        }
    }
}