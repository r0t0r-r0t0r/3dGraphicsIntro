using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public class World
    {
        private readonly WorldObject _worldObject;

        public World(WorldObject worldObject)
        {
            _worldObject = worldObject;
        }

        public RenderMode RenderMode { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 CameraDirection { get; set; }

        public Matrix4x4 ViewportTransform { get; set; }
        public Matrix4x4 ProjectionTransform { get; set; }
        public Matrix4x4 ViewTransform { get; set; }

        public WorldObject WorldObject
        {
            get { return _worldObject; }
        }
    }

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
        public Matrix4x4 ModelTransform { get; set; }
    }
}
