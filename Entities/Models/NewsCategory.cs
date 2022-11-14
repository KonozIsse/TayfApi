namespace Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class NewsCategory : BaseEntity
    {
        public string CategoryName { get; set; }
        public int MainCategoryId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        [ForeignKey(nameof(Image))]
        public int? ImgId { get; set; }
        public Image Image { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }
        public List<News> News { get; set; }
    }
}
