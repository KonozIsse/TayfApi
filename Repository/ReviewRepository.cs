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

        public async Task<PagedList<Review>> GetReviewsByProductId(int productId , PostsParameters postsParameters)
        {
            var review = await FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active,false)
                .Include(u => u.Customer).OrderByDescending(r => r.Rating).ToListAsync();
            return PagedList<Review>.ToPagedList(review, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<IEnumerable<Review>> Last3Reviews(int productId)
         => await FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active, false)
                .Include(u => u.Customer).OrderByDescending(r => r.Rating).Take(3).ToListAsync();
        public async Task<IEnumerable<Review>> GetReviewsProductId(int productId)
         => await FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active, false).ToListAsync();
        public async Task<Review> GetReviewProductIdToCustomerId(int productId, int customerId)
         => await FindByCondition(c => c.ProductId == productId && c.CustomerId == customerId , false).FirstOrDefaultAsync();
        public async Task<Review> GetActiveReviewProductCustomer(int productId, int customerId , bool trackChanges)
         => await FindByCondition(c => c.ProductId == productId && c.CustomerId == customerId && c.IsStatus == Status.Active, trackChanges).FirstOrDefaultAsync();
        
        public int GetReviewsCount(int productId)
        =>  FindByCondition(c => c.ProductId == productId && c.IsStatus == Status.Active, false).Include(u => u.Customer).Count();
        public bool IsReview(int productId, int customerId)
        => FindByCondition(r => r.ProductId == productId && r.CustomerId == customerId && r.IsStatus == Status.Active,false).Any();
         public void AddReview(Review review) => Create(review);
        public void DeleteReview(Review review) => Delete(review);
    }
}
