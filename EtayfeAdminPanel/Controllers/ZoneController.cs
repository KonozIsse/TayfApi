using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/Zone")]
    [ApiController]
    public class ZoneController : MyBaseController
    {
        public ZoneController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet()]
        [Route("GetAllZones")]
        public async Task<IActionResult> GetAllZones(string serch,string filter, PostsParameters postsParameters)
        {
            var result = await _locationTaxBL.GetAllZones(serch, filter,GetLanguage() , postsParameters);
            return Ok(result);
        }
        [HttpGet()]
        [Route("GetZones")]
        public async Task<IActionResult> GetZones()
        {
            var result = await _locationTaxBL.GetZones();
            return Ok(result);
        }
        [HttpGet("GetZonesCountry/{id}")]
        public async Task<IActionResult> GetZonesCountry(int id)
        {
            var result = await  _locationTaxBL.GetZonesCountryId(id);
            return Ok(result);
        }

        [HttpPost()]
        [Route("CreateZone")]
        public async Task<IActionResult> CreateZone(CreateZoneDto create)
        {
            var result = await _locationTaxBL.AddZone(create);
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
        public async Task<IActionResult> UpdateZone(int id , UpdateZoneDto update)
        {
            var result = await _locationTaxBL.EditZone(id,update);
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
        public async Task<IActionResult> DeleteZone(int id)
        {
            var result = await _locationTaxBL.DeleteZone(id);
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
