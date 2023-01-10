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
    public class NewsDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public short IsFeature { get; set; }
        public int? IsViewed { get; set; }
        public int ImageId { get; set; }
        public int CountComment { get; set; }
        public int? NewsCategoryId { get; set; }
        public List<CommentsDto> Comments{ get; set; }
}
    public class CreateNewsDto
    {
        public string TitleAr { get; set; }
        public string DecriptionAr { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public Status IsStatus { get; set; }
        public int ImageId { get; set; }
    }
    public class UpdateNewsDto : CreateNewsDto
    {
        public int NewsId { get; set; }
    } 
    public class CreateCommentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }
    public class CommentsDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int NewsId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
