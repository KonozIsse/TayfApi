using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Twilio.Rest.Serverless.V1.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : MyBaseController
    {
        public CustomersController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllCustomers(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetCustomers(search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto create)
        {
            var result = await _userBL.RegisterCustomer(create, GetLanguage());
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
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerDto update)
        {
            var result = await _userBL.UpdateCustomerCP(update);
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
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _userBL.DeleteCustomer(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        [HttpPut("deactive")]
        public async Task<IActionResult> DeactiveCustomer(int id)
        {
            var result = await _userBL.DeactiveCustomer(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("active")]
        public async Task<IActionResult> ActiveCustomer(int id)
        {
            var result = await _userBL.ActiveCustomer(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        //---------------------------------------------------------
        [HttpGet("getAllAddressesCustomer/{customerId}")]
        public async Task<IActionResult> GetAllAddressesCustomer(int customerId, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _locationTaxBL.GetAddressesCustomerId(customerId, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPost("createAddress")]
        public async Task<IActionResult> CreateAddressCustomer(int customerId ,CreateAddressDto create)
        {
            var result = await _locationTaxBL.CreateAddress(customerId, create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("updateAddress")]
        public async Task<IActionResult> EditAddressCustomer(int customerId, UpdateAddressDto update)
        {
            var result = await _locationTaxBL.EditAddress(customerId, update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("deleteAddress")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _locationTaxBL.DeleteAddress(id);
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
