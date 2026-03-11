using System.Collections.Generic;

namespace Data
{
    public class InMemoryDataRepository : IDataRepository
    {
        private readonly List<string> _history = new List<string>();

        public void SaveResult(string operation, double result)
        {
            _history.Add($"{operation} = {result}");
        }

        public IEnumerable<string> GetHistory()
        {
            return _history;
        }
    }
}
