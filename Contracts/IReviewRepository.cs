using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReviewRepository
    {
        Task<Review> GetReviewId(int id, bool trackChanges);
        Task<List<Review>> GetReviews();
        Task<List<Review>> GetAllReviewsProduct(int productId);
        Task<Review> GetReviewProductIdToCustomerId(int productId, int customerId, bool trackChanges);
        Task<IEnumerable<Review>> GetReviewsActiveProductId(int productId);
        void DeleteReview(Review review);
        void AddReview(Review review);
    }
}
