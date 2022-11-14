using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IWishListRepository
    {
        Task<WishList> GetWishListProductIdCustomerId(int customerId, int productId);
        Task<WishList> GetLikeCustomerId(int id, int customerId);
        int GetCountLikesToProductId(int productId);
        int GetCountLikesByCustomersId(int customerId);
        Task<IEnumerable<WishList>> GetLikesCustomerId(int customerId);
        Task<IEnumerable<WishList>> GetLikesProductId(int productId);
        void Addlike(int productId, WishList like);
        void DeleteLike(WishList like);
    }
}
