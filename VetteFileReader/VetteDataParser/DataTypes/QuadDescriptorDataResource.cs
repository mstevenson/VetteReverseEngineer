using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class QuadDescriptorDataResource : ResourceBase
    {
        public uint fileLength;
        public List<QuadDescriptor> quads = new();
        
        public override void Parse(ref ReadOnlySpan<byte> span)
        {
            fileLength = BinaryReaderBigEndian.ReadUInt16(ref span);
            quads = new List<QuadDescriptor>();

            while (span.Length > 0)
            {
                var quad = QuadDescriptor.Parse(ref span);
                quads.Add(quad);
            }
        }
    }
}