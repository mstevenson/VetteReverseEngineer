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

            // if (GUILayout.Button("Import patterns"))
            // {
            //     // The second pattern seems to be the one used in the game
            //     var patterns = dataAsset.data.patterns[1];
            //     
            //     CreatePatterns(patterns, Palettes.MainMap);
            // }
            
            if (GUILayout.Button("Import models"))
            {
                CreatePrefabs();
            }

            if (GUILayout.Button("Import quads"))
            {
                CreateQuads();
            }

            if (GUILayout.Button("Instantiate Main Map"))
            {
                CreateMainMap();
            }
            
            if (GUILayout.Button("Create Colored Main Map"))
            {
                CreateColoredQuads();
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

        public static VetteAssetDatabase GetOrCreateAssetDatabase()
        {
            var path = "Assets/Vette Asset Database.asset";
            
            var db = AssetDatabase.LoadAssetAtPath<VetteAssetDatabase>(path);
            if (db == null)
            {
                db = CreateInstance<VetteAssetDatabase>();
                AssetDatabase.CreateAsset(db, path);
                AssetDatabase.Refresh();
            }
            return db;
        }
        
        public void CreatePrefabs()
        {
            var db = GetOrCreateAssetDatabase();
            
            // Create pattern materials to apply to new meshes

            int paletteId = 160;
            
            var patterns = dataAsset.data.patterns[1]; // First pattern is used in Main Map
            var patternMaterials = CreatePatterns(patterns, Palettes.MainMap);
            
            // Import Vette objects as meshes
            
            var objsFolder = CreateFolder("Objects");
            var meshesFolder = CreateFolder("Meshes");
            
            foreach (var obj in dataAsset.data.objs)
            {
                var prefabRef = new VetteObjReference
                {
                    id = obj.id
                };

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
                var meshPath = $"{meshesFolder}/{mesh.name}.mesh";
                AssetDatabase.CreateAsset(mesh, meshPath);
                var meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = meshAsset;
                var mr = go.AddComponent<MeshRenderer>();

                // Apply materials
                var objMaterials = new Material[mf.sharedMesh.subMeshCount];
                for (int i = 0; i < patternIndexes.Length; i++)
                {
                    var mat = patternMaterials[patternIndexes[i]];
                    objMaterials[i] = mat;
                }
                mr.materials = objMaterials;
                
                go.name = meshAsset.name;

                var path = $"{objsFolder}/{mesh.name}.prefab";
                PrefabUtility.SaveAsPrefabAsset(go, path);
                AssetDatabase.Refresh();

                DestroyImmediate(go);

                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                prefabRef.prefab = goPrefab;
                prefabRef.mesh = mesh;
                db.objs.Add(prefabRef);
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
            var db = GetOrCreateAssetDatabase();
            
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
                
                db.patternMaterials.Add(new VetteMaterialReference { patternId = i, material = mat});
            }

            AssetDatabase.Refresh();
            
            return mats;
        }

        public void CreateQuads()
        {
            var db = GetOrCreateAssetDatabase();
            
            for (var i = 0; i < dataAsset.data.quadDescriptors.quads.Count; i++)
            {
                var quadData = dataAsset.data.quadDescriptors.quads[i];
                var quad = new GameObject
                {
                    name = $"Quad {i}"
                };

                foreach (var objInfo in quadData.objects)
                {
                    foreach (var objRefs in db.objs)
                    {
                        // values not divisible by 100 are temp files and model revisions that
                        // are not accessible in-game, so model 1505 is a revision of 1500, which is
                        // accessed by id 15.
                        float targetId = objRefs.id / 100f;
                        if (Math.Abs(objInfo.objectId - targetId) > 0.0001f)
                        {
                            continue;
                        }

                        var obj = Instantiate(objRefs.prefab, quad.transform, true);

                        // z-up
                        var pos = new Vector3(objInfo.position.x, objInfo.position.z, objInfo.position.y);
                        obj.transform.localPosition = pos;
                    }
                }
                
                var quadsFolder = CreateFolder("Quads");

                var path = $"{quadsFolder}/{quad.name}.prefab";
                PrefabUtility.SaveAsPrefabAsset(quad, path);
                AssetDatabase.Refresh();
                
                DestroyImmediate(quad);
                
                var quadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                db.quads.Add(new VetteQuadReference() {id = i, prefab = quadPrefab});
            }
        }
        
        private void CreateMainMap()
        {
            var db = GetOrCreateAssetDatabase();
            
            var map = dataAsset.data.mainMap;

            for (var chunkIndex = 0; chunkIndex < map.chunks.Count; chunkIndex++)
            {
                var chunk = map.chunks[chunkIndex];
                var chunkObj = new GameObject
                {
                    name = "Chunk " + chunkIndex
                };

                for (var quadIndex = 0; quadIndex < chunk.quads.Length; quadIndex++)
                {
                    var chunkQuad = chunk.quads[quadIndex];
                    int quadDescriptorIndex = chunkQuad.quadDescriptorIndex;
                    var quadRef = db.quads.Where(q => q.id == quadDescriptorIndex).FirstOrDefault(q => q.prefab);
                    if (quadRef == null)
                    {
                        Debug.LogError("Missing quad: " + quadDescriptorIndex);
                        continue;
                    }

                    var quadObj = Instantiate(quadRef.prefab);
                    quadObj.transform.localScale = Vector3.one * 0.0005f;
                    quadObj.transform.position = Vector3.forward * quadIndex * 10;
                    quadObj.transform.parent = chunkObj.transform;
                }

                chunkObj.transform.position = Vector3.right * chunkIndex * 10;
            }
        }

        private List<Color> colors = new List<Color>();
        
        private void CreateColoredQuads()
        {
            colors.Clear();
            
            for (int i = 0; i < 300; i++)
            {
                colors.Add(Color.HSVToRGB(UnityEngine.Random.value, 1, (UnityEngine.Random.value / 2.5f) + 0.2f));
            }
            
            var db = GetOrCreateAssetDatabase();

            var mat = new Material(Shader.Find("Sprites/Default"));
            
            for (var i = 0; i < dataAsset.data.mainMap.chunks.Count; i++)
            {
                var chunk = dataAsset.data.mainMap.chunks[i];
                for (var j = 0; j < chunk.quads.Length; j++)
                {
                    var quad = chunk.quads[j];
                    var index = quad.quadDescriptorIndex;

                    var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    var mr = go.GetComponent<MeshRenderer>();
                    // mr.material.color = Color.HSVToRGB(index / 200f, 1, (index / 300f) + 0.2f);
                    mr.sharedMaterial = mat;
                    mr.material.color = colors[index];
                    go.name = $"id {index} - y{i} x{j}";
                    go.transform.position = new Vector2(j, i);
                    
                    void SetColor(Color color, string suffix = null)
                    {
                        mr.material.color = color;//("_Tint", color);
                        if (suffix != null)
                        {
                            go.name += " - " + suffix;
                        }
                    }
                    
                    switch (index)
                    {
                        case 0:
                            SetColor(new Color(0f, 0.72f, 0.75f));
                            break;
                        case 1: // water
                            SetColor(Color.cyan);
                            break;
                        case 2: // transamerica
                            SetColor(Color.magenta, "Transamerica");
                            break;
                        case 3: // gas station
                        case 235: // alameda gas station
                            SetColor(new Color(1f, 0f, 0.26f), "gas station");
                            break;
                        case 56: // plain concrete?
                            SetColor(Color.white);
                            break;
                        case 60: // tall downtown buildings
                        case 75: // market street
                            SetColor(Color.gray);
                            break;
                        case 62: // tall downtown buildings, dead end alleys 
                            SetColor(new Color(0.4f, 0.4f, 0.4f));
                            break;
                        case 47: // bay bridge tile
                            SetColor(new Color(1f, 0.3f, 0f));
                            break;
                        case 37: // park tile
                        case 158: // park corner
                            SetColor(new Color(0f, 0.69f, 0f));
                            break;

                        // unique buildings
                        case 90: // zoo
                            SetColor(Color.magenta, "zoo");
                            break;
                        case 98:
                            SetColor(Color.magenta, "Fisherman's Wharf");
                            break;
                        case 86:
                            SetColor(Color.magenta, "Pier 39");
                            break;
                        case 84:
                            SetColor(Color.magenta, "Coit Tower");
                            break;
                        case 53:
                            SetColor(Color.magenta);
                            break;
                        // city hall and macworld??
                        case 83:
                        case 99:
                        case 107:
                            SetColor(Color.magenta);
                            break;
                        case 93: // ghirardelli
                            SetColor(Color.magenta, "Ghirardelli");
                            break;
                        case 59:
                            SetColor(Color.magenta, "palace of fine arts");
                            break;
                        case 81:
                            SetColor(Color.magenta, "Japantown");
                            break;
                        case 106:
                            SetColor(Color.magenta, "Saint Mary's");
                            break;

                        case 85: // piers
                            SetColor(new Color(0.86f, 0.55f, 0.56f));
                            break;
                        
                        case 58: // flat concrete
                            SetColor(new Color(0.83f, 0.83f, 0.83f));
                            break;
                        
                        // generic blocks
                        case 88:
                            SetColor(new Color(0.29f, 0.33f, 0.38f));
                            break;
                        case 89:
                            SetColor(new Color(0.32f, 0.35f, 0.41f));
                            break;
                        case 94:
                        case 95:
                            SetColor(new Color(0.35f, 0.38f, 0.44f));
                            break;

                        // rainbow buildings
                        case 96:
                        case 160:
                        case 91:
                            SetColor(new Color(0.29f, 0.55f, 0.67f));
                            break;
                        
                        // yellow walls
                        case 110: // corner wall
                        case 111: // right wall
                        case 112: // lower wall
                        case 232: // upper wall
                        case 126: // upper wall, uphill to left
                        case 109: // corner wall
                        case 189: // market street wall
                            SetColor(Color.yellow);
                            break;
                        // dirt leading to cliff house
                        case 156:
                        case 157:
                            SetColor(new Color(0.71f, 0.32f, 0.13f));
                            break;
                        case 168:
                        case 190:
                        case 114:
                        case 115:
                            SetColor(new Color(0.01f, 0.57f, 0f));
                            break;
                        
                        // Hills
                        case 68:
                            SetColor(new Color(0.78f, 0.78f, 0.78f));
                            break;
                        case 65:
                        case 161:
                        case 169:
                            SetColor(new Color(0.63f, 0.63f, 0.63f));
                            break;
                        case 70:
                            SetColor(new Color(0.29f, 0.29f, 0.29f));
                            break;
                        case 72:
                        case 175:
                            SetColor(new Color(0.19f, 0.19f, 0.19f));
                            break;
                        case 64:
                            SetColor((new Color(0.78f, 0.78f, 0.78f) + new Color(0.63f, 0.63f, 0.63f)) / 2);
                            break;
                        case 67:
                        case 171:
                            SetColor((new Color(0.63f, 0.63f, 0.63f) + new Color(0.29f, 0.29f, 0.29f)) / 2);
                            break;
                        case 71:
                            SetColor((new Color(0.78f, 0.78f, 0.78f) + new Color(0.19f, 0.19f, 0.19f)) / 2);
                            break;
                        case 74:
                            SetColor((new Color(0.29f, 0.29f, 0.29f) + new Color(0.19f, 0.19f, 0.19f)) / 2);
                            break;
                        
                        case 165: // blue houses on grass
                        case 26:
                        case 27: // blue buildings on concrete
                            SetColor(new Color(0.11f, 0.26f, 0.55f));
                            break;
                        

                        // freeway entrance
                        case 55:
                        case 148:
                        case 229:
                        case 230:
                        case 136:
                        case 180:
                            SetColor(new Color(0.71f, 1f, 0f), "freeway entrance");
                            break;
                        // freeway
                        case 120: // freeway up
                        case 154: // freeway curve right
                            SetColor(new Color(0f, 0.08f, 0.46f));
                            break;
                    }
                }
            }
        }
    }
}