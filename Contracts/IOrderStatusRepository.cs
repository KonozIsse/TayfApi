using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderStatusRepository
    {
        Task<OrderStatus> GetOrderStatusById(int id, bool trackChanges);
        Task<List<OrderStatus>> GetOrderStatusesList(bool trackChanges);
        Task<OrderStatus> GetOrderStatusEnum(OrderStatusEnum orderStatus, bool trackChanges);
    } 
  
}
