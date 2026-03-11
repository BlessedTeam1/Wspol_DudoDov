using Microsoft.VisualStudio.TestTools.UnitTesting;
using PresentationModel;

namespace PresentationModelTest
{
    [TestClass]
    public class CalculatorModelTests
    {
        [TestMethod]
        public void CalculatorModel_DefaultValues_AreCorrect()
        {
            var model = new CalculatorModel();

            Assert.AreEqual(0, model.FirstNumber);
            Assert.AreEqual(0, model.SecondNumber);
            Assert.AreEqual("+", model.SelectedOperation);
            Assert.AreEqual(0, model.Result);
            Assert.IsNull(model.ErrorMessage);
        }

        [TestMethod]
        public void CalculatorModel_SetProperties_WorkCorrectly()
        {
            var model = new CalculatorModel
            {
                FirstNumber = 10,
                SecondNumber = 5,
                SelectedOperation = "-",
                Result = 5
            };

            Assert.AreEqual(10, model.FirstNumber);
            Assert.AreEqual(5, model.SecondNumber);
            Assert.AreEqual("-", model.SelectedOperation);
            Assert.AreEqual(5, model.Result);
        }
    }
}
