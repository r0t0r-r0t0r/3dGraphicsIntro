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
}