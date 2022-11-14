namespace Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TaxRate : BaseEntity
    {
        public decimal Tax_Rate { get; set; }
        [ForeignKey(nameof(TaxClass))]
        public int TaxClassId { get; set; }
        public TaxClass TaxClass { get; set; }
        [ForeignKey(nameof(Zone))]
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
    }
}
