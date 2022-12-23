using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Role>> GetRolesAdminStore()
            => await FindByCondition(c => c.IsStatus == Status.Active && c.Id != 2, false).ToListAsync();  
        public async Task<Role> GetRoleId(int id ,bool trackChanges)
            => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync(); 
        public bool IsExistRole(string name)
            =>  FindByCondition(c => c.Name.Equals(name), false).Count()>0;
    }
}
