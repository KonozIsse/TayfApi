using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {
        public ReviewRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<Review> GetReviewId(int id , bool trackChanges)
       => await FindByCondition(c => c.Id == id , trackChanges).FirstOrDefaultAsync();
        public async Task<List<Review>> GetAllReviewsProduct(int productId)
      => await FindByCondition(c=>c.ProductId == productId,false).ToListAsync();
        public async Task<List<Review>> GetReviews()
       =>await FindAll(false).Include(c => c.Customer).Include(c => c.Product).ToListAsync();
        public async Task<IEnumerable<Review>> GetReviewsActiveProductId(int productId)
         => await FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active, false)
            .Include(c=>c.Customer).Include(c=>c.Product).ToListAsync();
        public async Task<Review> GetReviewProductIdToCustomerId(int productId, int customerId, bool trackChanges)
         => await FindByCondition(c => c.ProductId == productId && c.CustomerId == customerId , trackChanges).FirstOrDefaultAsync();
          public void AddReview(Review review) => Create(review);
        public void DeleteReview(Review review) => Delete(review);
    }
}
