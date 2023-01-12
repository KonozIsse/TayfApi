using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : MyBaseController
    {
        public CouponsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllCoupons(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _orderBL.GetAllCoupons(search,postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto create)
        {
            var storeId = GetStoreId() == 0 ? 0 : GetStoreId();
            var adminId = GetAdminId() == 0 ? 0 : GetAdminId();
            var result = await _orderBL.AddCoupon(create, storeId, adminId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCoupon(UpdateCouponDto update)
        {
            var storeId = GetStoreId() == 0 ? 0 : GetStoreId();
            var adminId = GetAdminId() == 0 ? 0 : GetAdminId();
            var result = await _orderBL.UpdateCoupon(update, storeId, adminId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("delete")]
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
