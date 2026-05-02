using System;
using System.Collections.ObjectModel;

namespace Data
{
    public abstract class DataAbsApi
    {
        public abstract IBalls AddBall(double boardX, double boardY, double r, double mass, double velX = 0, double velY = 0);
        public abstract void RemoveBall(IBalls ball);
        public abstract ObservableCollection<IBalls> GetBalls();

        public static DataAbsApi CreateApi() => new DataApi();
    }

    internal class DataApi : DataAbsApi
    {
        private readonly ObservableCollection<IBalls> _balls = new ObservableCollection<IBalls>();
        private readonly Random _random = new Random();

        public override IBalls AddBall(double boardX, double boardY, double r, double mass, double velX = 0, double velY = 0)
        {
            double x = r + _random.NextDouble() * (boardX - 2 * r);
            double y = r + _random.NextDouble() * (boardY - 2 * r);

            if (velX == 0 && velY == 0)
            {
                double speed = 1.5 + _random.NextDouble() * 3.5;
                double angle = _random.NextDouble() * 2 * Math.PI;
                velX = speed * Math.Cos(angle);
                velY = speed * Math.Sin(angle);
            }

            var ball = new Ball(x, y, r, mass, velX, velY);
            _balls.Add(ball);
            return ball;
        }

        public override void RemoveBall(IBalls ball) => _balls.Remove(ball);

        public override ObservableCollection<IBalls> GetBalls() => _balls;
    }
}