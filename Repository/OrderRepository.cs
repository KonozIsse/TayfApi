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
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Order> GetOrderId(int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<Order> GetLastOrderCustomer(int customerId, bool trackChanges)
         => await FindByCondition(c => c.CustomerId == customerId , trackChanges).OrderByDescending(c=>c.Id).SingleOrDefaultAsync();
        public async Task<List<Order>> GetAllOrders()
         => await FindAll(false).OrderByDescending(r => r.CreatedAt).ToListAsync();
        public async Task<List<Order>> GetOrdersToStore(int storeId)
       => await FindByCondition(c => c.StoreId == storeId, false).ToListAsync(); 
        public async Task<List<Order>> GetsAllTransactionOrders()
       => await FindByCondition(c => c.TransactionId != null, false).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<List<Order>> GetOrdersToCustomer(int customerId)
        => await FindByCondition(c => c.CustomerId == customerId, false).ToListAsync();
        public async Task<Order> GetOrderDetailsByCustomer(int id, int customerId)
        => await FindByCondition(c => c.Id == id && c.CustomerId == customerId, false).SingleOrDefaultAsync();
        public async Task<Order> GetByHashedCsp(string csp)
        => await FindByCondition(c => c.HashedCtpAndPayment == csp,false).FirstOrDefaultAsync();
        public async Task<List<Order>> OrdersByProductId(int productId)
        => await FindByCondition(c => c.OrderProducts.Where(x => x.ProductId == productId).Count() > 0,false).ToListAsync();
        public async Task<Order> GetCustomerNewOlderByStore(int vendorId, int customerId)
         => await FindByCondition(c => c.StoreId == vendorId && c.CustomerId == customerId && c.OrderStatusId == 1, false).SingleOrDefaultAsync();
        public async Task<List<Order>> Get15OrdersVendor(int vendorId)
        => await FindByCondition(c => c.StoreId == vendorId,false).Take(15).ToListAsync();
        public async Task<List<Order>> GetPendingOrders()
         => await FindByCondition(c => c.OrderStatusId == 1,false).ToListAsync();
        public async Task<List<Order>> GetPendOrdersByStore(int vendorId)
            => await FindByCondition(c => c.OrderStatusId == 1 && c.StoreId == vendorId, false).ToListAsync();
        public async Task<List<Order>> GetCompleteOrders()
         => await FindByCondition(c => c.OrderStatusId == 2, false).ToListAsync();
        public async Task<List<Order>> GetCompleteOrdersByStore(int vendorId)
         => await FindByCondition(c => c.OrderStatusId == 2 && c.StoreId == vendorId, false).ToListAsync();
        public async Task<List<Order>> GetCancelOrders()
         => await FindByCondition(c => c.OrderStatusId == 3, false).ToListAsync();
        public async Task<List<Order>> GetCancelOrdersByStore(int vendorId)
         => await FindByCondition(c => c.OrderStatusId == 3 && c.StoreId == vendorId, false).ToListAsync();
        public async Task<List<Order>> GetPendingOrdersByCustomer(int customerId)
         => await FindByCondition(c => c.OrderStatusId == 1 && c.CustomerId == customerId, false).ToListAsync();
        public async Task<List<Order>> GetCompletedOrdersByCustomer(int customerId)
         => await FindByCondition(c => c.OrderStatusId == 2 && c.CustomerId == customerId, false).ToListAsync();
        public async Task<Order> OrderIdByPandingStatus(int id)
         => await FindByCondition(c => c.Id == id && c.OrderStatusId == 1, false).SingleOrDefaultAsync();
        public async Task<Order> GetOrderIdAndStatusId(int id, int status)
         => await FindByCondition(c => c.Id == id && c.OrderStatusId == status, false).SingleOrDefaultAsync();
        public int GetOrdersByVendor(int vendorId) => FindByCondition(c => c.StoreId == vendorId, false).Count();
        public bool GetOrderByStore(int storeId) => FindByCondition(c => c.StoreId == storeId, false).Count() > 0;
        public int GetAllOrdersCount() => FindAll(false).Count();
        public void DeleteOrder(Order order) => Delete(order);

        public void CreateOrder(Order order) => Create(order);
    }
   
}
