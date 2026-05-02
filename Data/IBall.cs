using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    public interface IBalls : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double R { get; }
        double Mass { get; }
        double VelX { get; set; }
        double VelY { get; set; }
        void Move(double boardX, double boardY);
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
            private set
            {
                bool changed;
                lock (_sync) { changed = _x != value; _x = value; }
                if (changed) OnPropertyChanged();
            }
        }

        public double Y
        {
            get { lock (_sync) return _y; }
            private set
            {
                bool changed;
                lock (_sync) { changed = _y != value; _y = value; }
                if (changed) OnPropertyChanged();
            }
        }

        public double R    => _r;
        public double Mass => _mass;

        public double VelX
        {
            get { lock (_sync) return _velX; }
            set  { lock (_sync) _velX = value; }
        }

        public double VelY
        {
            get { lock (_sync) return _velY; }
            set  { lock (_sync) _velY = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Ball(double x, double y, double r, double mass, double velX, double velY)
        {
            _x = x; _y = y; _r = r; _mass = mass; _velX = velX; _velY = velY;
        }

        public void Move(double boardX, double boardY)
        {
            double nx, ny, vx, vy;

            lock (_sync)
            {
                vx = _velX;
                vy = _velY;

                nx = _x + vx;
                ny = _y + vy;

                if (nx - _r <= 0)
                {
                    nx = _r;
                    vx = -vx;
                }
                else if (nx + _r >= boardX)
                {
                    nx = boardX - _r;
                    vx = -vx;
                }

                if (ny - _r <= 0)
                {
                    ny = _r;
                    vy = -vy;
                }
                else if (ny + _r >= boardY)
                {
                    ny = boardY - _r;
                    vy = -vy;
                }

                _x = nx; _y = ny; _velX = vx; _velY = vy;
            }

            
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}