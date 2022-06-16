using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2DQuadInfo : MonoBehaviour
{
    [Serializable]
    public class ObjectInfo
    {
        public int id;
        public string name;
    }

    public List<ObjectInfo> objs = new();
}
