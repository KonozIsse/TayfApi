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

        public TestController(IServiceProvider serviceProvider , HomeBL home, NewsBL news, UserBL userBL, 
            LocationTaxBL locationTaxBL , ProductBL productBL , OrderBL orderBL , ImageBL imageBL) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
            _userBL = userBL;
            _locationTaxBL = locationTaxBL;
            _productBL = productBL;
            _orderBL = orderBL;
            _imageBL = imageBL;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add([FromForm] CreateImageDto create)
        {
            await _imageBL.AddImage(create);
            return StatusCode(201);
        }
        [HttpPost("adduser")]
        public async Task<IActionResult> addAdd ([FromForm] int id , CreateProductDto create)
        {
            await _productBL.AddProduct(id ,create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromForm] AvaterDto update)
        {
            await _imageBL.UpdateAvatarCustomer(update);
            return NoContent();
        } 
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id)
        {
            await _userBL.LogOutDevice(id);
            return NoContent();
        }  
        [HttpGet("get")]
        public IActionResult get(int id)
        {
            var langs =   _imageBL.GetListImagesProductIdAsync(id);
            return Ok(langs);
        }
    }
}
