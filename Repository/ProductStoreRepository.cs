using Contracts;
using Entities;
using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ProductStoreRepository : RepositoryBase<ProductsStore>, IProductStoreRepository
    {
        public ProductStoreRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<ProductsStore>> GetProductsToStoreId(int storeId)
       => await FindByCondition(c => c.VendorId == storeId && c.IsStatus == Status.Active, false).ToListAsync();
        public async Task<IEnumerable<ProductsStore>> GetAllProductsStoreProductId(int productId)
        => await FindByCondition(t => t.ProductId == productId, false).ToListAsync();
        public void DeleteProductsStore(ProductsStore productsStore) => Delete(productsStore);
        public bool GetProductByStore(int vendorId)
         => FindByCondition(c => c.VendorId == vendorId && c.IsStatus == Status.Active, false).Count() > 0;

    }
}
