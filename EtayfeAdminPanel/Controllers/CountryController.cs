using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : MyBaseController
    {
        public CountryController(IServiceProvider provider) : base(provider)
        {
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAllCountries(string search, string filter, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _locationTaxBL.GetAllCountries(GetLanguage(), search,filter, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            var result = await _locationTaxBL.GetAllCountries();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCountry(CreateCountryDto create)
        {
            var result = await _locationTaxBL.AddCountry(create);
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
        public async Task<IActionResult> UpdateCountry(UpdateCountryDto update)
        {
            var result = await _locationTaxBL.EditCountry(update);
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
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var result = await _locationTaxBL.DeleteCountry(id);
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
