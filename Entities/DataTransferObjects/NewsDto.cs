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
        public DateTime AddDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public short IsFeature { get; set; }
        public int? IsViewed { get; set; }
        public int? ImageId { get; set; }
        public int CountComment { get; set; }
        public int? NewsCategoryId { get; set; }
    }
    public class CreateNewsDto
    {
        public string Title { get; set; }
        public string Decription { get; set; }
        public string Url { get; set; }
        public DateTime AddDate { get; set; }
        public short IsFeature { get; set; }
        public int? IsViewed { get; set; }
        public int? NewsCategoryId { get; set; }
    }
    public class UpdateNewsDto : CreateNewsDto
    {
        public DateTime? LastUpdateDate { get; set; }
        public int CountComment { get; set; }
    } 
    public class CreateCommentsDto
    {
        public string Text { get; set; }
    }
    public class CommentsDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public int NewsId { get; set; }
    }
}
