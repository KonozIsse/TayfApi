using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class DeliveryTimeDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Time { get; set; }
    }
}
