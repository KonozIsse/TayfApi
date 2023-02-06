using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : MyBaseController
    {
        public CategoriesController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getMain")]
        public async Task<IActionResult> GetMainCategories(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GethMainCategoriesCP(search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("getSub")]
        public async Task<IActionResult> GetSubCategories(int id,string search , [FromQuery] PostsParameters postsParameters)
        {
            var result =  await _productBL.GetSubCategoriesCP(id,search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto create)
        {
            var result = await _productBL.CreateCategory(create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("edit")]
        public async Task<IActionResult> EditCategory(UpdateCategoryDto update)
        {
            var result = await _productBL.EditCategory(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _productBL.DeleteCategory(id);
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
