using System.Numerics;

namespace Render
{
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