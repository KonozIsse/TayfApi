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
        [HttpGet("allProductsCP")]
        public async Task<IActionResult> GetProductsCP(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetProductsCP(GetStoreId(), search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetProducts(string search)
        {
            var result = await _productBL.GetProductsHome(GetStoreId(), search, GetLanguage());
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
      
        [HttpGet("productId")]
        public async Task<IActionResult> GetDetailProduct(int productId)
        {
            var result = await _productBL.GetDetailProduct(productId,GetCustomerId(), GetLanguage());
            return Ok(result);
        } 
        [HttpPost("addReview")]
        public async Task<IActionResult> CreateReviewProduct(int productId ,CreateReviewDto create )
        {
            var result = await _productBL.AddReview(productId, GetCustomerId(), create);
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
