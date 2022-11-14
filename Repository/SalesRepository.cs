using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class SalesRepository : RepositoryBase<ProductSales>, ISalesRepository
    {
        public SalesRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<ProductSales> CheckFlashExists(int productId , bool trackChanges)
        => await FindByCondition(r => r.ProductId == productId , trackChanges).FirstOrDefaultAsync();
        public async Task<ProductSales> GetFlashProductId(int productId)
        => await FindByCondition(r => r.ProductId == productId && r.EndDate > DateTime.UtcNow && r.IsStatus == Status.Active,false).FirstOrDefaultAsync();
        public List<ProductSales> GetAllSales()
         =>  FindByCondition(r => r.EndDate > DateTime.UtcNow && r.IsStatus == Status.Active, false).ToList();
        public async Task<IEnumerable<ProductSales>> GetAllSalesProductId(int productId)
        => await FindByCondition(r => r.ProductId ==  productId, false).ToListAsync();
        public void AddFlashSale(ProductSales sale) => Create(sale);
        public void DeleteFlashSale(ProductSales sale) => Delete(sale);
    }
}
