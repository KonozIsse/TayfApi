using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string PaymentMethod { get; set; }
    }
}
