namespace Entities.Models
{
    using System.Collections.Generic;
    public class OrderStatus : BaseEntity
    {
        public string StatusName { get; set; }
        public string StatusNameAr { get; set; }
        public int? Option { get; set; }
        public List<Order> Orders { get; set; }
    }
}
