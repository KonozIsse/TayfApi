using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface INewsRepository
    {
        Task<List<News>> SearchNews(string search);
        Task<List<News>> GetBlogsImage(int imageId);
        Task<List<News>> GetWithComments();
        Task<News> GetBlogById(int id, bool trackChanges, bool Included = false);
        void CreateBlog(News blog);
        void DeleteBlog(News blog);
    }
    public interface ICommentNewsRepository
    {
        void DeleteCommentNews(CommentNews commentNews);
        Task<List<CommentNews>> GetCommentsByNewsId(int id);
        void CreateCommentNews(int newsId, CommentNews commentNews);
        Task<List<CommentNews>> SearchCommets(int newId, string search);
        Task<CommentNews> GetCommentId(int id);
    }
}
