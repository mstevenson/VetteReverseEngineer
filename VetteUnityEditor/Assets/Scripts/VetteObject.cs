using System.Collections.Generic;
using UnityEngine;
using Vette;

public static class VetteObject
{
    public static Mesh CreateMesh(ObjResource obj)
    {
        var combiner = new CombineInstance[obj.polygons.polyCount];

        // TODO ignore the 4th point? We only need 3 points for a triangle, but Vette models might give all four points
        
        // Winding order is opposite of Unity
        for (var i = obj.polygons.polyCount - 1; i >= 0; i--)
        {
            var polygon = obj.polygons.polys[i];
            // Ignore lines which have three points
            if (polygon.vertexCount < 3)
            {
                return null;
            }
            
            var newPoly = new Poly2Mesh.Polygon
            {
                outside = new List<Vector3>()
            };
            foreach (var vertexIndex in polygon.vertexIndices)
            {
                var vert = obj.vertices.vertices[vertexIndex];
                
                // Y axis is inverted
                vert.y *= -1;
                
                newPoly.outside.Add(new Vector3(vert.x, vert.y, vert.z));
            }
            
            // TODO z-facing polys need their normals flipped

            var mesh = Poly2Mesh.CreateMesh(newPoly);
            combiner[i].mesh = mesh;
            combiner[i].transform = Matrix4x4.identity;
        }

        // Merge meshes

        var combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combiner);
        // combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }
}
