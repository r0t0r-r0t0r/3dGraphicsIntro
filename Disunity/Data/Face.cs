using Disunity.Data.Common;

namespace Disunity.Data
{
    public class Face
    {
        private readonly Triple<int> _vertices;
        private readonly Triple<int> _textureVertices;
        private readonly Triple<int> _normals;

        public Face(Triple<int> vertices, Triple<int> textureVertices, Triple<int> normals)
        {
            _vertices = vertices;
            _textureVertices = textureVertices;
            _normals = normals;
        }

        public int GetVertexIndex(int i)
        {
            return _vertices.GetValue(i);
        }

        public int GetTextureVertexIndex(int i)
        {
            return _textureVertices.GetValue(i);
        }

        public int GetNormalIndex(int i)
        {
            return _normals.GetValue(i);
        }
    }
}