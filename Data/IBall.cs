using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public interface IBalls : INotifyPropertyChanged
    {
        double X { get; set; }
        double Y { get; set; }
        double R { get; }
        double Mass { get; }
        double VelX { get; set; }
        double VelY { get; set; }
        void Start(CancellationToken token, Action<IBalls> loggerAction = null);
    }

    internal class Ball : IBalls
    {
        private readonly object _sync = new object();

        private double _x;
        private double _y;
        private double _velX;
        private double _velY;
        private readonly double _r;
        private readonly double _mass;

        public double X
        {
            get { lock (_sync) return _x; }
            set { bool changed; lock (_sync) { changed = _x != value; _x = value; } if (changed) OnPropertyChanged(); }
        }

        public double Y
        {
            get { lock (_sync) return _y; }
            set { bool changed; lock (_sync) { changed = _y != value; _y = value; } if (changed) OnPropertyChanged(); }
        }

        public double R => _r;
        public double Mass => _mass;

        public double VelX
        {
            get { lock (_sync) return _velX; }
            set { lock (_sync) _velX = value; }
        }

        public double VelY
        {
            get { lock (_sync) return _velY; }
            set { lock (_sync) _velY = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Ball(double x, double y, double r, double mass, double velX, double velY)
        {
            _x = x; _y = y; _r = r; _mass = mass; _velX = velX; _velY = velY;
        }

        public async void Start(CancellationToken token, Action<IBalls> loggerAction = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var logTimer = new Stopwatch();
            logTimer.Start();

            while (!token.IsCancellationRequested)
            {
                double deltaSeconds = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                lock (_sync)
                {
                    _x += _velX * deltaSeconds * 60;
                    _y += _velY * deltaSeconds * 60;
                }

                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));

                if (loggerAction != null && logTimer.ElapsedMilliseconds > 100)
                {
                    loggerAction(this);
                    logTimer.Restart();
                }

                try
                {
                    await Task.Delay(16, token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}