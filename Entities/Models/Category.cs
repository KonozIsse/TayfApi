namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } 
        public string CategoryNameAr { get; set; }
        public int MainCategoryId { get; set; }
       
        [ForeignKey(nameof(Image))]
        public int ImgId { get; set; }
        public Image Image { get; set; }
        public List<ProductCategory> ProductCategories { get; set; } 
        public List<User> Stores { get; set; }
    }
}
