
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
        public async Task<CartAttributeProduct> CartAttributeProductId(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<List<CartAttributeProduct>> CartAttributeProductsCartId(int cartId)
        => await FindByCondition(c => c.CartId == cartId, false).ToListAsync();
        public void DeleteCartAttributeProduct(CartAttributeProduct cartAttribute) => Delete(cartAttribute);
    }
}
