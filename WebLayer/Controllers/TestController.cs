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
        public TestController(IServiceProvider serviceProvider , HomeBL home, NewsBL news, UserBL userBL, 
            LocationTaxBL locationTaxBL , ProductBL productBL , OrderBL orderBL) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
            _userBL = userBL;
            _locationTaxBL = locationTaxBL;
            _productBL = productBL;
            _orderBL = orderBL;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add(int id , CreateAttributeDto create)
        {
            await _productBL.AddAttribute(id ,create);
            return StatusCode(201);
        }
        [HttpPost("adduser")]
        public async Task<IActionResult> addAdd (int id , CreateAddressDto create)
        {
            await _locationTaxBL.CreateAddress(id ,create);
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
            await _productBL.DeleteCategory(id);
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get(int id , int idw)
        {
            var langs =  await _productBL.GetProductsCatId(id , idw);
            return Ok(langs);
        }
    }
}
