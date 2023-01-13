using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxesController : MyBaseController
    {
        public TaxesController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetTaxes(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _locationTaxBL.GetTaxes(search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTax(CreateTaxClassDto create)
        {
            var result = await _locationTaxBL.AddTaxClass(GetStoreId(),create);
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
        public async Task<IActionResult> UpdateTax(UpdateTaxClassDto update)
        {
            var result = await _locationTaxBL.EditTaxClass(GetStoreId(),update);
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
        public async Task<IActionResult> DeleteTax(int id)
        {
            var result = await _locationTaxBL.DeleteTaxClass(id);
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
