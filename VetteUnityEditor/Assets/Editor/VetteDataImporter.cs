using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace;
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

            // CreateMaterials(dataAsset.data);
            
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
        
        public static Mesh CreateMesh(ObjResource vetteObj)
        {
            // TODO ignore the 4th point? We only need 3 points for a triangle, but Vette models might give all four points

            var faceMeshes = new List<Mesh>();

            var polyCount = vetteObj.polygons.polyCount;
            
            // Create a triangulated mesh for each face
            for (var i = 0; i < polyCount; i++)
            {
                var vettePolygon = vetteObj.polygons.polys[i];
                
                // Ignore lines which have three points
                // TODO use submesh with topology wireframe type
                if (vettePolygon.vertexCount < 3)
                {
                    return null;
                }
            
                // Create triangulated meshes
                
                var newPoly = new Poly2Mesh.Polygon
                {
                    outside = new List<Vector3>()
                };
                foreach (var vertexIndex in vettePolygon.vertexIndices)
                {
                    var vert = vetteObj.vertices.vertices[vertexIndex];
                    vert.y *= -1; // Vette model Y axis is inverted
                    newPoly.outside.Add(new Vector3(vert.x, vert.y, vert.z));
                }
                
                var mesh = Poly2Mesh.CreateMesh(newPoly);
                faceMeshes.Add(mesh);
            }
            
            // Collect all face meshes that share a pattern index

            var patternIndexToSubmeshes = new Dictionary<int, List<Mesh>>();
            
            for (int i = 0; i < polyCount; i++)
            {
                var patternIndex = vetteObj.polygons.polys[i].patternIndex;
                if (!patternIndexToSubmeshes.ContainsKey(patternIndex))
                {
                    patternIndexToSubmeshes[patternIndex] = new List<Mesh>();
                }
                patternIndexToSubmeshes[patternIndex].Add(faceMeshes[i]);
            }

            // Combine all faces with a shared pattern index into a single mesh
            
            // Maps pattern index to combined mesh
            var patternMeshes = new Dictionary<int, Mesh>();
            
            foreach (var meshesWithSharedPattern in patternIndexToSubmeshes.OrderBy(kvp => kvp.Key))
            {
                var mesh = new Mesh();
                
                var pattern = meshesWithSharedPattern.Key;
                var meshes = meshesWithSharedPattern.Value;
                
                var facesCombine = new CombineInstance[meshes.Count];
                for (int i = 0; i < meshes.Count; i++)
                {
                    facesCombine[i].mesh = meshes[i];
                    facesCombine[i].transform = Matrix4x4.identity;
                }
                
                mesh.CombineMeshes(facesCombine);
                mesh.RecalculateBounds();
                
                patternMeshes.Add(pattern, mesh);
            }
            
            // Merge all pattern meshes as submeshes of a single mesh

            var patternMaterialIndexes = new List<int>();
            var patternMeshesCombine = new CombineInstance[patternMeshes.Count];
            int combineIndex = 0;
            foreach (var patternMesh in patternMeshes)
            {
                patternMaterialIndexes.Add(patternMesh.Key);
                patternMeshesCombine[combineIndex].mesh = patternMesh.Value;
                patternMeshesCombine[combineIndex].transform = Matrix4x4.identity;
                
                combineIndex++;
            }
            var finalMesh = new Mesh();
            finalMesh.CombineMeshes(patternMeshesCombine, false);
            finalMesh.RecalculateBounds();

            // var materials = new List<Material>();
            // foreach (var materialIndex in patternMaterialIndexes)
            // {
            //     // TODO map pattern indexes to materials, set materials on our renderer
            // }
            
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
        public static Material[] CreateMaterials(VetteData data)
        {
            // Create pattern textures
            var patterns = data.patterns[0];
            var palettes = new Palettes(); // hard-coded values
            var patternTextures = CreatePatterns(patterns, palettes);
            
            var matsFolder = CreateFolder("Materials");
            
            var mats = new Material[patternTextures.Length];
            for (int i = 0; i < patternTextures.Length; i++)
            {
                var matPath = $"{matsFolder}/Pattern {i}.material";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Vette/Pattern Shader"));
                    AssetDatabase.CreateAsset(mat, matPath);
                }
                mat.mainTexture = patternTextures[i];
                mats[i] = mat;
            }
        
            return mats;
        }

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