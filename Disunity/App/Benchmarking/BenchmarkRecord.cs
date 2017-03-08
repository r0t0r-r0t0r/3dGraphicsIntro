using System;

namespace Disunity.App.Benchmarking
{
    public sealed class BenchmarkRecord
    {
        private readonly DateTime _dateTime;
        private readonly double _frameRenderingDuration;

        public BenchmarkRecord(DateTime dateTime, double frameRenderingDuration)
        {
            _dateTime = dateTime;
            _frameRenderingDuration = frameRenderingDuration;
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
        }

        public double FrameRenderingDuration
        {
            get { return _frameRenderingDuration; }
        }
    }
}