using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelViewportRenderer : MonoBehaviour
{
    public VetteRawDataAsset data;

    public int modelId;
    public int maxFaces = 50;

    private void OnDrawGizmos()
    {
        var obj = data.data.objs[modelId];

        for (var index = 0; index < obj.polygons.polys.Length; index++)
        {
            if (index > maxFaces)
            {
                break;
            }
            
            var polygon = obj.polygons.polys[index];
            for (var i = 1; i < polygon.vertexIndices.Length; i++)
            {
                var vertexIndexA = polygon.vertexIndices[i - 1];
                var vertexIndexB = polygon.vertexIndices[i];

                var v1 = obj.vertices.vertices[vertexIndexA];
                var v2 = obj.vertices.vertices[vertexIndexB];

                Gizmos.DrawLine(new Vector3(v1.x, -v1.y, v1.z), new Vector3(v2.x, -v2.y, v2.z));
            }
        }
    }
}
