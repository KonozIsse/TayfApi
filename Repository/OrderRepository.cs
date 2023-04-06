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
        public async Task<List<Order>> GetAllOrders(string search)
        {
            var orders = FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(c => c.Customer.FullName.Contains(search) || c.Store.FirstName.Contains(search)
                || c.OrderStatus.StatusName.Contains(search) || c.Id.ToString().Contains(search));
            }
            return await orders.Include(c => c.Customer).Include(c => c.OrderStatus).Include(c=>c.Currency)
                .Include(c => c.DeliveryTime).Include(c => c.Store).OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
        public async Task<List<Order>> GetOrders(bool trackChanges)
         => await FindAll(trackChanges).Include(c => c.Customer).Include(c => c.OrderStatus).Include(c => c.Currency)
                .Include(c => c.DeliveryTime).Include(c => c.Store).OrderByDescending(r => r.CreatedAt).ToListAsync();
        
        public async Task<List<Order>> GetOrdersToStore(int storeId)
          => await FindByCondition(c => c.StoreId == storeId, false).ToListAsync(); 
        public async Task<List<Order>> GetsAllTransactionOrders()
         => await FindByCondition(c => c.TransactionId != null, false).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<List<Order>> GetOrdersToCustomer(int customerId)
        => await FindByCondition(c => c.CustomerId == customerId, false).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<Order> GetCustomerNewOlderByStore(int vendorId, int customerId)
         => await FindByCondition(c => c.StoreId == vendorId && c.CustomerId == customerId && c.OrderStatusId == 1, false).SingleOrDefaultAsync();
         public void DeleteOrder(Order order) => Delete(order);
         public void CreateOrder(Order order) => Create(order);

        public async Task<List<Order>> GetAllSalesOrders(string search, int customerId,int storeId,int statusId, DateTime? dateFrom, DateTime? dateTo)
        {
            var orders = FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(c => c.Customer.FirstName.Contains(search) || c.Store.FirstName.Contains(search)
                         || c.OrderStatus.StatusName.Contains(search));
            }
            if (dateFrom != null)
            {
                orders = orders.Where(x => x.CreatedAt.Date >= dateFrom);
            }
            if (dateTo != null)
            {
                orders = orders.Where(x => x.CreatedAt.Date <= dateTo);
            }
            if (customerId != 0)
            {
                orders = orders.Where(x => x.CustomerId == customerId);
            }
            if (storeId != 0)
            {
                orders = orders.Where(x => x.StoreId == storeId);
            }
            if (statusId != 0)
            {
                orders = orders.Where(x => x.OrderStatusId == statusId);
            }
            return await orders.Include(c => c.Customer).Include(c=>c.Store).Include(c => c.OrderStatus).Include(c => c.Currency).Include(c => c.DeliveryTime)
                .Take(100).OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
        public async Task<List<Order>> GetAllCansalOrders(bool trackChanges)
       => await FindByCondition(c => c.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderCanceled, trackChanges).ToListAsync();
        public async Task<List<Order>> GetAllPandingOrders(bool trackChanges)
        => await FindByCondition(c => c.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderPending, trackChanges).ToListAsync();
        public async Task<List<Order>> GetAllCompleteOrders(bool trackChanges)
      => await FindByCondition(c => c.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderCompleted, trackChanges).ToListAsync();
    }
   
}
