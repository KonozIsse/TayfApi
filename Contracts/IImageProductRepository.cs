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
        Task<IEnumerable<ProductImage>> GetImagesProduct(int productId, bool trackChanges);
        void CreateImageProduct(ProductImage image);  
        void DeleteImageProduct(ProductImage image);
    }
}
