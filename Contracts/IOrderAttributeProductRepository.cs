using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderAttributeProductRepository
    {
        Task<OrderAttributProduct> GetOrderAttributesProduct(int orderProductId, bool trackChanges);
        Task<IEnumerable<OrderAttributProduct>> GetAllOrderAttributesProducts(int orderProductId, bool trackChanges);
        Task<IEnumerable<OrderAttributProduct>> GetAttributesOrderProduct(int orderId, int productId);
        void DeleteOrderAttributProduct(OrderAttributProduct orderAttributProduct);
    }
}
