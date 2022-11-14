using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Unit : BaseEntity
    {
        public string UnitName { get; set; }

        [ForeignKey(nameof(Store))]
        public int? StoreId { get; set; }
        public User Store { get; set; }
    }
}
