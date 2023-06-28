namespace SmartTester
{
    public interface IDataLogger
    {
        //int Id { get; set; }
        string FilePath { get; set; }
        void AddData(string log);
        void Flush();
        void Close();
    }
}