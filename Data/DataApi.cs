using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;


namespace Data
{

    public abstract class DataAbsApi
    {
        public abstract void AddBall(double Boardx, double Boardy, double r, double velX, double velY);
        public abstract void RemoveBall(Iballs ball);

        public abstract ObservableCollection<Iballs> GetBalls();

        public static DataAbsApi CreateApi()
        {
            return new DataApi();
        }
    }



    internal class DataApi : DataAbsApi
    {
        private ObservableCollection<Iballs> _balls = new ObservableCollection<Iballs>();

        private Random _random = new Random();

        public override void AddBall(double Boardx, double Boardy, double r, double velX, double velY)
        {
            double x = _random.NextDouble() * (Boardx - 2 * r);
            double y = _random.NextDouble() * (Boardy - 2 * r);

            if (velX == 0 && velY == 0)
            {
                velX = (_random.NextDouble() - 0.5) * 5;
                velY = (_random.NextDouble() - 0.5) * 5;
            }

            _balls.Add(new Ball(x, y, r, velX, velY));
        }

        public override void RemoveBall(Iballs ball)
        {
            _balls.Remove(ball);
        }

        public override ObservableCollection<Iballs> GetBalls()
        {
            return _balls;
        }
    }
}