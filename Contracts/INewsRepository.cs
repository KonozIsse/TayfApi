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
        Task<List<News>> SearchNews(int vendorId, string search);
        Task<List<News>> GetWithComments();
        Task<List<News>> GetBlogs();
        Task<News> GetBlogById(int id, bool trackChanges);
        void CreateBlog(News blog);
        void DeleteBlog(News blog);
    }
    public interface ICommentNewsRepository
    {
        void DeleteCommentNews(CommentNews commentNews);
        Task<List<CommentNews>> GetCommentsByNewsId(int id);
        Task<CommentNews> GetCommentIdNewsId(int id, int blog );
        void CreateCommentNews(int newsId, CommentNews commentNews);
        Task<List<CommentNews>> SearchCommets(int newId, string search);
        Task<CommentNews> GetCommentId(int id);
        int GetCountComments(int blog);
    }
}
