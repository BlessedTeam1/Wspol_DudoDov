using System.ComponentModel;
using Data; 

namespace PresentationModel
{
    
    public class BallModel : INotifyPropertyChanged
    {
        private readonly Iballs _ball;

        
        public double Diameter => _ball.R * 2;

        public double X => _ball.X - _ball.R;
        public double Y => _ball.Y - _ball.R;

        public BallModel(Iballs ball)
        {
            _ball = ball;

            _ball.PropertyChanged += (sender, args) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(args.PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}