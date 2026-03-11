using Data;
using System;

namespace BusinessLogic
{
    public class Calculator : ICalculator
    {
        private readonly IDataRepository _repository;

        // Dependency Injection through constructor
        public Calculator(IDataRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public double Add(double a, double b)
        {
            double result = a + b;
            _repository.SaveResult($"{a} + {b}", result);
            return result;
        }

        public double Subtract(double a, double b)
        {
            double result = a - b;
            _repository.SaveResult($"{a} - {b}", result);
            return result;
        }

        public double Multiply(double a, double b)
        {
            double result = a * b;
            _repository.SaveResult($"{a} * {b}", result);
            return result;
        }

        public double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            double result = a / b;
            _repository.SaveResult($"{a} / {b}", result);
            return result;
        }
    }
}
