using System;
using System.IO;
using MacResourceFork;
using VetteFileReader;

class App
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            return;
        }
        
        string filePath = args[0];
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File doesn't exist: {filePath}");
            return;
        }
        
        // ResourceForkParser.LogOutput = true;
        
        var vetteData = VetteData.Parse(filePath);
        
        // Console.WriteLine($"main map chunks: {vetteData.mainMap.chunks}");
        // Console.WriteLine($"quads: {vetteData.quadDescriptors.Count}");
        // Console.WriteLine($"objs: {vetteData.objs.Count}");
        // Console.WriteLine($"patterns: {vetteData.patterns.Count}");
        // Console.WriteLine($"streets: {vetteData.streetNames.Count}");
    }
}
