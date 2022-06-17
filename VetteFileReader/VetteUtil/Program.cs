using MacResourceFork;
using Vette;

if (args.Length < 1)
{
    Console.WriteLine("Usage: VetteUtil.exe <file>");
    return 1;
}

string filePath = args[0];

if (!File.Exists(filePath))
{
    Console.WriteLine($"File doesn't exist: {filePath}");
    return 1;
}

ResourceForkParser.LogOutput = true;

try
{
    var vetteData = VetteData.LoadResourceFork(filePath);    
    
    Console.WriteLine($"main map rows: {vetteData.mainMap.rows}");
    Console.WriteLine($"quads: {vetteData.quadDescriptors.quads.Count}");
    Console.WriteLine($"objs: {vetteData.objs.Count}");
    Console.WriteLine($"patterns: {vetteData.patterns.Count}");
    Console.WriteLine($"streets: {vetteData.streetNames.Count}");
}
catch (FileLoadException ex)
{
    Console.WriteLine($"Error loading file: {ex.Message}");
    return 1;
}

return 0;
