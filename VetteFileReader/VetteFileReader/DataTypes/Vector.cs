namespace VetteFileReader
{
    public struct Vector : IParsable
    {
        public int x;
        public int y;
        public int z;

        public void Parse(BinaryReaderBigEndian reader)
        {
            x = reader.ReadInt16();
            y = reader.ReadInt16();
            z = reader.ReadInt16();
        }
    }
}