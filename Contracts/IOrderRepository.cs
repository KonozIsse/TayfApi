using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrders(bool trackChanges);
        Task<Order> GetOrderId(int id, bool trackChanges, bool included = false);
        Task<List<Order>> GetAllOrders(string search, int customerId, int storeId, int statusId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<Order>> GetsAllTransactionOrders();
        Task<List<Order>> GetOrdersToCustomer(int customerId);
        Task<List<Order>> GetOrdersToStore(int storeId);
        Task<Order> GetCustomerNewOlderByStore(int vendorId, int customerId);
        void CreateOrder(Order order);
        void DeleteOrder(Order order) ;
        Task<List<Order>> GetAllCansalOrders(bool trackChanges);
        Task<List<Order>> GetAllPandingOrders(bool trackChanges);
        Task<List<Order>> GetAllCompleteOrders(bool trackChanges);
    }
   
}
