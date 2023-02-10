using Entities.DataTransferObjects;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace WebLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : MyBaseController
    {
        public HomeController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getHome")]
        public async Task<IActionResult> GetHome()
        {
            var result = await _homeBL.GetHome(GetCurrentUserId(), GetCurrentCurrency(), GetLanguage());
            return Ok(result);
        } 
        [HttpGet("getAllAddressesToCustomer")]
        public async Task<IActionResult> GetAddressesCustomer()
        {
            var result = await _locationTaxBL.GetAddressesCustomer(GetCurrentUserId());
            return Ok(result);
        }
        [HttpGet("getLogoWeb")]
        public async Task<IActionResult> GetLogoWeb()
        {
            var result = await _homeBL.GetLogo();
            return Ok(result);
        }
        [HttpGet("getAllStores")]
        public async Task<IActionResult> GetAllStores([FromQuery] PostsParameters postsParameters,int? sort)
        {
            var result = await _userBL.GetAllActiveStores(postsParameters , sort);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPut("changeDefaultLanguage")]
        public async Task<IActionResult> ChangeDefaultLanugage(int id)
        {
            await _homeBL.ChangeDefaultLanugage(id);
            return Ok();
        }
        [HttpGet("getAboutUsPage")]
        public async Task<IActionResult> AboutUsPage()
        {
            var result = await _homeBL.GetTypePage(PageType.AboutUs, GetLanguage());
            return Ok(result);
        }  
        [HttpGet("getPrivacyPage")]
        public async Task<IActionResult> PrivacyPage()
        {
            var result = await _homeBL.GetTypePage(PageType.Privacy, GetLanguage());
            return Ok(result);
        }
        [HttpGet("getBecomeVendorPage")]
        public async Task<IActionResult> BecomeVendorPage()
        {
            var result = await _homeBL.GetTypePage(PageType.BecomeVendor, GetLanguage());
            return Ok(result);
        } 
        [HttpGet("getDeliveryInformationPage")]
        public async Task<IActionResult> DeliveryInformationPage()
        {
            var result = await _homeBL.GetTypePage(PageType.DeliveryInformation, GetLanguage());
            return Ok(result);
        }
        [HttpGet("getTermsconditionsPage")]
        public async Task<IActionResult> TermsConditionsPage()
        {
            var result = await _homeBL.GetTypePage(PageType.Termsconditions, GetLanguage());
            return Ok(result);
        }
        [HttpGet("getActiveReviewsProduct/{productId}")]
        public async Task<IActionResult> GetActiveReviewsProduct(int productId)
        {
            var result = await _productBL.GetActiveReviews(productId);
            return Ok(result);
        }
        [HttpGet("getAllMainCategories")]
        public async Task<IActionResult> GetAllMainCategories()
        {
            var result = await _productBL.GetAllActiveMainCategories(GetLanguage());
            return Ok(result);
        } 
        [HttpGet("getAllSubCategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var result = await _productBL.GetAllSubActiveCategories(GetLanguage());
            return Ok(result);
        } 
        [HttpGet("GetSocialSetting")]
        public async Task<IActionResult> GetSocialSetting()
        {
            var result = await _homeBL.GetSocialSetting();
            return Ok(result);
        }  
        [HttpGet("GetContactSetting")]
        public async Task<IActionResult> GetContactSetting()
        {
            var result = await _homeBL.GetContactSetting();
            return Ok(result);
        } 
        [HttpGet("getCountAllLikesByCustomer")]
        public async Task<IActionResult> GetCountLikesByCustomer()
        {
            int count = 0;
            var customerId = GetCurrentUserId();
            if (customerId > 0)
            {
                var likes = await _repositoryManager.WishList.GetLikesCustomerId(customerId);
                count = likes.Count();
            }
            return Ok(count);
        } 
        [HttpGet("getAllCartsToCustomer")]
        public async Task<IActionResult> GetAllCartsToCustomer()
        {
            var result = await _cartBL.GetCarts(GetCurrentUserId());
            return Ok(result);
        }
        [HttpPost("AddContact")]
        public async Task<IActionResult> AddContact(CreateContactDto create)
        {
            var result = await _homeBL.AddContact(create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail(string email)
        {
            await _homeBL.SendUserEmail(email);
            return Ok();
        }
        [Authorize]
        [HttpPost("addProductsToCart")]
        public async Task<IActionResult> AddToCart(CreateCartDto create)
        {
            var result = await _cartBL.AddedToCart(GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        } 
        [HttpPut("updateCartToCustomer")]
        public async Task<IActionResult> UpdateCart(int id, int Quantity = 1)
        {
            var create = new UpdateCartDto
            {
                Id = id,
                Qty = Quantity
            };
            decimal final = await _cartBL.UpdateTotalCart(GetCurrentUserId(), create);
            return Ok(final);
        }
        [HttpGet("getAllProductsToCategory")]
        public async Task<IActionResult> GetAllProductsCategory(int catId,int? sort,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetAllProductsToCategory(catId, GetCurrentUserId(), GetLanguage(), sort, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        } 
        [HttpGet("getAllProductsToStore")]
        public async Task<IActionResult> GetAllProductsToStore(int storeId,int? catId,int type, int? price1 , int? price2)
        {
            var result = await _productBL.GetAllProductsToStore(storeId,catId, GetCurrentUserId(), GetLanguage(),type,price1,price2);
            return Ok(result);
        }
        [HttpGet("getAllSubActiveCategoriesToMainCategory")]
        public async Task<IActionResult> GetAllSubCatsToMainCatogry(int mainCatId,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _productBL.GetActiveSubCategoriesMainId(mainCatId, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
    }
}
