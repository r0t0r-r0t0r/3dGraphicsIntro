using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Disunity.App.Benchmarking;
using Disunity.App.Filesystem;
using Disunity.Data;
using Disunity.Data.Common;
using Disunity.Rendering;
using Disunity.WorldManaging.Loading;
using Disunity.WorldManaging.StateChanging;

namespace Disunity.App
{
    public partial class RenderForm : Form
    {
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;

        private const string ModelPath = @"Model";

        private static readonly string BenchmarkLogFileName = Path.Combine(FilesystemUtils.LocalHome, "benchmark.log");
        
        private readonly WorldBuilder _worldBuilder = new WorldBuilder(ModelLoader.LoadModel(
            ModelPath,
            "african_head.obj",
            "african_head_diffuse.bmp",
            "african_head_nm.png",
            "african_head_spec.bmp"));
        private readonly Renderer _renderer = new Renderer(ViewportWidth, ViewportHeight);
        private readonly BenchmarkRecordsIo _benchmarkRecordsIo = new BenchmarkRecordsIo(BenchmarkLogFileName);

        private Bitmap _frontBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);
        private Bitmap _backBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);

        private MouseMode _mouseMode;

        private IReadOnlyCollection<BenchmarkRecord> _benchmarkRecords = new List<BenchmarkRecord>();

        private WorldState _worldState = CreateInitialState();

        public RenderForm()
        {
            InitializeComponent();
            SetupWorldStateControls();
        }

        private static WorldState CreateInitialState()
        {
            return new WorldState(renderMode: RenderMode.Fill, lightMode: LightMode.NormalMapping,
                fillMode: FillMode.Texture, perspectiveProjection: true, viewportScale: 0.9f,
                viewportWidth: ViewportWidth, viewportHeight: ViewportHeight, viewportLightX: ViewportWidth/2,
                viewportLightY: ViewportHeight/2, modelRotationX: 0, modelRotationY: 0);
        }

        private class ControlsWorldStateChangeAware: IWorldStateChangeAware<Unit>
        {
            private readonly RenderForm _form;

            public ControlsWorldStateChangeAware(RenderForm form)
            {
                _form = form;
            }

            public Unit Empty()
            {
                return Unit.Value;
            }

            public Unit ChangeRenderMode(RenderMode mode)
            {
                switch (mode)
                {
                    case RenderMode.Borders:
                        _form.bordersRadioButton.Checked = true;
                        break;
                    case RenderMode.Fill:
                        _form.fillRadioButton.Checked = true;
                        break;
                    case RenderMode.BordersAndFill:
                        _form.bordersAndFillRadioButton.Checked = true;
                        break;
                }

                return Unit.Value;
            }

            public Unit ChangeLightMode(LightMode mode)
            {
                switch (mode)
                {
                    case LightMode.None:
                        _form.flatRadioButton.Checked = true;
                        break;
                    case LightMode.Simple:
                        _form.simpleRadioButton.Checked = true;
                        break;
                    case LightMode.Gouraud:
                        _form.gouraudRadioButton.Checked = true;
                        break;
                    case LightMode.Phong:
                        _form.phongRadioButton.Checked = true;
                        break;
                    case LightMode.NormalMapping:
                        _form.normalMappingRadioButton.Checked = true;
                        break;
                }

                return Unit.Value;
            }

            public Unit ChangeFillMode(FillMode mode)
            {
                switch (mode)
                {
                    case FillMode.Texture:
                        _form.textureRadioButton.Checked = true;
                        break;
                    case FillMode.SolidColor:
                        _form.solidColorRadioButton.Checked = true;
                        break;
                }

                return Unit.Value;
            }

            public Unit ChangePerspectiveProjection(bool projection)
            {
                _form.perspectiveProjectionCheckBox.Checked = projection;

                return Unit.Value;
            }

            public Unit ChangeViewportScale(float scale)
            {
                _form.viewportScaleNumericUpDown.Value = (decimal) scale;

                return Unit.Value;
            }

            public Unit ChangeViewportSize(int width, int height)
            {
                return Unit.Value;
            }

            public Unit ChangeLightPosition(int x, int y)
            {
                return Unit.Value;
            }

            public Unit ChangeModelRotation(float x, float y)
            {
                return Unit.Value;
            }
        }

        private void SetupWorldStateControls()
        {
            var changes = _worldState.AsChanges();
            var aware = new ControlsWorldStateChangeAware(this);
            foreach (var change in changes)
            {
                change.Perform(aware);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw(WorldStateChange.Empty);
            LoadLastBenchmarkResult();
        }

        private void CameraZPositionHandler(object sender, EventArgs e)
        {
            var viewportScale = (float) viewportScaleNumericUpDown.Value;
            Draw(WorldStateChange.ChangeViewportScale(viewportScale));
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            var perspectiveProjection = perspectiveProjectionCheckBox.Checked;
            Draw(WorldStateChange.ChangePerspectiveProjection(perspectiveProjection));
        }

        private void bordersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeRenderMode(RenderMode.Borders));
        }

        private void fillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeRenderMode(RenderMode.Fill));
        }

        private void bordersAndFillRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeRenderMode(RenderMode.BordersAndFill));
        }

        private void flatRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeLightMode(LightMode.None));
        }

        private void simpleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeLightMode(LightMode.Simple));
        }

        private void gouraudRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeLightMode(LightMode.Gouraud));
        }

        private void phongRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeLightMode(LightMode.Phong));
        }

        private void normalMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeLightMode(LightMode.NormalMapping));
        }

        private void solidColorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeFillMode(FillMode.SolidColor));
        }

        private void textureRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Draw(WorldStateChange.ChangeFillMode(FillMode.Texture));
        }

        private async void startBenchmarkButton_Click(object sender, EventArgs e)
        {
            var benchmark = BenchmarkFactory.Create();

            _worldState = benchmark.State;
            SetupWorldStateControls();
            Draw(WorldStateChange.Empty);

            var dateTime = DateTime.Now;
            lastBenchmarkTimeLabel.Text = "Benchmarking...";
            lastBenchmarkRunLabel.Text = dateTime.ToString(Thread.CurrentThread.CurrentCulture);

            mainPanel.Enabled = false;

            try
            {
                var result = await benchmark.Start().ConfigureAwait(false);

                var runtime = result.FrameRenderingDuration;
                WriteBenchmarkResult(dateTime, runtime);
            }
            finally
            {
                mainPanel.Enabled = true;
            }
        }

        private void LoadLastBenchmarkResult()
        {
            _benchmarkRecords = _benchmarkRecordsIo.Read();

            var lastBenchmark =
                _benchmarkRecords.LastOption()
                    .Select(
                        x =>
                            new
                            {
                                RunAt = x.DateTime.ToString(Thread.CurrentThread.CurrentCulture),
                                Result = x.FrameRenderingDuration.ToString("F4")
                            })
                    .OrElse(new { RunAt = "Never", Result = "Unknown" });

            lastBenchmarkTimeLabel.Text = lastBenchmark.Result;
            lastBenchmarkRunLabel.Text = lastBenchmark.RunAt;
        }

        private void WriteBenchmarkResult(DateTime dateTime, double benchmarkTime)
        {
            lastBenchmarkTimeLabel.Text = benchmarkTime.ToString("F4");
            lastBenchmarkRunLabel.Text = dateTime.ToString(Thread.CurrentThread.CurrentCulture);

            _benchmarkRecordsIo.Append(new BenchmarkRecord(dateTime, benchmarkTime));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            switch (_mouseMode)
            {
                case MouseMode.RotateLight:
                    Draw(WorldStateChange.ChangeLightPosition(e.X, e.Y));
                    break;
                case MouseMode.RotateModel:
                    var x = ((float)ViewportWidth/2 - e.X)/((float)ViewportWidth/2)*(float)Math.PI/2;
                    var y = ((float)ViewportHeight/2 - e.Y)/((float)ViewportHeight/2)*(float)Math.PI/2;
                    
                    Draw(WorldStateChange.ChangeModelRotation(x, y));
                    break;
            }
        }

        private void rotateLightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _mouseMode = MouseMode.RotateLight;
        }

        private void rotateModelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _mouseMode = MouseMode.RotateModel;
        }

        private void Draw(WorldStateChange change)
        {
            _worldState = change.Perform(WorldStateChangeAware.Instance)(_worldState);

            var world = _worldBuilder.BuildWorld(_worldState);
            _renderer.Render(world, _backBuffer);
            pictureBox1.Image = _backBuffer;

            var exchange = _backBuffer;
            _backBuffer = _frontBuffer;
            _frontBuffer = exchange;
        }
    }

    internal enum MouseMode
    {
        RotateLight,
        RotateModel
    }
}
