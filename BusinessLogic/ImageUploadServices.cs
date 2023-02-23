using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using BusinessLogic.ApiClasses;
using Contracts;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public  class ImageUploadServices
    {
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly ILoggerManager _logger;
        protected readonly  IConfiguration Configuration;
        public  ImageUploadServices (IWebHostEnvironment webHostEnvironment , ILoggerManager logger, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            Configuration = configuration;
        }

        public string Upload(IFormFile obj)
        {
            if (obj?.Length > 0)
            {
                try
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(obj.FileName);
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\img\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\img\\");
                    }
                    using (var stream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\img\\" + fileName))
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
                return null;
            }
        }
        public string UploadBase64(string base64Image, string fileName)
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

                fileName = Guid.NewGuid() + Path.GetExtension(fileName);

                System.IO.File.WriteAllBytes(_webHostEnvironment.WebRootPath + "\\img\\" + fileName, binData);
                return fileName;
            }
            catch
            {
                return "-1";
            }
        }
        public  bool DeleteImage(string link)
        {
            try
            {
                System.IO.File.Delete(link);
                return true;
            }
            catch (Exception)
            {
               return false;
            }
        }

    }
}