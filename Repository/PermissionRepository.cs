using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
    {
        public PermissionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<Permission>> GetPermissionsShowRole (int roleId)
        => await FindByCondition(c => c.RoleId == roleId && c.Link.Show == true, false).Include(r => r.Link).ToListAsync();   
        public async Task<IEnumerable<Permission>> GetPermissionsRole (int roleId , bool trackChanges)
        => await FindByCondition(c => c.RoleId == roleId , trackChanges).ToListAsync();
        public void AddPermission(Permission permission) => Create(permission);
        public void DeletePermission(Permission permission) => Delete(permission);
    }
    public class LinkRepository : RepositoryBase<Link>, ILinkRepository
    {
        public LinkRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<Link>> GetLinks()
        => await FindByCondition(c => c.Show == true, false).Include(r => r.Permissions).ToListAsync();
    }
}
