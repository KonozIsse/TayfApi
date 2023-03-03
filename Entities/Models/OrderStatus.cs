namespace Entities.Models
{
    using Entities.Models.Enums;
    using System.Collections.Generic;
    public class OrderStatus : BaseEntity
    {
        public string StatusName { get; set; }
        public string StatusNameAr { get; set; }
        public OrderStatusEnum OrderStatusEnum { get; set; } 
        public List<Order> Orders { get; set; }
        
    }
}
