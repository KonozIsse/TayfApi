using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class OrderStatusDto
    {
        public int Id { get; set; }
        public string StatusName { get; set; } 
        public string Option { get; set; }
    }
    public class UpdateOrderStatusDto
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
    }
}
