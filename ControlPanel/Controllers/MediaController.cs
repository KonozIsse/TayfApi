using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : MyBaseController
    {
        public MediaController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpPost("addimages")]
        public async Task<IActionResult> CreateImages([FromForm] CreateImageDto create)
        {
            var result = await _imageBL.CreateImages(GetStoreId(), create);
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
