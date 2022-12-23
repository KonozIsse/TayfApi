using Entities.Models.Enums;
using System;
using BusinessLogic;
using Entities.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using AutoMapper;
using Contracts;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Net;
using System.Net.Http;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Image = Entities.Models.Image;
using Org.BouncyCastle.Crypto;

namespace BusinessLogic.ApiClasses
{
    public class ImageBL
    {
        protected static readonly IConfiguration Configuration;
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;  
        protected readonly ImageUploadServices _imageUploadServices;
        private readonly LocService _locService;
        string url;
        protected static readonly string urlImg = Configuration.GetSection("ImagesUrl").ToString();

        public ImageBL(IRepositoryManager repositoryManager, IMapper mapper, ImageUploadServices imageUploadServices, LocService locService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageUploadServices = imageUploadServices;
            _locService = locService;
        }
        public List<string> GetImagesCategoriesConstants()
        {
            return Enum.GetNames(typeof(ImageCategory)).ToList();
        }
        public async Task UpdateAvatarCustomer (AvaterDto avaterDto)
        {
            var customer = await _repositoryManager.User.GetCustomerId(avaterDto.CustomerId, true);
            var fileName = avaterDto.Avater;
            var pic = _imageUploadServices.Upload(fileName);
            customer.Avater = pic;
            var oldLink = customer.Avater;
            await _repositoryManager.SaveAsync();
            if (!string.IsNullOrEmpty(oldLink))
            {
                _imageUploadServices.DeleteImage(oldLink);
            }
        }
        public async Task<int> AddImage (CreateImageDto imageDto)
        {
            var image = _mapper.Map<Image>(imageDto);
            _repositoryManager.Image.AddImage(image);
            try
            {
                await _repositoryManager.SaveAsync();
            }
            catch { }
            return image.Id;
        }
        public async Task<BussnessResultModel> AddImage1 (CreateImageDto create)
        {
            var image = _mapper.Map<Image>(create);
            foreach (var file in create.files)
            {
                //check size of image category
                System.Drawing.Image bitfile = System.Drawing.Image.FromStream(file.OpenReadStream());

                if ((create.Category == ImageCategory.Banners && (bitfile.Width != 1140 || bitfile.Height != 240)) ||
                   (create.Category == ImageCategory.Categories && (bitfile.Width != 250 || bitfile.Height != 200)) ||
                   (create.Category == ImageCategory.Sliders && (bitfile.Width != 1400 || bitfile.Height != 600)) ||
                   (create.Category == ImageCategory.Products && (bitfile.Width != 1000 || bitfile.Height != 600)) ||
                   (create.Category == ImageCategory.Services && (bitfile.Width != 200 || bitfile.Height != 200)) ||
                   (create.Category == ImageCategory.Blogs && (bitfile.Width != 250 || bitfile.Height != 250)) ||
                   (create.Category == ImageCategory.Stores && (bitfile.Width != 250 || bitfile.Height != 200)))
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
                }
                Bitmap sourceImage = new Bitmap(bitfile);
                var name = _imageUploadServices.Upload(file/*, "original/" + name*/);
                image.Name = name;
                image.AdminId = 1;

                var set = await  _repositoryManager.Setting.GetAllSettings(false);
                var thh = Convert.ToInt32(set.Where(x => x.Key == "Thumbnail_height").FirstOrDefault().Value);
                var thw = Convert.ToInt32(set.Where(x => x.Key == "Thumbnail_width").FirstOrDefault().Value);
                var mh = Convert.ToInt32(set.Where(x => x.Key == "Medium_height").FirstOrDefault().Value);
                var mw = Convert.ToInt32(set.Where(x => x.Key == "Medium_width").FirstOrDefault().Value);
                var lh = Convert.ToInt32(set.Where(x => x.Key == "Large_height").FirstOrDefault().Value);
                var lw = Convert.ToInt32(set.Where(x => x.Key == "Large_width").FirstOrDefault().Value);

                //var t = _cpBl.SaveThumImg(d);

                _repositoryManager.Image.AddImage(image);
                await _repositoryManager.SaveAsync();

                var imgCat = new CreateImageSettingDto
                {
                    Width = bitfile.Width,
                    Height = bitfile.Height,
                    Path = "/original/" + name,
                    ImageType = ImageType.ACTUAL,
                    ImgId = image.Id,
                };
                await AddImageSetting(imgCat);
                try
                {
                    //For THUMB
                    using (var stream = new MemoryStream())
                    {
                        using (Bitmap objBitmap = new Bitmap(thw, thh))
                        {
                            objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                            using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                            {
                                // Set the graphic format for better result cropping   
                                objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                                objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                objGraphics.DrawImage(sourceImage, 0, 0, thw, thh);

                                // Save the file path, note we use png format to support png file   
                                objBitmap.Save(stream, ImageFormat.Png);
                                string base64Image = Convert.ToBase64String(stream.ToArray());
                                _imageUploadServices.UploadBase64(base64Image, "thumb/" + name);
                            }
                        }

                        //For Medium
                        using (Bitmap objBitmap = new Bitmap(mw, mh))
                        {
                            objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                            using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                            {
                                // Set the graphic format for better result cropping   
                                objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                                objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                objGraphics.DrawImage(sourceImage, 0, 0, mw, mh);

                                // Save the file path, note we use png format to support png file   
                                objBitmap.Save(stream, ImageFormat.Png);
                                string base64Image = Convert.ToBase64String(stream.ToArray());
                                _imageUploadServices.UploadBase64(base64Image, "medium/" + name);
                            }
                        }
                        //For Large
                        using (Bitmap objBitmap = new Bitmap(lw, lh))
                        {
                            objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                            using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                            {
                                // Set the graphic format for better result cropping   
                                objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                                objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                objGraphics.DrawImage(sourceImage, 0, 0, lw, lh);

                                // Save the file path, note we use png format to support png file   
                                objBitmap.Save(stream, ImageFormat.Png);
                                string base64Image = Convert.ToBase64String(stream.ToArray());
                                _imageUploadServices.UploadBase64(base64Image, "large/" + name);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                   // Logger.Error("Exception Occured while generate thum Images : " + ex, ex);
                    throw ex;
                }
                //thumb image
                var imgCat2 = new CreateImageSettingDto
                {
                    Width = thw,
                    Height = thh,
                    Path = "/thumb/" + name,
                    ImageType = ImageType.THUMBNAIL,
                    ImgId = image.Id,
                };
                await AddImageSetting(imgCat2);
                //medium image
                var imgCat3 = new CreateImageSettingDto
                {
                    Width = thw,
                    Height = thh,
                    Path = "/medium/" + name,
                    ImageType = ImageType.MEDIUM,
                    ImgId = image.Id,
                };
                await AddImageSetting(imgCat3);
                //large image
                var imgCat4 = new CreateImageSettingDto
                {
                    Width = lw,
                    Height = lh,
                    Path = "/large/" + name,
                    ImageType = ImageType.LARGE,
                    ImgId = image.Id,
                };
                await AddImageSetting(imgCat4);
               
            }
            return new BussnessResultModel(image, _locService.GetLocalizedStringValue("successSave"));
        } 
        public async Task<BussnessResultModel> EditImage(int id , string img)
        {
            var image = await _repositoryManager.Image.GetImage(id, true);
            if(image != null)
            {
                image.Name = img;
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(image);
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink") , false);
            }
        }
        public async Task DeleteImageIds(string Ids)
        {
            char[] delimiters = new char[] { ',' };
            string[] stringArray = Ids.Split(delimiters);
            int[] intArray = Array.ConvertAll(stringArray, s => int.Parse(s));

            foreach (var c in intArray)
            {
                var cats = await _repositoryManager.ImageSetting.GetImageSettings(c);
                foreach (var t in cats)
                {
                    _repositoryManager.ImageSetting.DeleteImageSetting(t);
                    _imageUploadServices.DeleteImage(t.Path);
                }
                var image = await _repositoryManager.Image.GetImage(c, false);
                _repositoryManager.Image.DeleteImage(image);
            }
            await _repositoryManager.SaveAsync();
        } 
        public async Task DeleteImage(int id)
        {
            var image = await _repositoryManager.Image.GetImage(id, false);
            if(image != null)
            {
                var settings = await _repositoryManager.ImageSetting.GetImageSettings(id);
                if(settings != null)
                {
                    foreach (var setting in settings)
                    {
                        _repositoryManager.ImageSetting.DeleteImageSetting(setting);
                        _imageUploadServices.DeleteImage(setting.Path);
                    }

                }
                _repositoryManager.Image.DeleteImage(image);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task AddImageSetting(CreateImageSettingDto settingDto)
        {
            var imageSetting = _mapper.Map<ImageSetting>(settingDto);
            _repositoryManager.ImageSetting.AddImageSetting(imageSetting);
            await _repositoryManager.SaveAsync();
        }  
        public async Task EditImageSetting(UpdateImageSettingDto update)
        {
            var itemFromDB = await _repositoryManager.ImageSetting.GetImageSettingId(update.Id , true);
            string folder = "";
            if (update.ImageType == ImageType.THUMBNAIL) folder = "thumb";
            else if (update.ImageType == ImageType.MEDIUM) folder = "medium";
            else if (update.ImageType == ImageType.LARGE) folder = "large";

            var extention = itemFromDB.Path.Split('.')[1];
            string name = itemFromDB.Path;
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var Res = client.GetAsync(urlImg + name);

                //Checking the response is successful or not which is sent using HttpClient  
                var result = Res.Result;
                if (result.IsSuccessStatusCode)
                {
                    string name2 = Guid.NewGuid() + "." + extention;
                    //Storing the response details recieved from web api   
                    var Response = Res.Result.Content.ReadAsStreamAsync().Result;
                    System.Drawing.Image bitfile = System.Drawing.Image.FromStream(Response);
                    Bitmap sourceImage = new Bitmap(bitfile);

                    using (var stream = new MemoryStream())
                    {
                        using (Bitmap objBitmap = new Bitmap(update.Width, update.Height))
                        {
                            objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                            using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                            {
                                // Set the graphic format for better result cropping   
                                objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                                objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                objGraphics.DrawImage(sourceImage, 0, 0, update.Width, update.Height);

                                // Save the file path, note we use png format to support png file   
                                objBitmap.Save(stream, ImageFormat.Png);
                                string base64Image = Convert.ToBase64String(stream.ToArray());
                                //upload new file
                                _imageUploadServices.UploadBase64(base64Image, folder + "/" + name2);
                                //delete previous file
                                _imageUploadServices.DeleteImage(name);
                            }
                        }

                    }
                    itemFromDB.Path = "/" + folder + "/" + name2;
                    _mapper.Map(update, itemFromDB);
                    await _repositoryManager.SaveAsync();
                }
            }
        }
        public async Task<string> GetImageMedium(string img)
        {
            int imgId = 0;
            string image = "";
            try
            {

                if (!string.IsNullOrEmpty(img))
                {
                    imgId = Convert.ToInt32(img);
                }
            }
            catch { }
            if (imgId != 0)
            {
                var imageMedium = await _repositoryManager.ImageSetting.GetByType(imgId, ImageType.MEDIUM.ToString());

                if (imageMedium != null)
                {
                    image = url + imageMedium.Path;
                }
            }
            return image;
        }
        public async Task<string> GetImageThumbnail(string img)
        {
            int imgId = 0;
            string image = "";
            try
            {

                if (!string.IsNullOrEmpty(img))
                {
                    imgId = Convert.ToInt32(img);
                }
            }
            catch { }
            if (imgId != 0)
            {
                var imageThumbnail = await _repositoryManager.ImageSetting.GetByType(imgId, ImageType.THUMBNAIL.ToString());

                if (imageThumbnail != null)
                {
                    image = url + imageThumbnail.Path;
                }
            }
            return image;
        }
        public async Task<string> GetImageOriginal(string img)
        {
            int imgId = 0;
            string image = "";
            try
            {

                if (!string.IsNullOrEmpty(img))
                {
                    imgId = Convert.ToInt32(img);
                }
            }
            catch { }
            if (imgId != 0)
            {
                var imageOriginal = await _repositoryManager.ImageSetting.GetByType(imgId);

                if (imageOriginal != null)
                {
                    image = imageOriginal.Path;
                }
            }
            return image;
        }
        public async Task<List<string>> GetListImagesProductIdAsync (int productId)
        {
            List<String> listImages = new List<String>();
            var images = await  _repositoryManager.Image.GetProductImages(productId);
            if (images != null)
            {
                foreach(var image in images)
                {
                    listImages.Add(await GetImageOriginal(image.Id.ToString()));
                }
                //images.ForEach( async x1 => listImages.Add(await GetImageOriginal(x1.Id.ToString())));
            }
            return listImages;
        }
        public async Task<IEnumerable<ImageSetting>> GetImagesCategories(int vendorId = 0, string category = "")
        {
            return await _repositoryManager.ImageSetting.GetImagesStoreId(vendorId, category);
        } 
        public async Task<IEnumerable<Image>> GetImages( string category , int vendorId = 0)
        {
            var images =  await _repositoryManager.Image.GetImages(category);
            if (vendorId != 0)
            {
                images =  await _repositoryManager.Image.GetImagesVendor(vendorId, category);
            }
            return images;
        }
        public async Task<IEnumerable<ImageSetting>> GetImageSettingImg(  int imgId)
        {
            var images =  await _repositoryManager.ImageSetting.GetImageSettings(imgId);
            return images;
        } 
        public async Task<ImageSetting> GetImageSetting(int settingId)
        {
            var images =  await _repositoryManager.ImageSetting.GetImageSettingId(settingId , false);
            return images;
        }
        public async Task EditMediaSetting(int thh, int thw, int mh, int mw, int lh, int lw)
        {

            var settingImage = await _repositoryManager.Setting.GetSettingByValue("Thumbnail_height");
            settingImage.Value = thh + "";
            var settingImage1 = await _repositoryManager.Setting.GetSettingByValue("Thumbnail_width");
            settingImage1.Value = thw + "";
            var settingImage2 = await _repositoryManager.Setting.GetSettingByValue("Medium_height");
            settingImage2.Value = mh + "";
            var itemFromDB4 = await _repositoryManager.Setting.GetSettingByValue("Medium_width");
            itemFromDB4.Value = mw + "";
            var itemFromDB5 = await _repositoryManager.Setting.GetSettingByValue("Large_height");
            itemFromDB5.Value = lh + "";
            var itemFromDB6 = await _repositoryManager.Setting.GetSettingByValue("Large_width");
            itemFromDB6.Value = lw + "";

            await _repositoryManager.SaveAsync();
        } 
        public async Task<IEnumerable<Setting>> GetMediaSetting()
        {
            return await _repositoryManager.Setting.GetMediaSetting();
        }
    }
}
