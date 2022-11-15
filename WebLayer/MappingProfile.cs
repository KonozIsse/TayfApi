using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebLayer.Helpers;

namespace WebLayer
{
    public class MappingProfile : Profile
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public MappingProfile(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            //MapContact
            CreateMap<CreateContactDto, Contact>().ReverseMap();
            //MapLanguage
            CreateMap<LanguageDto, Language>().ReverseMap();
            CreateMap<UpdateLanguageDto, Language>().ReverseMap();
            //MapCurrency
            CreateMap<CurrencyDto, Currency>().ReverseMap();
            CreateMap<CreateCurrencyDto, Currency>().ReverseMap();
            CreateMap<UpdateCurrencyDto, Currency>().ReverseMap();
            //MapCart
            CreateMap<CartDto, Cart>().ReverseMap();
            CreateMap<CreateCartDto, Cart>().ReverseMap();
            CreateMap<UpdateCartDto, Cart>().ReverseMap();
            //MapCartProduct
            CreateMap<CartProductDto, CartProduct>().ReverseMap();
            //MapCartAttributeProduct
            CreateMap<CartAttributeProductDto, CartAttributeProduct>().ReverseMap();
            //MapCustomerProduct
            CreateMap<CustomerProductDto, CustomerProduct>().ReverseMap(); 
            CreateMap<CreateCustomerProductDto, CustomerProduct>().ReverseMap();
            CreateMap<UpdateCustomerProductDto, CustomerProduct>().ReverseMap();
            //MapCoupon
            CreateMap<CreateCouponDto, Coupon>().ReverseMap();
            CreateMap<CouponDto, Coupon>().ReverseMap();
            //MapCategory
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<MainCategoryDto, Category>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();
            //MapCountry
            CreateMap<CountryDto, Country>().ReverseMap();
            CreateMap<CreateCountryDto, Country>().ReverseMap(); 
            CreateMap<UpdateCountryDto, Country>().ReverseMap();
            //MapZone
            CreateMap<ZoneDto, Zone>().ReverseMap();
            CreateMap<CreateZoneDto, Zone>().ReverseMap();
            CreateMap<UpdateZoneDto, Zone>().ReverseMap();
            //MapNews
            CreateMap<NewsDto, News>().ReverseMap();
            CreateMap<CreateNewsDto, News>().ReverseMap();
            CreateMap<UpdateNewsDto, News>().ReverseMap();
            //MapCommentNews
            CreateMap<CreateCommentsDto, CommentNews>().ReverseMap();
            CreateMap<CommentsDto, CommentNews>().ReverseMap();
            //MapProduct
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();
            //MapProductSales
            CreateMap<ProductSalesDto, ProductSales>().ReverseMap();
            CreateMap<CreateProductSalesDto, ProductSales>().ReverseMap();
            CreateMap<UpdateSalesProductDto, ProductSales>().ReverseMap();
            //MapSpecialProducts
            CreateMap<SpecialProductsDto, SpecialProducts>().ReverseMap();
            CreateMap<CreateSpecialProductsDto, SpecialProducts>().ReverseMap();
            CreateMap<UpdateSpecialProductDto, SpecialProducts>().ReverseMap();
            //MapAttributesProduct
            CreateMap<UpdateAttributeDto, ProductAttribut>().ReverseMap();
            //MapOption
            CreateMap<OptionDto, ProductOption>().ReverseMap();
            CreateMap<CreateOptionDto, ProductOption>().ReverseMap();
            //MapValue
            CreateMap<ValueDto, ProductOptionValue>().ReverseMap();
            CreateMap<CreateValueDto, ProductOptionValue>().ReverseMap();
            CreateMap<UpdateValueDto, ProductOptionValue>().ReverseMap();
            //MapAddress
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<CreateAddressDto, Address>().ReverseMap();
            CreateMap<UpdateAddressDto, Address>().ReverseMap();
            //MapReview
            CreateMap<ReviewDto, Review>().ReverseMap();
            CreateMap<CreateReviewDto, Review>().ReverseMap(); 
            CreateMap<UpdateReviewDto, Review>().ReverseMap();
            //MapWishList
            CreateMap<CreateLikeDto, WishList>().ReverseMap();
            CreateMap<WishListDto, WishList>().ReverseMap();
            //MapOrder
            CreateMap<CreateOrderDto, Order>().ReverseMap();
            CreateMap<UpdateOderDto, Order>().ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
            //MapOrderProduct
            CreateMap<OrderProductDto, OrderProduct>().ReverseMap();
            //MapStore
            CreateMap<StoreDto, User>().ReverseMap();
            CreateMap<CreateStoreDto, User>().ReverseMap();
            CreateMap<UpdateStoreDto, User>().ReverseMap(); 
            //MapCustomer
            CreateMap<CreateCustomerDto, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
            CreateMap<UserForRegistrationDto, User>().ReverseMap();
            //MapTaxRate
            CreateMap<TaxRateDto, TaxRate>().ReverseMap();
            CreateMap<CreateTaxRateDto, TaxRate>().ReverseMap();
            CreateMap<UpdateTaxRateDto, TaxRate>().ReverseMap();
            //MapTaxClass
            CreateMap<TaxClassDto, TaxClass>().ReverseMap();
            CreateMap<CreateTaxClassDto, TaxClass>().ReverseMap();
            CreateMap<UpdateTaxClassDto, TaxClass>().ReverseMap();
            //MapSetting
            CreateMap<SettingDto, Setting>().ReverseMap(); 
            CreateMap<UpdateSettingDto, Setting>().ReverseMap();
            //MapService
            CreateMap<ServiceDto, Service>().ReverseMap(); 
            CreateMap<UpdateServiceDto, Service>().ReverseMap();
            //MapSliders
            CreateMap<SliderDto, Sliders>().ReverseMap(); 
            CreateMap<CreateSliderDto, Sliders>().ReverseMap();
            CreateMap<UpdateSliderDto, Sliders>().ReverseMap();
            //MapBanner
            CreateMap<BannerDto, Banner>().ReverseMap(); 
            CreateMap<UpdateBannerDto, Banner>().ReverseMap();
            //MapImage
            CreateMap<ImageDto, Image>().ReverseMap();
            CreateMap<CreateImageDto, Image>().ReverseMap();
            //MapImageSetting
            CreateMap<ImageSettingDto, ImageSetting>().ReverseMap();
            //MapDeliveryTime
            CreateMap<DeliveryTimeDto, DeliveryTime>().ReverseMap();
            //MapDevice
            CreateMap<DeviceDto, Device>().ReverseMap();
            CreateMap<CreateDeviceDto, Device>().ReverseMap();
            CreateMap<UpdateDeviceDto, Device>().ReverseMap();
            //MapMailList
            CreateMap<MailListDto, MailList>().ReverseMap();
            CreateMap<SendMailListDto, MailList>().ReverseMap();
            //MapMessageTemplate
            CreateMap<MessageTemplateDto, MessageTemplate>().ReverseMap();
            CreateMap<UpdateTemplateDto, MessageTemplate>().ReverseMap();

            ForAllMaps((typeMap, m) =>
            {
                Expression<Action<object, object>> afterFunction = (sourceObj, destObj) => FillEnumDesc(destObj);
                typeMap.AddAfterMapAction(afterFunction);
            });
        }

        private void FillEnumDesc(object destObj)
        {
            var hasEnum = destObj.GetType().GetProperties().Any(x => x.IsDefined(typeof(EnumBindAttribute), false));
            if (hasEnum)
            {
                EnumHelper.FillEnumDesc(destObj);
            }
        }
    }
}
