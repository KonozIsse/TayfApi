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
using System.Runtime.InteropServices;
using AutoMapper;
using Contracts;

namespace BusinessLogic.ApiClasses
{
    public class ImageBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;  
        protected readonly ImageUploadServices _imageUploadServices;
        string url;

        public ImageBL(IRepositoryManager repositoryManager, IMapper mapper, ImageUploadServices imageUploadServices)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageUploadServices = imageUploadServices;
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
                ImageUploadServices.DeleteImage(oldLink);
            }
        }
        public async Task<int> AddImage (CreateImageDto imageDto)
        {
            var image = _mapper.Map<Image>(imageDto);
            var fileName = imageDto.Name;
            var pic = _imageUploadServices.Upload(fileName);
            image.Name = pic;
            _repositoryManager.Image.AddImage(image);
            try
            {
                await _repositoryManager.SaveAsync();
            }
            catch { }
            return image.Id;
        }
        public async Task AddImageSetting(CreateImageSettingDto settingDto)
        {
            var imageSetting = _mapper.Map<ImageSetting>(settingDto);
            _repositoryManager.ImageSetting.AddImageSetting(imageSetting);
            await _repositoryManager.SaveAsync();
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

    }
}
