using BusinessLogic;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BusinessLogicTest
{
    [TestClass]
    public class CalculatorTests
    {
        private ICalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            IDataRepository repo = new InMemoryDataRepository();
            _calculator = new Calculator(repo);
        }

        [TestMethod]
        public void Add_TwoPositiveNumbers_ReturnsCorrectSum()
        {
            // Arrange
            double a = 5, b = 3;

            // Act
            double result = _calculator.Add(a, b);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void Subtract_TwoPositiveNumbers_ReturnsCorrectDifference()
        {
            double result = _calculator.Subtract(10, 4);
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void Multiply_TwoNumbers_ReturnsCorrectProduct()
        {
            double result = _calculator.Multiply(3, 7);
            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void Divide_TwoNumbers_ReturnsCorrectQuotient()
        {
            double result = _calculator.Divide(10, 2);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            _calculator.Divide(10, 0);
        }

        [TestMethod]
        public void Add_NegativeNumbers_ReturnsCorrectSum()
        {
            double result = _calculator.Add(-3, -7);
            Assert.AreEqual(-10, result);
        }

        [TestMethod]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Calculator(null));
        }
    }
}
