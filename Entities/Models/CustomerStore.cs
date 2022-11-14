using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CustomerStore : BaseEntity
    {
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }
        public User Store { get; set; }
    }
}
