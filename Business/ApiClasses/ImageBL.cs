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
        string url;

        public ImageBL(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task UpdateAvatarCustomer (int customerId, string avatar)
        {
            var customer = await _repositoryManager.User.GetCustomerId(customerId , true);
            var oldLink = customer.Avater;
            customer.Avater = avatar;
            await _repositoryManager.SaveAsync();
            if (!string.IsNullOrEmpty(oldLink))
            {
                ImageUploadServices.DeleteImage(oldLink);
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
        public async Task AddImageSetting(ImageSettingDto settingDto)
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
        public List<String> GetListImagesProductId (int productId)
        {
            List<String> listImages = new List<String>();
            var images =  _repositoryManager.Image.GetProductImages(productId);
            if (images != null)
            {
                images.ForEach( async x1 => listImages.Add(await GetImageOriginal(x1.Id.ToString())));
            }
            return listImages;
        }

    }
}
