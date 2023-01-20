using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussnessResultModel = Entities.Exception.BussnessResultModel;
namespace BusinessLogic.ApiClasses
{//BusinessException
    public class NewsBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        protected readonly LocService _locService;
        public NewsBL(IRepositoryManager repositoryManager, IMapper mapper, LocService locService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _locService = locService;
        }
        //News------------------------------------------------
        public async Task<List<NewsDto>> GetNews(string lang= "en")
        {
            var news = await _repositoryManager.News.GetWithComments();
            var list = new List<NewsDto>();
            news.ForEach(x => list.Add(new NewsDto
            {
                Id = x.Id,
                Url = x.Url,
                CountComment = x.Comments.Count(),
                Title = lang == "en" ? x.Title : x.TitleAr,
                Decription = lang == "en" ? x.Decription : x.DecriptionAr,
                CreatedAt = x.CreatedAt,
                // ImageId = Convert.ToInt32((x.ImgId == 0 || x.Image == null ) ? "" :
                //(x.Image.ImageSettings.Count() > 0 ? urlImg + x.Image.ImageSettings
                //  .FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path : ""))
            }));; 
            return list;
        }
        public async Task<NewsDto> GetBlog(int blogId, string lang = "en")
        {
            var blog = await _repositoryManager.News.GetBlogById(blogId , false, true);  
            var newsDto = _mapper.Map<NewsDto>(blog);
            newsDto.Title = lang == "en" ? blog.Title : blog.TitleAr;
            newsDto.Decription = lang == "en" ? blog.Decription : blog.DecriptionAr;
            newsDto.CountComment = blog.Comments.Count() == 0 ? 0 : blog.Comments.Count();
            return newsDto;
        }
        public async Task<BussnessResultModel> AddNews (int storeId,CreateNewsDto create)
        {
            if (create.TitleAr == null || create.TitleAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"),false);
            }
            if (create.ImageId == 0)
            { 
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"),false);
            }
            var blog = _mapper.Map<News>(create);
            blog.IsFeature = 1;
            blog.IsViewed = 0;
            blog.VendorId = storeId ;
            _repositoryManager.News.CreateBlog(blog);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(blog, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditNews (int storeId,UpdateNewsDto updateDto)
        {
            var blog = await _repositoryManager.News.GetBlogById(updateDto.NewsId, true);
            if(blog == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            if (updateDto.DecriptionAr == null || updateDto.TitleAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            blog.Id = updateDto.NewsId;
            blog.IsFeature = 1;
            blog.IsViewed = 0;
            blog.VendorId = storeId ;
            if (updateDto.ImageId != 0)
            {
                blog.ImgId = updateDto.ImageId;
            }
            _mapper.Map(updateDto, blog);

            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(blog, _locService.GetLocalizedStringValue("successSave")); 
        }
        public async Task<BussnessResultModel> DeleteNews(int newsId)
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
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(blog, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        public async Task<PagedList<NewsDto>> SearchBlog(int vendorId, string search, PostsParameters postsParameters)
        {
            var searchBlog = await _repositoryManager.News.SearchNews(vendorId, search);
            if (vendorId != 0) { searchBlog.Where(c => c.VendorId == vendorId); }
            var newsDto = _mapper.Map<List<NewsDto>>(searchBlog);
            return PagedList<NewsDto>.ToPagedList(newsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        //CommentNews------------------------------------------------
        public async Task<CommentNews> GetCommentId (int commentId)
        {
            return await _repositoryManager.CommentNews.GetCommentId(commentId);
        }
        public async Task<BussnessResultModel> DeleteNewsComments(int id )
        {
            var comment = await _repositoryManager.CommentNews.GetCommentId(id);
            if (comment == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }

            var blog = await _repositoryManager.News.GetBlogById(comment.NewsId, true);
             blog.CountComment--;
            _repositoryManager.CommentNews.DeleteCommentNews(comment);

            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(comment, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> AddNewsComments(int newsId,int userId , CreateCommentDto createCommentsDto)
        {
            if(userId != 0)
            {
                var blog = await _repositoryManager.News.GetBlogById(newsId, true);
                if (blog == null)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
                }
                blog.CountComment++;
                var commentNews = _mapper.Map<CommentNews>(createCommentsDto);
                    commentNews.CustomerId = userId;
                _repositoryManager.CommentNews.CreateCommentNews(newsId, commentNews);
                await _repositoryManager.SaveAsync();
                
                return new BussnessResultModel(commentNews, _locService.GetLocalizedStringValue("CommentAdded"));
            }
           
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("goLogin"),false);
            }
        }
        public async Task<PagedList<CommentsDto>> SearchCommetsNews(int newId, string search, PostsParameters postsParameters)
        {
            var searchComments = await _repositoryManager.CommentNews.SearchCommets(newId, search);
            var commentsDtos = _mapper.Map<List<CommentsDto>>(searchComments);
            return PagedList<CommentsDto>.ToPagedList(commentsDtos, postsParameters.PageNumber, postsParameters.PageSize);
        }
      
    }
}