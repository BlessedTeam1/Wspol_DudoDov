using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DataTest
{
    [TestClass]
    public class InMemoryDataRepositoryTests
    {
        private IDataRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryDataRepository();
        }

        [TestMethod]
        public void SaveResult_SingleEntry_HistoryContainsOneItem()
        {
            _repository.SaveResult("5 + 3", 8);

            IEnumerable<string> history = _repository.GetHistory();

            Assert.AreEqual(1, history.Count());
        }

        [TestMethod]
        public void SaveResult_MultipleEntries_HistoryContainsAllItems()
        {
            _repository.SaveResult("5 + 3", 8);
            _repository.SaveResult("10 - 4", 6);
            _repository.SaveResult("3 * 7", 21);

            IEnumerable<string> history = _repository.GetHistory();

            Assert.AreEqual(3, history.Count());
        }

        [TestMethod]
        public void GetHistory_EmptyRepository_ReturnsEmptyCollection()
        {
            IEnumerable<string> history = _repository.GetHistory();
            Assert.IsFalse(history.Any());
        }

        [TestMethod]
        public void SaveResult_EntryFormat_IsCorrect()
        {
            _repository.SaveResult("5 + 3", 8);

            string entry = _repository.GetHistory().First();

            Assert.AreEqual("5 + 3 = 8", entry);
        }
    }
}
