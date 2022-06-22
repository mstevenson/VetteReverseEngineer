using UnityEditor;
using UnityEngine;

public class VetteTools : EditorWindow
{
    public VetteRawDataAsset dataAsset;
    
    [MenuItem("Window/Vette Data")]
    public static void Initialize()
    {
        var window = GetWindow<VetteTools>();
    }
    
    private void OnGUI()
    {
        dataAsset = EditorGUILayout.ObjectField(new GUIContent("Data"), dataAsset, typeof(VetteRawDataAsset), false) as VetteRawDataAsset;

        if (dataAsset == null)
        {
            GUI.enabled = false;
        }
        
        if (GUILayout.Button("Create OBJ prefabs"))
        {
            try
            {
                VetteAssetImporter.CreateOBJPrefabs(dataAsset);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        if (GUILayout.Button("Create QUAD prefabs"))
        {
            try
            {
                VetteAssetImporter.CreateQuadPrefabs(dataAsset.data.quadDescriptors.quads);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        if (GUILayout.Button("Instantiate Main Map"))
        {
            VetteAssetImporter.CreateMainMap(dataAsset.data.mainMap);
        }
        
        if (GUILayout.Button("Instantiate 2D Map"))
        {
            VetteAssetImporter.Create2DMap(dataAsset);
        }

        EditorGUILayout.Space();
    }
}
