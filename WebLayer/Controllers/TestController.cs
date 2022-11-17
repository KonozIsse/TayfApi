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
        private readonly ProductBL _productBL;
        public TestController(IServiceProvider serviceProvider , HomeBL home, NewsBL news, UserBL userBL, 
            LocationTaxBL locationTaxBL , ProductBL productBL) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
            _userBL = userBL;
            _locationTaxBL = locationTaxBL;
            _productBL = productBL;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add( CreateSaleDto create)
        {
            await _productBL.AddFlashSale(create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(int id  )
        {
            await _productBL.ActiveReview(id);
            return NoContent();
        } 
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id)
        {
            await _productBL.DeleteFlashSale(id);
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get()
        {
            var langs =  await _productBL.GetProducts();
            return Ok(langs);
        }
    }
}
