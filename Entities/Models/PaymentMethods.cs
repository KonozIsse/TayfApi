namespace Entities.Models
{
    using System.Collections.Generic;
    public class PaymentMethods : BaseEntity
    {
        public string PaymentMethod { get; set; }
        public short Environment { get; set; }
        public int StoreId { get; set; }
        public List<PaymentMethodDetail> PaymentMethodDetails { get; set; }
    }
}
