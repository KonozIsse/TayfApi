using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAcceptedProducts();
        Task<List<Product>> GetProducts();
        Task<Product> GetProductById(int id, bool trackChanges);
        Task<Product> GetActiveProductById(int id, bool trackChanges);
        Task<Product> GetAcceptAdminActiveProduct(int id);
        Task<List<Product>> GetProductsCatId(int categoryId);
        Task<List<Product>> GetProductsTOStoreId(int storeId);
        Task<Product> CheckApproveProduct(int id); 
        Task<Product> GetProductStore(int productId , int storeId);
        Task<List<Product>> GetFeartureProducts(int pageSize);
        Task<List<Product>> TopRatedPage(int pageSize);
        Task<List<Product>> GetBestProducts(int pageSize);
        Task<List<Product>> GetPopularProducts(int pageSize);
        Task<List<Product>> GetLatestPage(int pageSize);
        Task<List<Product>> SpecialsPage(int pageSize);
        Task<List<Product>> DailyDeals();
        Task<PagedList<Product>> GetProductsCP(int? storeId, string search, PostsParameters postsParameters);
        Task<List<Product>> SearshProductByCategoryAndStore(int storeId, string search, int categoryId);
        void AddProduct(Product product);
        void DeleteProduct(Product product);
    }
    public interface IProductTypeRepository
    {
        Task<List<ProductType>> GetProductTypes();
    }
}
