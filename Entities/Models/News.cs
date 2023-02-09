namespace Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class News : BaseEntity
    {
        [Required]
        [StringLength(64)]
        public string Title { get; set; }
        public string Decription { get; set; }
        public string TitleAr { get; set; }
        public string DecriptionAr { get; set; }
        public string Url { get; set; }
        public short IsFeature { get; set; }
        public int? IsViewed { get; set; }
        [ForeignKey(nameof(NewsCategory))]
        public int? NewsCategoryId { get; set; }
        public NewsCategory NewsCategory { get; set; }

        [ForeignKey(nameof(Image))]
        public int? ImgId { get; set; }
        public Image Image { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }
        public List<CommentNews> Comments { get; set; }
    }
}
