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
                    //string _filesRootPath = Configuration.GetSection("filesRootPath").Value;
                    // var fullFileName = Path.Combine("/" + _filesRootPath + "/" + fileName);
                    var fileName = Guid.NewGuid() + Path.GetExtension(obj.FileName);
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\img\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\img\\");
                    }
                    using (var stream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\img\\" + fileName))
                    {
                        obj.CopyTo(stream);
                        stream.Flush();
                        return  fileName;
                    }
                }
                catch (Exception ex)
                {
                    //_logger.Error("Exception Occured while uploading to Amazon S3 : " + ex, ex);
                    throw;
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
                var fullFileName = Path.Combine("/img/" + fileName);
                System.IO.File.WriteAllBytes(fullFileName, binData);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occured while uploading to Amazon S3 : " + ex, ex);
                return null;
            }
        }
        public Stream ResizeImageFile(Stream imageFileStream, int targetSize)
        {
            byte[] imageFile = StreamToByteArray(imageFileStream);
            using (System.Drawing.Image oldImage = System.Drawing.Image.FromStream(new MemoryStream(imageFile)))
            {
                Size newSize = CalculateDimensions(oldImage.Size, targetSize);
                using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics canvas =  Graphics.FromImage(newImage))
                    {
                        canvas.SmoothingMode = SmoothingMode.AntiAlias;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
                        MemoryStream m = new MemoryStream();
                        newImage.Save(m, ImageFormat.Jpeg);
                        return new MemoryStream(m.GetBuffer());
                    }
                }
            }
        }
        public  byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Size CalculateDimensions(Size oldSize, int targetSize)
        {
            Size newSize = new Size();
            if (oldSize.Height > oldSize.Width)
            {
                newSize.Width = (int)(oldSize.Width * (targetSize / (float)oldSize.Height));
                newSize.Height = targetSize;
            }
            else
            {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * (targetSize / (float)oldSize.Width));
            }
            return newSize;
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
                //_logger.Error("Exception Occured while uploading to Amazon S3 : " + ex, ex);
               // return false;
               return false;
            }
        }

    }
}