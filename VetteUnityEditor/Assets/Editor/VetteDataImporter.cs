using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Vette;

namespace Editor
{
    public class VetteDataImporter : EditorWindow
    {
        public VetteDataAsset dataAsset;
        
        [MenuItem("Window/Vette Data")]
        public static void Initialize()
        {
            var window = GetWindow<VetteDataImporter>();
        }

        public void ImportData()
        {
            dataAsset.data = dataAsset.fileFork == FileForkType.DataFork ?
                VetteData.LoadDataFork(dataAsset.dataFilePath) :
                VetteData.LoadResourceFork(dataAsset.dataFilePath);
        }
        
        public void CreateGameObjects()
        {
            var objsFolder = CreateFolder("Objects");
            var meshesFolder = CreateFolder("Meshes");
            var matsFolder = CreateFolder("Materials");
            
            // import models
            foreach (var obj in dataAsset.data.objs)
            {
                var mesh = CreateMesh(obj);
                if (mesh == null)
                {
                    continue;
                }

                mesh.name = $"{obj.name} {obj.id}";
                AssetDatabase.CreateAsset(mesh, $"{meshesFolder}/{mesh.name}.mesh");
                
                var go = new GameObject();
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                
                go.name = $"{obj.name} ({obj.id})";

                // PrefabUtility.SaveAsPrefabAsset(go, $"{objsFolder}/{obj.name} {obj.id}.prefab");
                // AssetDatabase.Refresh();

                // DestroyImmediate(go);
            }
        }
        
        public static Mesh CreateMesh(ObjResource obj)
        {
            // var allMaterials = CreateMaterials();
            
            // TODO ignore the 4th point? We only need 3 points for a triangle, but Vette models might give all four points

            var faceMeshes = new List<Mesh>();
            var materials = new List<Material>();
            
            // maps a Vette color index to a material
            var colorIndexToMaterialMap = new Dictionary<int, int>();
            
            // Winding order is opposite of Unity
            for (var i = obj.polygons.polyCount - 1; i >= 0; i--)
            {
                var vettePolygon = obj.polygons.polys[i];
                // var colorIndex = vettePolygon.patternIndex
                
                // Ignore lines which have three points
                // TODO use submesh with topology wireframe type
                if (vettePolygon.vertexCount < 3)
                {
                    return null;
                }
            
                var newPoly = new Poly2Mesh.Polygon
                {
                    outside = new List<Vector3>()
                };
                foreach (var vertexIndex in vettePolygon.vertexIndices)
                {
                    var vert = obj.vertices.vertices[vertexIndex];
                
                    // Y axis is inverted
                    vert.y *= -1;
                
                    newPoly.outside.Add(new Vector3(vert.x, vert.y, vert.z));
                }
            
                // TODO z-facing polys need their normals flipped

                var mesh = Poly2Mesh.CreateMesh(newPoly);
                faceMeshes.Add(mesh);
            }
            
            var finalMesh = new Mesh();
            finalMesh.subMeshCount = faceMeshes.Count;

            // TODO this allows submesh topology to be set to wireframe
            // var md = new SubMeshDescriptor();

            
            
            
            for (int i = 0; i < faceMeshes.Count; i++)
            {
                // combinedMesh.subm
            }
            
            // combinedMesh.CombineMeshes(combiner);
            finalMesh.RecalculateBounds();

            return finalMesh;
        }

        public static Texture2D[] CreatePatterns(PatternsResource patterns, Palettes palettes)
        {
            var patternsFolder = CreateFolder("Patterns");

            // int paletteId = 128;
            int paletteId = 160;
            
            // the first palette is used for Main Map
            var palette = palettes.palettes[paletteId];
            var textures = new Texture2D[patterns.patterns.Count];
            
            for (var patternIndex = 0; patternIndex < patterns.patterns.Count; patternIndex++)
            {
                var pattern = patterns.patterns[patternIndex];
                var tex = new Texture2D(8, 8, TextureFormat.RGB24, 0, false);
                var pixelColors = new Color32[64];
                for (int i = 0; i < pixelColors.Length; i++)
                {
                    var colorIndex = pattern.pixelColorIndexes[i];
                    pixelColors[i] = palette[colorIndex];
                }
                tex.SetPixels32(pixelColors);
                tex.Apply();
                var bytes = tex.EncodeToPNG();

                var patternFilename = $"Pattern {patternIndex}.png";
                
                File.WriteAllBytes($"{Application.dataPath}/Patterns/{patternFilename}", bytes);
                DestroyImmediate(tex);

                textures[patternIndex] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{patternsFolder}/{patternFilename}");
            }
            
            AssetDatabase.Refresh();

            return textures;
        }
        
        // Create materials for each color in the Vette Main Map palette
        // public static Material[] CreateMaterials(PatternsResource patterns)
        // {
        //     // TODO create a material for each pattern, which can be multiple materials
        //
        //     foreach (var pattern in patterns.patterns)
        //     {
        //         
        //     }
        //     
        //     var matFolder = CreateFolder("Materials");
        //     
        //     var palette = Palettes.palettes[128];
        //     var mats = new Material[palette.Length];
        //
        //     for (int i = 0; i < palette.Length; i++)
        //     {
        //         var matPath = $"{matFolder}/{i}.material";
        //         var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        //         if (mat == null)
        //         {
        //             mat = new Material(Shader.Find("Vette/VetteShader"))
        //             {
        //                 color = palette[i]
        //             };
        //             AssetDatabase.CreateAsset(mat, matPath);
        //         }
        //
        //         mats[i] = mat;
        //     }
        //
        //     return mats;
        // }

        private static string CreateFolder(string folderName)
        {
            var folder = $"Assets/{folderName}";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", folderName);
            }

            return folder;
        }

        private void OnGUI()
        {
            dataAsset = EditorGUILayout.ObjectField(new GUIContent("Data"), dataAsset, typeof(VetteDataAsset), false) as VetteDataAsset;

            if (GUILayout.Button("Import models"))
            {
                CreateGameObjects();
            }
            
            if (GUILayout.Button("Import patterns"))
            {
                // The second pattern seems to be the one used in the game
                var patterns = dataAsset.data.patterns[1];
                
                CreatePatterns(patterns, new Palettes());
            }
        }
    }
}