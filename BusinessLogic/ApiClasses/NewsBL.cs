using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using System.Collections.Generic;

namespace BusinessLogic.ApiClasses
{
    public class NewsBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        protected readonly LocService _locService;
        protected readonly ImageBL _imageBL;
        public NewsBL(IRepositoryManager repositoryManager, IMapper mapper, LocService locService, ImageBL imageBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _locService = locService;
            _imageBL = imageBL;
        }
        //News------------------------------------------------
        public async Task<List<NewsDto>> GetNews(string lang)
        {
            var news = await _repositoryManager.News.GetWithComments();
            var list = news.Select(x =>
            {
                var blogDto = _mapper.Map<NewsDto>(x);
                blogDto.CountComment = x.Comments.Count();
                blogDto.Title = lang == "en" ? x.Title : x.TitleAr;
                blogDto.Decription = lang == "en" ? x.Decription : x.DecriptionAr;
                blogDto.CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy");
                blogDto.Image = _imageBL.GetImageOriginal(Convert.ToInt32(x.ImgId));
                return blogDto;
            }).ToList(); 
            return list;
        }
        public async Task<NewsDto> GetBlog(int blogId, string lang)
        {
            var blog = await _repositoryManager.News.GetBlogById(blogId , false, true);  
            var newsDto = _mapper.Map<NewsDto>(blog);
            newsDto.Title = lang == "en" ? blog.Title : blog.TitleAr;
            newsDto.Decription = lang == "en" ? blog.Decription : blog.DecriptionAr;
            newsDto.CountComment = blog.Comments == null ? 0 : blog.Comments.Count();
            newsDto.Image = _imageBL.GetImageOriginal(blog.ImgId.Value);
            return newsDto;
        }
        public async Task<UpdateNewsDto> GetBlogId(int blogId)
        {
            var blog = await _repositoryManager.News.GetBlogById(blogId, false, false);
            var newsDto = _mapper.Map<UpdateNewsDto>(blog);
            newsDto.Image = _imageBL.GetImageOriginal(blog.ImgId.Value);
            return newsDto;
        }
        public async Task<BussnessResultModel> AddNews (int storeId, int im,CreateNewsDto create)
        {
            var blog = _mapper.Map<News>(create);
            blog.ImgId = im;
            blog.IsFeature = 1;
            blog.IsViewed = 0;
            blog.VendorId = storeId ;
            _repositoryManager.News.CreateBlog(blog);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(blog, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditNews (int storeId,UpdateNewsDto updateDto)
        {
            var blog = await _repositoryManager.News.GetBlogById(updateDto.Id, true);
            if(blog == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            if (updateDto.DecriptionAr == null || updateDto.TitleAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            blog.IsFeature = 1;
            blog.IsViewed = 0;
            blog.VendorId = storeId ;
            //if (updateDto.ImageId != 0)
            //{
            //    blog.ImgId = updateDto.ImageId;
            //}
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
        public async Task<List<NewsDto>> GetAllBlogstest(int vendorId, string lang)
        {
            var searchBlog = await _repositoryManager.News.SearchNews("");
            var user = await _repositoryManager.User.GetActiveUserId(vendorId,false);
            if (user.UserType == UserType.Store)
            {
                searchBlog = searchBlog.Where(c => c.VendorId == vendorId).ToList();
            }
            var newsDto = searchBlog.Select(x =>
            {
                var blogDto = _mapper.Map<NewsDto>(x);
                //blogDto.CountComment = x.Comments.Count();
                blogDto.Title = lang == "en" ? x.Title : x.TitleAr;
                blogDto.Decription = lang == "en" ? x.Decription : x.DecriptionAr;
                blogDto.CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy");
                blogDto.Image = _imageBL.GetImageOriginal(Convert.ToInt32(x.ImgId));
                return blogDto;
            }).ToList();
            return newsDto;
        }

        public async Task<PagedList<NewsDto>> GetAllBlogs (int vendorId, string lang, string search, PostsParameters postsParameters)
        {
            var searchBlog = await _repositoryManager.News.SearchNews(search);
            if (vendorId != 0) 
            { 
                searchBlog =  searchBlog.Where(c => c.VendorId == vendorId).ToList(); 
            }
            var newsDto = searchBlog.Select(x =>
            {
                var blogDto = _mapper.Map<NewsDto>(x);
                //blogDto.CountComment = x.Comments.Count();
                blogDto.Title = lang == "en" ? x.Title : x.TitleAr;
                blogDto.Decription = lang == "en" ? x.Decription : x.DecriptionAr;
                blogDto.CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy");
                blogDto.Image = _imageBL.GetImageOriginal(Convert.ToInt32(x.ImgId));
                return blogDto;
            }).ToList();
            return PagedList<NewsDto>.ToPagedList(newsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        //CommentNews------------------------------------------------
        public async Task<BussnessResultModel> DeleteNewsComments(int id )
        {
            var comment = await _repositoryManager.CommentNews.GetCommentId(id);
            if (comment == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }
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