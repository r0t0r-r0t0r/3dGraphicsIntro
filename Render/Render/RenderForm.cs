using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Numerics;
using Render.Benchmarking;

namespace Render
{
    public partial class RenderForm : Form
    {
        private const string BenchmarkLogFileName = @"..\..\BenchmarkLog.txt";
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;
        
        private readonly RenderCore _renderCore = new RenderCore(ViewportWidth, ViewportHeight);
        private readonly WorldBuilder _builder = new WorldBuilder(ViewportWidth, ViewportHeight);

        private Bitmap _frontBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);
        private Bitmap _backBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);

        private MouseMode _mouseMode;

        public RenderForm()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            switch (_builder.RenderMode)
            {
                case RenderMode.Borders:
                    bordersRadioButton.Checked = true;
                    break;
                case RenderMode.Fill:
                    fillRadioButton.Checked = true;
                    break;
                case RenderMode.BordersAndFill:
                    bordersAndFillRadioButton.Checked = true;
                    break;
            }

            switch (_builder.FillMode)
            {
                case FillMode.Texture:
                    textureRadioButton.Checked = true;
                    break;
                case FillMode.SolidColor:
                    solidColorRadioButton.Checked = true;
                    break;
            }

            switch (_builder.LightMode)
            {
                case LightMode.None:
                    flatRadioButton.Checked = true;
                    break;
                case LightMode.Simple:
                    simpleRadioButton.Checked = true;
                    break;
                case LightMode.Gouraud:
                    gouraudRadioButton.Checked = true;
                    break;
                case LightMode.Phong:
                    phongRadioButton.Checked = true;
                    break;
                case LightMode.NormalMapping:
                    normalMappingRadioButton.Checked = true;
                    break;
            }

            viewportScaleNumericUpDown.Value = (decimal) _builder.ViewportScale;
            perspectiveProjectionCheckBox.Checked = _builder.PerspectiveProjection;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw();
            LoadLastBenchmarkResult();
        }

        private void Draw()
        {
            var world = _builder.BuildWorld();
            _renderCore.Render(world, _backBuffer);
            pictureBox1.Image = _backBuffer;

            var exchange = _backBuffer;
            _backBuffer = _frontBuffer;
            _frontBuffer = exchange;
        }

        private void CameraZPositionHandler(object sender, EventArgs e)
        {
            _builder.ViewportScale = (float) viewportScaleNumericUpDown.Value;
            Draw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _builder.PerspectiveProjection = perspectiveProjectionCheckBox.Checked;
            Draw();
        }

        private void bordersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = RenderMode.Borders;
            Draw();
        }

        private void fillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = RenderMode.Fill;
            Draw();
        }

        private void bordersAndFillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.RenderMode = RenderMode.BordersAndFill;
            Draw();
        }

        private void flatRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = LightMode.None;
            Draw();
        }

        private void simpleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = LightMode.Simple;
            Draw();
        }

        private void gouraudRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = LightMode.Gouraud;
            Draw();
        }

        private void phongRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = LightMode.Phong;
            Draw();
        }

        private void normalMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = LightMode.NormalMapping;
            Draw();
        }

        private void solidColorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.FillMode = FillMode.SolidColor;
            Draw();
        }

        private void textureRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.FillMode = FillMode.Texture;
            Draw();
        }

        private async void startBenchmarkButton_Click(object sender, EventArgs e)
        {
            var benchmark = BenchmarkFactory.Create();

            _builder.SetFrom(benchmark.Settings);
            InitializeSettings();
            Draw();
            lastBenchmarkTimeLabel.Text = "Started";
            mainPanel.Enabled = false;

            try
            {
                var result = await benchmark.Start().ConfigureAwait(false);

                var runtime = result.FrameRenderingDuration;
                lastBenchmarkTimeLabel.Text = runtime.ToString("F4");
                WriteBenchmarkResult(DateTime.Now, runtime);
            }
            finally
            {
                mainPanel.Enabled = true;
            }
        }

        private void LoadLastBenchmarkResult()
        {
            var lastLine = new []{"0 0"}
                .Concat(File.ReadLines(BenchmarkLogFileName))
                .Last();
            var lineValues = lastLine.Split(' ');
            var lastBenchmarkTime = double.Parse(lineValues[1], CultureInfo.InvariantCulture);

            lastBenchmarkTimeLabel.Text = lastBenchmarkTime.ToString("F4");
        }

        private void WriteBenchmarkResult(DateTime dateTime, double benchmarkTime)
        {
            var line = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dateTime.Ticks, benchmarkTime);
            using (var benchmarkLog = File.AppendText(BenchmarkLogFileName))
            {
                benchmarkLog.WriteLine(line);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            switch (_mouseMode)
            {
                case MouseMode.RotateLight:
                    _builder.ViewportLightX = e.X;
                    _builder.ViewportLightY = e.Y;
                    break;
                case MouseMode.RotateModel:
                    _builder.ModelRotationX = ((float)ViewportWidth/2 - e.X)/((float)ViewportWidth/2)*(float)Math.PI/2;
                    _builder.ModelRotationY = ((float)ViewportHeight/2 - e.Y)/((float)ViewportHeight/2)*(float)Math.PI/2;
                    break;
            }

            Draw();
        }

        private void rotateLightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _mouseMode = MouseMode.RotateLight;
        }

        private void rotateModelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _mouseMode = MouseMode.RotateModel;
        }
    }

    internal enum MouseMode
    {
        RotateLight,
        RotateModel
    }
}
