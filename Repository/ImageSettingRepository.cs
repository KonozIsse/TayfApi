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
    public class ImageSettingRepository : RepositoryBase<ImageSetting>, IImageSettingRepository
    {
        public ImageSettingRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<ImageSetting>> GetImageSettings(int imageId)
        => await FindByCondition(y => y.ImgId == imageId, false).ToListAsync();
        public async Task<ImageSetting> GetImageSettingId(int id , bool trackChanges)
         => await FindByCondition(y => y.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<ImageSetting> GetByType(int imageId, string ImageType = null )
        {
            var query = FindByCondition(y => y.ImgId == imageId, false);
            if (!string.IsNullOrWhiteSpace(ImageType))
            {
                query = query.Where(y => y.ImageType.Equals(ImageType));
            }
            return await query.SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<ImageSetting>> GetImagesStoreId(int vendorId , string category)
        {
            var items = await FindAll(false).Include(c => c.Image).Where(c => (c.Image != null && c.Image.VendId == vendorId))
                .OrderByDescending(e => e.CreatedAt).ToListAsync();
            if (vendorId == 0)
            {
                items = await FindAll(false).Include(c => c.Image).OrderByDescending(e => e.CreatedAt).ToListAsync();
            }
            return (category == "" ? items : items.Where(c => c.Image != null && c.Image.Category.Equals(category)).ToList());
        }
        public void AddImageSetting(ImageSetting image) => Create(image);
        public void DeleteImageSetting(ImageSetting image) => Delete(image);
    }
}
