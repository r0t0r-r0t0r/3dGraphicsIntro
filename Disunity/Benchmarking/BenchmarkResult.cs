namespace Disunity.Benchmarking
{
    public class BenchmarkResult
    {
        private readonly double _frameRenderingDuration;

        public BenchmarkResult(double frameRenderingDuration)
        {
            _frameRenderingDuration = frameRenderingDuration;
        }

        public double FrameRenderingDuration
        {
            get { return _frameRenderingDuration; }
        }
    }
}