using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using PresentationModel;
using PresentationViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PresentationViewModelTest
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Testy MainViewModel
    // ─────────────────────────────────────────────────────────────────────────────

    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _vm = new MainViewModel();
        }

        // ── właściwości domyślne ──────────────────────────────────────────────────

        [TestMethod]
        public void BallCount_DefaultIs5()
        {
            Assert.AreEqual(5, _vm.BallCount);
        }

        [TestMethod]
        public void CanvasWidth_DefaultIs800()
        {
            Assert.AreEqual(800, _vm.CanvasWidth);
        }

        [TestMethod]
        public void CanvasHeight_DefaultIs400()
        {
            Assert.AreEqual(400, _vm.CanvasHeight);
        }

        // ── INotifyPropertyChanged ────────────────────────────────────────────────

        [TestMethod]
        public void MainViewModel_ImplementsINotifyPropertyChanged()
        {
            Assert.IsInstanceOfType(_vm, typeof(INotifyPropertyChanged));
        }

        [TestMethod]
        public void BallCount_Setter_UpdatesValue()
        {
            _vm.BallCount = 20;
            Assert.AreEqual(20, _vm.BallCount);
        }

        [TestMethod]
        public void CanvasWidth_Setter_UpdatesValue()
        {
            _vm.CanvasWidth = 1280;
            Assert.AreEqual(1280, _vm.CanvasWidth);
        }

        [TestMethod]
        public void CanvasHeight_Setter_UpdatesValue()
        {
            _vm.CanvasHeight = 720;
            Assert.AreEqual(720, _vm.CanvasHeight);
        }

        [TestMethod]
        public void BallCount_PropertyChanged_IsFired()
        {
            bool fired = false;
            _vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_vm.BallCount)) fired = true;
            };

            _vm.BallCount = 10;

            Assert.IsTrue(fired, "PropertyChanged nie zostało wywołane dla BallCount");
        }

        [TestMethod]
        public void CanvasWidth_PropertyChanged_IsFired()
        {
            bool fired = false;
            _vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_vm.CanvasWidth)) fired = true;
            };

            _vm.CanvasWidth = 1024;

            Assert.IsTrue(fired, "PropertyChanged nie zostało wywołane dla CanvasWidth");
        }

        [TestMethod]
        public void CanvasHeight_PropertyChanged_IsFired()
        {
            bool fired = false;
            _vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_vm.CanvasHeight)) fired = true;
            };

            _vm.CanvasHeight = 768;

            Assert.IsTrue(fired, "PropertyChanged nie zostało wywołane dla CanvasHeight");
        }

        // ── komendy ───────────────────────────────────────────────────────────────

        [TestMethod]
        public void StartCommand_IsNotNull()
        {
            Assert.IsNotNull(_vm.StartCommand);
        }

        [TestMethod]
        public void StopCommand_IsNotNull()
        {
            Assert.IsNotNull(_vm.StopCommand);
        }

        [TestMethod]
        public void StartCommand_ImplementsICommand()
        {
            Assert.IsInstanceOfType(_vm.StartCommand, typeof(ICommand));
        }

        [TestMethod]
        public void StopCommand_ImplementsICommand()
        {
            Assert.IsInstanceOfType(_vm.StopCommand, typeof(ICommand));
        }

        [TestMethod]
        public void StartCommand_CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_vm.StartCommand.CanExecute(null));
        }

        [TestMethod]
        public void StopCommand_CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_vm.StopCommand.CanExecute(null));
        }

        [TestMethod]
        public void StartCommand_Execute_DoesNotThrow()
        {
            _vm.CanvasWidth = 800;
            _vm.CanvasHeight = 600;
            _vm.BallCount = 3;

            try
            {
                _vm.StartCommand.Execute(null);
                _vm.StopCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"Nie oczekiwano wyjątku: {ex.Message}");
            }
        }

        [TestMethod]
        public void StopCommand_Execute_DoesNotThrow()
        {
            try
            {
                _vm.StopCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"Nie oczekiwano wyjątku: {ex.Message}");
            }
        }

        [TestMethod]
        public void StartThenStop_DoesNotThrow()
        {
            _vm.BallCount = 2;

            try
            {
                _vm.StartCommand.Execute(null);
                _vm.StopCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"Nie oczekiwano wyjątku: {ex.Message}");
            }
        }

        // ── Balls ─────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Balls_IsNotNull()
        {
            Assert.IsNotNull(_vm.Balls);
        }

        [TestMethod]
        public void Balls_Initially_IsEmpty()
        {
            Assert.AreEqual(0, _vm.Balls.Count);
        }

        [TestMethod]
        public void Balls_IsObservableCollection()
        {
            Assert.IsInstanceOfType(_vm.Balls, typeof(ObservableCollection<BallModel>));
        }

        [TestMethod]
        public void Balls_IncreasesAfterStart()
        {
            _vm.CanvasWidth = 800;
            _vm.CanvasHeight = 600;
            _vm.BallCount = 3;

            _vm.StartCommand.Execute(null);

            Assert.AreEqual(3, _vm.Balls.Count);

            _vm.StopCommand.Execute(null);
        }

        [TestMethod]
        public void Balls_ClearedAfterStopAndRestart()
        {
            _vm.CanvasWidth = 800;
            _vm.CanvasHeight = 600;
            _vm.BallCount = 3;

            _vm.StartCommand.Execute(null);
            _vm.StopCommand.Execute(null);

            System.Threading.Thread.Sleep(150);

            _vm.BallCount = 2;
            _vm.StartCommand.Execute(null);

            Assert.AreEqual(2, _vm.Balls.Count);

            _vm.StopCommand.Execute(null);
        }
    }
}