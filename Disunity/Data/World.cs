using System.Numerics;

namespace Disunity.Models
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

        public bool TwoPhaseRendering { get; set; }

        public WorldObject WorldObject
        {
            get { return _worldObject; }
        }
    }
}
