using System.ComponentModel;
using Data;

namespace PresentationModel
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBalls _ball;
        private readonly double _scaleX;
        private readonly double _scaleY;

        public BallModel(IBalls ball, double logicWidth, double logicHeight,
            double canvasWidth, double canvasHeight)
        {
            _ball = ball;
            _scaleX = canvasWidth / logicWidth;
            _scaleY = canvasHeight / logicHeight;

            _ball.PropertyChanged += (s, e) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        public BallModel(IBalls ball) : this(ball, 1, 1, 1, 1) { }

        public double Diameter => _ball.R * 2 * _scaleX;
        public double X => (_ball.X - _ball.R) * _scaleX;
        public double Y => (_ball.Y - _ball.R) * _scaleY;
        public double Mass => _ball.Mass;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}