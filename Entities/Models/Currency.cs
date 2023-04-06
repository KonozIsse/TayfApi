namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Currency : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }  
        public string NameAr { get; set; }
        public string Position { get; set; }
        public string Symbol { get; set; }
        public string DecimalPlaces { get; set; }
        public double? Value { get; set; }
        public int IsDefault { get; set; }
    }
}
