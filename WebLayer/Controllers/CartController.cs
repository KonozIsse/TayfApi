
using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpGet("getcartToCustomer")]
        public async Task<IActionResult> GetAllCartToCustomer()
        {
            var result =  await _cartBL.GetCarts(GetCurrentUserId());
            return Ok(result);
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
        
    }
}
