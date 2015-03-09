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
                return settings(global::Render.RenderMode.Borders());
            }

            LightMode lightMode;
            switch (LightMode)
            {
                case FlatLightMode.None:
                    lightMode = global::Render.LightMode.None;
                    break;
                case FlatLightMode.Simple:
                    lightMode = global::Render.LightMode.Simple;
                    break;
                case FlatLightMode.Gouraud:
                    lightMode = global::Render.LightMode.Gouraud;
                    break;
                case FlatLightMode.Phong:
                    lightMode = global::Render.LightMode.Phong;
                    break;
                case FlatLightMode.NormalMapping:
                    lightMode = global::Render.LightMode.NormalMapping;
                    break;
                default:
                    throw new ArgumentException();
            }

            FillMode fillMode;
            switch (FillMode)
            {
                case FlatFillMode.SolidColor:
                    fillMode = global::Render.FillMode.SolidColor(Color.WhiteSmoke);
                    break;
                case FlatFillMode.Texture:
                    fillMode = global::Render.FillMode.Texture();
                    break;
                default:
                    throw new ArgumentException();
            }

            if (RenderMode == FlatRenderMode.Fill)
            {
                return settings(global::Render.RenderMode.Fill(fillMode, lightMode));
            }

            if (RenderMode == FlatRenderMode.BordersAndFill)
            {
                return settings(global::Render.RenderMode.BordersAndFill(fillMode, lightMode));
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

    public static class FlatRenderModeUtils
    {
        public static bool UseFill(this FlatRenderMode renderMode)
        {
            return renderMode == FlatRenderMode.Fill || renderMode == FlatRenderMode.BordersAndFill;
        }

        public static bool UseBorders(this FlatRenderMode renderMode)
        {
            return renderMode == FlatRenderMode.Borders || renderMode == FlatRenderMode.BordersAndFill;
        }
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
