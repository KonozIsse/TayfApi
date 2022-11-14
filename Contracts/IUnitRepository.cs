using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUnitRepository
    {
        Task<Unit> GetUnitId(int id, bool trackChanges);
        Task<List<Unit>> GetAlActivelUnit();
        Task<List<Unit>> GetUnitsByVendor(int vendorId);
    }
}
