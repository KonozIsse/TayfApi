namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Address : BaseEntity
    {
        public string AddressTitle { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Street { get; set; }
        public string Post_Code { get; set; }
        public string CityName { get; set; }
        public string Flat { get; set; }
        [Required]
        [ForeignKey(nameof(Zone))]
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        [Required]
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public  Country Country { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public  User User { get; set; }
    }
}
