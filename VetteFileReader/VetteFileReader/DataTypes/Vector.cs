namespace VetteFileReader
{
    public struct Vector
    {
        public int x;
        public int y;
        public int z;
        
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