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
        [HttpGet("GetLinksRole/{roleId}")]
        public async Task<IActionResult> GetLinksRole(int roleId)
        {
            var result = await _userBL.GetLinks(roleId);
            return Ok(result);
        }
        [HttpGet("GetTotalOrdersProducts")]
        public async Task<IActionResult> GetTotalOrdersProducts()
        {
            var result = await _homeBL.GetHomeCP(GetCurrentUserId());
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
            var result = await _homeBL.GetRecentProducts(GetCurrentUserId(), GetLanguage());
            return Ok(result);
        } 
        [HttpGet("getNewOrders")]
        public async Task<IActionResult> GetNewOrders()
        {
            var result = await _homeBL.GetOrders(GetCurrentUserId());
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
