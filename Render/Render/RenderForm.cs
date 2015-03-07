﻿using System;
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
using Render.Experiments;
using System.IO;

namespace Render
{
    public partial class RenderForm : Form
    {
        private const string BenchmarkLogFileName = @"..\..\BenchmarkLog.txt";
        
        private readonly RenderCore _renderCore = new RenderCore();
        private readonly RenderSettingsBuilder _builder = new RenderSettingsBuilder();

        private Bitmap _frontBuffer = new Bitmap(800, 800, PixelFormat.Format32bppRgb);
        private Bitmap _backBuffer = new Bitmap(800, 800, PixelFormat.Format32bppRgb);

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
                case FlatLightMode.NormalMapping:
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
            pictureBox2.Image = FilledTriangle.Render();
            pictureBox3.Image = YBuffer.Render();
            pictureBox4.Image = Line.Render();
        }

        private void Draw()
        {
            _renderCore.Render(_builder.Build(), _backBuffer);
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

        private void normalMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _builder.LightMode = FlatLightMode.NormalMapping;
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

        private void startBenchmarkButton_Click(object sender, EventArgs e)
        {
            _builder.ViewportScale = 1.5f;
            _builder.FillMode = FlatFillMode.Texture;
            _builder.LightMode = FlatLightMode.Gouraud;
            _builder.PerspectiveProjection = true;
            _builder.RenderMode = FlatRenderMode.Fill;
            InitializeSettings();
            Draw();

            var buffer = new Bitmap(800, 800, PixelFormat.Format32bppRgb);
            var renderCore = new RenderCore();

            var settings = _builder.Build();

            const int count = 200;
            var start = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; i++)
            {
                renderCore.Render(settings, buffer);
            }
            var end = Stopwatch.GetTimestamp();

            double runticks = end - start;
            double runtime = runticks/Stopwatch.Frequency/count;

            lastBenchmarkTimeLabel.Text = runtime.ToString("F3");
            WriteBenchmarkResult(DateTime.Now, runtime);
        }

        private void LoadLastBenchmarkResult()
        {
            var lastLine = new []{"0 0"}
                .Concat(File.ReadLines(BenchmarkLogFileName))
                .Last();
            var lineValues = lastLine.Split(' ');
            var lastBenchmarkTime = double.Parse(lineValues[1], CultureInfo.InvariantCulture);

            lastBenchmarkTimeLabel.Text = lastBenchmarkTime.ToString("F3");
        }

        private void WriteBenchmarkResult(DateTime dateTime, double benchmarkTime)
        {
            var line = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dateTime.Ticks, benchmarkTime);
            using (var benchmarkLog = File.AppendText(BenchmarkLogFileName))
            {
                benchmarkLog.WriteLine(line);
            }
        }
    }
}
