using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using BusinessLogic.ApiClasses;
using Contracts;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public  class ImageUploadServices
    {
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly ILoggerManager _logger;
        public  ImageUploadServices (IWebHostEnvironment webHostEnvironment , ILoggerManager logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public string Upload(IFormFile obj , string path)
        {
            if (obj?.Length > 0)
            {
                try
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(obj.FileName);
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + path))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + path);
                    }
                    using (var stream = File.Create(_webHostEnvironment.WebRootPath + path + fileName))
                    {
                        obj.CopyTo(stream);
                        stream.Flush();
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    return "-1";
                }
            }
            else
            {
                return "-1";
            }
        }
        public string UploadBase64(string base64Image, string path, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                {
                    return null;
                }
                if (base64Image.StartsWith("data"))
                {
                    var matchGroups = Regex.Match(base64Image, @"^data:((?<type>[\w\/]+))?;base64,(?<data>.+)$").Groups;
                    base64Image = matchGroups["data"].Value;
                }
                var binData = Convert.FromBase64String(base64Image);

               // fileName = Guid.NewGuid() + Path.GetExtension(fileName);

                File.WriteAllBytes(_webHostEnvironment.WebRootPath + path + fileName, binData);
                return fileName;
            }
            catch
            {
                return "-1";
            }
        }
        public bool DeleteImage(string link)
        {
            var file = _webHostEnvironment.WebRootPath + link;
            if (File.Exists(file))
            {
                File.Delete(file);
                return true;
            }
            else 
            { 
                return false;
            }
        }
    }
  
   
}