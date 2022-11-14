using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class WishListDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public int? ProductId { get; set; }
        public int CustomerId { get; set; }
    } 
    public class CreateLikeDto
    {
        public int? ProductId { get; set; }
        public int CustomerId { get; set; }
    }
}
