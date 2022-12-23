namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Language : BaseEntity
    {
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Direction { get; set; } 
        public short Sort { get; set; }
        public short? IsDefault { get; set; }
        public int? ImgId { get; set; }
        [ForeignKey(nameof(ImgId))]
        public Image Image { get; set; }
    }
}
