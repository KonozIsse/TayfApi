using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderProductsRepository
    {
        Task<List<OrderProduct>> GetOrderProducts(int productId, int orderId);
        Task<OrderProduct> GetOrderProductsId(int productId, int orderId);
        Task<List<OrderProduct>> GetAllProductsToOrderId(int orderId);
        void DeleteOrderProduct(OrderProduct orderProducts);
    }  

}
