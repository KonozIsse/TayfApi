using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EtayfeWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : MyBaseController
    {
        public BlogController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getBlog/{id}")]
        public async Task<IActionResult> GetBlogId(int id)
        {
            var result = await _newsBL.GetBlog(id,GetLanguage());
            return Ok(result);
        }
        [HttpGet("getAllBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var result = await _newsBL.GetNews(GetLanguage());
            return Ok(result);
        }
        [HttpPost("createCommentToBlog")]
        public async Task<IActionResult> CreateComment(int newsId, CreateCommentDto create)
        {
            var result = await _newsBL.AddNewsComments(newsId, GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
