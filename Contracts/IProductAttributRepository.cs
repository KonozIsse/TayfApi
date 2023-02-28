using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductAttributRepository
    {
        Task<ProductAttribut> GetAttributeId(int id, bool trackChanges);
        Task<ProductAttribut> GetAttributeIdProductId(int id, int productId);
        Task<List<ProductAttribut>> GetAttributesProductId(int productId);
        Task<List<ProductAttribut>> GetAttributesOptionId(int optionId);
        Task<ProductAttribut> GetProductOptionValue(int productId, int optionId, int valueId);
        void AddAttributesProduct(int productId, ProductAttribut option);
        void DeleteAttributesProduct(ProductAttribut option);
    }
}
