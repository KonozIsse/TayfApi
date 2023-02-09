using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WishListRepository : RepositoryBase<WishList>, IWishListRepository
    {
        public WishListRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<WishList> GetWishListProductIdCustomerId(int customerId, int productId)
         => await FindByCondition(x => x.CustomerId == customerId && x.ProductId == productId ,false).FirstOrDefaultAsync();
        public async Task<WishList> GetLikeCustomerId(int id,int customerId )
         => await FindByCondition(x => x.Id == id && x.CustomerId == customerId , false).FirstOrDefaultAsync();

        public async Task<IEnumerable<WishList>> GetLikesCustomerId(int customerId)
        => await FindByCondition(x => x.CustomerId == customerId, false).ToListAsync();
        public async Task<IEnumerable<WishList>> GetLikesProductId(int productId)
        => await FindByCondition(x => x.ProductId == productId, false).ToListAsync();
        public void Addlike(int productId , WishList like)
        {
            like.ProductId = productId;
            Create(like);
        }
        public void DeleteLike(WishList like) => Delete(like);
    }
}
