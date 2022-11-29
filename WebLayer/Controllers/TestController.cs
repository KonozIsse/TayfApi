using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using MailKit.Search;
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
        public async Task<IActionResult> add( CreateOrderDto create)
        {
            await _orderBL.AddOrder(create);
            return StatusCode(201);
        }
        [HttpPost("add2")]
        public async Task<IActionResult> addAdd (int id , CreateCartDto create)
        {
            await _cartBL.AddCart(id , create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(int id , UpdateOderDto st )
        {
          //  await _orderBL.EditOrder(id , st);
            return NoContent();
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id )
        {
            await _orderBL.DeleteOrder(id);
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get(int id , int ut )
        {
            var langs = await _productBL.GetProduct( id , ut );
            return Ok(langs);
        }
    }
}
