using Contracts;
using Entities.Models;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;

namespace Repository
{
    public class OrderStatusRepository : RepositoryBase<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<OrderStatus> GetOrderStatusById(int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<OrderStatus> GetOrderStatusEnum(OrderStatusEnum orderStatus, bool trackChanges)
         => await FindByCondition(c => c.OrderStatusEnum.Equals(orderStatus), trackChanges).SingleOrDefaultAsync();
        public async Task<List<OrderStatus>> GetOrderStatusesList(bool trackChanges)
        => await FindAll(trackChanges).ToListAsync();
    } 
}
