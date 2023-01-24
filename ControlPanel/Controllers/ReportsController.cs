using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : MyBaseController
    {
        public ReportsController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("get-Inventories")]
        public async Task<IActionResult> GetAllInventory([FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllInventory(GetCurrentUserId(), GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("get-historyInventories")]
        public async Task<IActionResult> GetAllViewInventory(int productId)
        {
            var result = await _productBL.GetAllViewInventory(GetCurrentUserId(),GetLanguage(), productId);
            return Ok(result);
        }
        [HttpGet("get-Inventories-Out")]
        public async Task<IActionResult> GetAllOutInventory([FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllOutInventory(GetCurrentUserId(), GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("get-salesOrders")]
        public async Task<IActionResult> GetAllSalesOrders(string search, DateTime? dateFrom, DateTime? dateTo )
        {
            var result = await _orderBL.GetAllSalesOrders(GetCurrentUserId(), search, dateFrom , dateTo);
            return Ok(result);
        }
        [HttpGet("GetCustomerTotal")]
        public async Task<IActionResult> GetCustomerTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetCustomerTotal(search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetVendorTotal")]
        public async Task<IActionResult> GetVendorTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetVendorTotal(search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetLikedProducts")]
        public async Task<IActionResult> GetLikedProducts(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetProductsCP(GetCurrentUserId(), search,GetLanguage(),  postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

    }      
}
