using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISalesRepository
    {
        Task<ProductSales> CheckFlashExists(int productId, bool trackChanges);
        Task<ProductSales> GetFlashProductId(int productId);
        Task<IEnumerable<ProductSales>> GetAllSalesProductId(int productId, bool trackChanges);
        void AddFlashSale(ProductSales sale);
        void DeleteFlashSale(ProductSales sale);
        void CreateListSales(List<ProductSales> entity);
        Task DeleteListSales(List<int> Ids);
    }
}
