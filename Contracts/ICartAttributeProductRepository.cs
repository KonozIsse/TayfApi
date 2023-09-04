using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICartAttributeProductRepository
    {
        Task<List<CartAttributeProduct>> CartAttributeProductsCartId(int cartId, bool trackChanges);
        Task<CartAttributeProduct> GetItemId(int id, bool trackChanges);
        void DeleteCartAttributeProduct(CartAttributeProduct cartAttribute);
        void CreatCartAttributeRange(List<CartAttributeProduct> cartAttributes); 
        Task DeleteRowRange(List<int> Ids);
    }
}
