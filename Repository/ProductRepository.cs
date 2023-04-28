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
using MimeKit.IO;

namespace Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Product>> GetAllAcceptedProducts()
          => await FindByCondition(c =>  c.IsStatus == Status.Active && c.IsAcceptAdmin == true, false)
            .Include(s => s.SpecialProducts).Include(c => c.ProductSales).Include(e => e.WishLists)
            .Include(c => c.AttributesProducts).Include(c=>c.Store)
            .OrderByDescending(c => c.CreatedAt).ToListAsync();
        public async Task<Product> GetProductById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<Product> GetActiveProductById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges).FirstOrDefaultAsync();  
        public async Task<Product> GetAcceptAdminActiveProduct(int id)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active && c.IsAcceptAdmin == true, false)
            .Include(c=>c.Store).Include(c=>c.Images).Include(c=>c.ProductCategories).Include(s=>s.SpecialProducts).Include(c=>c.ProductSales)
            .Include(c=>c.Reviews).Include(e=>e.WishLists).Include(c=>c.AttributesProducts).FirstOrDefaultAsync();
        
        public async Task<List<Product>> GetProductsTOStoreId(int storeId)
        => await FindByCondition(c => c.StoreId == storeId && c.IsStatus == Status.Active, false).OrderByDescending(c => c.Id).ToListAsync();
       public async Task<List<Product>> GetAllProducts()
       => await FindAll(false).OrderByDescending(c => c.CreatedAt).ToListAsync();
        public async Task<Product> CheckApproveProduct(int id)
         => await FindByCondition(c => c.Id == id && c.IsAcceptAdmin == true, false).FirstOrDefaultAsync();
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
         => await FindByCondition(c => c.IsStatus == Status.Active, false).Include(i => i.Images).Include(r => r.Reviews)
            .OrderByDescending(p => p.CreatedAt).Take(pageSize).ToListAsync();

        public async Task<List<Product>> SpecialsPage(int pageSize)
          => await FindByCondition(c => c.SpecialProducts.IsStatus == Status.Active && c.SpecialProducts.EndDate > DateTime.Now, false)
                .Include(i => i.Images).Include(r => r.Reviews).OrderByDescending(p => p.Id).Take(pageSize).ToListAsync();

        public async Task<List<Product>> DailyDeals()
        => await FindByCondition(c => c.Reviews.Any(), false).Include(p => p.Images).Include(p => p.Reviews).ToListAsync();

       
        public async Task<List<Product>> GetProductsCP(string search, int? filter)
        {
            var list = FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(r => r.ProductName.Contains(search));
            }
            if (filter != 0 && filter != null)
            {
                list = list.Where(c=>c.ProductCategories.Any(c => c.CategoryId == Convert.ToInt32(filter)));
            }
            return await list.Include(c => c.ProductCategories).ThenInclude(c=>c.Category).Include(c => c.Images).Include(c=>c.WishLists).OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
        public void AddProduct( Product product)=>Create(product);
        public void DeleteProduct(Product product) => Delete(product);
      
}

   
}
