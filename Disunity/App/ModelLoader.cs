using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Disunity.App.Lib.Parsing;
using Disunity.Data;

namespace Disunity.App
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

    internal static class TripleUtils
    {
        public static Triple<T> ToTriple<T>(this T[] values, int index1, int index2, int index3) where T : struct
        {
            return new Triple<T>(values[index1], values[index2], values[index3]);
        }

        public static Vector3 ToVector3(this IEnumerable<float> values)
        {
            var materialized = values.ToArray();
            return new Vector3(materialized[0], materialized[1], materialized[2]);
        }
    }

    internal static class MatchUtils
    {
        public static IEnumerable<T> ParseGroups<T>(this Match match, int count, Func<string, T> parser)
        {
            return match.Groups.Cast<Group>().Skip(1).Take(count).Select(g => parser(g.Value));
        }
    }

    internal static class GeometryParsers
    {
        private static readonly Parser<Vector3> Vector3 =
            Parsers.Float().And1(PrimitiveParsers.String(" "))
                .And(Parsers.Float())
                .And1(PrimitiveParsers.String(" "))
                .And(Parsers.Float())
                .Select(x => new Vector3(x._1, x._2, x._3));

        private static readonly Parser<Line> Vertex =
            AndParsers.String1("v ").And(Vector3).And1(Parsers.NewLine()).Select(Line.Vertex).Scope("Vertex");

        private static readonly Parser<Line> TextureVertex =
            AndParsers.String1("vt ").And(Vector3).And1(Parsers.NewLine()).Select(Line.TextureVertex).Scope("Texture Vertex");

        private static readonly Parser<Line> VertexNormal =
            AndParsers.String1("vn ").And(Vector3).And1(Parsers.NewLine()).Select(Line.VertexNormal).Scope("Vertex Normal");

        private static readonly Parser<Line> Face = CreateFaceParser().Scope("Face");

        private static Parser<Line> CreateFaceParser()
        {
            var threeIndices =
                Parsers.Int().And1(PrimitiveParsers.String("/"))
                    .And(Parsers.Int())
                    .And1(PrimitiveParsers.String("/"))
                    .And(Parsers.Int())
                    .Select(x => new {v = x._1, vt = x._2, vn = x._3});

            return
                AndParsers.String1("f ")
                    .And(threeIndices)
                    .And1(PrimitiveParsers.String(" "))
                    .And(threeIndices)
                    .And1(PrimitiveParsers.String(" "))
                    .And(threeIndices)
                    .And1(Parsers.NewLine())
                    .Select(x =>
                    {
                        var v1 = x._1;
                        var v2 = x._2;
                        var v3 = x._3;

                        var vs = new Triple<int>(v1.v, v2.v, v3.v);
                        var vts = new Triple<int>(v1.vt, v2.vt, v3.vt);
                        var vns = new Triple<int>(v1.vn, v2.vn, v3.vn);

                        var f = new Face(vs, vts, vns);

                        return Line.Face(f);
                    });
        }

        public static Parser<Geometry> GeometryParser
        {
            get
            {
                var line = Vertex.Or(TextureVertex).Or(VertexNormal).Or(Face);

                var lines = Parsers.Many(line);

                var geometry = lines.Select(LinesToGeometry);

                return geometry;
            }

        }

        private static Geometry LinesToGeometry(IEnumerable<Line> lines)
        {
            var vertices = new List<Vector3>();
            var textureVertices = new List<Vector3>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<Face>();

            foreach (var line in lines)
            {
                line.Match(
                    vertex: v => vertices.Add(v),
                    textureVertex: tv => textureVertices.Add(tv),
                    vertexNormal: vn => vertexNormals.Add(vn),
                    face: f => faces.Add(f)
                    );
            }

            return new Geometry(vertices, textureVertices, vertexNormals, faces);
        }

        internal abstract class Line
        {
            private class VertexLine : Line
            {
                private readonly Vector3 _vector;

                public VertexLine(Vector3 vector)
                {
                    _vector = vector;
                }

                protected override void MatchCore(MatchParams parameters)
                {
                    parameters.VertexAction(_vector);
                }
            }

            private class TextureVertexLine : Line
            {
                private readonly Vector3 _vector;

                public TextureVertexLine(Vector3 vector)
                {
                    _vector = vector;
                }

                protected override void MatchCore(MatchParams parameters)
                {
                    parameters.TextureVertexAction(_vector);
                }
            }

            private class VertexNormalLine : Line
            {
                private readonly Vector3 _vector;

                public VertexNormalLine(Vector3 vector)
                {
                    _vector = vector;
                }

                protected override void MatchCore(MatchParams parameters)
                {
                    parameters.VertexNormalAction(_vector);
                }
            }

            private class FaceLine : Line
            {
                private readonly Face _face;

                public FaceLine(Face face)
                {
                    _face = face;
                }

                protected override void MatchCore(MatchParams parameters)
                {
                    parameters.FaceAction(_face);
                }
            }

            public static Line Vertex(Vector3 vector)
            {
                return new VertexLine(vector);
            }

            public static Line TextureVertex(Vector3 vector)
            {
                return new TextureVertexLine(vector);
            }

            public static Line VertexNormal(Vector3 vector)
            {
                return new VertexNormalLine(vector);
            }

            public static Line Face(Face face)
            {
                return new FaceLine(face);
            }

            public void Match(Action<Vector3> vertex, Action<Vector3> textureVertex, Action<Vector3> vertexNormal, Action<Face> face)
            {
                MatchCore(new MatchParams(vertex, textureVertex, vertexNormal, face));
            }

            protected abstract void MatchCore(MatchParams parameters);

            protected class MatchParams
            {
                public MatchParams(Action<Vector3> vertex, Action<Vector3> textureVertex, Action<Vector3> vertexNormal, Action<Face> face)
                {
                    VertexAction = vertex;
                    TextureVertexAction = textureVertex;
                    VertexNormalAction = vertexNormal;
                    FaceAction = face;
                }

                public Action<Vector3> VertexAction { get; }
                public Action<Vector3> TextureVertexAction { get; }
                public Action<Vector3> VertexNormalAction { get; }
                public Action<Face> FaceAction { get; }
            }
        }
    }
}
