namespace SmartTesterLib.DataAccess
{
    public interface ITesterRepository
    {
        IEnumerable<ITester> GetAllTesters();
        ITester GetTesterByName(string name);
        ITester GetTesterById(int id);
        void AddTester(ITester tester);
        bool UpdateTester(string name, ITester tester);
        bool DeleteTester(ITester tester);
    }
}
