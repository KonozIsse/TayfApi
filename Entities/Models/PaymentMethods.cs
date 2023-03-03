namespace Entities.Models
{
    using Entities.Models.Enums;
    using System.Collections.Generic;
    public class PaymentMethods : BaseEntity
    {
        public string PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int StoreId { get; set; }
        public List<PaymentMethodDetail> PaymentMethodDetails { get; set; }
    }
}
