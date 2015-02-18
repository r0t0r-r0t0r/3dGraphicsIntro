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
        private List<IRender> _renders = new List<IRender>
        {
            new TextureRender()
        };

        private float _cameraZPosition = 10;

        public RenderForm()
        {
            InitializeComponent();
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
                renders.Add(new TextureRender());
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
            pictureBox1.Image = RenderCore.Render(_renders, _cameraZPosition);
        }

        private void CameraZPositionHandler(object sender, EventArgs e)
        {
            _cameraZPosition = (float) numericUpDown1.Value;
            Draw();
        }
    }
}
