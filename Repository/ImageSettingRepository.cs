using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public ImageSetting GetByType(int ImgId , ImageType key)
        {
            var image =  FindByCondition(c => c.ImgId == ImgId, true);
            if (key != 0) 
            {
                image = image.Where(c => c.ImageType == key);
            }
            return  image.SingleOrDefault(); 
        }
        public async Task<ImageSetting> GetImageSettingId(int id , bool trackChanges)
         => await FindByCondition(y => y.Id == id, trackChanges).FirstOrDefaultAsync();
       
        public void AddImageSetting(ImageSetting image) => Create(image);
        public void DeleteImageSetting(ImageSetting image) => Delete(image);
        public async Task<IEnumerable<ImageSetting>> GetImageSettingOriginal()
       => await FindByCondition(y => y.ImageType == ImageType.ACTUAL, false).Include(c=>c.Image).ToListAsync();
    }
}
