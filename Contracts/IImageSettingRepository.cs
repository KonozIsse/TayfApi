using Entities.Models;
using Entities.Models.Enums;
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
        Task<ImageSetting> GetByType(int ImgId , ImageType key);
        void AddImageSetting(ImageSetting image);
        void DeleteImageSetting(ImageSetting image);
    }
}
