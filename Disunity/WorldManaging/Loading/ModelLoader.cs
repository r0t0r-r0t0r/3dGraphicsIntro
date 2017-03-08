using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Disunity.Data;

namespace Disunity.WorldManaging.Loading
{
    public class ModelLoader
    {
        public static Model LoadModel(string rootDir, string geometry, string texture, string normalMap,
            string specularMap)
        {
            var g = LoadGeometry(Path.Combine(rootDir, geometry));
            var t = new Texture(Image.FromFile(Path.Combine(rootDir, texture)));
            var nm = new Texture(Image.FromFile(Path.Combine(rootDir, normalMap)));
            var sm = new Texture(Image.FromFile(Path.Combine(rootDir, specularMap)));

            return new Model(g, t, nm, sm);
        }

        private static Geometry LoadGeometry(string fileName)
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

            TextParser.Empty
                .AddLineParser(vertexLine, ParseVector3, x => vertices.Add(x))
                .AddLineParser(textureVertexLine, ParseVector3, x => textureVertices.Add(x))
                .AddLineParser(vertexNormalLine, ParseVector3, x => vertexNormals.Add(x))
                .AddLineParser(faceLine, ParseFace, x => faces.Add(x))
                .Parse(lines);

            return new Geometry(vertices, textureVertices, vertexNormals, faces);
        }

        private static Face ParseFace(Match faceMatch)
        {
            Func<string, int> parseInt = value => int.Parse(value, CultureInfo.InvariantCulture);

            var indexes = faceMatch.ParseGroups(9, parseInt).Select(x => x - 1).ToArray();
            var v = indexes.ToTriple(0, 3, 6);
            var tv = indexes.ToTriple(1, 4, 7);
            var vn = indexes.ToTriple(2, 5, 8);
            var face = new Face(v, tv, vn);
            return face;
        }

        private static Vector3 ParseVector3(Match match)
        {
            Func<string, float> parseFloat = value => float.Parse(value, CultureInfo.InvariantCulture);
            return match.ParseGroups(3, parseFloat).ToVector3();
        }
    }
}
