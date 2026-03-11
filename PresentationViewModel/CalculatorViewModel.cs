using BusinessLogic;
using Data;
using PresentationModel;
using System;

namespace PresentationViewModel
{
    public class CalculatorViewModel
    {
        private readonly ICalculator _calculator;
        public CalculatorModel Model { get; private set; }

        public CalculatorViewModel(ICalculator calculator)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
            Model = new CalculatorModel();
        }

        // Factory method for convenience (creates with default InMemory repo)
        public static CalculatorViewModel CreateDefault()
        {
            IDataRepository repo = new InMemoryDataRepository();
            ICalculator calc = new Calculator(repo);
            return new CalculatorViewModel(calc);
        }

        public void Calculate()
        {
            try
            {
                Model.ErrorMessage = null;
                switch (Model.SelectedOperation)
                {
                    case "+":
                        Model.Result = _calculator.Add(Model.FirstNumber, Model.SecondNumber);
                        break;
                    case "-":
                        Model.Result = _calculator.Subtract(Model.FirstNumber, Model.SecondNumber);
                        break;
                    case "*":
                        Model.Result = _calculator.Multiply(Model.FirstNumber, Model.SecondNumber);
                        break;
                    case "/":
                        Model.Result = _calculator.Divide(Model.FirstNumber, Model.SecondNumber);
                        break;
                    default:
                        Model.ErrorMessage = "Unknown operation.";
                        break;
                }
            }
            catch (DivideByZeroException ex)
            {
                Model.ErrorMessage = ex.Message;
            }
        }
    }
}
