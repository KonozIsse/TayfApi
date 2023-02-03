using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : MyBaseController
    {
        public OrderController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getFailOrder")]
        public async Task<IActionResult> GetFailOrder(string csp, string Err)
        {
            try
            {
                _logger.LogInfo(string.Format("{0} failure order payment by order id :" + csp, Err));
            }
            catch (Exception) { }
            if (csp != null)
            {
                await _orderBL.GetFailOrder(Convert.ToInt32(csp));
            }
            return Ok("Error Occurs , Please Confirm Your Data");
        }
    }
}
