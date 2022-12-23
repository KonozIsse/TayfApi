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
        Task<Image> GetImage(int id, bool trackChanges);
        Task<List<Image>> GetProductImages(int productId);
        Task<List<Image>> GetImages(string category);
        Task<List<Image>> GetImagesVendor(int vendorId, string category);
        void AddImage(Image image);
        void DeleteImage(Image image);
    }
}
