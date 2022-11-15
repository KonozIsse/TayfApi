using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ApiClasses
{//BusinessException
    public class NewsBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        public NewsBL(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        //News------------------------------------------------
        public async Task<List<NewsDto>> GetNews()
        {
            var news = await _repositoryManager.News.GetWithComments();
            var list = new List<NewsDto>();
            news.ForEach(x => list.Add(new NewsDto
            {
                Id = x.Id,
                IsStatus = x.IsStatus,
                Title = x.Title == null ? "" : x.Title,
                // ImageId = Convert.ToInt32((x.ImgId == 0 || x.Image == null ) ? "" :
                //(x.Image.ImageSettings.Count() > 0 ? urlImg + x.Image.ImageSettings
                //  .FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path : "")),
                Decription = String.IsNullOrEmpty(x.Decription) ? "" : x.Decription,
                Url = x.Url,
                CountComment = x.Comments.Count() 
            })); 
            return list;
        }
        public async Task<List<NewsDto>> GetListNews()
        {
            var blogs = await _repositoryManager.News.GetWithComments();  
            var newsDto = _mapper.Map<List<NewsDto>>(blogs);
            foreach (var blog in blogs)
            {
               var blogDto =  newsDto.FirstOrDefault();
                blogDto.CountComment = blog.Comments.Count();
               // blogDto.ImageId = Convert.ToInt32(_imageApi.GetImageMedium( blog.ImgId.ToString()));
               // blog.VendorId = GetCurrentUserId();
            }
            return newsDto;
        }
        public async Task AddNews(CreateNewsDto createNewsDto)
        {
            var blog = _mapper.Map<News>(createNewsDto);
            //blog.VendorId = GetCurrentUserId();
            _repositoryManager.News.CreateBlog(blog);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteNews(int newsId)
        {
            var blog = await _repositoryManager.News.GetBlogById(newsId, false);
            if (blog != null)
            {
                var commentNews = await _repositoryManager.CommentNews.GetCommentsByNewsId(newsId);
                if (commentNews != null)
                {
                    foreach(var commentNew in commentNews)
                    {
                        _repositoryManager.CommentNews.DeleteCommentNews(commentNew);
                    }
                }
                _repositoryManager.News.DeleteBlog(blog);
            }
            await _repositoryManager.SaveAsync();
        }
       
        public async Task<List<NewsDto>> SearchBlog(int vendorId, string search)
        {
            var searchBlog = await _repositoryManager.News.SearchNews(vendorId, search);
           searchBlog.Where(c=>c.VendorId == vendorId );
            var newsDto = _mapper.Map<List<NewsDto>>(searchBlog);
            return newsDto;
        }
        //CommentNews------------------------------------------------
        public async Task<CommentNews> CommentNews(int commentId)
        {
            return await _repositoryManager.CommentNews.GetCommentId(commentId);
        }
        public async Task DeleteNewsComments(int id , int newsId)
        {
            var blog = await _repositoryManager.News.GetBlogById(newsId, true);
            blog.CountComment--;
            var commentNews = await _repositoryManager.CommentNews.GetCommentIdNewsId(id, newsId , true);
            _repositoryManager.CommentNews.DeleteCommentNews(commentNews);
            await _repositoryManager.SaveAsync();
        }
        public async Task AddNewsComments(int newsId ,CreateCommentsDto createCommentsDto)
        {
            var blog = await _repositoryManager.News.GetBlogById(newsId, true);
            blog.CountComment++;
            var commentNews = _mapper.Map<CommentNews>(createCommentsDto);
           // commentNews.UserId = userId;
            _repositoryManager.CommentNews.CreateCommentNews(newsId, commentNews);
            await _repositoryManager.SaveAsync();
        }
        public async Task<List<CommentsDto>> SearchCommetsNews(int newId, string search)
        {
            var searchComments = await _repositoryManager.CommentNews.SearchCommets(newId, search);
            var commentsDtos = _mapper.Map<List<CommentsDto>>(searchComments);
            return commentsDtos;
        }
    }
}
