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
        Task<CartAttributeProduct> GetCartIdAttributeId(int cartId, int attr, bool trackChanges);
        Task<List<CartAttributeProduct>> CartAttributeProductsCartId(int cartId, bool trackChanges);
        void DeleteCartAttributeProduct(CartAttributeProduct cartAttribute);
    }
}
