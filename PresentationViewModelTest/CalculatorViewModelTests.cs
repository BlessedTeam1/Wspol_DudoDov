using BusinessLogic;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PresentationViewModel;

namespace PresentationViewModelTest
{
    [TestClass]
    public class CalculatorViewModelTests
    {
        private CalculatorViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            IDataRepository repo = new InMemoryDataRepository();
            ICalculator calc = new Calculator(repo);
            _viewModel = new CalculatorViewModel(calc);
        }

        [TestMethod]
        public void Calculate_Addition_ResultIsCorrect()
        {
            _viewModel.Model.FirstNumber = 10;
            _viewModel.Model.SecondNumber = 5;
            _viewModel.Model.SelectedOperation = "+";

            _viewModel.Calculate();

            Assert.AreEqual(15, _viewModel.Model.Result);
            Assert.IsNull(_viewModel.Model.ErrorMessage);
        }

        [TestMethod]
        public void Calculate_Division_ResultIsCorrect()
        {
            _viewModel.Model.FirstNumber = 20;
            _viewModel.Model.SecondNumber = 4;
            _viewModel.Model.SelectedOperation = "/";

            _viewModel.Calculate();

            Assert.AreEqual(5, _viewModel.Model.Result);
        }

        [TestMethod]
        public void Calculate_DivisionByZero_SetsErrorMessage()
        {
            _viewModel.Model.FirstNumber = 10;
            _viewModel.Model.SecondNumber = 0;
            _viewModel.Model.SelectedOperation = "/";

            _viewModel.Calculate();

            Assert.IsNotNull(_viewModel.Model.ErrorMessage);
        }

        [TestMethod]
        public void Calculate_UnknownOperation_SetsErrorMessage()
        {
            _viewModel.Model.SelectedOperation = "%";

            _viewModel.Calculate();

            Assert.IsNotNull(_viewModel.Model.ErrorMessage);
        }
    }
}
