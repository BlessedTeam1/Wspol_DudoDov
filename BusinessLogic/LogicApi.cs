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

        // Sekcja krytyczna — wyłączny dostęp do listy kul podczas kroku symulacji
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
            StopSimulation();

            var balls = _dataApi.GetBalls();
            while (balls.Count > 0)
                _dataApi.RemoveBall(balls[0]);

            var rnd = new Random();
            for (int i = 0; i < ballCount; i++)
            {
                double r    = 10 + rnd.NextDouble() * 20;  // promień 10..30 px
                double mass = r * r;                         // masa proporcjonalna do pola
                _dataApi.AddBall(boardX, boardY, r, mass);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _simulationTask = Task.Run(() =>
                SimulationLoop(boardX, boardY, _cancellationTokenSource.Token));
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
                    ResolveCollisions(balls);
                    foreach (var ball in balls)
                        ball.Move(boardX, boardY);
                }
                finally
                {
                    _gate.Release();
                }

                try { await Task.Delay(16, token).ConfigureAwait(false); }
                catch (OperationCanceledException) { break; }
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

                    // Wektor jednostkowy A→B
                    double nx = dx / dist;
                    double ny = dy / dist;

                    // Prędkość względna wzdłuż normalnej
                    double dvx  = a.VelX - b.VelX;
                    double dvy  = a.VelY - b.VelY;
                    double vRel = dvx * nx + dvy * ny;

                    // Jeśli kule już się oddalają — pomijamy
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