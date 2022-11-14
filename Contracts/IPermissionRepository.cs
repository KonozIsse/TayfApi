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
        Task<IEnumerable<Permission>> GetPermissions(int roleId);
    }
    public interface ILinkRepository
    {
        Task<IEnumerable<Link>> GetLinks();
    }
}
