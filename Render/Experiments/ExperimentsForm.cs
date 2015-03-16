using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Experiments
{
    public partial class ExperimentsForm : Form
    {
        public ExperimentsForm()
        {
            InitializeComponent();

            pictureBox2.Image = FilledTriangle.Render();
            pictureBox3.Image = YBuffer.Render();
            pictureBox4.Image = Line.Render();
        }
    }
}
