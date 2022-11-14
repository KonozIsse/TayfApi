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
         => await FindByCondition(c => (c.VendorId == vendorId || c.VendorId == 0 || c.VendorId == null) &&
         ( c.Title.Contains(search) || c.Decription.Contains(search)), false).Include(r => r.Image).ToListAsync();
        public async Task<List<News>> GetWithComments()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).Include(r => r.Image).ThenInclude(c=>c.ImageSettings).Include(r => r.Comments).ToListAsync();
        public async Task<List<News>> GetBlogs()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).ToListAsync();
        public async Task<News> GetBlogById(int id , bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges).Include(c=>c.Comments).SingleOrDefaultAsync();
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
        public void CreateCommentNews(int newsId, CommentNews commentNews) 
        {
            commentNews.NewsId = newsId;
            Create(commentNews);
        }
        public void DeleteCommentNews(CommentNews commentNews) => Delete(commentNews);
        public async Task<CommentNews> GetCommentId(int id)
        => await FindByCondition(c => c.Id == id, false).FirstOrDefaultAsync();
        public async Task<CommentNews> GetCommentIdNewsId(int id,int blog )
         => await FindByCondition(c => c.Id == id && c.NewsId == blog, false).FirstOrDefaultAsync();
        public async Task<List<CommentNews>> SearchCommets(int newId, string search)
       => await FindByCondition(c => c.NewsId == newId && c.Text.Contains(search) , false).ToListAsync();
        public int GetCountComments(int blog)
         =>FindByCondition(c => c.NewsId == blog, false).Count();

    }
    
}
