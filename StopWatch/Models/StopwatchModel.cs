using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StopWatch.Models
{
    public class StopwatchModel
    {
        public TimeSpan Elapsed => _sw.Elapsed;
        public bool IsRunning => _sw.IsRunning;
        public bool IsStopped => !_sw.IsRunning && _sw.ElapsedTicks == 0;
        public bool IsPaused => !(IsRunning || IsStopped);
        public readonly ReadOnlyObservableCollection<TimeSpan> Laps;

        private readonly Stopwatch _sw = new Stopwatch();
        private readonly ObservableCollection<TimeSpan> _laps = new ObservableCollection<TimeSpan>();

        public StopwatchModel()
        {
            Laps = new ReadOnlyObservableCollection<TimeSpan>(_laps);
        }

        public void Start() => _sw.Start();

        public void Stop() => _sw.Stop();

        public void Lap() => _laps.Add(_sw.Elapsed);

        public void Reset()
        {
            _sw.Reset();
            ResetLaps();
        }

        private void ResetLaps() => _laps.Clear();
    }
}
