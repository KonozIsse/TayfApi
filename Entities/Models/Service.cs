namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class Service : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionAr { get; set; }
        [ForeignKey(nameof(Image))]
        public int? ImgId { get; set; }
        public Image Image { get; set; }
    }
}
