using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.RequestFeatures;

namespace Repository
{
    public class UnitRepository : RepositoryBase<Unit>, IUnitRepository
    {
        public UnitRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Unit> GetUnitId(int id, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        public async Task<List<Unit>> GetAlActivelUnit()
        => await FindByCondition(c => c.IsStatus == Status.Active,false).ToListAsync();
        public async Task<List<Unit>> GetUnitsByVendor(int vendorId)
        => await FindByCondition(c => c.IsStatus == Status.Active && c.StoreId == vendorId, false).ToListAsync();

    }
}
