using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaSettingsController : MyBaseController
    {
        public MediaSettingsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetMediaSetting()
        {
            var result = await _imageBL.GetMediaSetting();
            return Ok(result);
        }
       
        [HttpPut("update")]
        public async Task<IActionResult> EditMediaSetting(SettingImageVM update)
        {
            var result = await _imageBL.EditMediaSetting(update);
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
