using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IImageSettingRepository
    {
        Task<IEnumerable<ImageSetting>> GetImageSettings(int imageId);
        Task<ImageSetting> GetImageSettingId(int id, bool trackChanges);
        Task<ImageSetting> GetByType(int imageId, string ImageType = null);
        Task<IEnumerable<ImageSetting>> GetImagesStoreId(int vendorId, string category);
        void AddImageSetting(ImageSetting image);
        void DeleteImageSetting(ImageSetting image);
    }
}
