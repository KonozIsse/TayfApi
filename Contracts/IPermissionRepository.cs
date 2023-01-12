using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissionsShowRole(int roleId);
        Task<IEnumerable<Permission>> GetPermissionsRole(int roleId, bool trackChanges);
        Task<IEnumerable<Permission>> GetLinksRole(int roleId, List<int> Ids, bool trackChanges);
        void AddPermission(Permission permission);
        void DeletePermission(Permission permission);
    }
    public interface ILinkRepository
    {
        Task<IEnumerable<Link>> GetLinks();
    }
}
