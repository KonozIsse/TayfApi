namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Country : BaseEntity
    {
        public string CountryName { get; set; }
        public string CountryNameAr { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
        public int? MobileCode { get; set; }
        [ForeignKey(nameof(Image))]
        public int? ImgId { get; set; }
        public Image Image { get; set; }
        public  List<Address> Address { get; set; }
        public  List<Zone> Zones { get; set; }
    }
}
