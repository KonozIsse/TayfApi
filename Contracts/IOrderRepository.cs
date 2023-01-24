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
        Task<Order> GetOrderId(int id, bool trackChanges);
        Task<List<Order>> GetAllOrders(string search);
        Task<List<Order>> GetAllSalesOrders(string search, DateTime? dateFrom, DateTime? dateTo);
        Task<List<Order>> GetsAllTransactionOrders();
        Task<List<Order>> GetOrdersToCustomer(int customerId);
        Task<List<Order>> GetOrdersToStore(int storeId);
        Task<Order> GetCustomerNewOlderByStore(int vendorId, int customerId);
        void CreateOrder(Order order);
        void DeleteOrder(Order order) ;
    }
   
}
