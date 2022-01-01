using System.IO;

namespace VetteFileReader
{
    public interface IResource
    {
        void Parse(BinaryReaderBigEndian reader);
    }
}