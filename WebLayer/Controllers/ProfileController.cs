using BusinessLogic;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [HttpGet("getMyWishListToProducts")]
        public async Task<IActionResult> GetMyWishList(int catId, int type, int price1, int price2,int sort)
        {
            var result = await _productBL.GetMyWishList(catId, GetCurrentUserId(), GetLanguage(), sort,type, price1, price2);
            return Ok(result);
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
        //Account----------------------------------------------------------
        [HttpPut("edit-account-customer")]
        public async Task<IActionResult> EditCustomer(UpdateCustomerDto update)
        {
            var result = await _userBL.EditCustomer(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("editSubscribeletter")]
        public async Task<IActionResult> EditSubscribeLetter(string subscribe)
        {
            var result = await _userBL.EditSubscribeletter(subscribe,GetCurrentUserId());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (model is null)
                    return BadRequest(_locService.GetLocalizedStringValue("enterPassword"));
                var user = GetCurrentUser();
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(resetToken))
                    return BadRequest(_locService.GetLocalizedStringValue("Error while generating reset token."));
                //if (user.PasswordHash != model.OldPassword)
                //    return BadRequest(_locService.GetLocalizedStringValue("passwnotequal"));

                if (model.ConfirmPassword != model.NewPassword)
                    return BadRequest(_locService.GetLocalizedStringValue(_locService.GetLocalizedStringValue("ConfirmPassAtLeast")));
                var decrpass = _util.decr(model.OldPassword);
                
                var result = await _userManager.ChangePasswordAsync(user, decrpass, model.NewPassword);

                if (result.Succeeded)
                    return Ok(_locService.GetLocalizedStringValue("PasswordChangedSuccessfully"));
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPut("add-image-customer")]
        public async Task<IActionResult> AddImageToCustomer(IFormFile image)
        {
            var result = await _imageBL.AddImageCustomer(GetCurrentUserId(),image);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        //Order----------------------------------------------------------
        [HttpGet("getHistoryAllMyOrders")]
        public async Task<IActionResult> GetHistoryMyOrders([FromQuery]PostsParameters postsParameters)
        {
            var result = await _orderBL.GetHistoryOrder(GetCurrentUserId(), GetCurrentCurrency(),postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        } 
        [HttpGet("getDetailsHistoryOrderToCustomer")]
        public async Task<IActionResult> GetInvoiceOrder(int orderId)
        {
            var result = await _orderBL.GetOrder(orderId);
            if(result.CustomerId == GetCurrentUserId())
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(_locService.GetLocalizedStringValue("Error"));
            }
        }
        //Address----------------------------------------------------------
        [HttpGet("getAllAddressesToCustomer")]
        public async Task<IActionResult> GetAddressesCustomer()
        {
            var result = await _locationTaxBL.GetAddressesCustomer(GetCurrentUserId());
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
       
    }
}
