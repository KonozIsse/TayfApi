namespace Entities.Models
{
    using Entities.Models.Enums;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ImageSetting : BaseEntity
    {
        public ImageType ImageType { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        [ForeignKey(nameof(Image))]
        public int ImgId { get; set; }
        public Image Image { get; set; }
    }
}
