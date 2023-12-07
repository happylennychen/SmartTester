using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTesterLib.DataAccess
{
    public interface IChamberRepository
    {
        IEnumerable<IChamber> GetAllChambers();
        IChamber GetChamberByName(string name);
        IChamber GetChamberById(int id);
        void AddChamber(IChamber chamber);
        bool UpdateChamber(string name, IChamber chamber);
        bool DeleteChamber(IChamber chamber);
    }
}
