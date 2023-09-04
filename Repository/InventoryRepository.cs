using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.Models.Enums;

namespace Repository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        public InventoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            
        }
        public async Task<List<Inventory>> GetProductIdAttributeId(int productId, int attributeId, Predicate<Inventory>? predicateStock)
        {
            var result = FindByCondition(r => r.ProductId == productId && r.AttributesProductId == attributeId, false);
            if (predicateStock != null)
            {
                result = result.Where(r => predicateStock(r));
            }
            return await result.ToListAsync();
        }
           public List<Inventory> GetAllInventoryByPrductId(int productId)
         => FindByCondition(r => r.ProductId == productId, false).Include(c => c.Product).ToList();
        public async Task<IEnumerable<Inventory>> GetAllInventory() 
            => await FindAll(false).Include(c=>c.Product).ThenInclude(c=>c.Images).OrderByDescending(c=>c.CreatedAt)
            .ToListAsync();
        public async Task<List<Inventory>> GetAllOutStock()
        => await FindByCondition(r =>  r.StockType == "out", false).ToListAsync();
        public async Task<Inventory> GetStockProduct(int productId)
         => await FindByCondition(r => r.ProductId == productId && r.Stock > 0 && r.StockType == "in", false).FirstOrDefaultAsync(); 
        public async Task<Inventory> GetStockProductAttribut(int productId , int attributeId)
         => await FindByCondition(r => r.ProductId == productId && r.AttributesProductId == attributeId && r.Stock > 0 && r.StockType == "in", false).FirstOrDefaultAsync();
        public void AddInventory(Inventory inventory) => Create(inventory);
        public void DeleteInventory(Inventory inventory) => Delete(inventory);// delete inventory if was exsit in stock

        public async Task<List<Inventory>> GetPredicateStockProduct(int productId, Predicate<Inventory> predicateStock)
         => await FindByCondition(r => r.ProductId == productId && predicateStock(r), false).ToListAsync();
    }
}

