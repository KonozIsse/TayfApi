
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CartAttributeProductRepository : RepositoryBase<CartAttributeProduct>, ICartAttributeProductRepository
    {
        public CartAttributeProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<CartAttributeProduct> GetCartIdAttributeId(int cartId, int attr ,bool trackChanges)
        => await FindByCondition(c => c.CartId == cartId && c.AttributesProductId == attr, trackChanges).SingleOrDefaultAsync();
        public async Task<List<CartAttributeProduct>> CartAttributeProductsCartId(int cartId, bool trackChanges)
        => await FindByCondition(c => c.CartId == cartId, trackChanges).ToListAsync();
        public void DeleteCartAttributeProduct(CartAttributeProduct cartAttribute) => Delete(cartAttribute);

        public async Task<CartAttributeProduct> GetItemId(int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();

        public void CreatCartAttributeRange(List<CartAttributeProduct> cartAttributes)=>CreateRange(cartAttributes);

        public async Task DeleteRowRange(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }
    }
}
