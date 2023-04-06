using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : MyBaseController
    {
        public OptionsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllOptions ([FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllOptions(postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOption(CreateOptionDto create)
        {
            var result = await _productBL.AddOption(create);
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
        public async Task<IActionResult> UpdateOption(UpdateOptionDto update)
        {
            var result = await _productBL.EditOption(update);
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
        public async Task<IActionResult> DeleteOption(int id)
        {
            var result = await _productBL.DeleteOptionProduct(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        //-------------------------------------------------------------
        [HttpGet("getAllValuesOption/{optionId}")]
        public async Task<IActionResult> GetAllValuesOption(int optionId ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetListValues(optionId, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("createValue")]
        public async Task<IActionResult> CreateValueOption(int optionId, CreateValueDto create)
        {
            var result = await _productBL.AddValue(optionId, create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("updateValue")]
        public async Task<IActionResult> UpdateValue(UpdateValueDto update)
        {
            var result = await _productBL.UpdateValue(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("deleteValue")]
        public async Task<IActionResult> DeleteValue(int id)
        {
            var result = await _productBL.DeleteValueProduct(id);
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
