using System;

namespace Disunity.Data
{
    public sealed class Model: IDisposable
    {
        private readonly Geometry _geometry;
        private readonly Texture _texture;
        private readonly Texture _normalMap;
        private readonly Texture _specularMap;

        public Model(Geometry geometry, Texture texture, Texture normalMap, Texture specularMap)
        {
            _geometry = geometry;
            _texture = texture;
            _normalMap = normalMap;
            _specularMap = specularMap;
        }

        public Geometry Geometry
        {
            get { return _geometry; }
        }

        public Texture Texture
        {
            get { return _texture; }
        }

        public Texture NormalMap
        {
            get { return _normalMap; }
        }

        public Texture SpecularMap
        {
            get { return _specularMap; }
        }

        public void Dispose()
        {
            _texture.Dispose();
            _normalMap.Dispose();
            _specularMap.Dispose();
        }
    }
}
