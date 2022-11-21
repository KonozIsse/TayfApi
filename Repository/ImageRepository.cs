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

        public async Task<List<Image>> GetProductImages(int productId)
          => await FindByCondition(c => c.ProductId == productId ,false).ToListAsync();
        public async Task<List<Image>> GetImages(ImageCategory category)
        => await FindByCondition(c => c.Category == category, false).OrderByDescending(r => r.CreatedAt).ToListAsync();
        public async Task<List<Image>> GetImageCategoryByVendor(int vendorId, ImageCategory category)
        => await FindByCondition(c=>c.VendId == vendorId && c.Category == category, false).OrderByDescending(r => r.CreatedAt).ToListAsync();
        public void AddImage (Image image) => Create(image);
        public void DeleteImage(Image image) => Delete(image);

    }
}
