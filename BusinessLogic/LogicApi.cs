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
        public abstract ObservableCollection<IBalls> GetBalls();

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

        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

        public LogicApi(DataAbsApi dataApi)
        {
            _dataApi = dataApi;
        }

        public override ObservableCollection<IBalls> GetBalls()
        {
            return _dataApi.GetBalls();
        }

        public override void StartSimulation(double boardX, double boardY, int ballCount)
        {
            if (_simulationTask != null) return;

            var balls = _dataApi.GetBalls();
            while (balls.Count > 0)
                _dataApi.RemoveBall(balls[0]);

            var rnd = new Random();
            for (int i = 0; i < ballCount; i++)
            {
                double r    = 10 + rnd.NextDouble() * 20;
                double mass = r * r;
                _dataApi.AddBall(boardX, boardY, r, mass);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            // Запускаем многопоточность для каждого шара
            foreach (var ball in _dataApi.GetBalls())
            {
                ball.Start(token);
            }

            // Таск для проверки коллизий и границ
            _simulationTask = Task.Run(() => SimulationLoop(boardX, boardY, token));
        }

        public override void StopSimulation()
        {
            _cancellationTokenSource?.Cancel();
            try { _simulationTask?.Wait(500); } catch { }
            _cancellationTokenSource = null;
            _simulationTask = null;
        }

        private async Task SimulationLoop(double boardX, double boardY, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await _gate.WaitAsync(token).ConfigureAwait(false);
                try
                {
                    var balls = _dataApi.GetBalls();
                    CheckBoundaries(balls, boardX, boardY);
                    ResolveCollisions(balls);
                }
                finally
                {
                    _gate.Release();
                }

                try { await Task.Delay(16, token).ConfigureAwait(false); }
                catch (OperationCanceledException) { break; }
            }
        }

        // Логика отскока от стен перенесена сюда (выполняем требование чеклиста)
        private static void CheckBoundaries(ObservableCollection<IBalls> balls, double boardX, double boardY)
        {
            foreach (var ball in balls)
            {
                if (ball.X - ball.R <= 0)
                {
                    ball.X = ball.R;
                    ball.VelX = Math.Abs(ball.VelX);
                }
                else if (ball.X + ball.R >= boardX)
                {
                    ball.X = boardX - ball.R;
                    ball.VelX = -Math.Abs(ball.VelX);
                }

                if (ball.Y - ball.R <= 0)
                {
                    ball.Y = ball.R;
                    ball.VelY = Math.Abs(ball.VelY);
                }
                else if (ball.Y + ball.R >= boardY)
                {
                    ball.Y = boardY - ball.R;
                    ball.VelY = -Math.Abs(ball.VelY);
                }
            }
        }

        // Zderzenia sprężyste: https://en.wikipedia.org/wiki/Elastic_collision
        private static void ResolveCollisions(ObservableCollection<IBalls> balls)
        {
            int n = balls.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var a = balls[i];
                    var b = balls[j];

                    double dx = b.X - a.X;
                    double dy = b.Y - a.Y;
                    double dist2 = dx * dx + dy * dy;
                    double minDist = a.R + b.R;

                    if (dist2 >= minDist * minDist || dist2 < 1e-9)
                        continue;

                    double dist = Math.Sqrt(dist2);

                    double nx = dx / dist;
                    double ny = dy / dist;

                    double dvx  = a.VelX - b.VelX;
                    double dvy  = a.VelY - b.VelY;
                    double vRel = dvx * nx + dvy * ny;

                    if (vRel <= 0) continue;

                    double ma = a.Mass, mb = b.Mass;
                    double impulse = 2 * vRel / (ma + mb);

                    a.VelX -= impulse * mb * nx;
                    a.VelY -= impulse * mb * ny;
                    b.VelX += impulse * ma * nx;
                    b.VelY += impulse * ma * ny;
                }
            }
        }
    }
}