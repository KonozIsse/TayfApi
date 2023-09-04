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
        Task<List<Inventory>> GetProductIdAttributeId(int productId, int attributeId, Predicate<Inventory>? predicateStock);
        List<Inventory> GetAllInventoryByPrductId(int productId);
        Task<IEnumerable<Inventory>> GetAllInventory();
        Task<List<Inventory>> GetAllOutStock();
        Task<Inventory> GetStockProductAttribut(int productId, int attributeId);
        Task<Inventory> GetStockProduct(int productId);
        Task<List<Inventory>> GetPredicateStockProduct(int productId, Predicate<Inventory> predicateStock);
        void AddInventory(Inventory inventory);
        void DeleteInventory(Inventory inventory); 
    }
}
