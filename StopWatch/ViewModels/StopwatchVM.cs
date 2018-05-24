using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using StopWatch.Models;

namespace StopWatch.ViewModels
{
    public class StopwatchVM : BindableBase
    {
        public TimeSpan Elapsed => _model.Elapsed;
        public ReadOnlyObservableCollection<TimeSpan> Laps => _model.Laps;

        public DelegateCommand StartCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand ResetCommand { get; }
        public DelegateCommand LapCommand { get; }

        private readonly StopwatchModel _model = new StopwatchModel();
        private readonly DispatcherTimer _timer;

        public StopwatchVM()
        {
            StartCommand = new DelegateCommand(StopwatchStart, () => !_model.IsRunning);
            StopCommand = new DelegateCommand(StopwatchStop, () => _model.IsRunning);
            ResetCommand = new DelegateCommand(StopwatchReset, () => _model.IsPaused);
            LapCommand = new DelegateCommand(StopwatchLap, () => _model.IsRunning);

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(10),
            };
            _timer.Tick += (sender, args) => RaisePropertyChanged(nameof(Elapsed));
        }

        private void StopwatchStart()
        {
            _model.Start();
            _timer.Start();
            StopwatchStateChanged();
        }

        private void StopwatchStop()
        {
            _model.Stop();
            _timer.Stop();
            StopwatchStateChanged();
        }

        private void StopwatchReset()
        {
            _model.Reset();
            RaisePropertyChanged(nameof(Elapsed));
            StopwatchStateChanged();
        }

        private void StopwatchLap()
        {
            _model.Lap();
        }

        private void StopwatchStateChanged()
        {
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            ResetCommand.RaiseCanExecuteChanged();
            LapCommand.RaiseCanExecuteChanged();
        }
    }
}
