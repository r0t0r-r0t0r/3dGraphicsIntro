using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Render
{
    public class ModelLoader
    {
        public static Model LoadModel(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            var vertices = new List<Vector3>();
            var vertexLine = new Regex("^v ([^ ]+) ([^ ]+) ([^ ]+)$");

            var textureVertices = new List<Vector3>();
            var textureVertexLine = new Regex(@"^vt\s+([^ ]+) ([^ ]+) ([^ ]+)$");

            var vertexNormals = new List<Vector3>();
            var vertexNormalLine = new Regex(@"^vn\s+([^ ]+) ([^ ]+) ([^ ]+)$");

            var faces = new List<Face>();
            var faceLine = new Regex("^f ([^/]+)/([^/]+)/([^/]+) ([^/]+)/([^/]+)/([^/]+) ([^/]+)/([^/]+)/([^/]+)$");

            foreach (var line in lines)
            {
                var vertMatch = vertexLine.Match(line);
                var faceMatch = faceLine.Match(line);
                var textureVertMatch = textureVertexLine.Match(line);
                var vertexNormalMatch = vertexNormalLine.Match(line);

                if (vertMatch.Success)
                {
                    var x = float.Parse(vertMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(vertMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(vertMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var vertex = new Vector3(x, y, z);

                    vertices.Add(vertex);
                }
                else if (faceMatch.Success)
                {
                    var a = int.Parse(faceMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var ta = int.Parse(faceMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var na = int.Parse(faceMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var b = int.Parse(faceMatch.Groups[4].Value, CultureInfo.InvariantCulture);
                    var tb = int.Parse(faceMatch.Groups[5].Value, CultureInfo.InvariantCulture);
                    var nb = int.Parse(faceMatch.Groups[6].Value, CultureInfo.InvariantCulture);
                    var c = int.Parse(faceMatch.Groups[7].Value, CultureInfo.InvariantCulture);
                    var tc = int.Parse(faceMatch.Groups[8].Value, CultureInfo.InvariantCulture);
                    var nc = int.Parse(faceMatch.Groups[9].Value, CultureInfo.InvariantCulture);
                    var face = new Face(a - 1, b - 1, c - 1, ta - 1, tb - 1, tc - 1, na - 1, nb - 1, nc - 1);

                    faces.Add(face);
                }
                else if (textureVertMatch.Success)
                {
                    var x = float.Parse(textureVertMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(textureVertMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(textureVertMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var vertex = new Vector3(x, y, z);

                    textureVertices.Add(vertex);
                }
                else if (vertexNormalMatch.Success)
                {
                    var x = float.Parse(vertexNormalMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(vertexNormalMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(vertexNormalMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var vertex = new Vector3(x, y, z);

                    vertexNormals.Add(vertex); 
                }
            }

            return new Model(vertices, textureVertices, faces, vertexNormals);
        }
    }
}
