using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICartProductRepository
    {
        Task<CartProduct> GetCartProductId(int id, bool trackChanges);
         Task<CartProduct> GetCartProductDetail(int productId, int cartId, int CustomerId, int optionId);
        Task<CartProduct> GetCartIdProductId(int productId, int cartId);
         Task<IEnumerable<CartProduct>> GetAllCartProductToCatId(int cartId);
        Task<IEnumerable<CartProduct>> GetAllCartProductProductId(int productId);
        void AddCartProduct(CartProduct cartProducts);
         void DeleteCartProduct(CartProduct cartProducts);
        int CartCount();
        Task DeleteCartProductList(List<int> Ids);
    }
}
