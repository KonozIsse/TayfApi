using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
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
        public async Task<IActionResult> EditMediaSetting(string thh, string thw, string mh, string mw, string lh, string lw)
        {
            var result = await _imageBL.EditMediaSetting(thh, thw, mh, mw, lh, lw);
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
