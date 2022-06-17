namespace MacResourceFork
{
    public record Resource(int Id, byte[] Type, string TypeString, ResourceAttrs Attributes, string Name, byte[] Data);
    
    public class ResourceFork
    {
        public List<Resource> resources = new();
        
        public IEnumerable<Resource> GetResourcesOfType(string typeName)
        {
            return resources.Where(r => r.TypeString == typeName);
        }
        
        public Resource? GetResourceWithName(string typeName, string resourceName)
        {
            return GetResourcesOfType(typeName).FirstOrDefault(r => r.Name == resourceName);
        }
    }
}