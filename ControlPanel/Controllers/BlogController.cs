using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : MyBaseController
    {
        public BlogController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetBlogs(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _newsBL.SearchBlog(GetStoreId(), search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("getCommments")]
        public async Task<IActionResult> GetCommentsBlog(int id , string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _newsBL.SearchCommetsNews(id , search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPost("createBlog")]
        public async Task<IActionResult> CreateBlog(CreateNewsDto create)
        {
            var storeId = GetStoreId() == 0 ? 0 : GetStoreId();
            var result = await _newsBL.AddNews(storeId, create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("editBlog")]
        public async Task<IActionResult> EditBlog(UpdateNewsDto update)
        {
            var storeId = GetStoreId() == 0 ? 0 : GetStoreId();
            var result = await _newsBL.EditNews(storeId, update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
       
        [HttpDelete("removeBlog")]
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
        [HttpDelete("removeComment")]
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
