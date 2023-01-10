namespace Entities.Models
{
    using Entities.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Sliders : BaseEntity
    {
        [Required]
        [StringLength(64)]
        public string Title { get; set; }
        public string Decription { get; set; }
        public string TitleAr { get; set; }
        public string DecriptionAr { get; set; }
        public string Url { get; set; }
        public SlidersImageType Type { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }
        [ForeignKey(nameof(Image))]
        public int ImgId { get; set; }
        public Image Image { get; set; }
    }
}
