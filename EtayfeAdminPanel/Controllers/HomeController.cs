using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : MyBaseController
    {
        public HomeController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("GetTotalOrdersProducts")]
        public async Task<IActionResult> GetTotalOrdersProducts()
        {
            var result = await _homeBL.GetHomeCP(3);
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
            var result = await _homeBL.GetRecentProducts(3,GetLanguage());
            return Ok(result);
        } 
        [HttpGet("getNewOrders")]
        public async Task<IActionResult> GetNewOrders()
        {
            var result = await _homeBL.GetOrders(3);
            return Ok(result);
        } 
        [HttpGet("getNewStores")]
        public async Task<IActionResult> GetNewStores()
        {
            var result = await _homeBL.GetStores();
            return Ok(result.Take(10));
        } 
      
       
    }
}
