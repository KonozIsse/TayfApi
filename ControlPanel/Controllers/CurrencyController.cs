using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : MyBaseController
    {
        public CurrencyController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllCurrencies([FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetAllCurrencies(GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCurrency(CreateCurrencyDto create)
        {
            var result = await _homeBL.AddCurrency(create);
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
        public async Task<IActionResult> UpdateCurrency(UpdateCurrencyDto update)
        {
            var result = await _homeBL.UpdatCurrency(update);
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
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var result = await _homeBL.DeleteCurrency(id);
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
