using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Twilio.Http;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : MyBaseController
    {
        public ReviewsController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("get-all-reviews")]
        public async Task<IActionResult> GetAllOrders([FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllReviews(GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPut("active-review")]
        public async Task<IActionResult> ActiveReview(int id)
        {
            var result = await _productBL.ActiveReview(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("deactive-review")]
        public async Task<IActionResult> DeactiveReview(int id)
        {
            var result = await _productBL.DeactiveReview(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
       
    }
}
