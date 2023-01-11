using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [Route("api/product")]
    [ApiController]
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
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct(CreateProductDto create )
        {
            var result = await _productBL.AddProduct(create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
               return BadRequest(result.Message);
            }
        }
        [HttpPut("updateProduct")]
        public async Task<IActionResult> UpdateProduct( UpdateProductDto update)
        {
            var result = await _productBL.EditProduct(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("acceptProduct")]
        public async Task<IActionResult> AcceptProduct(int productId)
        {
            var result = await _productBL.ApproveProduct(productId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("removeProduct")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            var result = await _productBL.RemoveProduct(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        [HttpPost("addAttribute")]
        public async Task<IActionResult> AddAttributeProduct(int productId, CreateAttributeDto item)
        {
            var result = await _productBL.AddAttribute(productId, item);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("updateAttribute")]
        public async Task<IActionResult> UpdateAttributeProduct(UpdateAttributeDto item)
        {
            var result = await _productBL.UpdateAttribute(item);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        } 
        [HttpDelete("deleteAttribute")]
        public async Task<IActionResult> DeleteAttributeProduct(int id )
        {
            var result = await _productBL.DeleteAttribute(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        } 
        [HttpPut("editImage")]
        public async Task<IActionResult> EditImageProduct(int id , string image)
        {
            var result = await _imageBL.EditImage(id, image);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        } 
        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImageProduct(int id )
        {
            var result = await _imageBL.DeleteImage(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        
    }
}
