namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public int MainCategoryId { get; set; }
       
        [ForeignKey(nameof(Images))]
        public int? ImgId { get; set; }
        public Image Images { get; set; }
        public List<Product> Products { get; set; }
        public List<CategoriesStore> CategoriesStores { get; set; }
    }
}
