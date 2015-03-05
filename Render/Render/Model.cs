using System.Collections.Generic;
using System.Numerics;

namespace Render
{
    public class Model
    {
        private readonly List<Vector3> _vertices;
        private readonly List<Vector3> _textureVertices;
        private readonly List<Face> _faces;
        private readonly List<Vector3> _vertexNormals;

        public Model(List<Vector3> vertices, List<Vector3> textureVertices, List<Face> faces, List<Vector3> vertexNormals)
        {
            _vertices = vertices;
            _textureVertices = textureVertices;
            _faces = faces;
            _vertexNormals = vertexNormals;
        }

        public List<Vector3> Vertices
        {
            get { return _vertices; }
        }

        public List<Vector3> TextureVertices
        {
            get { return _textureVertices; }
        }

        public List<Face> Faces
        {
            get { return _faces; }
        }

        public List<Vector3> VertexNormals
        {
            get { return _vertexNormals; }
        }
    }

    public static class ModelUtils
    {
        public static Vector3 GetVertex(this Model model, int faceIndex, int vertexIndex)
        {
            var index = model.Faces[faceIndex][vertexIndex];
            var vertex = model.Vertices[index];

            return vertex;
        }

        public static Vector3 GetVertexNormal(this Model model, int faceIndex, int vertexIndex)
        {
            var index = model.Faces[faceIndex].GetNormalIndex(vertexIndex);
            var vertex = model.VertexNormals[index];

            return vertex;
        }

        public static Vector3 GetTextureVertex(this Model model, int faceIndex, int vertexIndex)
        {
            var index = model.Faces[faceIndex].GetVtIndex(vertexIndex);
            var vertex = model.TextureVertices[index];

            return vertex;
        }
    }
}