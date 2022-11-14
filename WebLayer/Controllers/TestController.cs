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
        public TestController(IServiceProvider serviceProvider , HomeBL home, NewsBL news) : base(serviceProvider)
        {
            _homeBL = home;
            _newsBL = news;
        }
        [HttpPost("add")]
        public async Task<IActionResult> add(CreateCommentsDto create)
        {
            await _newsBL.AddNewsComments(create);
            return StatusCode(201);
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(int id , UpdateCurrencyDto update)
        {
            await _homeBL.UpdatCurrency(id , update);
            return NoContent();
        } 
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(int id , int cid )
        {
            await _newsBL.DeleteNewsComments(id , cid);
            return NoContent();
        }  
        [HttpGet("get")]
        public async Task<IActionResult> get(int id , string search )
        {
            var langs =  await _newsBL.SearchCommetsNews(id , search );
            return Ok(langs);
        }
    }
}
