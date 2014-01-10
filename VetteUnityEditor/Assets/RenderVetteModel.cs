using UnityEngine;
using System.Collections;
using VetteFileReader;

// Display a vette model in the editor

[ExecuteInEditMode]
public class RenderVetteModel : MonoBehaviour {

	public Obj obj;

	void OnEnable ()
	{
		obj = FileParser.Parse<Obj> (@"/Users/mike/Desktop/vette_models/lawyer");
	}

	void OnDrawGizmosSelected () {
		if (obj.vertices.vertexCount > 0) {
			var pivot = obj.vertices.vertices[0];
			var xMod = obj.vertices.vertices[1];
			var yMod = obj.vertices.vertices[2];
			var zMod = obj.vertices.vertices[3];

			for (int i = 0; i < obj.vertices.vertices.Length; i++) {
				var v = obj.vertices.vertices[i];
				if (i == 0) {
					Gizmos.color = Color.yellow;
				} else if (i == 1) {
					Gizmos.color = Color.red;
				} else if (i == 2) {
					Gizmos.color = Color.green;
				} else if (i == 3) {
					Gizmos.color = Color.blue;
				} else {
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawCube (new Vector3 (v.x, -v.y, v.z), Vector3.one * 20f);
			}

			for (int i = 0; i < obj.geoElements.geoCount; i++) {
				var g = obj.geoElements.geos[i];
				var verts = obj.vertices.vertices;
				if (g.drawMode == DrawMode.Line) {
					var va = verts[g.line.vertIndexA];
					var vb = verts[g.line.vertIndexB];
					Gizmos.DrawLine (new Vector3 (va.x, -va.y, va.z), new Vector3 (vb.x, -vb.y, vb.z));
				}
			}
		}
	}
}
