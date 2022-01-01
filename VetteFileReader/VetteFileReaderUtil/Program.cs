using System;
using System.IO;
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
        
        var resourceFork = ResourceForkParser.LoadResourceFork(filePath);
        
        foreach (var resource in resourceFork.GetResourcesOfType("MAPS"))
        {
            Console.WriteLine(resource.name);
        }
    }
}
