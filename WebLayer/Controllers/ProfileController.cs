using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : MyBaseController
    {
        public ProfileController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("GetMyWishListToProducts")]
        public async Task<IActionResult> GetMyWishList(int catId, int type, int price1, int price2,int sort)
        {
            var result = await _productBL.GetMyWishList(catId, GetCurrentUserId(), GetLanguage(), sort,type, price1, price2);
            return Ok(result);
        }
        [HttpPost("createAddressToCustomer")]
        public async Task<IActionResult> CreateAddress(CreateAddressDto create)
        {
            var result = await _locationTaxBL.CreateAddress( GetCurrentUserId(),create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("updateAddressToCustomer")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressDto create)
        {
            var result = await _locationTaxBL.EditAddress( GetCurrentUserId(),create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("deleteAddressToCustomer/{addressId}")]
        public async Task<IActionResult> DeleteAddressCustomer(int addressId)
        {
            var result = await _locationTaxBL.DeleteAddressCustomer(addressId, GetCurrentUserId());
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        [HttpPost("addWishListCustomerToProduct{productId}")]
        public async Task<IActionResult> AddWishList(int productId)
        {
            var result = await _productBL.AddWishList(GetCurrentUserId(), productId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("deleteLikeProduct")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            var result = await _productBL.DeleteLike(id, GetCurrentUserId());
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
