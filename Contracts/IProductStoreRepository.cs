using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductStoreRepository
    {
        Task<IEnumerable<ProductsStore>> GetProductsToStoreId(int storeId);
        Task<IEnumerable<ProductsStore>> GetAllProductsStoreProductId(int productId);
        void DeleteProductsStore(ProductsStore productsStore);
    }
}
