using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.AccessControl;
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
        public async Task<IActionResult> GetPopularProducts( int catId , int type, int price1 , int price2)
        {
            var result = await _productBL.GetProductsHome(catId,GetCurrentUserId(),GetLanguage(),type,price1,price2);
            return Ok(result);
        }
        [HttpGet("topRated")]
        public async Task<IActionResult> GetTopRatedProducts(int catId, int type, int price1, int price2)
        {
            var result = await _productBL.GetProductsHome(catId, GetCurrentUserId(), GetLanguage(), type, price1, price2);
            result.Products = result.Products.Where(c => c.Rate != 0).OrderByDescending(r => r.Rate).ToList();
            return Ok(result);
        }
        [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetAllProducts(int catId, int type, int price1, int price2 , PostsParameters postsParameters)
        {
            var result = await _productBL.GetProductsHome(catId, GetCurrentUserId(), GetLanguage(), type, price1, price2);
            result.Products = result.Products.Where(c => c.Rate != 0).ToList();
            return Ok(result);
        } 
        [HttpGet("specialProducts")]
        public async Task<IActionResult> GetSpecialsProducts(int catId, int type, int price1, int price2)
        {
            var result = await _productBL.GetProductsHome(catId, GetCurrentUserId(), GetLanguage(), type, price1, price2);
            result.Products = await _productBL.GetSpecialsProducts(GetCurrentUserId(), GetLanguage());
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
