using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.Filesystem;
using Render.Lib.Parsing;
using static Render.Lib.Parsing.Parsers;
using static Render.Lib.Parsing.PrimitiveParsers;

namespace Render.Benchmarking
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

    public static class BenchmarkRecordsIo
    {
        private static readonly string BenchmarkLogFileName = Path.Combine(FilesystemUtils.LocalHome, "benchmark.log");

        public static IReadOnlyCollection<BenchmarkRecord> Read()
        {
            EnsureFileExists(BenchmarkLogFileName);
            var records = File
                .ReadLines(BenchmarkLogFileName)
                .Select(x =>
                {
                    var parts = x.Split(' ');
                    var dateTimeLong = long.Parse(parts[0], CultureInfo.InvariantCulture);
                    var dateTime = DateTime.FromFileTimeUtc(dateTimeLong).ToLocalTime();

                    var duration = double.Parse(parts[1], CultureInfo.InvariantCulture);

                    return new BenchmarkRecord(dateTime, duration);
                })
                .ToList();

            return records.AsReadOnly();
        }

        public static void Append(BenchmarkRecord record)
        {
            EnsureFileExists(BenchmarkLogFileName);
            using (var benchmarkLog = File.AppendText(BenchmarkLogFileName))
            {
                var line = string.Format(CultureInfo.InvariantCulture, "{0} {1}", record.DateTime.ToFileTimeUtc(),
                    record.FrameRenderingDuration);

                benchmarkLog.WriteLine(line);
            }
        }

        private static void EnsureFileExists(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            if (fi.Exists) return;
            using (fi.Create())
            {
            }
        }
    }

    internal static class BenchmarkFileParsers
    {
        private static Parser<DateTime> DateTimeFromLong(long rawDateTime)
        {
            throw new NotImplementedException();
        }

        public static Parser<ImmutableList<BenchmarkRecord>> BenchmarkFileParser
        {
            get
            {
                var newLine = String(Environment.NewLine);
                var dateTime = Long().SelectMany(DateTimeFromLong);

                var line = dateTime.Product1(String(" ")).Product(Double()).Product1(newLine).Select(x => new BenchmarkRecord(x._1, x._2));

                var lines = Many(line);

                return lines;
            }
        }
    }
}
