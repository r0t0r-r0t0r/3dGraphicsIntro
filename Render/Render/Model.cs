using System.Collections.Generic;
using System.Numerics;

namespace Render
{
    public class Model
    {
        private readonly List<Vector3> _vertices;
        private readonly List<Vector3> _textureVertices;
        private readonly List<Face> _faces;

        public Model(List<Vector3> vertices, List<Vector3> textureVertices, List<Face> faces)
        {
            _vertices = vertices;
            _textureVertices = textureVertices;
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

        public List<Face> Faces
        {
            get { return _faces; }
        }
    }
}