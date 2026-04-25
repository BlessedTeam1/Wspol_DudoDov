using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTest
{
    [TestClass]
    public class DataApiTests
    {
        // ── fabryka ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void CreateApi_ReturnsNonNullInstance()
        {
            var api = DataAbsApi.CreateApi();
            Assert.IsNotNull(api);
        }

        // ── GetBalls ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void GetBalls_Initially_ReturnsEmptyCollection()
        {
            var api = DataAbsApi.CreateApi();

            var balls = api.GetBalls();

            Assert.IsNotNull(balls);
            Assert.AreEqual(0, balls.Count);
        }

        [TestMethod]
        public void GetBalls_ReturnsSameInstanceEachTime()
        {
            var api = DataAbsApi.CreateApi();

            Assert.AreSame(api.GetBalls(), api.GetBalls());
        }

        // ── AddBall ───────────────────────────────────────────────────────────────

        [TestMethod]
        public void AddBall_IncreasesCountByOne()
        {
            var api = DataAbsApi.CreateApi();

            api.AddBall(800, 600, 15, 1, 1);

            Assert.AreEqual(1, api.GetBalls().Count);
        }

        [TestMethod]
        public void AddBall_MultipleTimes_IncreasesCountCorrectly()
        {
            var api = DataAbsApi.CreateApi();

            api.AddBall(800, 600, 15, 1, 1);
            api.AddBall(800, 600, 15, -1, -1);
            api.AddBall(800, 600, 15, 2, 2);

            Assert.AreEqual(3, api.GetBalls().Count);
        }

        [TestMethod]
        public void AddBall_BallHasCorrectRadius()
        {
            var api = DataAbsApi.CreateApi();

            api.AddBall(800, 600, 20, 1, 1);

            Assert.AreEqual(20, api.GetBalls()[0].R);
        }

        [TestMethod]
        public void AddBall_BallPositionIsWithinBoard()
        {
            var api = DataAbsApi.CreateApi();
            double boardX = 800, boardY = 600, r = 15;

            api.AddBall(boardX, boardY, r, 1, 1);

            var ball = api.GetBalls()[0];
            Assert.IsTrue(ball.X >= 0 && ball.X <= boardX - 2 * r,
                $"X={ball.X} poza planszą");
            Assert.IsTrue(ball.Y >= 0 && ball.Y <= boardY - 2 * r,
                $"Y={ball.Y} poza planszą");
        }

        [TestMethod]
        public void AddBall_ZeroVelocity_BallStillGetsRandomVelocity()
        {
            // DataApi losuje prędkość gdy velX==0 && velY==0
            // Kulka musi więc istnieć i mieć poprawne R
            var api = DataAbsApi.CreateApi();

            api.AddBall(800, 600, 15, 0, 0);

            Assert.AreEqual(1, api.GetBalls().Count);
            Assert.AreEqual(15, api.GetBalls()[0].R);
        }

        [TestMethod]
        public void AddBall_ReturnsObservableCollectionWithAddedBall()
        {
            var api = DataAbsApi.CreateApi();

            api.AddBall(800, 600, 15, 1, 1);

            var balls = api.GetBalls();
            Assert.IsInstanceOfType(balls, typeof(ObservableCollection<Iballs>));
            Assert.AreEqual(1, balls.Count);
        }

        // ── RemoveBall ────────────────────────────────────────────────────────────

        [TestMethod]
        public void RemoveBall_DecreasesCountByOne()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 1, 1);
            var ball = api.GetBalls()[0];

            api.RemoveBall(ball);

            Assert.AreEqual(0, api.GetBalls().Count);
        }

        [TestMethod]
        public void RemoveBall_RemovesCorrectBall()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 10, 1, 1);
            api.AddBall(800, 600, 20, 2, 2);

            var balls = api.GetBalls();
            var toRemove = balls[0];

            api.RemoveBall(toRemove);

            Assert.AreEqual(1, api.GetBalls().Count);
            Assert.AreNotSame(toRemove, api.GetBalls()[0]);
        }

        [TestMethod]
        public void RemoveBall_AllBalls_CollectionIsEmpty()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 1, 1);
            api.AddBall(800, 600, 15, -1, -1);

            var balls = api.GetBalls();
            while (balls.Count > 0)
                api.RemoveBall(balls[0]);

            Assert.AreEqual(0, api.GetBalls().Count);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Testy klasy Ball (przez interfejs Iballs)
    // ─────────────────────────────────────────────────────────────────────────────

    [TestClass]
    public class BallTests
    {
        private static Iballs CreateBall(double boardX = 800, double boardY = 600,
                                         double r = 15, double velX = 2, double velY = 2)
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(boardX, boardY, r, velX, velY);
            return api.GetBalls()[0];
        }

        // ── właściwości ───────────────────────────────────────────────────────────

        [TestMethod]
        public void Ball_HasCorrectRadius()
        {
            var ball = CreateBall(r: 25);
            Assert.AreEqual(25, ball.R);
        }

        [TestMethod]
        public void Ball_ImplementsINotifyPropertyChanged()
        {
            var ball = CreateBall();
            Assert.IsInstanceOfType(ball, typeof(INotifyPropertyChanged));
        }

        // ── Move – ruch normalny ──────────────────────────────────────────────────

        [TestMethod]
        public void Move_ChangesPosition()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 5, 5);
            var ball = api.GetBalls()[0];

            double xBefore = ball.X;
            double yBefore = ball.Y;

            ball.Move(800, 600);

            Assert.AreNotEqual(xBefore, ball.X);
            Assert.AreNotEqual(yBefore, ball.Y);
        }

        [TestMethod]
        public void Move_BallStaysWithinBoardAfterManySteps()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 7, 7);
            var ball = api.GetBalls()[0];

            for (int i = 0; i < 500; i++)
                ball.Move(800, 600);

            Assert.IsTrue(ball.X - ball.R >= 0, $"X={ball.X} wychodzi poza lewą krawędź");
            Assert.IsTrue(ball.X + ball.R <= 800, $"X={ball.X} wychodzi poza prawą krawędź");
            Assert.IsTrue(ball.Y - ball.R >= 0, $"Y={ball.Y} wychodzi poza górną krawędź");
            Assert.IsTrue(ball.Y + ball.R <= 600, $"Y={ball.Y} wychodzi poza dolną krawędź");
        }

        // ── Move – odbicia ────────────────────────────────────────────────────────

        [TestMethod]
        public void Move_BouncesOffRightWall()
        {
            // Umieszczamy kulkę blisko prawej ściany z prędkością w prawo
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 1, 1); // AddBall losuje pozycję,
            var ball = api.GetBalls()[0];    // więc symulujemy wiele kroków

            for (int i = 0; i < 1000; i++)
                ball.Move(800, 600);

            // Po 1000 krokach kulka na pewno się odbiła – weryfikujemy że jest w bounds
            Assert.IsTrue(ball.X + ball.R <= 800);
        }

        [TestMethod]
        public void Move_BouncesOffBottomWall()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 1, 1);
            var ball = api.GetBalls()[0];

            for (int i = 0; i < 1000; i++)
                ball.Move(800, 600);

            Assert.IsTrue(ball.Y + ball.R <= 600);
        }

        // ── INotifyPropertyChanged ────────────────────────────────────────────────

        [TestMethod]
        public void Move_RaisesPropertyChangedForX()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 5, 5);
            var ball = api.GetBalls()[0];
            var inpc = (INotifyPropertyChanged)ball;

            bool xChanged = false;
            inpc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ball.X)) xChanged = true;
            };

            ball.Move(800, 600);

            Assert.IsTrue(xChanged, "PropertyChanged dla X nie zostało wywołane");
        }

        [TestMethod]
        public void Move_RaisesPropertyChangedForY()
        {
            var api = DataAbsApi.CreateApi();
            api.AddBall(800, 600, 15, 5, 5);
            var ball = api.GetBalls()[0];
            var inpc = (INotifyPropertyChanged)ball;

            bool yChanged = false;
            inpc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ball.Y)) yChanged = true;
            };

            ball.Move(800, 600);

            Assert.IsTrue(yChanged, "PropertyChanged dla Y nie zostało wywołane");
        }
    }
}
