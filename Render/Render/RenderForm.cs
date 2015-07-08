using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Render.Benchmarking;

namespace Render
{
    public partial class RenderForm : Form
    {
        private const string BenchmarkLogFileName = @"..\..\BenchmarkLog.txt";
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;
        
        private readonly RenderCore _renderCore = new RenderCore(ViewportWidth, ViewportHeight);

        private Bitmap _frontBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);
        private Bitmap _backBuffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);

        private MouseMode _mouseMode;

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

        private WorldState _worldState = CreateInitialState();

        private void Draw(WorldStateChange change)
        {
            _worldState = change.Perform(WorldStateChangeAware.Instance)(_worldState);

            var world = WorldBuilder.BuildWorld(_worldState);
            _renderCore.Render(world, _backBuffer);
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
