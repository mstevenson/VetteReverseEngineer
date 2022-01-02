using MacResourceFork;

namespace Vette
{
    public interface IResource
    {
        int Id { get; set; }
        string Name { get; set; }
        
        void Parse(BinaryReaderBigEndian reader);
    }
}