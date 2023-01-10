using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class NewsRepository : RepositoryBase<News>, INewsRepository
    {
        public NewsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<News>> SearchNews(int vendorId, string search)
         => await FindByCondition(c => (c.VendorId == vendorId  || c.VendorId == null)  &&
         ( c.Title.Contains(search) || c.Decription.Contains(search)), false)
            .Include(r => r.Image).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<List<News>> GetWithComments()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).Include(r => r.Image)
            .ThenInclude(c=>c.ImageSettings)
            .Include(r => r.Comments).ToListAsync();
      
        public async Task<News> GetBlogById(int id, bool trackChanges, bool Included = false)
        {
            var blog = FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges);
            if (Included == true)
            {
                blog.Include(c => c.Comments).Include(v => v.Image);
            }
            return await blog.SingleOrDefaultAsync();
        }
        public void CreateBlog(News blog) => Create(blog);
        public void DeleteBlog(News blog) => Delete(blog);
    }
    public class CommentNewsRepository : RepositoryBase<CommentNews>, ICommentNewsRepository
    {
        public CommentNewsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<CommentNews>> GetCommentsByNewsId(int newId)
        => await FindByCondition(c => c.NewsId == newId, false).ToListAsync();
        public async Task<CommentNews> GetCommentId(int id)
        => await FindByCondition(c => c.Id == id, false).FirstOrDefaultAsync();
        public async Task<CommentNews> GetCommentIdNewsId(int id,int blog , bool trackChanges)
         => await FindByCondition(c => c.Id == id && c.NewsId == blog, trackChanges).FirstOrDefaultAsync();
        public async Task<List<CommentNews>> SearchCommets(int newId, string search)
       => await FindByCondition(c => c.NewsId == newId && c.Comment.Contains(search) , false).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public int GetCountComments(int blog)
         =>FindByCondition(c => c.NewsId == blog, false).Count();
        public void CreateCommentNews(int newsId, CommentNews commentNews)
        {
            commentNews.NewsId = newsId;
            Create(commentNews);
        }
        public void DeleteCommentNews(CommentNews commentNews) => Delete(commentNews);
        public async Task DeleteListCommentNews(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }
    }
    
}
