using UnityEngine;
using Vette;

// Display a vette model in the editor

[ExecuteInEditMode]
public class VetteEditorDisplay : MonoBehaviour
{
    public VetteDataAsset vetteDataAsset;

    // ObjResource obj;
    
    void OnEnable ()
    {
        // var path = Application.dataPath + "/../../Data/Resources/OBJS/" + objectFile;
        // obj = FileParser.Parse<Obj> (path);
        // foreach (var p in obj.polygons.polys) {
        //     Debug.Log (p.drawMode);
        // }
    }

    // void OnDrawGizmosSelected () {
    //     if (obj.vertices.vertexCount > 0) {
    //         var pivot = obj.vertices.vertices[0];
    //         var xMod = obj.vertices.vertices[1];
    //         var yMod = obj.vertices.vertices[2];
    //         var zMod = obj.vertices.vertices[3];
    //
    //         for (int i = 0; i < obj.vertices.vertices.Length; i++) {
    //             var v = obj.vertices.vertices[i];
    //             Vector3 position = new Vector3 (v.x, -v.y, v.z);
    //             if (i == 0) {
    //                 Gizmos.color = Color.yellow;
    //             } else if (i == 1) {
    //                 Gizmos.color = Color.red;
    //             } else if (i == 2) {
    //                 Gizmos.color = Color.green;
    //             } else if (i == 3) {
    //                 Gizmos.color = Color.blue;
    //             } else {
    //                 Gizmos.color = Color.cyan;
    //             }
    //             Gizmos.DrawCube (position, Vector3.one * 20f);
    //         }
    //
    //         var allVertices = obj.vertices.vertices;
    //         var polygons = obj.polygons.polys;
    //
    //         for (int p = 0; p < obj.polygons.polyCount; p++) {
    //             var vertIndices = polygons[p].vertexIndices;
    //             for (int v = 1; v < vertIndices.Length; v++) {
    //                 Vertex lineStart = allVertices [vertIndices [v - 1]];
    //                 Vertex lineEnd = allVertices [vertIndices [v]];
    //                 Gizmos.DrawLine (new Vector3 (lineStart.x, -lineStart.y, lineStart.z), new Vector3 (lineEnd.x, -lineEnd.y, lineEnd.z));
    //             }
    //         }
    //     }
    // }
}
