using System;
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
        
        private void OnGUI()
        {
            dataAsset = EditorGUILayout.ObjectField(new GUIContent("Data"), dataAsset, typeof(VetteDataAsset), false) as VetteDataAsset;

            if (GUILayout.Button("Import patterns"))
            {
                // The second pattern seems to be the one used in the game
                var patterns = dataAsset.data.patterns[1];
                
                CreatePatterns(patterns, Palettes.MainMap);
            }
            
            if (GUILayout.Button("Import models"))
            {
                CreatePrefabs();
            }

            if (GUILayout.Button("Import quads"))
            {
                CreateQuads();
            }

            EditorGUILayout.Space();
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

        public void ImportData()
        {
            dataAsset.data = dataAsset.fileFork == FileForkType.DataFork ?
                VetteData.LoadDataFork(dataAsset.dataFilePath) :
                VetteData.LoadResourceFork(dataAsset.dataFilePath);
        }
        
        public void CreatePrefabs()
        {
            // Create pattern materials to apply to new meshes

            int paletteId = 160;
            
            var patterns = dataAsset.data.patterns[1]; // First pattern is used in Main Map
            var patternMaterials = CreatePatterns(patterns, Palettes.MainMap);
            
            // Import Vette objects as meshes
            
            var objsFolder = CreateFolder("Objects");
            var meshesFolder = CreateFolder("Meshes");
            
            foreach (var obj in dataAsset.data.objs)
            {
                var (mesh, patternIndexes) = CreateMesh(obj);
                if (mesh == null)
                {
                    continue;
                }
                
                var go = new GameObject();
                var objComponent = go.AddComponent<ObjComponent>();
                objComponent.objName = obj.name;
                objComponent.id = obj.id;
                
                mesh.name = $"{obj.id} {obj.name}";
                AssetDatabase.CreateAsset(mesh, $"{meshesFolder}/{mesh.name}.mesh");
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                
                // Apply materials
                var objMaterials = new Material[mf.sharedMesh.subMeshCount];
                for (int i = 0; i < patternIndexes.Length; i++)
                {
                    var mat = patternMaterials[patternIndexes[i]];
                    objMaterials[i] = mat;
                }
                mr.materials = objMaterials;
                
                go.name = mesh.name;

                PrefabUtility.SaveAsPrefabAsset(go, $"{objsFolder}/{mesh.name}.prefab");
                AssetDatabase.Refresh();

                DestroyImmediate(go);
            }
        }
        
        public static (Mesh mesh, int[] patternIndexes) CreateMesh(ObjResource vetteObj)
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
                    return (null, Array.Empty<int>());
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
            
            return (finalMesh, patternMaterialIndexes.ToArray());
        }

        public static Material[] CreatePatterns(PatternsResource patterns, Color32[] palette)
        {
            var patternsFolder = CreateFolder("Patterns");

            // the first palette is used for Main Map
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
                AssetDatabase.Refresh();

                textures[patternIndex] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{patternsFolder}/{patternFilename}");
                textures[patternIndex].filterMode = FilterMode.Point;
            }
            
            var matsFolder = CreateFolder("Materials");
            
            var mats = new Material[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                var matPath = $"{matsFolder}/Pattern {i}.mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Vette/Pattern Shader"));
                    AssetDatabase.CreateAsset(mat, matPath);
                }
                mat.mainTexture = textures[i];
                mats[i] = mat;
            }

            AssetDatabase.Refresh();
            
            return mats;
        }

        public void CreateQuads()
        {
            // Get a list of all objects that can be instantiated
            
            // TODO this misses a lot of assets for some reason
            var objPrefabs = Resources.FindObjectsOfTypeAll<ObjComponent>();
            Debug.Log(objPrefabs.Length);
            
            for (var i = 0; i < dataAsset.data.quadDescriptors.quads.Count; i++)
            {
                var quadData = dataAsset.data.quadDescriptors.quads[i];
                var quad = new GameObject
                {
                    name = $"Quad {i}"
                };

                foreach (var objInfo in quadData.objects)
                {
                    foreach (var objPrefab in objPrefabs)
                    {
                        // values not divisible by 100 are temp files and model revisions that
                        // are not accessible in-game, so model 1505 is a revision of 1500, which is
                        // accessed by id 15.
                        var targetId = objPrefab.id / 100;
                        if (objInfo.objectId != targetId)
                        {
                            continue;
                        }

                        var obj = Instantiate(objPrefab.gameObject, quad.transform, true);

                        // z-up
                        var pos = new Vector3(objInfo.position.x, objInfo.position.z, objInfo.position.y);
                        obj.transform.localPosition = pos;
                    }
                }
            }
        }
    }
}