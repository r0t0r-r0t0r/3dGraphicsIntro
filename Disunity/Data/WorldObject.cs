using System;
using System.Numerics;
using Disunity.Data.Shaders;

namespace Disunity.Data
{
    public class WorldObject
    {
        private readonly Model _model;

        public WorldObject(Model model)
        {
            _model = model;
        }

        public Model Model
        {
            get { return _model; }
        }

        public Func<Shader> ShaderFactory { get; set; }
        public Func<Shader> FirstPhaseShaderFactory { get; set; }
        public Matrix4x4 ModelTransform { get; set; }
    }
}