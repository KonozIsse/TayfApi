using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        public InventoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            
        }
        public async Task<List<Inventory>> GetProductIdOptoinIdInStock(int productId, int option)
         => await FindByCondition(r => r.ProductId == productId && r.AttributesProductId == option && r.StockType == "in",false).ToListAsync();
        public async Task<List<Inventory>> GetProductIdOptoinIdOutStock(int productId, int option)
         => await FindByCondition(r => r.ProductId == productId && r.AttributesProductId == option && r.StockType == "out", false).ToListAsync();
        public async Task<IEnumerable<Inventory>> GetAllInventoryByProductIdOption (int productId, int attributeId)
         => await FindByCondition(r => r.ProductId == productId && r.AttributesProductId == attributeId, false).ToListAsync();
        public async Task<List<Inventory>> GetAllInventoryByPrductId(int productId)
         => await FindByCondition(r => r.ProductId == productId, false).ToListAsync();
        public async Task<Inventory> GetInventoryByProductId (int productId)
         => await FindByCondition(r => r.ProductId == productId, false).FirstOrDefaultAsync();
        public async Task<IEnumerable<Inventory>> GetAllInventory() => await FindAll(false).ToListAsync();
        public async Task<IEnumerable<Inventory>> AllInventoryByVendor(int vendorId)
        => await FindByCondition(r => r.VendorId == vendorId , false).ToListAsync();
        public async Task<List<Inventory>> GetOptionsByProductIdInStock(int productId)
        => await FindByCondition(r => r.ProductId == productId && r.StockType == "in", false).ToListAsync();
        public async Task<List<Inventory>> GetOptionsByProductIdOutStock(int productId)
        => await FindByCondition(r => r.ProductId == productId && r.StockType == "out", false).ToListAsync();
        public int getInventorySingleProduct(int productId)
         => FindByCondition(r => r.ProductId == productId && r.Stock > 0 && r.StockType == "in", false).FirstOrDefault().ProductId;
        public int GetInventorySingleStock(int productId)
        => FindByCondition(r => r.ProductId == productId && r.Stock > 0 && r.StockType == "in", false).FirstOrDefault().Stock;
        public void AddInventory(Inventory inventory) => Create(inventory);
        public void DeleteInventory(Inventory inventory) => Delete(inventory);// delete inventory if was exsit in stock
    }
}
