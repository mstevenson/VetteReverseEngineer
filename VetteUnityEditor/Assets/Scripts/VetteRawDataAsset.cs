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
        data = fileFork == FileForkType.DataFork ?
            VetteData.LoadDataFork(dataFilePath) :
            VetteData.LoadResourceFork(dataFilePath);
    }
}
