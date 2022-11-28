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
        Task<CartAttributeProduct> CartAttributeProductId(int id, bool trackChanges);
        Task<List<CartAttributeProduct>> CartAttributeProductsCartId(int cartId);
        void DeleteCartAttributeProduct(CartAttributeProduct cartAttribute);
    }
}
