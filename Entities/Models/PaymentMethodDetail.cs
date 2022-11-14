namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class PaymentMethodDetail : BaseEntity
    {
        [Required]
        [StringLength(191)]
        public string Key { get; set; }
        [Required]
        [StringLength(191)]
        public string Value { get; set; }
        [ForeignKey(nameof(PaymentMethods))]
        public int PaymentMethodsId { get; set; }
        public PaymentMethods PaymentMethods { get; set; }
    }
}
