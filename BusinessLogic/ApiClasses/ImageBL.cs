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
using System.Net.Security;
using System.Net;
using Microsoft.Extensions.Configuration;
using Image = Entities.Models.Image;
using BussnessResultModel = Entities.Exception.BussnessResultModel;
using Entities.RequestFeatures;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection;
using Entities.ViewModel;

namespace BusinessLogic.ApiClasses
{
    public class ImageBL
    {
        protected readonly IConfiguration Configuration;
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;  
        protected readonly ImageUploadServices _imageUploadServices;
        private readonly LocService _locService;

        public ImageBL(IRepositoryManager repositoryManager, IMapper mapper, ImageUploadServices imageUploadServices, LocService locService, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageUploadServices = imageUploadServices;
            _locService = locService;
            Configuration = configuration;
        }
      
        public async Task UpdateAvatarCustomer (AvaterDto avaterDto)
        {
            var customer = await _repositoryManager.User.GetCustomerId(avaterDto.CustomerId, true);
            var fileName = avaterDto.Avater;
            //var pic = _imageUploadServices.Upload(fileName);
            //customer.Avater = pic;
            var oldLink = customer.Avater;
            await _repositoryManager.SaveAsync();
            if (!string.IsNullOrEmpty(oldLink))
            {
                _imageUploadServices.DeleteImage(oldLink);
            }
        }
       
        public async Task<BussnessResultModel> CreateImages (int userId, CreateImageDto create)
        {
            
            foreach (var file in create.Files)
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
                var image = _mapper.Map<Image>(create);
                Bitmap sourceImage = new Bitmap(bitfile);
                var name =  _imageUploadServices.Upload(file);

                image.Name = name;
                image.IsStatus = Status.Active;
                var user = await _repositoryManager.User.GetActiveUserId(userId, false);
                if(user.UserType == UserType.Admin)
                {
                    image.AdminId = userId;
                }
                else
                {
                    image.VendId = userId;
                }
                var set = await  _repositoryManager.Setting.GetAllSettings(false);
                var thh = Convert.ToInt32(set.Where(x => x.Key == "Thumbnail_height").First().Value);
                var thw = Convert.ToInt32(set.Where(x => x.Key == "Thumbnail_width").First().Value);
                var mh = Convert.ToInt32(set.Where(x => x.Key == "Medium_height").First().Value);
                var mw = Convert.ToInt32(set.Where(x => x.Key == "Medium_width").First().Value);
                var lh = Convert.ToInt32(set.Where(x => x.Key == "Large_height").First().Value);
                var lw = Convert.ToInt32(set.Where(x => x.Key == "Large_width").First().Value);

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
            return new BussnessResultModel(create, _locService.GetLocalizedStringValue("successSave"));
        } 
        public async Task<BussnessResultModel> EditProductImage(int id , int imageId)
        {
            var image = await _repositoryManager.ImageProduct.GetImageProductId(id, true);
            if(image != null)
            {
                image.ImageId = imageId;
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(image);
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink") , false);
            }
        }
        public async Task<BussnessResultModel> DeleteImageIds(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var cats = await _repositoryManager.ImageSetting.GetImageSettings(id);
                    foreach (var t in cats)
                    {
                        _repositoryManager.ImageSetting.DeleteImageSetting(t);
                    }
                    var image = await _repositoryManager.Image.GetImage(id, false);
                    _repositoryManager.Image.DeleteImage(image);
                }
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(ids ,_locService.GetLocalizedStringValue("successDelete"));
            }
            catch
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false); 
            }
            
        } 
        public async Task<BussnessResultModel> DeleteImage(int id)
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
                    }

                }
                image.IsStatus = Status.NotActive;
                _repositoryManager.Image.DeleteImage(image);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(image , _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        public async Task AddImageSetting(CreateImageSettingDto settingDto)
        {
            var imageSetting = _mapper.Map<ImageSetting>(settingDto);
            _repositoryManager.ImageSetting.AddImageSetting(imageSetting);
            await _repositoryManager.SaveAsync();
        }  
        public async Task<BussnessResultModel> EditImageSetting(UpdateImageSettingDto update)
        {
            var itemFromDB = await _repositoryManager.ImageSetting.GetImageSettingId(update.Id , true);
            if(itemFromDB == null)
            {
                return new BussnessResultModel(null , _locService.GetLocalizedStringValue("is null"),false);
            }
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
                //string urlImg = Configuration.GetSection("ImagesUrl").Value;
                var Res = client.GetAsync("\\img\\" + name);

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
            return new BussnessResultModel(itemFromDB);
        }
        public string GetImageMedium(int img)
        {
            string image = "";
           
            if (img != 0)
            {
                var imageMedium = _repositoryManager.ImageSetting.GetByType(img, ImageType.MEDIUM).Result;
                if (imageMedium != null)
                {
                    image = "/img" + imageMedium.Path;
                }
            }
            return image;
        }
        public async Task<string> GetImageThumbnail(int img)
        {
            string image = "";
            if (img != 0)
            {
                var imageThumbnail = await _repositoryManager.ImageSetting.GetByType(img, ImageType.THUMBNAIL);
                if (imageThumbnail != null)
                {
                    image = "/img" + imageThumbnail.Path;
                }
            }
            return image;
        }
        public string GetImageOriginal(int img)
        {
            string image = "";
            if (img != 0)
            {
                var imageOriginal = _repositoryManager.ImageSetting.GetByType(img , ImageType.ACTUAL).Result;
                if (imageOriginal != null)
                {
                    image = "/img" + imageOriginal.Path;
                }
            }
            return image;
        }
        public async Task<List<string>> GetListImagesProductIdAsync (int productId)
        {
            List<String> listImages = new List<String>();
            var images = await  _repositoryManager.ImageProduct.GetAllImagesProduct(productId,false,true);
            if (images != null)
            {
                foreach(var image in images)
                {
                    listImages.Add(GetImageOriginal(image.ImageId));
                }
                //images.ForEach( async x1 => listImages.Add(await GetImageOriginal(x1.Id.ToString())));
            }
            return listImages;
        }
       
        public async Task<PagedList<ImageDto>> GetImages( string category , int? vendorId, PostsParameters postsParameters)
        {
            var images =  await _repositoryManager.Image.GetImages(category);
            if (vendorId != 0)
            {
                images =  await _repositoryManager.Image.GetImagesVendor(vendorId.Value, category);
            }
           var imagesDto =  _mapper.Map<List<ImageDto>>(images);
            return PagedList<ImageDto>.ToPagedList(imagesDto, postsParameters.PageNumber, postsParameters.PageSize); 
        }
        public async Task<List<ImageSettingDto>> GetImageSettingImg(int imgId)
        {
            var images =  await _repositoryManager.ImageSetting.GetImageSettings(imgId);
            var imagesDto = _mapper.Map<List<ImageSettingDto>>(images);
            return imagesDto;
        } 
       
        public async Task<BussnessResultModel> EditMediaSetting(SettingImageVM update)
        {
            PropertyInfo[] properties = update.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var itemDB = await _repositoryManager.Setting.GetSettingByValue(property.Name, true);
                itemDB.Value = property.GetValue(update)?.ToString();
            } 
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(update, _locService.GetLocalizedStringValue("successSave"))??
                 new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"));

        } 
        public async Task<IEnumerable<SettingDto>> GetMediaSetting()
        {
            var settings=  await _repositoryManager.Setting.GetMediaSetting();
            var settingsDto = _mapper.Map<List<SettingDto>>(settings);
            return settingsDto;
        }
    }
}
