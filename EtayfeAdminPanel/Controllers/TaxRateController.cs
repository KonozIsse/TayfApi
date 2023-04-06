using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxRateController : MyBaseController
    {
        public TaxRateController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetTaxeRates(string search, string filter ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _locationTaxBL.GetTaxeRates(search, filter,GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTaxRate(CreateTaxRateDto create)
        {
            var result = await _locationTaxBL.AddTaxRate(GetCurrentUserId(), create);
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
        public async Task<IActionResult> UpdateTaxRate(UpdateTaxRateDto update)
        {
            var result = await _locationTaxBL.EditTaxRate(GetCurrentUserId(), update);
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
        public async Task<IActionResult> DeleteTaxRate(int id)
        {
            var result = await _locationTaxBL.DeleteTaxRate(id);
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
