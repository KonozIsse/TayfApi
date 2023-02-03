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

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesProductId(int productId, bool trackChanges, bool isIncluded = false)
        {
             var cats = FindByCondition(c => c.ProductId == productId, trackChanges);
            if (isIncluded == true)
            {
                cats = cats.Include(c => c.Category);
            }
            return  await cats.ToListAsync();
        }
        public async Task<ProductCategory> GetCategoryToPrductId(int productId)
       => await FindByCondition(c => c.ProductId == productId, false).Include(c=>c.Category).SingleOrDefaultAsync();
    }
}
