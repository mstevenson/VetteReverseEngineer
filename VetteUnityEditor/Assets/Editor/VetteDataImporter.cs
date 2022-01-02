using System;
using UnityEditor;
using UnityEngine;
using Vette;

namespace Editor
{
    public class VetteDataImporter : EditorWindow
    {
        public VetteDataAsset dataAsset;
        
        [MenuItem("Window/Vette Data")]
        public static void Initialize()
        {
            GetWindow<VetteDataImporter>();
        }

        public void ImportData()
        {
            dataAsset.data = dataAsset.fileFork == FileForkType.DataFork ?
                VetteData.LoadDataFork(dataAsset.dataFilePath) :
                VetteData.LoadResourceFork(dataAsset.dataFilePath);
        }
        
        public void CreateGameObjects()
        {
            var folder = "Assets/Models";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "Models");
            }
            
            // import models
            foreach (var obj in dataAsset.data.objs)
            {
                var mesh = VetteObject.CreateMesh(obj);
                if (mesh == null)
                {
                    continue;
                }
            
                // objMeshes.Add(mesh);

                var go = new GameObject();
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.material = new Material(Shader.Find("Vette/VetteShader"));

                go.name = $"{obj.Name} ({obj.Id})";
            }
        }

        private void OnGUI()
        {
            dataAsset = EditorGUILayout.ObjectField(new GUIContent("Data"), dataAsset, typeof(VetteDataAsset), false) as VetteDataAsset;

            if (GUILayout.Button("Import models"))
            {
                CreateGameObjects();
            }
        }
    }
}