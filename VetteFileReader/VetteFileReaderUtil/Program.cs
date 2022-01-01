using System;
using System.IO;
using System.Linq;
using VetteFileReader;

class App
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            return;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("File doesn't exist: " + args[0]);
            return;
        }
        
        var resourceFork = ResourceForkParser.LoadResourceFork(args[0]);

        // resourceFork.resources.Select(r => r.typeString == "STRT");

        // Console.WriteLine(rf.dataOffset);
    }
}
