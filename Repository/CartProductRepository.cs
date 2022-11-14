using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.DataTransferObjects;

namespace Repository
{
    public class CartProductRepository : RepositoryBase<CartProduct>, ICartProductRepository
    {
        public CartProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<CartProduct> GetCartProductId(int id , bool trackChanges)
        => await FindByCondition(t => t.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<CartProduct> GetCartProductDetail(int productId, int cartId, int CustomerId, int optionId)
         => await FindByCondition(t => t.ProductId == productId && t.CartId == cartId && t.Cart.CustomerId == CustomerId
            && t.CartAttributeProducts.Any(c=>c.AttributesProductId== optionId) ,false ).FirstOrDefaultAsync();
        public async Task<CartProduct> GetCartIdProductId(int productId, int cartId)
        => await FindByCondition(t => t.ProductId == productId && t.CartId == cartId, false).FirstOrDefaultAsync();
        public async Task<IEnumerable<CartProduct>> GetAllCartProductToCatId (int cartId)
        => await FindByCondition(t => t.CartId == cartId, false).ToListAsync();
        public int CartCount()
        => FindAll(false).ToList().GroupBy(r => r.ProductId).Select(r => r.First()).Count();
        public async Task<IEnumerable<CartProduct>> GetAllCartProductProductId(int productId) 
            => await FindByCondition(t => t.ProductId == productId, false).ToListAsync();
        public void DeleteCartProduct(CartProduct cartProducts)=> Delete(cartProducts);
        public void AddCartProduct(CartProduct cartProducts) => Create(cartProducts);
        public async Task DeleteCartProductList(List<int> Ids)
        {
            var cartProducts = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(cartProducts);
        }
    }
}
