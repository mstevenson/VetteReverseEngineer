using System.Collections.Generic;
using System.Linq;

namespace MacResourceFork
{
    public class ResourceFork
    {
        public List<Resource> resources = new List<Resource>();
        
        public IEnumerable<Resource> GetResourcesOfType(string typeName)
        {
            return resources.Where(r => r.typeString == typeName);
        }
        
        public Resource GetResourceWithName(string typeName, string resourceName)
        {
            return GetResourcesOfType(typeName).FirstOrDefault(r => r.name == resourceName);
        }
    }
}