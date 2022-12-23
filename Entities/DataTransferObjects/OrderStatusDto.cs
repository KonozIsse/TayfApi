using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class OrderStatusDto
    {
    }
    public class UpdateOrderStatusDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> StatusesNames { get; set; }
    }
}
