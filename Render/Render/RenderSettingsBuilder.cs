using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public class RenderSettingsBuilder
    {
        public RenderSettingsBuilder()
        {
            RenderMode = FlatRenderMode.Fill;
            LightMode = FlatLightMode.NormalMapping;
            FillMode = FlatFillMode.Texture;
            PerspectiveProjection = true;
            ViewportScale = 0.9f;
        }

        public FlatRenderMode RenderMode { get; set; }
        public FlatLightMode LightMode { get; set; }
        public FlatFillMode FillMode { get; set; }
        public bool PerspectiveProjection { get; set; }
        public float ViewportScale { get; set; }

        public RenderSettings Build()
        {
            var settings = RenderSettings.Create(PerspectiveProjection, ViewportScale);

            if (RenderMode == FlatRenderMode.Borders)
            {
                return settings(Render.RenderMode.Borders());
            }

            LightMode lightMode;
            switch (LightMode)
            {
                case FlatLightMode.None:
                    lightMode = Render.LightMode.None;
                    break;
                case FlatLightMode.Simple:
                    lightMode = Render.LightMode.Simple;
                    break;
                case FlatLightMode.Gouraud:
                    lightMode = Render.LightMode.Gouraud;
                    break;
                case FlatLightMode.Phong:
                    lightMode = Render.LightMode.Phong;
                    break;
                case FlatLightMode.NormalMapping:
                    lightMode = Render.LightMode.NormalMapping;
                    break;
                default:
                    throw new ArgumentException();
            }

            FillMode fillMode;
            switch (FillMode)
            {
                case FlatFillMode.SolidColor:
                    fillMode = Render.FillMode.SolidColor(Color.WhiteSmoke);
                    break;
                case FlatFillMode.Texture:
                    fillMode = Render.FillMode.Texture();
                    break;
                default:
                    throw new ArgumentException();
            }

            if (RenderMode == FlatRenderMode.Fill)
            {
                return settings(Render.RenderMode.Fill(fillMode, lightMode));
            }

            if (RenderMode == FlatRenderMode.BordersAndFill)
            {
                return settings(Render.RenderMode.BordersAndFill(fillMode, lightMode));
            }

            throw new ArgumentException();
        }
    }

    public enum FlatRenderMode
    {
        Borders,
        Fill,
        BordersAndFill
    }

    public enum FlatLightMode
    {
        None,
        Simple,
        Gouraud,
        Phong,
        NormalMapping
    }

    public enum FlatFillMode
    {
        SolidColor,
        Texture
    }
}
