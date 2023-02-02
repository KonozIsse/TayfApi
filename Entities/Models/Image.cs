namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Entities.Models.Enums;
    public class Image : BaseEntity
    {
        [Required]
        [StringLength(191)]
        public string Name { get; set; }
        public ImageCategory Category { get; set; }
        [ForeignKey(nameof(Vender))]
        public int? VendId { get; set; }
        public User Vender { get; set; }
        //[ForeignKey(nameof(Admin))]
        //public int? AdminId { get; set; }
        //public User Admin { get; set; }
        public List<ImageSetting> ImageSettings { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
