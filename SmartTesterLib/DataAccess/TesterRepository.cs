using Microsoft.EntityFrameworkCore;

namespace SmartTesterLib.DataAccess
{
    public class TesterRepository : ITesterRepository
    {
        private readonly SmartTesterDbContext _context;

        public TesterRepository(SmartTesterDbContext context)
        {
            _context = context;
        }

        public void AddTester(ITester tester)
        {
            _context.Testers.Add(tester as DebugTester);
            _context.SaveChanges();
        }

        public bool DeleteTester(ITester tester)
        {
            _context.Testers.Remove(tester as DebugTester);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<ITester> GetAllTesters()
        {
            return _context.Testers.ToList();
        }

        public ITester GetTesterById(int id)
        {
            return _context.Testers.FirstOrDefault(t => t.Id == id);
        }

        public ITester GetTesterByName(string name)
        {
            return _context.Testers.FirstOrDefault(t => t.Name == name);
        }

        public bool UpdateTester(string name, ITester tester)
        {
            var existingTester = _context.Testers.FirstOrDefault(t => t.Name == name);
            if (existingTester != null)
            {
                // Update properties of existingTester
                _context.SaveChanges();
                return true;
            }
            else
                return false;
        }
    }
}
