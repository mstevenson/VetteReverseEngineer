using System.IO;
using UnityEngine;
using Vette;

public enum FileForkType
{
    DataFork,
    ResourceFork
}

[CreateAssetMenu(fileName = "Vette Data", menuName = "Vette Data")]
public class VetteRawDataAsset : ScriptableObject
{
    public FileForkType fileFork;
    public string dataFilePath;
    public VetteData data;
    
    [ContextMenu("Import Data")]
    public void Import()
    {
        var absolutePath = Path.GetFullPath(dataFilePath);
        if (!File.Exists(absolutePath))
        {
            Debug.LogError($"Could not find file at path: {absolutePath}");
            return;
        }
        Debug.Log($"Importing {(fileFork == FileForkType.DataFork ? "data" : "resource")} fork from {absolutePath}");
        data = fileFork == FileForkType.DataFork ?
            VetteData.LoadDataFork(dataFilePath) :
            VetteData.LoadResourceFork(dataFilePath);
    }
}
