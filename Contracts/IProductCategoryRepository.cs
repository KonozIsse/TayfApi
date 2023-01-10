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
        Task<ProductCategory> GetProductCategoryId(int id, bool trackChanges);
        Task<IEnumerable<ProductCategory>> GetAllProductCategory(bool trackChanges);
        Task<IEnumerable<ProductCategory>> GetAllProductsCatId(int catId ,bool trackChanges);
        Task<IEnumerable<ProductCategory>> GetAllCategoriesProdId(int prodId, bool trackChanges);
        Task<ProductCategory> GetCategoryToPrductId(int productId);
        void CreateProductCategory(ProductCategory category);
        void DeleteProductCategory(ProductCategory category);
    }
}
