using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Render.Experiments;

namespace Render
{
    public partial class RenderForm : Form
    {
        private RenderCore _renderCore = new RenderCore();
        private RenderSettingsBuilder _builder = new RenderSettingsBuilder();

        private List<IRender> _renders = new List<IRender>
        {
            new SolidRender()
        };

        private float _cameraZPosition = 10;

        private bool _usePerspectiveProjection = true;

        public RenderForm()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            switch (_builder.RenderMode)
            {
                case FlatRenderMode.Borders:
                    bordersRadioButton.Checked = true;
                    break;
                case FlatRenderMode.Fill:
                    fillRadioButton.Checked = true;
                    break;
                case FlatRenderMode.BordersAndFill:
                    bordersAndFillRadioButton.Checked = true;
                    break;
            }

            switch (_builder.FillMode)
            {
                case FlatFillMode.Texture:
                    textureRadioButton.Checked = true;
                    break;
                case FlatFillMode.SolidColor:
                    solidColorRadioButton.Checked = true;
                    break;
            }

            switch (_builder.LightMode)
            {
                case FlatLightMode.None:
                    flatRadioButton.Checked = true;
                    break;
                case FlatLightMode.Simple:
                    simpleRadioButton.Checked = true;
                    break;
                case FlatLightMode.Gouraud:
                    gouraudRadioButton.Checked = true;
                    break;
                case FlatLightMode.Phong:
                    phongRadioButton.Checked = true;
                    break;
            }

            distanceNumericUpDown.Value = (decimal) _builder.CameraZPosition;
            perspectiveProjectionCheckBox.Checked = _builder.PerspectiveProjection;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw();
            pictureBox2.Image = FilledTriangle.Render();
            pictureBox3.Image = YBuffer.Render();
            pictureBox4.Image = Line.Render();
        }

        private void RendersHandler(object sender, EventArgs e)
        {
            var renders = new List<IRender>();

            if (checkBox1.Checked)
            {
                renders.Add(new SolidRender());
            }
            if (checkBox2.Checked)
            {
                renders.Add(new WireRender());
            }
            _renders = renders;
            Draw();
        }

        private void Draw()
        {
            pictureBox1.Image = _renderCore.Render(_renders, _cameraZPosition, _usePerspectiveProjection);
        }

        private void CameraZPositionHandler(object sender, EventArgs e)
        {
            _cameraZPosition = (float) distanceNumericUpDown.Value;
            Draw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _usePerspectiveProjection = perspectiveProjectionCheckBox.Checked;
            Draw();
        }

        private void bordersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = FlatRenderMode.Borders;
            Draw();
        }

        private void fillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = FlatRenderMode.Fill;
            Draw();
        }

        private void bordersAndFillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = FlatRenderMode.BordersAndFill;
            Draw();
        }

        private void flatRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = FlatLightMode.None;
            Draw();
        }

        private void simpleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = FlatLightMode.Simple;
            Draw();
        }

        private void gouraudRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = FlatLightMode.Gouraud;
            Draw();
        }

        private void phongRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = FlatLightMode.Phong;
            Draw();
        }

        private void solidColorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.FillMode = FlatFillMode.SolidColor;
            Draw();
        }

        private void textureRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.FillMode = FlatFillMode.Texture;
            Draw();
        }
    }
}
