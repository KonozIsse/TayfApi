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
    public class SpecialProductsRepository : RepositoryBase<SpecialProducts>, ISpecialProductsRepository
    {
        public SpecialProductsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<SpecialProducts> GetSpecialProductId(int productId)
         => await FindByCondition(c => c.ProductId == productId && c.EndDate > DateTime.Now && c.IsStatus == Status.Active, false).FirstOrDefaultAsync();
        public async Task<SpecialProducts> CheckSpecialExists(int productId , bool trackChanges)
        => await FindByCondition(c => c.ProductId == productId , trackChanges).FirstOrDefaultAsync();
        public void AddSpecialProduct(SpecialProducts special) => Create(special);
        public void DeleteSpecialProduct(SpecialProducts special) => Delete(special);
    }
}
