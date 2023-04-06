using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public Image GetImage(int id, bool trackChanges,bool included = false)
        {
            var image = FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges);
            if (included == true)
            {
                image = image.Include(c => c.ProductImages);
            }
            return image.FirstOrDefault();
        }
        public Image GetImageId(int id)
        =>  FindByCondition(c => c.Id == id , false).FirstOrDefault();
        public async Task<List<Image>> GetAllImages()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).OrderByDescending(c => c.CreatedAt).ToListAsync();
        public async Task<List<Image>> GetImages(ImageCategory? category)
        {
            var images = FindByCondition(c => c.IsStatus == Status.Active,false);
            if(category != null)
            {
                images = images.Where(c => c.Category  == category);
            }
            return await images.OrderByDescending(c => c.CreatedAt).Include(c=>c.ImageSettings).ToListAsync();
        }
        public async Task<List<Image>> GetImagesVendor(int vendorId, ImageCategory? category)
        {
            var images = FindByCondition(c=>c.VendId == vendorId && c.IsStatus == Status.Active, false);
            if (category != null)
            {
                images.Where(c => c.Category.Equals(category));
            }
            return await images.OrderByDescending(c => c.CreatedAt).Include(c => c.ImageSettings).ToListAsync();
        }
        public void AddImage (Image image) => Create(image);
        public void DeleteImage(Image image) => Delete(image);

    }
}
