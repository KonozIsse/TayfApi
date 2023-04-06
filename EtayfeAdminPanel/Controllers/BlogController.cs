using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : MyBaseController
    {
        public BlogController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getblogsTest")]
        public async Task<IActionResult> GetBlogsTest()
        {
            var result = await _newsBL.GetAllBlogstest(GetCurrentUserId(),GetLanguage());
            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        } 
        [HttpGet("get-blogs")]
        public async Task<IActionResult> GetBlogs(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _newsBL.GetAllBlogs(GetCurrentUserId(),GetLanguage(), search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("get-commments")]
        public async Task<IActionResult> GetCommentsBlog(int id , string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _newsBL.SearchCommetsNews(id , search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPost("create-blog")]
        public async Task<IActionResult> CreateBlog(CreateNewsDto create)
        {
            var result = await _newsBL.AddNews(GetCurrentUserId(),1, create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("edit-blog")]
        public async Task<IActionResult> EditBlog(UpdateNewsDto update)
        {
            var result = await _newsBL.EditNews(GetCurrentUserId(), update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
       
        [HttpDelete("remove-blog")]
        public async Task<IActionResult> RemoveBlog(int id)
        {
            var result = await _newsBL.DeleteNews(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        } 
        [HttpDelete("remove-comment")]
        public async Task<IActionResult> RemoveComment(int commentId)
        {
            var result = await _newsBL.DeleteNewsComments(commentId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
    }
}
