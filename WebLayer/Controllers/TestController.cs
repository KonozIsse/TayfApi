using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : MyBaseController
    {
        private readonly HomeBL _homeBL;
        private readonly NewsBL _newsBL;
        private readonly UserBL _userBL;
        private readonly LocationTaxBL _locationTaxBL;
        private readonly OrderBL _orderBL;
        private readonly ProductBL _productBL;
        private readonly ImageBL _imageBL;
        private readonly CartBL _cartBL;

        public TestController(IServiceProvider serviceProvider, ProductBL productBL  , HomeBL home , NewsBL news, UserBL userBL, 
            LocationTaxBL locationTaxBL , OrderBL orderBL , ImageBL imageBL , CartBL cartBL) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
            _userBL = userBL;
            _locationTaxBL = locationTaxBL;
            _productBL = productBL;
            _orderBL = orderBL;
            _imageBL = imageBL;
            _cartBL = cartBL;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add(int id , CreateCartDto create)
        {
            await _cartBL.AddCart(id ,create);
            return StatusCode(201);
        }
        [HttpPost("adduser")]
        public async Task<IActionResult> addAdd ( int p ,int id , CreateCustomerProductDto create)
        {
            await _cartBL.AddCustomerProduct(p , id , create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update( int id  ,int cid, UpdateCartDto update)
        {
            await _cartBL.UpdateCart(id , cid, update);
            return NoContent();
        } 
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id , int sid)
        {
            //await _productBL.DeleteSpecialProduct(id , sid);
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get()
        {
            var langs = await _productBL.GetFlashProds();
            return Ok(langs);
        }
    }
}
