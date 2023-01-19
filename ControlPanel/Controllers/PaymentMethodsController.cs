using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : MyBaseController
    {
        public PaymentMethodsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get-all-payment")]
        public async Task<IActionResult> GetAllPayment(string search ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _orderBL.GetAllPayments(search,postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
    }
}
