using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Entities.RequestFeatures;

namespace Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Product> GetProductById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<Product> GetActiveProductById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges).FirstOrDefaultAsync();  
        public async Task<Product> GetAcceptAdminActiveProduct(int id)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active && c.IsAcceptAdmin == true, false).FirstOrDefaultAsync();
        public List<Product> GetAllProducts()
        =>  FindByCondition(c => c.IsStatus == Status.Active, false).OrderByDescending(c => c.CreatedAt).ToList();
        public async Task<List<int>> GetAllProductsToCategoryId(int categoryId)
        => await FindByCondition(c => c.CategoryId == categoryId && c.IsStatus == Status.Active, false).OrderByDescending(c => c.CreatedAt).Select(c=>c.Id).ToListAsync();

        public async Task<List<Product>> GetProductsTOStoreId(int storeId)
        => await FindByCondition(c => c.ProductsStores.Any(x => x.VendorId == storeId) && c.IsStatus == Status.Active, false).OrderByDescending(c => c.CreatedAt).ToListAsync();
        public async Task<List<Product>> GetAllProductLikeCustomersId(int customerId)
        => await FindByCondition(c => c.WishLists.Any(c => c.CustomerId == customerId), false).ToListAsync();
        public async Task<Product> CheckApproveProduct(int id)
         => await FindByCondition(c => c.Id == id && c.IsAcceptAdmin == true, false).FirstOrDefaultAsync();
        public async Task<List<Product>> GetFeartureProducts(int pageSize)
        => await FindByCondition(c => c.IsFeature == 1 && c.IsStatus == Status.Active, false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.Id).Take(pageSize).ToListAsync();
        public async Task<List<Product>> TopRatedPage(int pageSize)
        => await FindByCondition(c => c.Reviews.Any(r => r.IsStatus == Status.Active), false)
            .OrderByDescending(p => p.Reviews.Average(r => r.Rating)).Include(p => p.Images).Include(p => p.Reviews).Take(pageSize).ToListAsync();
        public async Task<List<Product>> GetBestProducts(int pageSize)
        => await FindByCondition(c => c.IsBest == 1 && c.IsStatus == Status.Active, false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.Id).Take(pageSize).ToListAsync();

        public async Task<List<Product>> GetPopularProducts(int pageSize)
         => await FindByCondition(c => c.IsPopular == 1 && c.IsStatus == Status.Active, false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.Id).Take(pageSize).ToListAsync();
        public async Task<List<Product>> GetLatestPage(int pageSize)
         => await FindByCondition(c => c.IsStatus == Status.Active, false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.CreatedAt).Take(pageSize).ToListAsync();

        public async Task<List<Product>> SpecialsPage(int pageSize)
          => await FindByCondition(c => c.SpecialProducts.Any(x => x.IsStatus == Status.Active && x.EndDate > DateTime.Now), false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.Id).Take(pageSize).ToListAsync();

        public async Task<List<Product>> DailyDeals()
        => await FindByCondition(c => c.Reviews.Any(), false).Include(p => p.Images).Include(p => p.Reviews).ToListAsync();
        //.ToList() .Select(p => ProductCard(p))

        public async Task<List<Product>> SearshProductByCategoryAndStore(int storeId, string search, int categoryId)
        {
            var list = FindByCondition(c => categoryId == 0 || c.Category.MainCategoryId == categoryId
            || (c.CategoryId == categoryId && c.Category.IsStatus == Status.Active && c.Category.MainCategoryId != 1)

            && storeId == 0 || c.ProductsStores.Any(s => s.VendorId == storeId && s.Vendor.Status == Status.Active)
            && c.IsAcceptAdmin == true && c.IsStatus == Status.Active

            && ((!String.IsNullOrEmpty(c.Category.CategoryName) && c.Category.CategoryName.Contains(search)) || String.IsNullOrEmpty(search))

            || ((!String.IsNullOrEmpty(c.ProductName) && c.ProductName.Contains(search)) || String.IsNullOrEmpty(search)), false);
            return await list.ToListAsync();
        }
        public async Task<List<Product>> SearshProductByCategoryAndSore(int storeId, string search, int categoryId)
        {
            var list = FindByCondition(c => categoryId == 0 || c.Category.MainCategoryId == categoryId
            || (c.CategoryId == categoryId && c.Category.IsStatus == Status.Active && c.Category.MainCategoryId != 1)

            && storeId == 0 || c.ProductsStores.Any(s => s.VendorId == storeId && s.Vendor.Status == Status.Active)
            && c.IsAcceptAdmin == true && c.IsStatus == Status.Active

            && ((!String.IsNullOrEmpty(c.Category.CategoryName) && c.Category.CategoryName.Contains(search)) || String.IsNullOrEmpty(search))

            || ((!String.IsNullOrEmpty(c.ProductName) && c.ProductName.Contains(search)) || String.IsNullOrEmpty(search)), false);
            return await list.ToListAsync();
        }
        public void AddProductOnCategory(int categoryId, Product product)
        {
            product.CategoryId = categoryId;
            Create(product);
        }
        public void DeleteProduct(Product product) => Delete(product);
    }

    public class ProductTypeRepository : RepositoryBase<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<ProductType>> GetProductTypes()
        => await FindAll(false).ToListAsync();
    }
}
