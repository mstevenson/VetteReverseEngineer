using MacResourceFork;

namespace VetteFileReader
{
    public interface IResource
    {
        void Parse(BinaryReaderBigEndian reader);
    }
}