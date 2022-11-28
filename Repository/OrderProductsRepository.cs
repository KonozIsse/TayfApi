using Contracts;
using Entities;
using Entities.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderProductsRepository : RepositoryBase<OrderProduct>, IOrderProductsRepository
    {
        public OrderProductsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<OrderProduct>> GetOrderProducts(int productId, int orderId)
        => await FindByCondition(r => r.ProductId == productId && r.OrderId == orderId, false).ToListAsync();
        public async Task<OrderProduct> GetOrderProductsId(int productId, int orderId , bool trackChanges)
        => await FindByCondition(r => r.ProductId == productId && r.OrderId == orderId, trackChanges).FirstOrDefaultAsync();
        public async Task<List<OrderProduct>> GetAllProductsToOrderId (int orderId)
        => await FindByCondition(r =>  r.OrderId == orderId, false).ToListAsync();
        public void CreateOrderProduct (OrderProduct orderProducts) => Create(orderProducts);
        public void DeleteOrderProduct(OrderProduct orderProducts) => Delete(orderProducts);
    }

}
