using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    public interface Iballs : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double R { get; }
        void Move(double boardX, double boardY);
    }

    internal class Ball : Iballs
    {
        private double _x;
        private double _y;
        private double _velX;
        private double _velY;
        private readonly double _r;

        public double X
        {
            get => _x;
            private set
            {
                if (value == _x) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            private set
            {
                if (value == _y) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        public double R => _r;

        public event PropertyChangedEventHandler PropertyChanged;

        public Ball(double x, double y, double r, double velX, double velY)
        {
            _x = x; _y = y; _r = r; _velX = velX; _velY = velY;
        }

        public void Move(double boardX, double boardY)
        {
            double nextX = X + _velX;
            double nextY = Y + _velY;

            if (nextX - _r <= 0 || nextX + _r >= boardX)
            {
                _velX = -_velX;
                nextX = X + _velX;
            }

            if (nextY - _r <= 0 || nextY + _r >= boardY)
            {
                _velY = -_velY;
                nextY = Y + _velY;
            }

            X = nextX;
            Y = nextY;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}