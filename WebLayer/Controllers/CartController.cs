
using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.BuilderProperties;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : MyBaseController
    {
        
        public CartController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            
        }
        [HttpGet("getAllCartsToCustomer")]
        public async Task<IActionResult> GetAllCartsToCustomer()
        {
            var result =  await _cartBL.GetCarts(GetCurrentUserId());
            return Ok(result);
        }
        [Authorize]
        [HttpPost("addProductsToCart")]
        public async Task<IActionResult> AddToCart(CreateCartDto create)
        {
            var result = await _cartBL.AddedToCart(GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("updateCart")]
        public async Task<IActionResult> UpdateCart(int cartId, int qty)
        {
            var result = await _cartBL.UpdateTotalCart(GetCurrentUserId(), cartId,qty);
           return Ok(result);
        } 
        [HttpPost("checkout-order")]
        public async Task<IActionResult> CheckoutOrder(CreateOrderDto create )
        {
            var result = await _orderBL.AddOrder(GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("deleteCart")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var result = await _cartBL.DeleteCart(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        //--------------------------------------------------
        [HttpGet("getAllStoresInCartsToCustomer")]
        public async Task<IActionResult> GetAllStoresInCarts()
        {
            var result =  await _cartBL.GetAllStoresInCartsToCustomer(GetCurrentUserId());
            return Ok(result);
        }
        [HttpDelete("deleteCartCustomerStore/{storeId}")]
        public async Task<IActionResult> DeleteCartCustomerStore(int storeId)
        {
            var result = await _cartBL.DeleteCartCustomerStore(GetCurrentUserId(), storeId);
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
