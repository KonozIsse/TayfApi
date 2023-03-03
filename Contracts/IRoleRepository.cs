using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetRolesAdminStore();
        Task<Role> GetRoleId(int id, bool trackChanges);
        Task<Role> IsExistRole(string name, bool trackChanges);
    }
}
