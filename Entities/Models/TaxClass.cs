namespace Entities.Models
{
    using System.Collections.Generic;
    public class TaxClass : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public string TitleAr { get; set; }
        public string DescriptionAr { get; set; }
        public int? StoreId { get; set; }
        public List<TaxRate> TaxRates { get; set; }
    }
}
