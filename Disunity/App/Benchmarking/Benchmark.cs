using System;
using System.Threading.Tasks;

namespace Disunity.App.Benchmarking
{
    public class Benchmark
    {
        private readonly WorldState _state;
        private readonly Func<Task<BenchmarkResult>> _startBenchmark;

        public Benchmark(WorldState state, Func<Task<BenchmarkResult>> startBenchmark)
        {
            _state = state;
            _startBenchmark = startBenchmark;
        }

        public WorldState State
        {
            get { return _state; }
        }

        public Task<BenchmarkResult> Start()
        {
            return _startBenchmark();
        }
    }
}