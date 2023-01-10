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
        Task<Order> GetLastOrderCustomer(int customerId, bool trackChanges);
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> GetsAllTransactionOrders();
        Task<List<Order>> GetOrdersToCustomer(int customerId);
        Task<List<Order>> GetOrdersToStore(int storeId);
        Task<Order> GetOrderDetailsByCustomer(int id, int customerId);
        Task<Order> GetByHashedCsp(string csp);
        Task<List<Order>> OrdersByProductId(int productId);
        Task<Order> GetCustomerNewOlderByStore(int vendorId, int customerId);
        Task<List<Order>> Get15OrdersVendor(int vendorId);
        Task<List<Order>> GetPendingOrders();
        Task<List<Order>> GetPendOrdersByStore(int vendorId);
        Task<List<Order>> GetCompleteOrders();
        Task<List<Order>> GetCompleteOrdersByStore(int vendorId);
        Task<List<Order>> GetCancelOrders();
        Task<List<Order>> GetCancelOrdersByStore(int vendorId);
        Task<List<Order>> GetPendingOrdersByCustomer(int customerId);
        Task<List<Order>> GetCompletedOrdersByCustomer(int customerId);
        Task<Order> OrderIdByPandingStatus(int id);
        Task<Order> GetOrderIdAndStatusId(int id, int status);
        int GetOrdersByVendor(int vendorId);
        bool GetOrderByStore(int storeId);
        int GetAllOrdersCount() ;
        void CreateOrder(Order order);
        void DeleteOrder(Order order) ;
    }
   
}
