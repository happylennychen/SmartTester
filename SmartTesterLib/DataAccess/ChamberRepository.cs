using Microsoft.EntityFrameworkCore;

namespace SmartTesterLib.DataAccess
{
    public class ChamberRepository : IChamberRepository
    {
        private readonly SmartTesterDbContext _context;
        public ChamberRepository(SmartTesterDbContext context)
        {
            _context = context;
        }
        public void AddChamber(IChamber chamber)
        {
            _context.Chambers.Add(chamber as DebugChamber);
            _context.SaveChanges();
        }

        public bool DeleteChamber(IChamber chamber)
        {
            try
            {
                _context.Remove(chamber);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<IChamber> GetAllChambers()
        {
            return _context.Chambers.OrderBy(c => c.Id).ToList();
        }

        public IChamber GetChamberById(int id)
        {
            return _context.Chambers.FirstOrDefault(c => c.Id == id);
        }

        public IChamber GetChamberByName(string name)
        {
            return _context.Chambers.FirstOrDefault(c => c.Name == name);
        }

        public bool UpdateChamber(string name, IChamber chamber)
        {
            var existingChamber = _context.Chambers.FirstOrDefault(c => c.Name == name);
            if (existingChamber != null)
            {
                try
                {
                    // 更新 existingChamber 的属性
                    _context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }
    }
}
