using System.Collections.Generic;
using System.Numerics;

namespace Render
{
    public class Geometry
    {
        private readonly List<Vector3> _vertices;
        private readonly List<Vector3> _textureVertices;
        private readonly List<Vector3> _vertexNormals;
        private readonly List<Face> _faces;

        public Geometry(List<Vector3> vertices, List<Vector3> textureVertices, List<Vector3> vertexNormals, List<Face> faces)
        {
            _vertices = vertices;
            _textureVertices = textureVertices;
            _vertexNormals = vertexNormals;
            _faces = faces;
        }

        public List<Vector3> Vertices
        {
            get { return _vertices; }
        }

        public List<Vector3> TextureVertices
        {
            get { return _textureVertices; }
        }

        public List<Vector3> VertexNormals
        {
            get { return _vertexNormals; }
        }

        public List<Face> Faces
        {
            get { return _faces; }
        }
    }
}