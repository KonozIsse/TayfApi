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
      
        [HttpGet("getPopularProducts")]
        public async Task<IActionResult> GetPopularProducts( int? catId , int type, int? price1 , int? price2)
        {
            var result = await _productBL.GetAllProducts(catId,GetCurrentUserId(),GetLanguage(),type,price1,price2);
            result.Products = result.Products.Where(c => c.IsPopular == 1).ToList();
            return Ok(result);
        }
        [HttpGet("topRatedProducts")]
        public async Task<IActionResult> GetTopRatedProducts(int? catId, int type, int? price1, int? price2)
        {
            var result = await _productBL.GetAllProducts(catId, GetCurrentUserId(), GetLanguage(), type, price1, price2);
            result.Products = result.Products.Where(c => c.Rate != 0).OrderByDescending(r => r.Rate).ToList();
            return Ok(result);
        }
        [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetAllProducts(string search , int? catId, int type, int? price1, int? price2, int pageId,int? sort)
        {
            var result = await _productBL.GetAllSearchProducts(catId, GetCurrentUserId(),search, GetLanguage(), pageId, sort, type, price1, price2);
            return Ok(result);
        } 
        [HttpGet("specialProducts")]
        public async Task<IActionResult> GetSpecialsProducts(int? catId, int type, int? price1, int? price2)
        {
            var result = await _productBL.GetAllProducts(catId, GetCurrentUserId(), GetLanguage(), type, price1, price2);
            result.Products = result.Products.Where(c => c.IsSpecial == true && c.EndDateSpecial > DateTime.Now).ToList();
            return Ok(result);
        }
        [HttpGet("getAllProductsToCategory")]
        public async Task<IActionResult> GetAllProductsCategory(int catId)
        {
            var result = await _productBL.GetAllProductsCategory(catId, GetCurrentUserId(), GetLanguage());
            return Ok(result);
        }

        [HttpPost("addReviewToProduct")]
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
