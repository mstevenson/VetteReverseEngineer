using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class VetteData
    {
        public MainMapResource mainMap;
        public QuadDescriptorDataResource quadDescriptors;
        public List<ObjResource> objs = new();
        public List<string> streetNames = new();
        public List<PatternsResource> patterns = new();
        
        public static VetteData LoadDataFork(string filePath)
        {
            var resourceData = ResourceForkParser.LoadDataFork(filePath);
            return Parse(ref resourceData);
        }
        
        public static VetteData LoadResourceFork(string filePath)
        {
            var resourceData = ResourceForkParser.LoadResourceFork(filePath);
            return Parse(ref resourceData);
        }

        private static VetteData Parse(ref ResourceFork resourceFork)
        {
            var data = new VetteData();

            var map = resourceFork.GetResourceWithName("MAPS", "Main_Map");
            data.mainMap = VetteResourceParser.Parse<MainMapResource>(map);

            var quads = resourceFork.GetResourcesOfType("QUAD").First();
            data.quadDescriptors = VetteResourceParser.Parse<QuadDescriptorDataResource>(quads);
            
            foreach (var objResource in resourceFork.GetResourcesOfType("OBJS"))
            {
                var objParsed = VetteResourceParser.Parse<ObjResource>(objResource);
                data.objs.Add(objParsed);
            }
            
            var streetsResource = resourceFork.GetResourcesOfType("STRT").FirstOrDefault();
            var streetsParsed = VetteResourceParser.Parse<StreetNamesResource>(streetsResource);
            foreach (var streetName in streetsParsed.names)
            {
                data.streetNames.Add(streetName);
            }
            
            foreach (var patternResource in resourceFork.GetResourcesOfType("PATN"))
            {
                var patternParsed = VetteResourceParser.Parse<PatternsResource>(patternResource);
                data.patterns.Add(patternParsed);
            }

            return data;
        }
    }
}