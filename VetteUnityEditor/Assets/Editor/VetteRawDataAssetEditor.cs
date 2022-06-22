using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(VetteRawDataAsset))]
    public class VetteRawDataAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Import"))
            {
                var t = (VetteRawDataAsset)target;
                t.Import();
            }
        }
    }
}