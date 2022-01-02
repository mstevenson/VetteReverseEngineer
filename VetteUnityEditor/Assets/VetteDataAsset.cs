using UnityEngine;
using Vette;

public enum FileForkType
{
    DataFork,
    ResourceFork
}

[CreateAssetMenu(fileName = "Vette Data", menuName = "Vette Data")]
public class VetteDataAsset : ScriptableObject
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
        
        // // import models
        // foreach (var obj in data.objs)
        // {
        //     var mesh = VetteObject.CreateMesh(obj);
        //     if (mesh == null)
        //     {
        //         continue;
        //     }
        //     
        //     var go = new GameObject();
        //     var mf = go.AddComponent<MeshFilter>();
        //     mf.mesh = mesh;
        //     var mr = go.AddComponent<MeshRenderer>();
        //     mr.material = new Material(Shader.Find("Vette/VetteShader"));
        //
        //     go.name = $"{obj.Name} ({obj.Id})";
        // }
    }
}
