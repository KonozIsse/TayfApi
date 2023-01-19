using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : MyBaseController
    {
        public HomeController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getTotal-products/orders")]
        public async Task<IActionResult> GetTotalOrdersProducts()
        {
            var result = await _homeBL.GetHomeCP(GetStoreId());
            return Ok(result);
        } 
        [HttpGet("getNewCustomers")]
        public async Task<IActionResult> GetNewCustomers()
        {
            var result = await _homeBL.GetNewCustomers();
            return Ok(result);
        }
        [HttpGet("getRecentProducts")]
        public async Task<IActionResult> GetRecentProducts()
        {
            var result = await _homeBL.GetRecentProducts(GetStoreId());
            return Ok(result);
        } 
        [HttpGet("getNewOrders")]
        public async Task<IActionResult> GetNewOrders()
        {
            var result = await _homeBL.GetOrders(GetStoreId());
            return Ok(result);
        } 
        [HttpGet("getNewStores")]
        public async Task<IActionResult> GetNewStores()
        {
            var result = await _userBL.GetSomeStores();
            return Ok(result);
        } 
      
       
    }
}
