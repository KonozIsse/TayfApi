using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IInventoryRepository
    {
        Task<List<Inventory>> GetProductIdOptoinIdInStock(int productId, int option);
        Task<List<Inventory>> GetProductIdOptoinIdOutStock(int productId, int option);
        Task<IEnumerable<Inventory>> GetAllInventoryByProductIdOption(int productId, int option);
        List<Inventory> GetAllInventoryByPrductId(int productId);
        Task<IEnumerable<Inventory>> GetAllInventory();
        Task<List<Inventory>> GetOptionsByProductIdInStock(int productId);
        Task<List<Inventory>> GetAllOutStock();
        Task<Inventory> GetStockProductAttribut(int productId, int attributeId);
        Task<Inventory> GetStockProduct(int productId);
        Task<List<Inventory>> GetOutStockProduct(int productId);
        Task<List<Inventory>> GetInStockProduct(int productId);
        void AddInventory(Inventory inventory);
        void DeleteInventory(Inventory inventory); 
    }
}
