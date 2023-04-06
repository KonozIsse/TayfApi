using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : MyBaseController
    {
        public OrdersController(IServiceProvider provider) : base(provider)
        {
        }
      
        [HttpGet("get-order-status")]
        public async Task<IActionResult> GetAllOrderStatus( [FromQuery] PostsParameters postsParameters)
        {
            var result = await _orderBL.GetOrderStatus(GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPut("update-order-status")]
        public async Task<IActionResult> EditStatus(UpdateOrderStatusDto update)
        {
            var result = await _orderBL.EditOrderStatus(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrders( string search , [FromQuery] PostsParameters postsParameters)
        {
            var result = await _orderBL.GetAllOrders(GetCurrentUserId(),search , postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpGet("get-order/{id}")]
        public async Task<IActionResult> GetViewOrder(int id)
        {
            var result = await _orderBL.GetOrder(id);
            return Ok(result);
        }
        [HttpPut("pending-order")]
        public async Task<IActionResult> OrderPending(int id)
        {
            var result = await _orderBL.OrderPending(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("shipped-order")]
        public async Task<IActionResult> ShippedOrder(int id)
        {
            var result = await _orderBL.ShippedOrder(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("complete-order")]
        public async Task<IActionResult> OrderComplete(int id)
        {
            var result = await _orderBL.OrderComplete(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPut("reject-order")]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var result = await _orderBL.OrderReject(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("delete-order")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderBL.DeleteOrder(id);
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
