using System.Collections.Generic;
using System.Linq;
using MacResourceFork;

namespace VetteFileReader
{
    public class VetteData
    {
        public MainMapResource mainMap;
        public List<QuadDescriptorDataResource> quadDescriptors;
        public List<ObjResource> objs;
        public List<string> streetNames;
        public List<PatternsResource> patterns;

        public static VetteData Parse(string filePath)
        {
            var data = new VetteData();
            
            var resourceFork = ResourceForkParser.LoadResourceFork(filePath);

            var map = resourceFork.GetResourceWithName("MAPS", "Main_Map");
            data.mainMap = VetteResourceParser.Parse<MainMapResource>(map);
            
            foreach (var quadResource in resourceFork.GetResourcesOfType("QUAD"))
            {
                var quadParsed = VetteResourceParser.Parse<QuadDescriptorDataResource>(quadResource);
                data.quadDescriptors.Add(quadParsed);
            }

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