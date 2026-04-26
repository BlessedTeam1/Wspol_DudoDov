using System.ComponentModel;
using Data;

namespace PresentationModel
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly Iballs _ball;
        private readonly double _scaleX;
        private readonly double _scaleY;

        // Full constructor: used by ModelApi with real scaling
        public BallModel(Iballs ball, double logicWidth, double logicHeight,
                                      double canvasWidth, double canvasHeight)
        {
            _ball = ball;
            _scaleX = canvasWidth / logicWidth;
            _scaleY = canvasHeight / logicHeight;

            _ball.PropertyChanged += (s, e) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        // Convenience constructor for tests: scale 1:1
        public BallModel(Iballs ball) : this(ball, 1, 1, 1, 1) { }

        public double Diameter => _ball.R * 2 * _scaleX;
        public double X => (_ball.X - _ball.R) * _scaleX;
        public double Y => (_ball.Y - _ball.R) * _scaleY;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
