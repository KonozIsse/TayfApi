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
    public class OrderAttributeProductRepository : RepositoryBase<OrderAttributProduct>, IOrderAttributeProductRepository
    {
        public OrderAttributeProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<OrderAttributProduct> GetOrderAttributesProduct(int orderProductId, bool trackChanges)
        => await FindByCondition(c => c.OrderProductsId == orderProductId, trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<OrderAttributProduct>> GetAllOrderAttributesProducts(int orderProductId, bool trackChanges)
        => await FindByCondition(c => c.OrderProductsId == orderProductId, trackChanges).ToListAsync();
        public async Task<IEnumerable<OrderAttributProduct>> GetAttributesOrderProduct(int orderId, int productId)
       => await FindByCondition(c => c.OrderProducts.OrderId == orderId && c.OrderProducts.ProductId == productId, false)
            .Include(c=>c.ProductAttribut).ThenInclude(c=>c.ProductOption).Include(c=>c.ProductAttribut).ThenInclude(c=>c.ProductOptionValue).ToListAsync();
        public void DeleteOrderAttributProduct(OrderAttributProduct orderAttributProduct) => Delete(orderAttributProduct);
    }
}
