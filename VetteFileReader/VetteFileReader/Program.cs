using System;

namespace VetteFileReader
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			var obj = FileParser.Parse<Obj> (@"/Users/mike/Desktop/vette_models/building400x400x600");
			var obj = FileParser.Parse<Obj> (@"/Users/mike/Desktop/vette_models/Hwy1");

			Console.WriteLine ("File length: " + obj.fileLength);
			Console.WriteLine ("Vertices: " + obj.vertices.vertexCount);
			foreach (var v in obj.vertices.vertices) {
				Console.WriteLine ("  {0,6} {1,6} {2,6}", v.x, v.y, v.z);
			}

			Console.WriteLine ("Geo Elements: " + obj.geoElements.geoCount);
			foreach (var q in obj.geoElements.geos) {
				Console.Write (q.drawMode);

				if (q.drawMode == DrawMode.Line) {
					Console.Write ("  " + q.line.vertIndexA + " " + q.line.vertIndexB);
				}

				Console.WriteLine ();

//				Console.WriteLine ("  {0,4} {1,4} / {2,4} {3,4} / {4,4} {5,4} / {6,4} {7,4}",
//				                   q.lines[0].vertIndex1,
//				                   q.lines[0].vertIndex2,
//				                   q.lines[1].vertIndex1,
//				                   q.lines[1].vertIndex2,
//				                   q.lines[2].vertIndex1,
//				                   q.lines[2].vertIndex2,
//				                   q.lines[3].vertIndex1,
//				                   q.lines[3].vertIndex2);
			}
		}
	}
}
