using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IImageRepository
    {
        Image GetImage(int id, bool trackChanges, bool included = false);
        Task<List<Image>> GetImages(ImageCategory? category);
        Task<List<Image>> GetImagesVendor(int vendorId, ImageCategory? category);
        void AddImage(Image image);
        void DeleteImage(Image image);
    }
}
