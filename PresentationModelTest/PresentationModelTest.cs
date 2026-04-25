using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Data;
using BusinessLogic;
using PresentationModel;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PresentationModelTest
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Pomocnicze fake'i
    // ─────────────────────────────────────────────────────────────────────────────

    internal class FakeBall : Iballs
    {
        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set { _x = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X))); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y))); }
        }

        public double R { get; set; } = 15;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Move(double boardX, double boardY) { }
    }

    internal class FakeLogicApi : LogicAbsApi
    {
        private readonly ObservableCollection<Iballs> _balls = new();

        public bool StartCalled { get; private set; }
        public bool StopCalled { get; private set; }
        public double LastBoardX { get; private set; }
        public double LastBoardY { get; private set; }
        public int LastCount { get; private set; }

        public override ObservableCollection<Iballs> GetBalls() => _balls;

        public override void StartSimulation(double boardX, double boardY, int ballCount)
        {
            StartCalled = true;
            LastBoardX = boardX;
            LastBoardY = boardY;
            LastCount = ballCount;
        }

        public override void StopSimulation()
        {
            StopCalled = true;
        }

        // Pomocnicze metody do sterowania kolekcją w testach
        public void AddFakeBall(Iballs ball) => _balls.Add(ball);
        public void RemoveFakeBall(Iballs ball) => _balls.Remove(ball);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Testy BallModel
    // ─────────────────────────────────────────────────────────────────────────────

    [TestClass]
    public class BallModelTests
    {
        [TestMethod]
        public void Diameter_IsDoubleRadius()
        {
            var ball = new FakeBall { R = 20 };
            var model = new BallModel(ball);

            Assert.AreEqual(40, model.Diameter);
        }

        [TestMethod]
        public void X_IsOffsetByRadius()
        {
            var ball = new FakeBall { X = 100, R = 15 };
            var model = new BallModel(ball);

            Assert.AreEqual(100 - 15, model.X);
        }

        [TestMethod]
        public void Y_IsOffsetByRadius()
        {
            var ball = new FakeBall { Y = 200, R = 15 };
            var model = new BallModel(ball);

            Assert.AreEqual(200 - 15, model.Y);
        }

        [TestMethod]
        public void BallModel_ImplementsINotifyPropertyChanged()
        {
            var model = new BallModel(new FakeBall());
            Assert.IsInstanceOfType(model, typeof(INotifyPropertyChanged));
        }

        [TestMethod]
        public void PropertyChanged_FiredWhenBallXChanges()
        {
            var ball = new FakeBall { X = 50, R = 15 };
            var model = new BallModel(ball);

            bool fired = false;
            model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ball.X)) fired = true;
            };

            ball.X = 100;

            Assert.IsTrue(fired, "PropertyChanged nie zostało wywołane po zmianie X");
        }

        [TestMethod]
        public void PropertyChanged_FiredWhenBallYChanges()
        {
            var ball = new FakeBall { Y = 50, R = 15 };
            var model = new BallModel(ball);

            bool fired = false;
            model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ball.Y)) fired = true;
            };

            ball.Y = 150;

            Assert.IsTrue(fired, "PropertyChanged nie zostało wywołane po zmianie Y");
        }

        [TestMethod]
        public void PropertyChanged_NotFiredWhenNoChange()
        {
            var ball = new FakeBall { X = 50 };
            var model = new BallModel(ball);

            int count = 0;
            model.PropertyChanged += (s, e) => count++;

            // Brak zmiany – nie ustawiamy żadnej właściwości ball
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void X_UpdatesAfterBallMoves()
        {
            var ball = new FakeBall { X = 50, R = 10 };
            var model = new BallModel(ball);

            ball.X = 80;

            Assert.AreEqual(80 - 10, model.X);
        }

        [TestMethod]
        public void Y_UpdatesAfterBallMoves()
        {
            var ball = new FakeBall { Y = 50, R = 10 };
            var model = new BallModel(ball);

            ball.Y = 120;

            Assert.AreEqual(120 - 10, model.Y);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Testy ModelApi
    // ─────────────────────────────────────────────────────────────────────────────

    [TestClass]
    public class ModelApiTests
    {
        // ── konstruktor ───────────────────────────────────────────────────────────

        [TestMethod]
        public void Constructor_WithoutArgument_CreatesInstance()
        {
            var model = new ModelApi();
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Constructor_WithFakeLogic_CreatesInstance()
        {
            var model = new ModelApi(new FakeLogicApi());
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Balls_Initially_IsEmpty()
        {
            var model = new ModelApi(new FakeLogicApi());
            Assert.AreEqual(0, model.Balls.Count);
        }

        // ── Start / Stop ──────────────────────────────────────────────────────────

        [TestMethod]
        public void Start_CallsLogicStartSimulation()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            model.Start(800, 600, 5);

            Assert.IsTrue(fakeLogic.StartCalled);
        }

        [TestMethod]
        public void Start_PassesCorrectParameters()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            model.Start(1024, 768, 10);

            Assert.AreEqual(1024, fakeLogic.LastBoardX);
            Assert.AreEqual(768, fakeLogic.LastBoardY);
            Assert.AreEqual(10, fakeLogic.LastCount);
        }

        [TestMethod]
        public void Stop_CallsLogicStopSimulation()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            model.Stop();

            Assert.IsTrue(fakeLogic.StopCalled);
        }

        // ── synchronizacja kolekcji ───────────────────────────────────────────────

        [TestMethod]
        public void Balls_AddsModelWhenLogicBallAdded()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            fakeLogic.AddFakeBall(new FakeBall());

            Assert.AreEqual(1, model.Balls.Count);
        }

        [TestMethod]
        public void Balls_AddedModelHasCorrectDiameter()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            fakeLogic.AddFakeBall(new FakeBall { R = 20 });

            Assert.AreEqual(40, model.Balls[0].Diameter);
        }

        [TestMethod]
        public void Balls_MultipleAdds_AllModelsPresent()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            fakeLogic.AddFakeBall(new FakeBall());
            fakeLogic.AddFakeBall(new FakeBall());
            fakeLogic.AddFakeBall(new FakeBall());

            Assert.AreEqual(3, model.Balls.Count);
        }

        [TestMethod]
        public void Balls_ClearedWhenLogicBallRemoved()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            var ball = new FakeBall();
            fakeLogic.AddFakeBall(ball);
            Assert.AreEqual(1, model.Balls.Count);

            fakeLogic.RemoveFakeBall(ball);

            // Zgodnie z implementacją: Remove wywołuje Balls.Clear()
            Assert.AreEqual(0, model.Balls.Count);
        }

        [TestMethod]
        public void Balls_IsObservableCollection()
        {
            var model = new ModelApi(new FakeLogicApi());
            Assert.IsInstanceOfType(model.Balls, typeof(ObservableCollection<BallModel>));
        }

        [TestMethod]
        public void Balls_RaisesCollectionChangedOnAdd()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            bool raised = false;
            model.Balls.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add) raised = true;
            };

            fakeLogic.AddFakeBall(new FakeBall());

            Assert.IsTrue(raised, "CollectionChanged nie zostało wywołane po dodaniu kulki");
        }

        [TestMethod]
        public void Balls_RaisesCollectionChangedOnClear()
        {
            var fakeLogic = new FakeLogicApi();
            var model = new ModelApi(fakeLogic);

            var ball = new FakeBall();
            fakeLogic.AddFakeBall(ball);

            bool raised = false;
            model.Balls.CollectionChanged += (s, e) => raised = true;

            fakeLogic.RemoveFakeBall(ball);

            Assert.IsTrue(raised, "CollectionChanged nie zostało wywołane po usunięciu kulki");
        }
    }
}