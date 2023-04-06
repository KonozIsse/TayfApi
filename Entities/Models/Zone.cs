namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Zone : BaseEntity
    {
        [Required]
        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<TaxRate> TaxRates { get; set; }
    }
}
