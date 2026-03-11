namespace Data
{
    public interface IDataRepository
    {
        void SaveResult(string operation, double result);
        System.Collections.Generic.IEnumerable<string> GetHistory();
    }
}
