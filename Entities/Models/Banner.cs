namespace Entities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Banner : BaseEntity
    {
        [Required]
        [StringLength(64)]
        public string Title { get; set; }
        public string TitleAr { get; set; }
        [Required]
        [StringLength(191)]
        public string Url { get; set; }
        [Required]
        [StringLength(250)]
        public string Type { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? EndDate { get; set; }
        [ForeignKey(nameof(Language))]
        public int? LangId { get; set; }
        public Language Language { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }
        [ForeignKey(nameof(Image))]
        public int ImgId { get; set; }
        public Image Image { get; set; }
    }
}
