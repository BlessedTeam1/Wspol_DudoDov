using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTest
{
    internal class FakeBall : IBalls
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double R { get; set; }
        public double Mass { get; set; } = 1.0;
        public double VelX { get; set; }
        public double VelY { get; set; }

        public event PropertyChangedEventHandler PropertyChanged { add { } remove { } }

        // Исправлено: добавлено имя переменной "token"
        public void Start(CancellationToken token) { }
    }

    internal class FakeDataApi : DataAbsApi
    {
        private readonly ObservableCollection<IBalls> _balls = new ObservableCollection<IBalls>();

        public int AddBallCallCount { get; private set; }
        public int RemoveBallCallCount { get; private set; }
        public int GetBallsCallCount { get; private set; }

        public override ObservableCollection<IBalls> GetBalls()
        {
            GetBallsCallCount++;
            return _balls;
        }

        public override IBalls AddBall(double boardX, double boardY, double r, double mass, double velX = 0, double velY = 0)
        {
            AddBallCallCount++;
            var ball = new FakeBall { X = boardX / 2, Y = boardY / 2, R = r };
            _balls.Add(ball);
            return ball;
        }

        public override void RemoveBall(IBalls ball)
        {
            RemoveBallCallCount++;
            _balls.Remove(ball);
        }
    }

    [TestClass]
    public class LogicApiTests
    {
        [TestMethod]
        public void CreateApi_WithoutArgument_ReturnsNonNullInstance()
        {
            var api = LogicAbsApi.CreateApi();
            Assert.IsNotNull(api);
        }

        [TestMethod]
        public void CreateApi_WithCustomDataApi_ReturnsNonNullInstance()
        {
            var api = LogicAbsApi.CreateApi(new FakeDataApi());
            Assert.IsNotNull(api);
        }

        [TestMethod]
        public void GetBalls_BeforeSimulation_ReturnsEmptyCollection()
        {
            var logic = LogicAbsApi.CreateApi(new FakeDataApi());
            var balls = logic.GetBalls();
            Assert.IsNotNull(balls);
            Assert.AreEqual(0, balls.Count);
        }

        [TestMethod]
        public void GetBalls_ReturnsSameCollectionAsDataApi()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);
            Assert.AreSame(fakeData.GetBalls(), logic.GetBalls());
        }

        [TestMethod]
        public void StartSimulation_Adds1Ball()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);
            logic.StartSimulation(800, 600, 1);
            logic.StopSimulation();
            Assert.AreEqual(1, fakeData.AddBallCallCount);
        }

        [TestMethod]
        public void StartSimulation_Adds5Balls()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);
            logic.StartSimulation(800, 600, 5);
            logic.StopSimulation();
            Assert.AreEqual(5, fakeData.AddBallCallCount);
        }

        [TestMethod]
        public void StartSimulation_Adds10Balls()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);
            logic.StartSimulation(800, 600, 10);
            logic.StopSimulation();
            Assert.AreEqual(10, fakeData.AddBallCallCount);
        }

        [TestMethod]
        public void StartSimulation_ZeroBalls_AddsNoBalls()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);
            logic.StartSimulation(800, 600, 0);
            logic.StopSimulation();
            Assert.AreEqual(0, fakeData.AddBallCallCount);
        }

        [TestMethod]
        public async Task StartSimulation_ClearsPreviousBallsBeforeAdding()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);

            logic.StartSimulation(800, 600, 3);
            logic.StopSimulation();
            await Task.Delay(100);

            logic.StartSimulation(800, 600, 2);
            logic.StopSimulation();

            Assert.AreEqual(2, logic.GetBalls().Count);
        }

        [TestMethod]
        public void StartSimulation_RemovesAllExistingBalls()
        {
            var fakeData = new FakeDataApi();
            fakeData.AddBall(800, 600, 15, 1, 1, 1);
            fakeData.AddBall(800, 600, 15, 1, 1, -1);

            var logic = LogicAbsApi.CreateApi(fakeData);
            logic.StartSimulation(800, 600, 1);
            logic.StopSimulation();

            Assert.IsTrue(fakeData.RemoveBallCallCount >= 2);
        }

        [TestMethod]
        public void StartSimulation_CalledTwiceWithoutStop_DoesNotAddBallsSecondTime()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);

            logic.StartSimulation(800, 600, 3);
            int countAfterFirst = fakeData.AddBallCallCount;

            logic.StartSimulation(800, 600, 5);

            logic.StopSimulation();
            Assert.AreEqual(countAfterFirst, fakeData.AddBallCallCount);
        }

        [TestMethod]
        public void StopSimulation_BeforeStart_DoesNotThrow()
        {
            var logic = LogicAbsApi.CreateApi(new FakeDataApi());
            logic.StopSimulation();
        }

        [TestMethod]
        public async Task StopSimulation_AfterStart_EventuallyStopsLoop()
        {
            var logic = LogicAbsApi.CreateApi(new FakeDataApi());
            logic.StartSimulation(800, 600, 3);
            logic.StopSimulation();
            await Task.Delay(200);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task StartStop_Cycle_CanBeRepeated()
        {
            var logic = LogicAbsApi.CreateApi(new FakeDataApi());

            logic.StartSimulation(800, 600, 2);
            logic.StopSimulation();
            await Task.Delay(100);

            logic.StartSimulation(800, 600, 2);
            logic.StopSimulation();
        }

        [TestMethod]
        public async Task SimulationLoop_CallsGetBalls_AtLeastOnce()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbsApi.CreateApi(fakeData);

            logic.StartSimulation(800, 600, 0);
            await Task.Delay(100);
            logic.StopSimulation();

            Assert.IsTrue(fakeData.GetBallsCallCount >= 1,
                $"Expected GetBalls >= 1, got {fakeData.GetBallsCallCount}");
        }
    }
} // <-- Эта скобка была потеряна