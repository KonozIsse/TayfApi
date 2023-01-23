using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : MyBaseController
    {
        public ReportsController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAllInventory([FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllInventory(GetCurrentUserId(), GetLanguage(), postsParameters);
            return Ok(result);
        }
        [HttpGet("GetCustomerTotal")]
        public async Task<IActionResult> GetCustomerTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetCustomerTotal(search, postsParameters);
            return Ok(result);
        }
        [HttpGet("GetVendorTotal")]
        public async Task<IActionResult> GetVendorTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetVendorTotal(search, postsParameters);
            return Ok(result);
        }
        [HttpGet("GetLikedProducts")]
        public async Task<IActionResult> GetLikedProducts(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetProductsCP(GetCurrentUserId(), search,GetLanguage(),  postsParameters);
            return Ok(result);
        }

    }      
}
