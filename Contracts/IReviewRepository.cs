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
        Task<IEnumerable<Review>> Last3Reviews(int productId);
        Task<IEnumerable<Review>> GetReviewsProductId(int productId);
        Task<Review> GetReviewProductIdToCustomerId(int productId, int customerId);
        Task<Review> GetActiveReviewProductCustomer(int productId, int customerId, bool trackChanges);
        int GetReviewsCount(int productId);
        bool IsReview(int productId, int customerId);
        void DeleteReview(Review review);
        void AddReview(Review review);
    }
}
