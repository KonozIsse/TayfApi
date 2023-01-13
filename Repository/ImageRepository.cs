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
    public class ImageRepository :RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<Image> GetImage(int id , bool trackChanges)
         => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges).FirstOrDefaultAsync();
        public async Task<List<Image>> GetProductImages(int productId)
          => await FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active ,false).ToListAsync();
        public async Task<List<Image>> GetImages(string category)
        {
            var images = FindByCondition(c => c.IsStatus == Status.Active,false);
            if(category != null && category != "")
            {
                images.Where(c => c.Category.Equals(category));
            }
            return await images.OrderByDescending(c => c.CreatedAt).Include(c=>c.ImageSettings).ToListAsync();
        }
        public async Task<List<Image>> GetImagesVendor(int vendorId, string category)
        {
            var images = FindByCondition(c=>c.VendId == vendorId && c.IsStatus == Status.Active, false);
            if (category != null && category != "")
            {
                images.Where(c => c.Category.Equals(category));
            }
            return await images.OrderByDescending(c => c.CreatedAt).Include(c => c.ImageSettings).ToListAsync();
        }
        public void AddImage (Image image) => Create(image);
        public void DeleteImage(Image image) => Delete(image);

    }
}
