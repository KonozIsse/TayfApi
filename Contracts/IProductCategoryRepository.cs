using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategory>> GetAllCategoriesProductId(int productId, bool trackChanges, bool isIncluded = false);
        Task<IEnumerable<ProductCategory>> GetAllProductsCategory(int catId, bool trackChanges);
        Task<ProductCategory> GetCategoryToPrductId(int productId);
        void CreateProductCategory(ProductCategory category);
        void DeleteProductCategory(ProductCategory category);
    }
}
