using Entities.Models.Enums;
using System;
using BusinessLogic;
using Entities.Models;
using Entities.DataTransferObjects;
using AutoMapper;
using Contracts;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net.Security;
using System.Net;
using Microsoft.Extensions.Configuration;
using Image = Entities.Models.Image;
using Entities.RequestFeatures;
using System.Reflection;
using Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using Entities.Exception;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;

namespace BusinessLogic.ApiClasses
{
    public class ImageBL
    {
        protected readonly IConfiguration Configuration;
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;  
        protected readonly ImageUploadServices _imageUploadServices;
        private readonly LocService _locService;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public ImageBL(IRepositoryManager repositoryManager, IMapper mapper, ImageUploadServices imageUploadServices, LocService locService, IConfiguration configuration , IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageUploadServices = imageUploadServices;
            _locService = locService;
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BussnessResultModel> AddImageCustomer(int CustomerId, IFormFile file)
        {
            var customer = await _repositoryManager.User.GetUserId(CustomerId, true);
            if(customer == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"),false);
            }
            var oldLink = customer.Avater;
            var upload = _imageUploadServices.Upload(file, "\\media_files\\avatars\\");
            if(upload == "-1")
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            customer.Avater = upload;

            await _repositoryManager.SaveAsync();
            if (!string.IsNullOrEmpty(oldLink))
            {
                _imageUploadServices.DeleteImage("/media_files/avatars/" + oldLink);
            }
            return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successSave"));
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
                var name =  _imageUploadServices.Upload(file, "\\media_files\\original\\");

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
                                _imageUploadServices.UploadBase64(base64Image, "\\media_files\\thumb\\", name);
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
                                _imageUploadServices.UploadBase64(base64Image, "\\media_files\\medium\\",  name);
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
                                _imageUploadServices.UploadBase64(base64Image, "\\media_files\\large\\", name);
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
        public async Task<BussnessResultModel> DeleteImageIds(List<int> ids)
        {
            foreach (var id in ids)
            {
                var image =  _repositoryManager.Image.GetImage(id, true);
                if (image != null)
                {
                    var sliders = await _repositoryManager.Slider.GetSlideImageId(id, false);
                    if(sliders != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var service = await _repositoryManager.Services.GetServiceImageId(id, false);
                    if (service != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var banner = await _repositoryManager.Banner.GetBannerImage(id, false);
                    if (banner != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var products = await _repositoryManager.ImageProduct.GetAllProductsImageId(id, false);
                    if (products != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var stores = await _repositoryManager.User.GetStoresImage(id, false);
                    if (stores != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var categories = await _repositoryManager.Categories.GetAllCategoriesImageId(id, false);
                    if (categories != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var languages = await _repositoryManager.Language.GetListLanguageImage(id, false);
                    if (languages != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var countries = await _repositoryManager.Country.GetCountriesImage(id);
                    if (countries != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var blogs = await _repositoryManager.News.GetBlogsImage(id);
                    if (blogs != null)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteImage"), false);
                    }
                    var settings = await _repositoryManager.ImageSetting.GetImageSettings(id);
                    foreach (var setting in settings)
                    {
                        _repositoryManager.ImageSetting.DeleteImageSetting(setting);
                        _imageUploadServices.DeleteImage("/media_files" + setting.Path);
                    }
                    _repositoryManager.Image.DeleteImage(image);
                }
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(ids, _locService.GetLocalizedStringValue("successDelete"));
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
            var image = _repositoryManager.Image.GetImage(itemFromDB.ImgId, true);
            string folder = ""; string path = "";
            if (update.ImageType == ImageType.THUMBNAIL)
            {
                folder = "\\media_files\\thumb\\";
                path = "thumb";
            }
            else if (update.ImageType == ImageType.MEDIUM)
            {
                folder = "\\media_files\\medium\\";
                path = "medium";
            }
            else if (update.ImageType == ImageType.LARGE) 
            {
                folder = "\\media_files\\large\\";
                path = "large"; 
            }
            else { 
                folder = "\\media_files\\original\\";
                path = "original";
            }
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

                var Res = client.GetAsync(baseUrl + "/media_files/" + itemFromDB.Path);

                //Checking the response is successful or not which is sent using HttpClient  
                var result = Res.Result;
                if (result.IsSuccessStatusCode)
                {
                    var extention = itemFromDB.Path.Split('.')[1];
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
                                name2 =  _imageUploadServices.UploadBase64(base64Image, folder ,name2);
                                //delete previous file
                                _imageUploadServices.DeleteImage("/media_files" + itemFromDB.Path);
                            }
                        }

                    }
                    image.Name = name2;
                    itemFromDB.Path = "/" + path + "/" + name2;
                    _mapper.Map(update, itemFromDB);
                    await _repositoryManager.SaveAsync();
                }
            }
            return new BussnessResultModel(itemFromDB);
        }
        public string GetImageMedium(int imageId)
        {
            var image = _repositoryManager.Image.GetImage(imageId, false,true);
            if (image != null)
            {
                var imageMedium = _repositoryManager.ImageSetting.GetByType(image.Id, ImageType.MEDIUM);
                if (imageMedium != null)
                {
                    var dto = _mapper.Map<ImageSettingDto>(imageMedium);
                    image.Name = dto.Path;
                }
                return image.Name;
            }
            else
            {
                return " ";
            }
        }
        public string GetImageThumbnail(int imageId)
        {
            var image = _repositoryManager.Image.GetImage(imageId, false,true);
            if (image != null)
            {
                var imageThumbnail = _repositoryManager.ImageSetting.GetByType(image.Id, ImageType.THUMBNAIL);
                if (imageThumbnail != null)
                {
                    var dto = _mapper.Map<ImageSettingDto>(imageThumbnail);
                    image.Name = dto.Path;
                }
                return image.Name;
            }
            else
            {
                return " ";
            }
        }
        public string GetImageOriginal(int imageId)
        {
            var image = _repositoryManager.Image.GetImage(imageId, false, true);
            if (image != null)
            {
                var imageOriginal = _repositoryManager.ImageSetting.GetByType(image.Id, ImageType.ACTUAL);
                if (imageOriginal != null)
                {
                    var dto = _mapper.Map<ImageSettingDto>(imageOriginal);
                    image.Name = dto.Path;
                }
                return image.Name;
            }
            else
            {
                return " ";
            }
        }
        public async Task<PagedList<ImageDto>> GetImages(ImageCategory? category , int userId, PostsParameters postsParameters)
        {
            var images =  await _repositoryManager.Image.GetImages(category);
            var user = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (user.UserType == UserType.Store)
            {
                images =  await _repositoryManager.Image.GetImagesVendor(userId, category);
            }
            var imagesDto =  _mapper.Map<List<ImageDto>>(images);
            return PagedList<ImageDto>.ToPagedList(imagesDto, postsParameters.PageNumber, postsParameters.PageSize); 
        }
        public async Task<List<ImageDto>> GetAllImages()
        {
            var images = await _repositoryManager.Image.GetAllImages();
            var imagesDto = _mapper.Map<List<ImageDto>>(images);
            return imagesDto ;
        }
        public async Task<List<ImageSettingDto>> GetImageSettingImg(int imgId)
        {
            var images =  await _repositoryManager.ImageSetting.GetImageSettings(imgId);
            var imagesDto = _mapper.Map<List<ImageSettingDto>>(images);
            return imagesDto;
        }
        public async Task<List<ImageSettingDto>> GetAllImageSettingOriginal(int userId)
        {
            var images = await _repositoryManager.ImageSetting.GetImageSettingOriginal();
            if (userId != 0)
            {
                images = images.Where(s=>s.Image.VendId== userId);
            }
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
        //------------------------------
        public async Task<List<ProductImagesDto>> GetAllImagesToProduct(int productId)
        {
            var images = await _repositoryManager.ImageProduct.GetAllImagesProductId(productId, false, true);
            var listImages = images.Select(image => new ProductImagesDto
            {
                Image = GetImageOriginal(image.ImageId),
            }).ToList();
            return listImages;
        }
        public async Task<BussnessResultModel> EditProductImage(int id, int imageId)
        {
            var image = await _repositoryManager.ImageProduct.GetImageProductId(id, true);
            if (image != null)
            {
                image.ImageId = imageId;
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(image);
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteImageProduct(int id)
        {
            var image = await _repositoryManager.ImageProduct.GetImageProductId(id, false);
            if (image == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.ImageProduct.DeleteImageProduct(image);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(image, _locService.GetLocalizedStringValue("successDelete"));
        }
    }
}
