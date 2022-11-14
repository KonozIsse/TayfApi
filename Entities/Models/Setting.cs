using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }

        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        public User Admin { get; set; }
    }
}
