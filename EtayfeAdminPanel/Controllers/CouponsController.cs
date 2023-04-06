using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : MyBaseController
    {
        public CouponsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get-coupons")]
        public async Task<IActionResult> GetAllCoupons(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _orderBL.GetAllCoupons(search,postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create-coupon")]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto create)
        {
            var result = await _orderBL.AddCoupon(create, GetCurrentUserId());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("update-coupon")]
        public async Task<IActionResult> UpdateCoupon(UpdateCouponDto update)
        {
            var result = await _orderBL.UpdateCoupon(update, GetCurrentUserId());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("delete-coupons")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _orderBL.DeleteCoupon(id);
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
