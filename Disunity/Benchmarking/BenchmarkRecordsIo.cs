using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Disunity.Benchmarking
{
    public class BenchmarkRecordsIo
    {
        private readonly string _logFileName;

        public BenchmarkRecordsIo(string logFileName)
        {
            if (logFileName == null) throw new ArgumentNullException(nameof(logFileName));

            _logFileName = logFileName;
        }

        public IReadOnlyCollection<BenchmarkRecord> Read()
        {
            EnsureFileExists(_logFileName);
            var records = File
                .ReadLines(_logFileName)
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

        public void Append(BenchmarkRecord record)
        {
            EnsureFileExists(_logFileName);
            using (var benchmarkLog = File.AppendText(_logFileName))
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
}
