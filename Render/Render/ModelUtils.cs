using System.Numerics;

namespace Render
{
    public static class ModelUtils
    {
        public static Vector3 GetVertex(this Geometry geometry, int faceIndex, int vertexIndex)
        {
            var index = geometry.Faces[faceIndex].GetVertexIndex(vertexIndex);
            var vertex = geometry.Vertices[index];

            return vertex;
        }

        public static Vector3 GetVertexNormal(this Geometry geometry, int faceIndex, int vertexIndex)
        {
            var index = geometry.Faces[faceIndex].GetNormalIndex(vertexIndex);
            var vertex = geometry.VertexNormals[index];

            return vertex;
        }

        public static Vector3 GetTextureVertex(this Geometry geometry, int faceIndex, int vertexIndex)
        {
            var index = geometry.Faces[faceIndex].GetTextureVertexIndex(vertexIndex);
            var vertex = geometry.TextureVertices[index];

            return vertex;
        }
    }
}