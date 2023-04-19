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
using Entities.Models.Enums;

namespace Repository
{
    public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
    {
        public PermissionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Permission>> GetPermissionsShowRole (int roleId)
        => await FindByCondition(c => c.RoleId == roleId && c.Link.IsStatus == Status.Active, false).Include(r => r.Link).ToListAsync();   
        public async Task<IEnumerable<Permission>> GetPermissionsRole (int roleId , bool trackChanges)
        => await FindByCondition(c => c.RoleId == roleId , trackChanges).ToListAsync();
        public async Task<IEnumerable<Permission>> GetLinksRole(int roleId, List<int> Ids, bool trackChanges)
        => await FindByCondition(c => c.RoleId == roleId && Ids.Contains(c.Id), trackChanges).ToListAsync();
        public void AddPermission(Permission permission) => Create(permission);
        public void DeletePermission(Permission permission) => Delete(permission);
    }
    public class LinkRepository : RepositoryBase<Link>, ILinkRepository
    {
        public LinkRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Link>> GetLinks()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).Include(r => r.Permissions).ToListAsync();
    }
}
