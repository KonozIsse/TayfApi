namespace Entities.Models
{
    using System.Collections.Generic;
    public class TaxClass : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<TaxRate> TaxRates { get; set; }
    }
}
