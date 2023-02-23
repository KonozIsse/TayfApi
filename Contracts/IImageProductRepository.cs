using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IImageProductRepository
    {
        Task<IEnumerable<ProductImage>> GetAllImagesProductId(int productId, bool trackChanges, bool isIncluded = false);
        Task<ProductImage> GetImageProductId(int id, bool trackChanges);
        void CreateImageProduct(ProductImage image);  
        void DeleteImageProduct(ProductImage image);
    }
}
