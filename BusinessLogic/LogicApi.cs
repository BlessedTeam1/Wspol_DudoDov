using Data;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public abstract class LogicAbsApi
    {
        public abstract void StartSimulation(double boardX, double boardY, int ballCount);
        public abstract void StopSimulation();
        public abstract ObservableCollection<Iballs> GetBalls();

        public static LogicAbsApi CreateApi(DataAbsApi dataApi = null)
        {
            return new LogicApi(dataApi ?? DataAbsApi.CreateApi());
        }
    }

    internal class LogicApi : LogicAbsApi
    {
        private readonly DataAbsApi _dataApi;

        private Task _simulationTask;
        private CancellationTokenSource _cancellationTokenSource;

        public LogicApi(DataAbsApi dataApi)
        {
            _dataApi = dataApi;
        }

        
        public override ObservableCollection<Iballs> GetBalls()
        {
            return _dataApi.GetBalls();
        }

        public override void StartSimulation(double boardX, double boardY, int ballCount)
        {
            
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                return;

            var balls = _dataApi.GetBalls();
            while (balls.Count > 0)
            {
                _dataApi.RemoveBall(balls[0]);
            }

            Random rnd = new Random();
            for (int i = 0; i < ballCount; i++)
            {
                
                double r = 15;
                double velX = (rnd.NextDouble() - 0.5) * 10;
                double velY = (rnd.NextDouble() - 0.5) * 10;

                _dataApi.AddBall(boardX, boardY, r, velX, velY);
            }

            _cancellationTokenSource = new CancellationTokenSource();

            
            _simulationTask = Task.Run(() => SimulationLoop(boardX, boardY, _cancellationTokenSource.Token));
        }

        public override void StopSimulation()
        {
            
            _cancellationTokenSource?.Cancel();
        }

        private async Task SimulationLoop(double boardX, double boardY, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var balls = _dataApi.GetBalls();

                foreach (var ball in balls)
                {
                    ball.Move(boardX, boardY);
                }

                try
                {
                    await Task.Delay(16, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}