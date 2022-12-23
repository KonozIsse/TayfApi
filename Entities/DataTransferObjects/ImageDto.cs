using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Entities.DataTransferObjects
{
    public class ImageDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Name { get; set; }
        public ImageCategory Category { get; set; }
        public int? ProductId { get; set; }
        public int? VendId { get; set; }
        public int AdminId { get; set; }
        public List<ImageSettingDto> ImageSettings { get; set; }
    } 
    public class CreateImageDto
    {
        public List<IFormFile> files { get; set; }
        public Status IsStatus { get; set; }
        public ImageCategory Category { get; set; }
        public int? ProductId { get; set; }
        public int? VendId { get; set; }
        public int? AdminId { get; set; }
        public List<CreateImageSettingDto> ImageSettings { get; set; }
    } 
    public class ImageSettingDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public ImageType ImageType { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ImgId { get; set; }
    }
    public class CreateImageSettingDto 
    {
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ImgId { get; set; }
        public ImageType ImageType { get; set; }
       
    } 
    public class UpdateImageSettingDto : CreateImageSettingDto 
    {
        public int Id { get; set; }
       
    }
    public class AvaterDto
    {
        public IFormFile Avater { get; set; }
        public int CustomerId { get; set; }
    }
}
