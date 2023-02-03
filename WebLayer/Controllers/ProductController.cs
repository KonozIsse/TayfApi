using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : MyBaseController
    {
        public ProductController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
           
        }
        [HttpGet("getProductDetails/{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var result = await _productBL.GetProductDetails(id , GetCurrentUserId(),  GetLanguage());
            return Ok(result);
        }
      
        [HttpGet("popular")]
        public async Task<IActionResult> PopularProducts()
        {
            var result = await _productBL.PopularsPage();
            return Ok(result);
        }
        [HttpGet("topRated")]
        public async Task<IActionResult> TopRatedProducts()
        {
            var result = await _productBL.TopRatedPage();
            return Ok(result);
        }
        [HttpPost("addReview")]
        public async Task<IActionResult> CreateReviewProduct(int productId ,CreateReviewDto create )
        {
            var result = await _productBL.AddReview(productId, GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
               return BadRequest(result.Message);
            }
        } 
       
    }
}
