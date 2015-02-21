using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public class RenderSettings
    {
        public RenderMode RenderMode { get; set; }
        bool PerspectiveProjection { get; set; }
        float CameraZPosition { get; set; }

    }

    public class RenderMode
    {
        private RenderMode()
        {
            
        }

        public static RenderMode Borders()
        {
            throw new NotImplementedException();
        }

        public static RenderMode Fill(FillMode fillMode, LightMode lightMode)
        {
            throw new NotImplementedException();
        }

        public static RenderMode BordersAndFill(FillMode mode, LightMode lightMode)
        {
            throw new NotImplementedException();
        }
    }

    public enum LightMode
    {
        None,
        Simple,
        Gouraud,
        Phong
    }

    public class FillMode
    {
        private FillMode()
        {
            
        }

        public static FillMode SolidColor(Color color)
        {
            throw new NotImplementedException();
        }

        public static FillMode Texture()
        {
            throw new NotImplementedException();
        }
    }

}
