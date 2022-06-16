using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VetteObjReference
{
    public int id; // unscaled, the raw resource fork ID
    public GameObject prefab;
    public Mesh mesh;
}

[Serializable]
public class VetteQuadReference
{
    public int id;
    public GameObject prefab;
}

[Serializable]
public class VetteMaterialReference
{
    public int patternId;
    public Material material;
}

[CreateAssetMenu(fileName = "Vette Asset Database", menuName = "Vette Asset Database")]
public class VetteAssetDatabase : ScriptableObject
{
    public List<VetteMaterialReference> patternMaterials = new List<VetteMaterialReference>();
    public List<VetteObjReference> objs = new List<VetteObjReference>();
    public List<VetteQuadReference> quads = new List<VetteQuadReference>();
}