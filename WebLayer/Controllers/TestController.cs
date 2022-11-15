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
        public TestController(IServiceProvider serviceProvider , HomeBL home, NewsBL news, UserBL userBL , LocationTaxBL locationTaxBL) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
            _userBL = userBL;
            _locationTaxBL = locationTaxBL;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add(int id ,CreateAddressDto create)
        {
            await _locationTaxBL.CreateAddress(id , create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(int id , UpdateAddressDto update , int ids)
        {
            await _locationTaxBL.EditAddres(id , update , ids);
            return NoContent();
        } 
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id  )
        {
            await _locationTaxBL.DeleteTaxRate(id );
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get(int id )
        {
            var langs =  await _locationTaxBL.DefaultAddress(id);
            return Ok(langs);
        }
    }
}
