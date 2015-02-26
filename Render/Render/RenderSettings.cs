﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public class RenderSettings
    {
        private readonly RenderMode _renderMode;
        private readonly bool _perspectiveProjection;
        private readonly float _cameraZPosition;

        private RenderSettings(RenderMode renderMode, bool perspectiveProjection, float cameraZPosition)
        {
            _renderMode = renderMode;
            _perspectiveProjection = perspectiveProjection;
            _cameraZPosition = cameraZPosition;
        }

        public static Func<RenderMode, RenderSettings> Create(bool perspectiveProjection, float cameraZPosition)
        {
            return rm => new RenderSettings(rm, perspectiveProjection, cameraZPosition);
        }

        public RenderMode RenderMode { get { return _renderMode; }}
        public bool PerspectiveProjection { get { return _perspectiveProjection; }}
        public float CameraZPosition { get { return _cameraZPosition; }}
    }

    public class RenderMode
    {
        private readonly bool _useFilling;
        private readonly bool _useBorders;
        private readonly FillMode _fillMode;
        private readonly LightMode _lightMode;

        private RenderMode(bool useFilling, bool useBorders, FillMode fillMode, LightMode lightMode)
        {
            _useFilling = useFilling;
            _useBorders = useBorders;
            _fillMode = fillMode;
            _lightMode = lightMode;
        }

        public bool UseFilling
        {
            get { return _useFilling; }
        }

        public bool UseBorders
        {
            get { return _useBorders; }
        }

        public FillMode FillMode
        {
            get
            {
                if (!_useFilling)
                {
                    throw new InvalidOperationException();
                }
                return _fillMode;
            }
        }

        public LightMode LightMode
        {
            get
            {
                if (!_useFilling)
                {
                    throw new InvalidOperationException();
                }
                return _lightMode;
            }
        }

        public static RenderMode Borders()
        {
            return new RenderMode(false, true, null, LightMode.None);
        }

        public static RenderMode Fill(FillMode fillMode, LightMode lightMode)
        {
            return new RenderMode(true, false, fillMode, lightMode);
        }

        public static RenderMode BordersAndFill(FillMode fillMode, LightMode lightMode)
        {
            return new RenderMode(true, true, fillMode, lightMode);
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
        private readonly bool _useTexture;
        private readonly Color _color;

        private FillMode(bool useTexture, Color color)
        {
            _useTexture = useTexture;
            _color = color;
        }

        public bool UseTexture
        {
            get { return _useTexture; }
        }

        public Color Color
        {
            get
            {
                if (_useTexture)
                {
                    throw new InvalidOperationException();
                }
                return _color;
            }
        }

        public static FillMode SolidColor(Color color)
        {
            return new FillMode(false, color);
        }

        public static FillMode Texture()
        {
            return new FillMode(true, new Color());
        }
    }

}