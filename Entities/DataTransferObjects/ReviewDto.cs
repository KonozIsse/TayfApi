using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ReviewDto
    {
        public int  Id { get; set; }
        public string Text { get; set; }
        public double Rating { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; } 
        public string CustomerName { get; set; } 
        public string CustomerImage { get; set; }
    }
    public class CreateReviewDto
    {
        public string Text { get; set; }
        public double Rating { get; set; }
        public int CustomerId { get; set; }
    } 
    public class UpdateReviewDto : CreateReviewDto
    {
    }
}
