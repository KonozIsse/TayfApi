using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : MyBaseController
    {
        public NotificationsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllNotifications(int PageId)
        {
            var result = await _homeBL.GetNotifications(PageId);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto create)
        {
            var result = await _homeBL.CreateNotification(create);
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
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await _homeBL.DeleteNotification(id);
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
