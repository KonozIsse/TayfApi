using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CategoriesStoreRepository : RepositoryBase<CategoriesStore>, ICategoriesStoreRepository
    {
        public CategoriesStoreRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<CategoriesStore>> GetCategoriesStoreId(int storeId)
            => await FindByCondition(c =>  c.VendorId == storeId, false).ToListAsync();
        public async Task<IEnumerable<CategoriesStore>> GetStoresCategoryId(int categoryId)
          => await FindByCondition(c => c.CategoryId == categoryId, false).ToListAsync();
    }
}
