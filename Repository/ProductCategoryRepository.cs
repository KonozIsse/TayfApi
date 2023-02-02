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
    public class ProductCategoryRepository : RepositoryBase<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreateProductCategory(ProductCategory category) => Create (category);

        public void DeleteProductCategory(ProductCategory category)=>Delete(category);

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesProdId(int prodId, bool trackChanges)
        => await FindByCondition(c => c.ProductId == prodId, trackChanges).Include(c => c.Category).ToListAsync();
        public async Task<IEnumerable<ProductCategory>> GetCategoriesProdId(int prodId, bool trackChanges)
        => await FindByCondition(c => c.ProductId == prodId, trackChanges).ToListAsync();
        public async Task<ProductCategory> GetCategoryToPrductId(int productId)
       => await FindByCondition(c => c.ProductId == productId, false).Include(c=>c.Category).SingleOrDefaultAsync();
    }
}
