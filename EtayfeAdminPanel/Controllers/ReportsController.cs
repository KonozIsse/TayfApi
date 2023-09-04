using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : MyBaseController
    {
        public ReportsController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("get-Inventories")]
        public async Task<IActionResult> GetAllInventory(string search ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllInventory(search ,GetCurrentUserId(), GetLanguage(), postsParameters);
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
        public async Task<IActionResult> GetAllOutInventory(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllOutInventory(search,GetCurrentUserId(), GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("get-salesOrders")]
        public async Task<IActionResult> GetAllSalesOrders(string search, int customerId, int storeId, int statusId, DateTime? dateFrom, DateTime? dateTo )
        {
            var result = await _orderBL.GetAllSalesOrders(GetCurrentUserId(), search, customerId, storeId, statusId, dateFrom , dateTo);
            return Ok(result);
        }
        [HttpGet("GetCustomerTotal")]
        public async Task<IActionResult> GetCustomerTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetCustomerTotal(GetCurrentUserId(), search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetVendorTotal")]
        public async Task<IActionResult> GetVendorTotal(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetVendorTotal(GetCurrentUserId(), search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetLikedProducts")]
        public async Task<IActionResult> GetLikedProducts(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetProductsCP(GetCurrentUserId(), search,null,GetLanguage(),  postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

    }      
}
