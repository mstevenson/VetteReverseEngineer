namespace Vette
{
    public abstract class ResourceBase
    {
        public int id;
        public string name;
        
        public abstract void Parse(ref ReadOnlySpan<byte> span);
    }
}